using Abp.Authorization.Users;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Runtime.Session;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using TACHYON.Authorization.Users;
using TACHYON.Configuration;
using TACHYON.PriceOffers;
using TACHYON.PriceOffers.Dto;
using TACHYON.PricePackages.TmsPricePackages;
using TACHYON.Shipping.DirectRequests;
using TACHYON.Shipping.DirectRequests.Dto;
using TACHYON.Shipping.ShippingRequests;

namespace TACHYON.PricePackages.TmsPricePackageOffers
{
    internal class TmsPricePackageOfferManager : TACHYONDomainServiceBase ,ITmsPricePackageOfferManager
    {
        private readonly IRepository<TmsPricePackageOffer,long> _tmsOfferRepository;
        private readonly IRepository<ShippingRequest, long> _shippingRequestRepository;
        private readonly IRepository<TmsPricePackage> _tmsPricePackageRepository;
        private readonly IRepository<NormalPricePackage> _normalPricePackageRepository;
        private readonly IShippingRequestDirectRequestAppService _directRequestAppService;
        private readonly PriceOfferManager _priceOfferManager;
        private readonly IRepository<PriceOffer, long> _priceOfferRepository;
        

        public TmsPricePackageOfferManager(
            IRepository<TmsPricePackageOffer, long> tmsOfferRepository,
            IRepository<ShippingRequest, long> shippingRequestRepository,
            IRepository<TmsPricePackage> tmsPricePackageRepository,
            IRepository<NormalPricePackage> normalPricePackageRepository,
            IShippingRequestDirectRequestAppService directRequestAppService,
            PriceOfferManager priceOfferManager,
            IRepository<PriceOffer, long> priceOfferRepository)
        {
            _tmsOfferRepository = tmsOfferRepository;
            _shippingRequestRepository = shippingRequestRepository;
            _tmsPricePackageRepository = tmsPricePackageRepository;
            _normalPricePackageRepository = normalPricePackageRepository;
            _directRequestAppService = directRequestAppService;
            _priceOfferManager = priceOfferManager;
            _priceOfferRepository = priceOfferRepository;
        }
        
        
        
        
        
        /// <summary>
        /// this method used for apply the pricing that the shipper/carrier agree it by price package
        /// the stage can be use after price package appendix is confirmed 
        /// </summary>
        /// <param name="pricePackageId"></param>
        /// <param name="srId"></param>
        /// <param name="isTmsPricePackage"></param>
        /// <exception cref="UserFriendlyException"></exception>
        public async Task ApplyPricingOnShippingRequest(int pricePackageId,long srId,bool isTmsPricePackage)
        {
            DisableTenancyFilters();

            var shippingRequest = await _shippingRequestRepository.GetAllIncluding(x=> x.ShippingRequestVases)
                .AsNoTracking().SingleAsync(x => x.Id == srId);
            
            if (isTmsPricePackage)
            {
                TmsPricePackage pricePackage = await _tmsPricePackageRepository.SingleAsync(x=> x.Id == pricePackageId);

                await HandleTmsPricePackage(pricePackage,shippingRequest);
                return;
            }

            // handle normal price package
            var normalPricePackage = await _normalPricePackageRepository.SingleAsync(x => x.Id == pricePackageId);



            if (!normalPricePackage.AppendixId.HasValue)
                throw new UserFriendlyException(L("PricePackageMustHaveAppendixToSendRequest"));

            await SendDirectRequestToCarrierByPricePackage(normalPricePackage, shippingRequest.Id);

        }

        
        public async Task<bool> HasDirectRequestByPricePackage(long shippingRequestId)
        {
            DisableTenancyFilters();
            return await _tmsOfferRepository.GetAll().AnyAsync(x => x.DirectRequestId.HasValue && x.DirectRequest.ShippingRequestId == shippingRequestId);
        }


        #region Helpers

        private async Task HandleTmsPricePackage(TmsPricePackage pricePackage,ShippingRequest request)
        {
            
            if (pricePackage.DestinationTenantId is null)
                throw new UserFriendlyException(L("PricePackageMustHaveDestinationTenant"));
            
            

            if (pricePackage.ProposalId.HasValue)
            {
                await SendOfferToShipperByPricePackage(request, pricePackage);
                await CurrentUnitOfWork.SaveChangesAsync();
                await AcceptOfferByPricePackage(pricePackage,request.Id);

            } else if (pricePackage.AppendixId.HasValue)
            {
                // send direct request to carrier by price package

                await SendDirectRequestToCarrierByPricePackage(pricePackage, request.Id);
            }
            else throw new UserFriendlyException(L("PricePackageMustHaveAppendixOrProposal"));


        }

