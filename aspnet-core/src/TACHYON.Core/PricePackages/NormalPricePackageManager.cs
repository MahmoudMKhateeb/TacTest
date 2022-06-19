using Abp.Application.Features;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Abp.Runtime.Session;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using NUglify.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TACHYON.Features;
using TACHYON.Invoices.Balances;
using TACHYON.Notifications;
using TACHYON.PriceOffers;
using TACHYON.PriceOffers.Dto;
using TACHYON.PricePackages.Dto.NormalPricePackage;
using TACHYON.Shipping.DirectRequests;
using TACHYON.Shipping.DirectRequests.Dto;
using TACHYON.Shipping.ShippingRequests;
using TACHYON.ShippingRequestVases;
using TACHYON.Vases;

namespace TACHYON.PricePackages
{
    public class NormalPricePackageManager : TACHYONDomainServiceBase
    {
        private readonly IRepository<NormalPricePackage> _normalPricePackageRepository;
        private readonly IRepository<ShippingRequest, long> _shippingRequestRepository;
        private readonly IRepository<ShippingRequestDirectRequest, long> _directShippingRequestRepository;
        private readonly IRepository<ShippingRequestVas, long> _shippingRequestVasRepository;
        private readonly IRepository<PriceOffer, long> _priceOfferRepository;
        private readonly IRepository<VasPrice> _priceVasRepository;
        private readonly IAppNotifier _appNotifier;
        private readonly IFeatureChecker _featureChecker;
        private readonly IAbpSession _abpSession;
        private readonly PriceOfferManager _priceOfferManager;
        private readonly IRepository<PricePackageOffer, long> _pricePackageOfferRepository;
        private readonly BalanceManager _balanceManager;


        public NormalPricePackageManager(
            IRepository<NormalPricePackage> normalPricePackageRepository,
            IRepository<ShippingRequest, long> shippingRequestRepository,
            IRepository<ShippingRequestVas, long> shippingRequestVasRepository,
            IRepository<VasPrice> priceVasRepository,
            PriceOfferManager priceOfferManager,
            IRepository<PricePackageOffer, long> pricePackageOfferRepository,
            IRepository<ShippingRequestDirectRequest, long> directShippingRequestRepository,
            BalanceManager balanceManager,
            IRepository<PriceOffer, long> priceOfferRepository,
            IAppNotifier appNotifier,
            IFeatureChecker featureChecker,
            IAbpSession abpSession)
        {
            _normalPricePackageRepository = normalPricePackageRepository;
            _shippingRequestRepository = shippingRequestRepository;
            _shippingRequestVasRepository = shippingRequestVasRepository;
            _priceVasRepository = priceVasRepository;
            _priceOfferManager = priceOfferManager;
            _pricePackageOfferRepository = pricePackageOfferRepository;
            _directShippingRequestRepository = directShippingRequestRepository;
            _balanceManager = balanceManager;
            _priceOfferRepository = priceOfferRepository;
            _appNotifier = appNotifier;
            _featureChecker = featureChecker;
            _abpSession = abpSession;
        }

        #region Main Functions
        /// <summary>
        /// Get Price Package Offer by Id for preview
        /// </summary>
        public async Task<PricePackageOfferDto> GetPricePackageOffer(int pricePackageOfferId, long shippingRequestId)
        {
            DisableTenancyFilters();

            var pricePackageOffer = await _pricePackageOfferRepository
                .GetAllIncluding(x => x.Items, v => v.TrucksTypeFk, b => b.DestinationCityFK, b => b.OriginCityFK)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == pricePackageOfferId);

            if (pricePackageOffer == null) throw new UserFriendlyException(L("ThePricePackageOfferDoesNotExist"));

            var pricePackageOfferDto = ObjectMapper.Map<PricePackageOfferDto>(pricePackageOffer);

            pricePackageOfferDto.Origin = pricePackageOffer.OriginCityFK.DisplayName;
            pricePackageOfferDto.Destination = pricePackageOffer.DestinationCityFK.DisplayName;
            pricePackageOfferDto.TruckType = pricePackageOffer.TrucksTypeFk.DisplayName;

            await SetVasNames(shippingRequestId, pricePackageOfferDto);

            pricePackageOfferDto.HasDirectRequest = await _directShippingRequestRepository.GetAll()
               .AnyAsync(x => x.CarrierTenantId == pricePackageOfferDto.TenantId && x.ShippingRequestId == shippingRequestId);

            return pricePackageOfferDto;
        }

