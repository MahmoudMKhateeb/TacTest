using Abp.Application.Features;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Linq.Extensions;
using Abp.UI;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TACHYON.Features;
using TACHYON.PriceOffers;
using TACHYON.PriceOffers.Dto;
using TACHYON.PricePackages.Dto;
using TACHYON.Shipping.DirectRequests;
using TACHYON.Shipping.DirectRequests.Dto;
using TACHYON.Shipping.ShippingRequests;
using TACHYON.ShippingRequestVases;
using TACHYON.Vases;

namespace TACHYON.PricePackages.PricePackageOffers
{
    internal class PricePackageOfferManager : TACHYONDomainServiceBase ,IPricePackageOfferManager
    {
        private readonly IRepository<PricePackageOffer,long> _pricePackageOfferRepository;
        private readonly IRepository<ShippingRequest, long> _shippingRequestRepository;
        private readonly IRepository<PricePackage,long> _pricePackageRepository;
        private readonly IShippingRequestDirectRequestAppService _directRequestAppService;
        private readonly PriceOfferManager _priceOfferManager;
        private readonly IRepository<PriceOffer, long> _priceOfferRepository;
        private readonly IRepository<ShippingRequestVas,long> _shippingRequestVasRepository;
        private readonly IRepository<VasPrice> _vasPriceRepository;
        private readonly IRepository<ShippingRequestDirectRequest,long> _directRequestRepository;
        private readonly IFeatureChecker _featureChecker;
        private readonly IConfigurationProvider _configurationProvider;


        public PricePackageOfferManager(
            IRepository<ShippingRequest, long> shippingRequestRepository,
            IRepository<PricePackage,long> pricePackageRepository,
            IShippingRequestDirectRequestAppService directRequestAppService,
            PriceOfferManager priceOfferManager,
            IRepository<PriceOffer, long> priceOfferRepository,
            IRepository<PricePackageOffer, long> pricePackageOfferRepository,
            IRepository<ShippingRequestVas, long> shippingRequestVasRepository,
            IRepository<VasPrice> vasPriceRepository, 
            IRepository<ShippingRequestDirectRequest, long> directRequestRepository,
            IFeatureChecker featureChecker)
        {
            _shippingRequestRepository = shippingRequestRepository;
            _pricePackageRepository = pricePackageRepository;
            _directRequestAppService = directRequestAppService;
            _priceOfferManager = priceOfferManager;
            _priceOfferRepository = priceOfferRepository;
            _pricePackageOfferRepository = pricePackageOfferRepository;
            _shippingRequestVasRepository = shippingRequestVasRepository;
            _vasPriceRepository = vasPriceRepository;
            _directRequestRepository = directRequestRepository;
            _featureChecker = featureChecker;
            _configurationProvider = IocManager.Instance.Resolve<IMapper>()?.ConfigurationProvider;
        }
        
        
        
        
        
        /// <summary>
        /// this method used for apply the pricing that the shipper/carrier agree it by price package
        /// the stage can be use after price package appendix is confirmed 
        /// </summary>
        /// <param name="pricePackageId"></param>
        /// <param name="srId"></param>
        /// <param name="isTmsPricePackage"></param>
        /// <exception cref="UserFriendlyException"></exception>
        public async Task ApplyPricingOnShippingRequest(long pricePackageId,long srId)
        {
            // what will be the usage of this method ?!!
            // Is it support price packages that created by carriers ?! Yes
            // The Price package that used in the method, Should it be has an appendix or a proposal ?! Yes all of P.Ps must have appendix
            
            DisableTenancyFilters();

            var shippingRequest = await _shippingRequestRepository.GetAllIncluding(x=> x.ShippingRequestVases)
                .AsNoTracking().SingleAsync(x => x.Id == srId);
            
            PricePackage pricePackage = await _pricePackageRepository.SingleAsync(x=> x.Id == pricePackageId);
            
            if (pricePackage.UsageType == PricePackageUsageType.AsTachyonManageService)
            {
                
                await HandleTmsPricePackage(pricePackage,shippingRequest);
                return;
            }


            if (!pricePackage.AppendixId.HasValue)
                throw new UserFriendlyException(L("PricePackageMustHaveAppendixToSendRequest"));

            await SendDirectRequestToCarrierByPricePackage(pricePackage, shippingRequest.Id);

        }

        
        public async Task<bool> HasDirectRequestByPricePackage(long shippingRequestId)
        {
            DisableTenancyFilters();
            return await _pricePackageOfferRepository.GetAll().AnyAsync(x => x.DirectRequestId.HasValue && x.DirectRequest.ShippingRequestId == shippingRequestId);
        }
        