        private async Task SendDirectRequestToCarrierByPricePackage(BasePricePackage pricePackage, long requestId)
        {
            bool isTmsPricePackage = pricePackage is TmsPricePackage;

            var createDirectRequestInput = new CreateShippingRequestDirectRequestInput()
            { ShippingRequestId = requestId };
            
            if (pricePackage is TmsPricePackage tmsPricePackage)
            {
                if (!tmsPricePackage.DestinationTenantId.HasValue)
                    throw new UserFriendlyException(L("PricePackageMustHaveDestinationCompany"));

                createDirectRequestInput.CarrierTenantId = tmsPricePackage.DestinationTenantId.Value;
            }
            else createDirectRequestInput.CarrierTenantId = pricePackage.TenantId;
            
            var directRequestId = await _directRequestAppService.Create(createDirectRequestInput);
               
            var createdPricePackageOffer = new TmsPricePackageOffer()
            {
                DirectRequestId = directRequestId
            };
            
            if (isTmsPricePackage)
                createdPricePackageOffer.TmsPricePackageId = pricePackage.Id;
            else createdPricePackageOffer.NormalPricePackageId = pricePackage.Id;

            await _tmsOfferRepository.InsertAsync(createdPricePackageOffer);
        }
        private async Task SendOfferToShipperByPricePackage(ShippingRequest shippingRequest,
            TmsPricePackage pricePackage)
        {
            DisableTenancyFilters();
            var itemDetails = shippingRequest.ShippingRequestVases?
                .Select(item => new PriceOfferDetailDto() { ItemId = item.Id, Price = 0 }).ToList();


            var parentPriceOffer = await GetParentOffer(shippingRequest.Id);

           if (parentPriceOffer == null)
               throw new UserFriendlyException(L("YouMustSendRequestToCarrierAndAcceptTheOffer"));
                
            var priceOfferDto = new CreateOrEditPriceOfferInput()
            {
                ShippingRequestId = shippingRequest.Id, 
                ItemPrice = parentPriceOffer.ItemPrice,
                ItemDetails = itemDetails,
                CommissionType = PriceOfferCommissionType.CommissionValue,
                CommissionPercentageOrAddValue = pricePackage.TotalPrice - parentPriceOffer.ItemPrice,
                ParentId = parentPriceOffer.Id,
                VasCommissionType = PriceOfferCommissionType.CommissionValue, VasCommissionPercentageOrAddValue = 0
            };

            var offerId = await _priceOfferManager.CreateOrEdit(priceOfferDto);

            var createdPricePackageOffer = new TmsPricePackageOffer()
            {
                PriceOfferId = offerId, TmsPricePackageId = pricePackage.Id
            };

            await _tmsOfferRepository.InsertAsync(createdPricePackageOffer);
        }

        public async Task<long> GetParentOfferId(long shippingRequestId)
        {
           return await (from priceOffer in _priceOfferRepository.GetAll()
                where priceOffer.ShippingRequestId == shippingRequestId
                      && (priceOffer.Status == PriceOfferStatus.Accepted || priceOffer.Status == PriceOfferStatus.Pending || priceOffer.Status == PriceOfferStatus.Pending
                          || priceOffer.Status == PriceOfferStatus.AcceptedAndWaitingForCarrier ||
                          priceOffer.Status == PriceOfferStatus.AcceptedAndWaitingForShipper)
                      && _tmsOfferRepository.GetAll()
                          .Any(x => x.DirectRequestId.HasValue &&
                                    x.DirectRequest.ShippingRequestId == shippingRequestId &&
                                    x.DirectRequest.CarrierTenantId == priceOffer.TenantId)
                select priceOffer.Id).FirstOrDefaultAsync();
        }

        public async Task<PriceOffer> GetParentOffer(long shippingRequestId)
        {
            return await (from priceOffer in _priceOfferRepository.GetAll()
                          where priceOffer.ShippingRequestId == shippingRequestId
                                && (priceOffer.Status == PriceOfferStatus.Accepted || priceOffer.Status == PriceOfferStatus.Pending || priceOffer.Status == PriceOfferStatus.Pending
                                    || priceOffer.Status == PriceOfferStatus.AcceptedAndWaitingForCarrier ||
                                    priceOffer.Status == PriceOfferStatus.AcceptedAndWaitingForShipper)
                                && _tmsOfferRepository.GetAll()
                                    .Any(x => x.DirectRequestId.HasValue &&
                                              x.DirectRequest.ShippingRequestId == shippingRequestId &&
                                              x.DirectRequest.CarrierTenantId == priceOffer.TenantId)
                          select priceOffer).FirstOrDefaultAsync();
        }

        private async Task AcceptOfferByPricePackage(TmsPricePackage pricePackage,long srId)
        {
            var priceOffer = await _tmsOfferRepository.GetAll()
                .Where(x => x.TmsPricePackageId == pricePackage.Id && x.PriceOffer.ShippingRequestId == srId)
                .Select(x => x.PriceOfferId).FirstOrDefaultAsync();
           
            if (!priceOffer.HasValue) throw new UserFriendlyException(L("ThereIsNoOfferInThisPricePackage"));
            
            await _priceOfferManager.AcceptOfferOnBehalfShipper(priceOffer.Value);
        }
        #endregion
    }
}