        /// <summary>
        /// Create direct request input for handling
        /// </summary>
        public async Task<CreateShippingRequestDirectRequestInput> GetDirectRequestToHandleByPricePackage(int pricePackageId, long shippingRequestId)
        {
            DisableTenancyFilters();

            var pricePackageBidDto = await GetPricePackageOfferDto(pricePackageId, shippingRequestId);

            var carrierTenantId = await _normalPricePackageRepository.GetAll()
                .Where(x => x.Id == pricePackageId)
                .Select(x => x.TenantId)
                .FirstOrDefaultAsync();

            return new CreateShippingRequestDirectRequestInput
            {
                CarrierTenantId = carrierTenantId,
                ShippingRequestId = shippingRequestId,
                BidNormalPricePackage = pricePackageBidDto
            };
        }

        /// <summary>
        /// Calculate shipping request based on price package and generate price offer dto for viewing or handling
        /// </summary>
        public async Task<PricePackageOfferDto> GetPricePackageOfferDto(int pricePackageId, long shippingRequestId)
        {
            DisableTenancyFilters();

            var priceOfferInput = await GeneratePriceOfferInput(pricePackageId, shippingRequestId);
            var priceOffer = await _priceOfferManager.InitPriceOffer(priceOfferInput);
            var pricePackageOfferDto = ObjectMapper.Map<PricePackageOfferDto>(priceOffer);
            //get  price package
            var pricePackage = await GetNormalPricePackage(pricePackageId);
            //get shipping request 
            var shippingRequest = await GetShippingRequest(shippingRequestId);

            ObjectMapper.Map(pricePackage, pricePackageOfferDto);
            pricePackageOfferDto.NumberOfDrops = shippingRequest.NumberOfDrops;
            pricePackageOfferDto.NumberOfTrips = shippingRequest.NumberOfTrips;

            await SetVasNames(shippingRequestId, pricePackageOfferDto);

            pricePackageOfferDto.HasDirectRequest = await _directShippingRequestRepository.GetAll()
                .AnyAsync(x => x.CarrierTenantId == pricePackageOfferDto.TenantId && x.ShippingRequestId == shippingRequestId);

            return pricePackageOfferDto;
        }

        /// <summary>
        /// Accept Price Package Offer from Carrier or Tachyon Dealer
        /// </summary>
        public async Task<ShippingRequestDirectRequestStatus> AcceptPricePackageOffer(int pricePackageOfferId)
        {
            DisableTenancyFilters();
            var pricePackageOffer = await GetPricePackageOfferById(pricePackageOfferId);
            if (pricePackageOffer == null) throw new UserFriendlyException(L("ThePricePackageOfferDoesNotExist"));

            var directRequest = await GetDirectRequestByPricePackageOfferId(pricePackageOfferId);
            if (directRequest == null) throw new UserFriendlyException(L("TheDirectRequestDoesNotExist"));

            if (directRequest.ShippingRequestFK.Status != ShippingRequestStatus.PrePrice && directRequest.ShippingRequestFK.Status != ShippingRequestStatus.NeedsAction)
                throw new UserFriendlyException(L("YouCannotAcceptTheOffer"));

            directRequest.Status = ShippingRequestDirectRequestStatus.Accepted;
            await CloseOtherDirectRequests(directRequest.ShippingRequestId, directRequest.Id);

            if (directRequest.ShippingRequestFK.IsTachyonDeal)
            {
                await AcceptTMSOffer(pricePackageOffer, directRequest);
                await _appNotifier.CarrierAcceptPricePackageOffer(directRequest.TenantId, directRequest.Carrier.Name, directRequest.ShippingRequestFK.ReferenceNumber, directRequest.ShippingRequestId);
                return directRequest.Status;
            }

            directRequest.ShippingRequestFK.Status = ShippingRequestStatus.PostPrice;
            directRequest.ShippingRequestFK.CarrierTenantId = pricePackageOffer.TenantId;
            SetShippingRequestPricing(directRequest.ShippingRequestFK, pricePackageOffer);
            await _balanceManager.ShipperCanAcceptOffer(pricePackageOffer, pricePackageOffer.TaxVat, directRequest.ShippingRequestId);
            await _appNotifier.CarrierAcceptPricePackageOffer(directRequest.TenantId, directRequest.Carrier.Name, directRequest.ShippingRequestFK.ReferenceNumber, directRequest.ShippingRequestId);
            return directRequest.Status;
        }

        /// <summary>
        /// Get Carriers Matching Price Packages to send notfication to them
        /// </summary>
        public async Task<List<CarrierPricePackageDto>> GetCarriersMatchingPricePackages(long? truckType, int? originCityId, int? destinationCityId)
        {
            DisableTenancyFilters();
            var query = MatchingPricePackageQuery(truckType, originCityId, destinationCityId);
            return await query.Select(c => new CarrierPricePackageDto { CarrierTenantId = c.TenantId, PricePackageReferance = c.PricePackageId }).Distinct().ToListAsync();
        }