        /// <summary>
        ///send price offer based on price package prices -- used with marketplace request --
        /// </summary>
        public async Task<long> SendPriceOfferByPricePackage(long pricePackageId, long shippingRequestId)
        {
            DisableTenancyFilters();
            var priceOfferInput = await GeneratePriceOfferInput(pricePackageId, shippingRequestId);
            return await _priceOfferManager.CreateOrEdit(priceOfferInput);
        }
        
        /// <summary>
        /// Generate price offer input for handling shipping request based on price package
        /// we need this input to use calculation process that exists in the price offer manager
        /// </summary>
        private async Task<CreateOrEditPriceOfferInput> GeneratePriceOfferInput(long pricePackageId, long shippingRequestId)
        {
            PricePackage pricePackage = await _pricePackageRepository.FirstOrDefaultAsync(pricePackageId);

            if (pricePackage.UsageType != PricePackageUsageType.AsCarrier)
                throw new UserFriendlyException(L("OnlyThePricePackagesCreatedByCarrierAllowed"));
            Tuple<ShippingRequestType, int> shippingRequest;
            
            using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MayHaveTenant, AbpDataFilters.MustHaveTenant))
            {
                shippingRequest = await (from sr in _shippingRequestRepository.GetAll()
                    where sr.Id == shippingRequestId
                    select Tuple.Create(sr.RequestType, sr.NumberOfDrops) ).SingleAsync();
            }
            
            decimal price;
            if (shippingRequest.Item1 != ShippingRequestType.TachyonManageService)
            {
                if (!pricePackage.DirectRequestPrice.HasValue)
                    throw new UserFriendlyException(L("ThisPricePackageHasNotDirectRequestPrice"));

                price = pricePackage.DirectRequestPrice.Value;
            }
            else price = pricePackage.TotalPrice;

            decimal tripPrice = price;
            if (pricePackage.RouteType == ShippingRequestRouteType.MultipleDrops && shippingRequest.Item2 > 1 && pricePackage.PricePerAdditionalDrop.HasValue) 
                tripPrice = price + (shippingRequest.Item2 - 1) * pricePackage.PricePerAdditionalDrop.Value;
             
            