        /// <summary>
        /// Get Matching Price Package Id based on truckType, originCityId and destinationCityId
        /// </summary>
        public async Task<int?> GetMatchingPricePackageId(long? truckType, int? originCityId, int? destinationCityId, int? CarrierId = null)
        {
            var query = MatchingPricePackageQuery(truckType, originCityId, destinationCityId, CarrierId);
            return await query.Select(x => x.Id).FirstOrDefaultAsync();
        }

        /// <summary>
        ///send price offer based on price package prices -- used with marketplace request --
        /// </summary>
        public async Task<long> SendPriceOfferByPricePackage(int pricePackageId, long shippingRequestId)
        {
            DisableTenancyFilters();
            var priceOfferInput = await GeneratePriceOfferInput(pricePackageId, shippingRequestId);
            return await _priceOfferManager.CreateOrEdit(priceOfferInput);
        }

        /// <summary>
        ///get accepted price package offer based on shipping request Id
        /// </summary>
        public async Task<PricePackageOffer> GetOfferByShippingRequestId(long shippingRequestId)
        {
            return await _directShippingRequestRepository.GetAll()
                  .Include(x => x.PricePackageOfferFK)
                  .ThenInclude(x => x.Items)
                  .Where(x => x.ShippingRequestId == shippingRequestId && x.Status == ShippingRequestDirectRequestStatus.Accepted)
                  .Select(x => x.PricePackageOfferFK).FirstOrDefaultAsync();
        }
        #endregion

        #region Helpers
        private async Task<ShippingRequestDirectRequest> GetDirectRequestByPricePackageOfferId(int pricePackageOfferId)
        {
            return await _directShippingRequestRepository.GetAllIncluding(x => x.ShippingRequestFK, b => b.Carrier)
                .Where(x => x.PricePackageOfferId == pricePackageOfferId && x.Status == ShippingRequestDirectRequestStatus.New)
                .WhereIf(_abpSession.TenantId.HasValue && await _featureChecker.IsEnabledAsync(AppFeatures.Carrier), x => x.CarrierTenantId == _abpSession.TenantId)
                .WhereIf(_abpSession.TenantId.HasValue && await _featureChecker.IsEnabledAsync(AppFeatures.Shipper), x => x.TenantId == _abpSession.TenantId)
                .WhereIf(!_abpSession.TenantId.HasValue || await _featureChecker.IsEnabledAsync(AppFeatures.TachyonDealer), x => true)
                .FirstOrDefaultAsync();
        }
        private async Task<PricePackageOffer> GetPricePackageOfferById(int pricePackageOfferId)
        {
            return await _pricePackageOfferRepository.GetAllIncluding(x => x.Items)
                .WhereIf(_abpSession.TenantId.HasValue && await _featureChecker.IsEnabledAsync(AppFeatures.Carrier), x => x.NormalPricePackageFK.TenantId == _abpSession.TenantId)
                .WhereIf(_abpSession.TenantId.HasValue && await _featureChecker.IsEnabledAsync(AppFeatures.Shipper), x => x.TenantId == _abpSession.TenantId)
                .WhereIf(!_abpSession.TenantId.HasValue || await _featureChecker.IsEnabledAsync(AppFeatures.TachyonDealer), x => true)
                .Where(x => x.Id == pricePackageOfferId)
                .FirstOrDefaultAsync();
        }
        private IQueryable<NormalPricePackage> MatchingPricePackageQuery(long? truckType, int? originCityId, int? destinationCityId, int? CarrierId = null)
        {
            return _normalPricePackageRepository.GetAll()
                .WhereIf(CarrierId.HasValue, c => c.TenantId == CarrierId.Value)
                .Where(x => x.TrucksTypeId == truckType && x.OriginCityId == originCityId && x.DestinationCityId == destinationCityId);
        }

        /// <summary>
        /// Accept Price Package Offer from Tachyon Dealer
        /// </summary>
        private async Task AcceptTMSOffer(PricePackageOffer pricePackageOffer, ShippingRequestDirectRequest directRequest)
        {
            var priceOffer = ObjectMapper.Map<PriceOffer>(pricePackageOffer);
            priceOffer.Id = 0;

            if (priceOffer.PriceOfferDetails != null && priceOffer.PriceOfferDetails.Any())
                priceOffer.PriceOfferDetails.ForEach(x => x.Id = 0);

            priceOffer.ShippingRequestId = directRequest.ShippingRequestId;
            var accepted = await CheckIfThereOfferAcceptedBefore(priceOffer.ShippingRequestId);

            priceOffer.Status = accepted ? PriceOfferStatus.New : PriceOfferStatus.Pending;
            directRequest.ShippingRequestFK.Status = ShippingRequestStatus.NeedsAction;
            await _priceOfferRepository.InsertAndGetIdAsync(priceOffer);

            await HandleTachyonDealerOffer(priceOffer);

            await CloseOtherDirectRequests(directRequest.ShippingRequestId, directRequest.Id);
            await _appNotifier.CarrierAcceptPricePackageOffer(directRequest.TenantId, directRequest.Carrier.Name, directRequest.ShippingRequestFK.ReferenceNumber, directRequest.ShippingRequestId);
        }
        /// <summary>
        /// check if there is submited offer from TD to Shipper and handle it
        /// </summary>
        private async Task HandleTachyonDealerOffer(PriceOffer priceOffer)
        {
            var tadOffer = await _priceOfferRepository
                   .GetAll()
                   .Where(x => x.ShippingRequestId == priceOffer.ShippingRequestId &&
                   x.Tenant.EditionId == TachyonEditionId &&
                   (x.Status == PriceOfferStatus.New || x.Status == PriceOfferStatus.Rejected))
                  .FirstOrDefaultAsync();


            if (tadOffer != null) /// If TMS send offer before accept carrier price then related offers togother
            {
                tadOffer.ParentId = priceOffer.Id;
                tadOffer.Status = PriceOfferStatus.AcceptedAndWaitingForShipper;

            }
        }
        /// <summary>
        /// Check If There Offer Accepted Before to prevent multiple  accepted offers
        /// </summary>
        private async Task<bool> CheckIfThereOfferAcceptedBefore(long shippingRequestId)
        {
            return await _priceOfferRepository.GetAll()
                             .AnyAsync(x => x.ShippingRequestId == shippingRequestId
                             && (x.Status == PriceOfferStatus.AcceptedAndWaitingForShipper
                             || x.Status == PriceOfferStatus.Pending));
        }
        /// <summary>
        /// Get Vases from Db and set names into DTO 
        /// </summary>
        private async Task SetVasNames(long shippingRequestId, PricePackageOfferDto pricePackageOfferDto)
        {
            var vases = await _shippingRequestVasRepository
                       .GetAll().Include(x => x.VasFk)
                       .Where(v => v.ShippingRequestId == shippingRequestId)
                       .AsNoTracking()
                       .ToListAsync();

            if (vases != null && vases.Any() && pricePackageOfferDto.Items != null && pricePackageOfferDto.Items.Any())
            {
                foreach (var vasItem in pricePackageOfferDto.Items)
                {
                    vasItem.ItemName = vases.FirstOrDefault(x => x.Id == vasItem.SourceId)?.VasFk?.Name ?? "";
                }
            }

        }
        /// <summary>
        /// when carrier accept the price package offer we need to close other direct requests
        /// </summary>
        private async Task CloseOtherDirectRequests(long shippingRequestId, long direcRequestId)
        {
            var directRequests = await _directShippingRequestRepository
                .GetAll().Where(x => x.ShippingRequestId == shippingRequestId
                            && x.Id != direcRequestId
                            && x.Status == ShippingRequestDirectRequestStatus.New).ToListAsync();

            if (directRequests != null && directRequests.Any())
                directRequests.ForEach(x => x.Status = ShippingRequestDirectRequestStatus.Closed);
        }
        /// <summary>
        /// Take Pricec from Price Package Offer an set it into shipping request
        /// </summary>
        private void SetShippingRequestPricing(ShippingRequest request, PricePackageOffer pricePackageOffer)
        {
            request.Price = pricePackageOffer.TotalAmountWithCommission;
            request.SubTotalAmount = pricePackageOffer.SubTotalAmountWithCommission;
            request.VatAmount = pricePackageOffer.VatAmountWithCommission;
            request.VatSetting = pricePackageOffer.TaxVat;
            request.TotalCommission = pricePackageOffer.TotalAmountWithCommission;
            request.CarrierPrice = pricePackageOffer.TotalAmount;
        }
        /// <summary>
        /// Get Normal Price Package from DB
        /// </summary>
        private async Task<NormalPricePackage> GetNormalPricePackage(int pricePackageId)
        {
            var pricePackage = await _normalPricePackageRepository
                .GetAllIncluding(x => x.OriginCityFK, b => b.DestinationCityFK, t => t.TrucksTypeFk)
                .AsNoTracking()
                 .WhereIf(_abpSession.TenantId.HasValue && await _featureChecker.IsEnabledAsync(AppFeatures.Carrier), x => x.TenantId == _abpSession.TenantId)
                .FirstOrDefaultAsync(x => x.Id == pricePackageId);
            if (pricePackage == null) throw new UserFriendlyException(L("ThePricePackageDoesNotExist"));
            return pricePackage;
        }
        /// <summary>
        /// Get Shipping Request from DB
        /// </summary>
        private async Task<ShippingRequest> GetShippingRequest(long shippingRequestId)
        {
            var shippingRequest = await _shippingRequestRepository.GetAll()
                .Include(x => x.Tenant)
                .Include(x => x.ShippingRequestVases)
                .ThenInclude(x => x.VasFk)
                .AsNoTracking()
                .WhereIf(!_abpSession.TenantId.HasValue || await _featureChecker.IsEnabledAsync(AppFeatures.TachyonDealer), x => x.IsTachyonDeal)
                .FirstOrDefaultAsync(x => x.Id == shippingRequestId);
            if (shippingRequest == null) throw new UserFriendlyException(L("ThesShippingRequestDoesNotExist"));
            return shippingRequest;
        }
        /// <summary>
        /// Calculate Shipping Request Vases prices 
        /// </summary>
        private async Task<List<PriceOfferDetailDto>> CalculateShippingRequestVases(long shippingRequestId, int carrierTenantId)
        {
            var shippingRequestVases = await _shippingRequestVasRepository.GetAll()
                .Include(x => x.VasFk)
                .Where(v => v.ShippingRequestId == shippingRequestId).AsNoTracking().ToListAsync();

            var vasesIds = shippingRequestVases.Select(x => x.VasId).ToList();

            var pricedVases = await _priceVasRepository.GetAll()
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
        /// <summary>
        /// to determind from where get the price to calculate
        /// </summary>
        private decimal GetPriceByShippingRequestType(NormalPricePackage pricePackage, ShippingRequest shippingRequest)
        {
            if (shippingRequest.IsDirectRequest) return pricePackage.DirectRequestPrice;
            else if (shippingRequest.IsBid) return pricePackage.MarcketPlaceRequestPrice;
            else return pricePackage.TachyonMSRequestPrice;
        }
        /// <summary>
        /// Calculate trip Price based on price packge
        /// </summary>
        private decimal CalculateTripPrice(decimal price, int numberOfDrops, bool isMultiDrops, decimal? pricePerDrop)
        {
            var totalPrice = price;
            if (isMultiDrops && numberOfDrops > 1 && pricePerDrop.HasValue) return totalPrice + (numberOfDrops - 1) * pricePerDrop.Value;
            else return totalPrice;
        }
        /// <summary>
        /// Generate price offer input for handling shipping request based on price package
        /// we need this input to use calculation process that exists in the price offer manager
        /// </summary>
        private async Task<CreateOrEditPriceOfferInput> GeneratePriceOfferInput(int pricePackageId, long shippingRequestId)
        {
            //get  price package
            var pricePackage = await GetNormalPricePackage(pricePackageId);
            //get shipping request 
            var shippingRequest = await GetShippingRequest(shippingRequestId);
            var price = GetPriceByShippingRequestType(pricePackage, shippingRequest);

            var tripPrice = CalculateTripPrice(price, shippingRequest.NumberOfDrops, pricePackage.IsMultiDrop, pricePackage.PricePerExtraDrop);

            return new CreateOrEditPriceOfferInput
            {
                ShippingRequestId = shippingRequestId,
                ItemPrice = tripPrice,
                ItemDetails = await CalculateShippingRequestVases(shippingRequestId, pricePackage.TenantId),
            };
        }
        /// <summary>
        /// Check If Name Price Package Is Exist per tenant
        /// </summary>
        public async Task<bool> CheckIfNamePricePackageIsExist(string pricePackageName, int? id)
        {
            return await _normalPricePackageRepository.GetAll()
                .WhereIf(id.HasValue, c => c.Id != id)
                .AnyAsync(x => x.DisplayName.ToLower().Equals(pricePackageName.ToLower()));
        }
        /// <summary>
        /// Generate  price package referance number based on id and type and creation date
        /// </summary>
        public string GeneratePricePackageReferanceNumber(int id, bool isMultipleDrop, DateTime creationDate)
        {
            string routType = isMultipleDrop ? "MUL" : "SDR";
            string formatDate = creationDate.ToString("ddMMyy");
            var referanceId = id + 1000;
            var referanceNumber = "{0}-{1}-{2}";
            return string.Format(referanceNumber, formatDate, routType, referanceId);
        }
        #endregion
    }
}