            return new CreateOrEditPriceOfferInput
            {
                ShippingRequestId = shippingRequestId,
                ItemPrice = tripPrice,
                ItemDetails = await CalculateShippingRequestVases(shippingRequestId, pricePackage.TenantId), 
            };
        }
        
        /// <summary>
        /// Calculate shipping request based on price package and generate price offer dto for viewing or handling
        /// </summary>
        public async Task<PricePackageForPriceCalculationDto> GetPricePackageForPriceCalculation(long pricePackageId, long shippingRequestId)
        {
            DisableTenancyFilters();

            var priceCalculationDto = await _pricePackageRepository.GetAll().Where(x=> x.Id == pricePackageId)
                .ProjectTo<PricePackageForPriceCalculationDto>(_configurationProvider).SingleAsync();
            
            var priceOfferInput = await GeneratePriceOfferInput(pricePackageId, shippingRequestId);
            var priceOffer = await _priceOfferManager.InitPriceOffer(priceOfferInput);
            priceCalculationDto.OfferDto = ObjectMapper.Map<PriceOfferDto>(priceOffer);

            Tuple<int, int> shippingRequest;
            
            using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MayHaveTenant, AbpDataFilters.MustHaveTenant))
            {
                shippingRequest = await (from sr in _shippingRequestRepository.GetAll()
                    where sr.Id == shippingRequestId select Tuple.Create(sr.NumberOfTrips, sr.NumberOfDrops)).SingleAsync();
            }
            
            priceCalculationDto.NumberOfDrops = shippingRequest.Item2; // item 2 is NumberOfDrops
            priceCalculationDto.NumberOfTrips = shippingRequest.Item1; // item 1 is NumberOfTrips

            await SetVasNames(shippingRequestId, priceCalculationDto.OfferDto);

            priceCalculationDto.HasDirectRequest = await _directRequestRepository.GetAll()
                .AnyAsync(x => x.CarrierTenantId == priceCalculationDto.TenantId && x.ShippingRequestId == shippingRequestId);
            if (await _featureChecker.IsEnabledAsync(AppFeatures.Carrier) || await _featureChecker.IsEnabledAsync(AppFeatures.CarrierAsASaas))
            {
                priceCalculationDto.OfferDto.CommissionAmount = priceCalculationDto.OfferDto.CommissionPercentageOrAddValue
                = priceCalculationDto.OfferDto.ItemCommissionAmount = priceCalculationDto.OfferDto.VatAmountWithCommission
                = priceCalculationDto.OfferDto.ItemVatAmountWithCommission = priceCalculationDto.OfferDto.ItemTotalAmountWithCommission 
                = priceCalculationDto.OfferDto.TotalAmountWithCommission = priceCalculationDto.OfferDto.ItemSubTotalAmountWithCommission
                = priceCalculationDto.OfferDto.SubTotalAmountWithCommission = priceCalculationDto.OfferDto.DetailsTotalPricePostCommissionPreVat
                =priceCalculationDto.OfferDto.DetailsTotalVatPostCommission = priceCalculationDto.OfferDto.ItemsTotalPricePostCommissionPreVat
                =priceCalculationDto.OfferDto.ItemsTotalVatPostCommission
                = 0;
            }
            else if(await _featureChecker.IsEnabledAsync(AppFeatures.Shipper))
            {
                priceCalculationDto.OfferDto.ItemPrice = priceCalculationDto.OfferDto.DetailsTotalPricePreCommissionPreVat
                = priceCalculationDto.OfferDto.DetailsTotalVatAmountPreCommission = priceCalculationDto.OfferDto.ItemsTotalPricePreCommissionPreVat
                = priceCalculationDto.OfferDto.ItemsTotalVatAmountPreCommission = 0;
            }
            return priceCalculationDto;
        }
        
        public async Task<CreateShippingRequestDirectRequestInput> GetDirectRequestToHandleByPricePackage(long pricePackageId, long shippingRequestId)
         {
             DisableTenancyFilters();

             var priceCalculationDto = await GetPricePackageForPriceCalculation(pricePackageId, shippingRequestId);

             int carrierTenantId = await (from pp in _pricePackageRepository.GetAll()
                     where pp.Id == pricePackageId
                     && pp.UsageType == PricePackageUsageType.AsCarrier
                         select pp.TenantId ).SingleAsync();

             return new CreateShippingRequestDirectRequestInput
             {
                 CarrierTenantId = carrierTenantId,
                 ShippingRequestId = shippingRequestId,
                 PriceCalculationDto = priceCalculationDto
             };
         }
        
        public async Task<ShippingRequestDirectRequestStatus> AcceptPricePackageOffer(long pricePackageOfferId)
         {
            // DisableTenancyFilters();
             // var pricePackageOffer = await _pricePackageRepository.FirstOrDefaultAsync(pricePackageOfferId);
             // if (pricePackageOffer == null) throw new UserFriendlyException(L("ThePricePackageOfferDoesNotExist"));
             //
             // var directRequest = await GetDirectRequestByPricePackageOfferId(pricePackageOfferId);
             // if (directRequest == null) throw new UserFriendlyException(L("TheDirectRequestDoesNotExist"));
             //
             // if (directRequest.ShippingRequestFK.Status != ShippingRequestStatus.PrePrice && directRequest.ShippingRequestFK.Status != ShippingRequestStatus.NeedsAction)
             //     throw new UserFriendlyException(L("YouCannotAcceptTheOffer"));
             //
             // directRequest.Status = ShippingRequestDirectRequestStatus.Accepted;
             // await CloseOtherDirectRequests(directRequest.ShippingRequestId, directRequest.Id);
             //
             // if (directRequest.ShippingRequestFK.IsTachyonDeal)
             // {
             //     await AcceptTMSOffer(pricePackageOffer, directRequest);
             //     await _appNotifier.CarrierAcceptPricePackageOffer(directRequest.TenantId, directRequest.Carrier.Name, directRequest.ShippingRequestFK.ReferenceNumber, directRequest.ShippingRequestId);
             //     return directRequest.Status;
             // }
             //
             // directRequest.ShippingRequestFK.Status = ShippingRequestStatus.PostPrice;
             // directRequest.ShippingRequestFK.CarrierTenantId = pricePackageOffer.TenantId;
             // SetShippingRequestPricing(directRequest.ShippingRequestFK, pricePackageOffer);
             // await _balanceManager.ShipperCanAcceptOffer(pricePackageOffer, pricePackageOffer.TaxVat, directRequest.ShippingRequestId);
             // await _appNotifier.CarrierAcceptPricePackageOffer(directRequest.TenantId, directRequest.Carrier.Name, directRequest.ShippingRequestFK.ReferenceNumber, directRequest.ShippingRequestId);
             // return directRequest.Status;
             return ShippingRequestDirectRequestStatus.Accepted; // todo rewrite this method
         }
        
        private IQueryable<PricePackage> MatchingPricePackageQuery(long? truckType, int? originCityId, int? destinationCityId, int? carrierId = null)
         {
             DisableTenancyFilters();
             return _pricePackageRepository.GetAll()
                 .WhereIf(carrierId.HasValue, c => c.TenantId == carrierId.Value)
                 .Where(x => x.TruckTypeId == truckType && x.OriginCityId == originCityId && x.DestinationCityId == destinationCityId);
         }

        #region Helpers
        
        
        
        private async Task SetVasNames(long shippingRequestId, PriceOfferDto offerDto)
        {
            var vases = await _shippingRequestVasRepository
                .GetAll().Include(x => x.VasFk)
                .Where(v => v.ShippingRequestId == shippingRequestId)
                .AsNoTracking()
                .ToListAsync();

            if (vases?.Count >= 1 && offerDto.Items?.Count >= 1)
            {
                foreach (var vasItem in offerDto.Items)
                {
                    vasItem.ItemName = vases.FirstOrDefault(x => x.Id == vasItem.SourceId)?.VasFk?.Name ?? "";
                }
            }

        }

        private async Task<List<PriceOfferDetailDto>> CalculateShippingRequestVases(long shippingRequestId, int carrierTenantId)
        {
            var shippingRequestVases = await _shippingRequestVasRepository.GetAll()
                .Include(x => x.VasFk)
                .Where(v => v.ShippingRequestId == shippingRequestId).AsNoTracking().ToListAsync();

            var vasesIds = shippingRequestVases.Select(x => x.VasId).ToList();

            var pricedVases = await _vasPriceRepository.GetAll()
                .Where(v => v.TenantId == carrierTenantId
                            && vasesIds.Contains(v.VasId)).AsNoTracking().ToListAsync();

            var res = new List<PriceOfferDetailDto>();

            foreach (var vas in shippingRequestVases)
            {
                var pricedVas = pricedVases.FirstOrDefault(x => x.VasId == vas.VasId);
                var price = 0;
                if (pricedVas != null) price = Convert.ToInt32(pricedVas.Price.HasValue ? (decimal)pricedVas.Price : 0);

                res.Add(new PriceOfferDetailDto { ItemId = vas.Id, Price = price });
            }
            return res;
        }
        
        private async Task HandleTmsPricePackage(PricePackage pricePackage,ShippingRequest request)
        {
            // the allowed P.P to use it in this method is the P.P that have price package usage `AsTachyonManageService`
            // when price package usage type is `AsTms` the `DestinationTenantId` must have a value
            if (pricePackage.DestinationTenantId is null)
                throw new UserFriendlyException(L("PricePackageMustHaveDestinationTenant"));
            

            if (pricePackage.ProposalId.HasValue)
            {
                await SendOfferToShipperByPricePackage(request, pricePackage);
                await CurrentUnitOfWork.SaveChangesAsync();
                await AcceptOfferByPricePackage(pricePackage.Id,request.Id);

            } else if (pricePackage.AppendixId.HasValue)
            {
                // send direct request to carrier by price package

                await SendDirectRequestToCarrierByPricePackage(pricePackage, request.Id);
            }
            else throw new UserFriendlyException(L("PricePackageMustHaveAppendixOrProposal"));


        }

        private async Task SendDirectRequestToCarrierByPricePackage(PricePackage pricePackage, long requestId)
        {

            var createDirectRequestInput = new CreateShippingRequestDirectRequestInput()
            { ShippingRequestId = requestId };

            createDirectRequestInput.CarrierTenantId = pricePackage.TenantId;
            
            var directRequestId = await _directRequestAppService.Create(createDirectRequestInput);
               
            var createdPricePackageOffer = new PricePackageOffer()
            {
                DirectRequestId = directRequestId,
                PricePackageId = pricePackage.Id
            };
            
            await _pricePackageOfferRepository.InsertAsync(createdPricePackageOffer);
        }
        private async Task SendOfferToShipperByPricePackage(ShippingRequest shippingRequest, PricePackage pricePackage)
        {
            // this method used only for p.p that created by Tachyon (check type and validate it)
            if (pricePackage.UsageType != PricePackageUsageType.AsTachyonManageService)
                throw new UserFriendlyException(L("SendingAnOfferRequireAPricePackageCreatedByTms"));
            
            DisableTenancyFilters();
            var itemDetails = shippingRequest.ShippingRequestVases?
                .Select(item => new PriceOfferDetailDto() { ItemId = item.Id, Price = 0 }).ToList();

            var parentOfferId = await GetParentOfferId(shippingRequest.Id);

           if (parentOfferId == default)
               throw new UserFriendlyException(L("YouMustSendRequestToCarrierAndAcceptTheOffer"));

           var priceOfferDto = new CreateOrEditPriceOfferInput()
           { 
               ShippingRequestId = shippingRequest.Id, 
               ItemPrice = pricePackage.TotalPrice,
               ItemDetails = itemDetails,
               CommissionType = PriceOfferCommissionType.CommissionValue,
               CommissionPercentageOrAddValue = 0,
               ParentId = parentOfferId,
               VasCommissionType = PriceOfferCommissionType.CommissionValue, VasCommissionPercentageOrAddValue = 0
           };
            var offerId = await _priceOfferManager.CreateOrEdit(priceOfferDto);

            var createdPricePackageOffer = new PricePackageOffer()
            {
                PriceOfferId = offerId, PricePackageId = pricePackage.Id
            };

            await _pricePackageOfferRepository.InsertAsync(createdPricePackageOffer);
        }

        public IQueryable<long> GetParentOfferIdAsQueryable(long shippingRequestId) 
        {
            return (from priceOffer in _priceOfferRepository.GetAll()
                where priceOffer.ShippingRequestId == shippingRequestId
                      && (priceOffer.Status == PriceOfferStatus.Accepted || priceOffer.Status == PriceOfferStatus.Pending
                                                                         || priceOffer.Status == PriceOfferStatus.AcceptedAndWaitingForCarrier ||
                                                                         priceOffer.Status == PriceOfferStatus.AcceptedAndWaitingForShipper)
                      && _pricePackageOfferRepository.GetAll()
                          .Any(x => x.DirectRequestId.HasValue &&
                                    x.DirectRequest.ShippingRequestId == shippingRequestId &&
                                    x.DirectRequest.CarrierTenantId == priceOffer.TenantId)
                select priceOffer.Id); 
            
        }
        public async Task<long> GetParentOfferId(long shippingRequestId)
        {
           return await (from priceOffer in _priceOfferRepository.GetAll()
                where priceOffer.ShippingRequestId == shippingRequestId
                      && (priceOffer.Status == PriceOfferStatus.Accepted || priceOffer.Status == PriceOfferStatus.Pending
                          || priceOffer.Status == PriceOfferStatus.AcceptedAndWaitingForCarrier ||
                          priceOffer.Status == PriceOfferStatus.AcceptedAndWaitingForShipper)
                      && _pricePackageOfferRepository.GetAll()
                          .Any(x => x.DirectRequestId.HasValue &&
                                    x.DirectRequest.ShippingRequestId == shippingRequestId &&
                                    x.DirectRequest.CarrierTenantId == priceOffer.TenantId)
                select priceOffer.Id).FirstOrDefaultAsync();
        }

        private async Task AcceptOfferByPricePackage(long pricePackageId,long srId)
        {
            var priceOffer = await _pricePackageOfferRepository.GetAll()
                .Where(x => x.PricePackageId == pricePackageId && x.PriceOffer.ShippingRequestId == srId)
                .Select(x => x.PriceOfferId).FirstOrDefaultAsync();
           
            if (!priceOffer.HasValue) throw new UserFriendlyException(L("ThereIsNoOfferInThisPricePackage"));
            
            await _priceOfferManager.AcceptOfferOnBehalfShipper(priceOffer.Value);
        }
        #endregion
    }
}