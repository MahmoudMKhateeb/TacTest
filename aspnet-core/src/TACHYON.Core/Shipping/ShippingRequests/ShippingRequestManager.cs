using Abp.Application.Features;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Abp.Runtime.Session;
using Abp.Timing;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System;
using System.Globalization;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using TACHYON.Features;
using TACHYON.Goods.GoodCategories;
using TACHYON.Net.Sms;
using TACHYON.Notifications;
using TACHYON.Packing.PackingTypes;
using TACHYON.PriceOffers.Dto;
using TACHYON.PriceOffers;
using TACHYON.PricePackages;
using TACHYON.Routs.RoutPoints;
using TACHYON.Shipping.ShippingRequests.Dtos;
using TACHYON.Shipping.ShippingRequests.Dtos.Dedicated;
using TACHYON.ShippingRequestVases;
using TACHYON.Trucks.TruckCategories.TransportTypes;
using TACHYON.Trucks.TrucksTypes;
using TACHYON.Url;
using static TACHYON.TACHYONDashboardCustomizationConsts.Widgets;
using TACHYON.Shipping.DirectRequests.Dto;
using TACHYON.Shipping.DirectRequests;
using TACHYON.Shipping.Trips;
using TACHYON.Shipping.ShippingRequestTrips;
using TACHYON.Shipping.Dedicated;

namespace TACHYON.Shipping.ShippingRequests
{
    public class ShippingRequestManager : TACHYONDomainServiceBase
    {
        private readonly IRepository<RoutPoint, long> _routPointRepository;
        private readonly IRepository<ShippingRequest, long> _shippingRequestRepository;
        private readonly IRepository<ShippingRequestTrip> _shippingRequestTripRepository;
        private readonly IFeatureChecker _featureChecker;
        private readonly IAbpSession _abpSession;
        protected readonly IWebUrlService WebUrlService;
        private readonly IRepository<GoodCategory, int> _lookup_goodCategoryRepository;
        private readonly IRepository<TransportType, int> _transportTypeRepository;
        private readonly IRepository<TrucksType, long> _lookup_trucksTypeRepository;
        private readonly IRepository<PackingType, int> _packingTypeRepository;
        private readonly IRepository<ShippingRequestVas, long> _shippingRequestVasRepository;
        private readonly NormalPricePackageManager _normalPricePackageManager;
        private readonly PriceOfferManager _priceOfferManager;
        private readonly IRepository<PriceOffer, long> _priceOfferRepository;
        private readonly IRepository<DedicatedShippingRequestDriver, long> _dedicatedShippingRequestDriverRepository;
        private readonly IRepository<DedicatedShippingRequestTruck, long> _dedicatedShippingRequestTrucksRepository;


        private readonly ISmsSender _smsSender;
        private readonly IAppNotifier _appNotifier;

        public ShippingRequestManager(ISmsSender smsSender,
            IRepository<RoutPoint, long> routPointRepository,
            IAppNotifier appNotifier,
            IRepository<ShippingRequest, long> shippingRequestRepository,
            IFeatureChecker featureChecker,
            IAbpSession abpSession,
            IWebUrlService webUrlService,
            IRepository<GoodCategory, int> lookup_goodCategoryRepository,
            IRepository<TransportType, int> transportTypeRepository,
            IRepository<TrucksType, long> lookup_trucksTypeRepository,
            IRepository<PackingType, int> packingTypeRepository,
            IRepository<ShippingRequestVas, long> shippingRequestVasRepository,
            NormalPricePackageManager normalPricePackageManager,
            PriceOfferManager priceOfferManager,
            IRepository<PriceOffer, long> priceOfferRepository,
            IRepository<ShippingRequestTrip> shippingRequestTripRepository,
            IRepository<DedicatedShippingRequestDriver, long> dedicatedShippingRequestDriverRepository,
            IRepository<DedicatedShippingRequestTruck, long> dedicatedShippingRequestTrucksRepository)
        {
            _smsSender = smsSender;
            _routPointRepository = routPointRepository;
            _appNotifier = appNotifier;
            _shippingRequestRepository = shippingRequestRepository;
            _featureChecker = featureChecker;
            _abpSession = abpSession;
            WebUrlService = webUrlService;
            _lookup_goodCategoryRepository = lookup_goodCategoryRepository;
            _transportTypeRepository = transportTypeRepository;
            _lookup_trucksTypeRepository = lookup_trucksTypeRepository;
            _packingTypeRepository = packingTypeRepository;
            _shippingRequestVasRepository = shippingRequestVasRepository;
            _normalPricePackageManager = normalPricePackageManager;
            _priceOfferManager = priceOfferManager;
            _priceOfferRepository = priceOfferRepository;
            _shippingRequestTripRepository = shippingRequestTripRepository;
            _dedicatedShippingRequestDriverRepository = dedicatedShippingRequestDriverRepository;
            _dedicatedShippingRequestTrucksRepository = dedicatedShippingRequestTrucksRepository;
        }

        /// <summary>
        /// Set shipping request status to post price
        /// </summary>
        /// <param name="shippingRequest"></param>
        /// <returns></returns>
        public async Task SetToPostPrice(ShippingRequest shippingRequest)
        {
            shippingRequest.Status = ShippingRequestStatus.PostPrice;
            await _appNotifier.ShippingRequestNotifyCarrirerWhenShipperAccepted(shippingRequest);
        }


        /// <summary>
        /// Send shipment code to receiver
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public async void SendSmsToReceiver(RoutPoint point, string Culture)
        {
            string number = point.ReceiverPhoneNumber;
            string formattedDate = point.EndTime?.ToString("dd/MM/yyyy hh:mm");
            var ratingLink = $"{L("ClickToRate")} {WebUrlService.WebSiteRootAddressFormat}account/RatingPage/{point.Code}";
            string message = L(TACHYONConsts.SMSShippingRequestReceiverCode, new CultureInfo(Culture), point.WaybillNumber, point.Code, ratingLink);

            if (point.ShippingRequestTripFk.ShippingRequestFk.IsSaas())
                message = L(TACHYONConsts.SMSSaasShippingRequestReceiverCode, new CultureInfo(Culture), point.WaybillNumber, point.Code);

            if (point.ReceiverFk != null)
            {
                number = point.ReceiverFk.PhoneNumber;
            }

            await _smsSender.SendAsync(number, message);
        }

        /// <summary>
        /// Send shipment code to receiver
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public async Task SendSmsToReceiver(RoutPoint point)
        {
            string number = point.ReceiverPhoneNumber;
            string formattedDate = point.EndTime?.ToString("dd/MM/yyyy hh:mm");
            var ratingLink =
                $"{L("ClickToRate")} {WebUrlService.WebSiteRootAddressFormat}account/RatingPage/{point.Code}";
            string message = L(TACHYONConsts.SMSShippingRequestReceiverCode, point.WaybillNumber, point.Code, ratingLink);

            if (point.ShippingRequestTripFk.ShippingRequestFk.IsSaas())
                message = L(TACHYONConsts.SMSSaasShippingRequestReceiverCode, point.WaybillNumber, point.Code);

            if (point.ReceiverFk != null)
            {
                await _smsSender.SendAsync(point.ReceiverFk.PhoneNumber, message);
            }

            if (!string.IsNullOrEmpty(number)) await _smsSender.SendAsync(number, message);
        }

        /// <summary>
        /// Send shipment code to receivers
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public async Task SendSmsToReceivers(int tripId)
        {
            var RoutePoints = await _routPointRepository.GetAll()
                .Include(r => r.ReceiverFk)
                .Include(x => x.ShippingRequestTripFk)
                .ThenInclude(z => z.ShippingRequestFk)
                .Where(x => x.ShippingRequestTripId == tripId && x.PickingType == PickingType.Dropoff).ToListAsync();
            RoutePoints.ForEach(async p =>
            {
                await SendSmsToReceiver(p);
            });
        }


        public async Task<ShippingRequest> GetShippingRequestWhenNormalStatus(long ShippingRequestId)
        {
            return await _shippingRequestRepository
                .GetAll()
                .WhereIf(await _featureChecker.IsEnabledAsync(AppFeatures.Shipper),
                    x => x.TenantId == _abpSession.TenantId && !x.IsTachyonDeal)
                .FirstOrDefaultAsync(r => r.Id == ShippingRequestId && (r.Status == ShippingRequestStatus.NeedsAction ||
                                                                        r.Status == ShippingRequestStatus.PrePrice ||
                                                                        r.Status == ShippingRequestStatus
                                                                            .AcceptedAndWaitingCarrier));
        }


        public async Task ValidateShippingRequestStep1(CreateOrEditShippingRequestStep1BaseDto input)
        {
            if (input.IsTachyonDeal)
            {
                if (!await _featureChecker.IsEnabledAsync(AppFeatures.SendTachyonDealShippingRequest))
                {
                    // throw new UserFriendlyException(L("feature SendTachyonDealShippingRequest not enabled"));
                }
            }
            else if (input.IsDirectRequest)
            {
                if (!await _featureChecker.IsEnabledAsync(AppFeatures.SendDirectRequest) && !await _featureChecker.IsEnabledAsync(AppFeatures.CarrierAsASaas))
                {
                    throw new UserFriendlyException(L("feature SendDirectRequest not enabled"));
                }
            }

        }


        public async Task CreateStep1Manager(ShippingRequest shippingRequest, CreateOrEditShippingRequestStep1BaseDto input)
        {
            if (input.ShipperId.HasValue && await _featureChecker.IsEnabledAsync(AppFeatures.TachyonDealer))
            {
                shippingRequest.TenantId = input.ShipperId.Value;
            }

            var isSaas = await _featureChecker.IsEnabledAsync(AppFeatures.Carrier) && await _featureChecker.IsEnabledAsync(AppFeatures.CarrierAsASaas);
            if (isSaas || input.IsInternalBrokerRequest)
            {
                shippingRequest.TenantId = _abpSession.TenantId.Value;
                shippingRequest.CarrierTenantId = _abpSession.TenantId.Value;
            }

            shippingRequest.CreatedByTachyonDealer = await _featureChecker.IsEnabledAsync(AppFeatures.TachyonDealer);

            //skip if edit
            if(shippingRequest.DraftStep == 0)
            {
                shippingRequest.IsDrafted = true;
                shippingRequest.DraftStep = 1;
            }

        }

        public async Task EditVasStep(ShippingRequest shippingRequest, EditVasStepBaseDto input)
        {
            foreach (var vas in shippingRequest.ShippingRequestVases)
            {
                if (!input.ShippingRequestVasList.Any(x => x.Id == vas.Id))
                {
                    await _shippingRequestVasRepository.DeleteAsync(vas);
                }
            }
        }

        public async Task PublishShippingRequestManager(ShippingRequest shippingRequest)
        {
            // Bid info
            if (shippingRequest.IsBid)
            {
                if (!shippingRequest.BidStartDate.HasValue)
                {
                    shippingRequest.BidStartDate = Clock.Now.Date;
                }
                else if (shippingRequest.BidStartDate.HasValue && shippingRequest.BidStartDate.Value < Clock.Now.Date)
                {
                    throw new UserFriendlyException(L("BidStartDateConnotBeBeforeToday"));
                }

                shippingRequest.BidStatus = shippingRequest.BidStartDate.Value.Date == Clock.Now.Date
                    ? ShippingRequestBidStatus.OnGoing
                    : ShippingRequestBidStatus.StandBy;
                await _normalPricePackageManager.SendNotificationToCarriersWithTheSameTrucks(shippingRequest);
            }

            shippingRequest.IsDrafted = false;
            //to make SR to non drafted .. 
            if (await _featureChecker.IsEnabledAsync(AppFeatures.TachyonDealer) && shippingRequest.CreatedByTachyonDealer)
                await _appNotifier.NotifyShipperWhenSrAddedByTms
                    (shippingRequest.Id, shippingRequest.ReferenceNumber, shippingRequest.TenantId);


           

            // handle carrier as saas SR pricing
            if (shippingRequest.IsSaas())
            {
                decimal carrierAsSaasCommissionValue = Convert.ToDecimal(await _featureChecker.GetValueAsync(shippingRequest.TenantId, AppFeatures.SaasCommission));


                var itemDetails = new List<PriceOfferDetailDto>();
                foreach (ShippingRequestVas vas in shippingRequest.ShippingRequestVases)
                {

                    itemDetails.Add(new PriceOfferDetailDto
                    {
                        ItemId = vas.Id,
                        Price = 0,
                        CommissionPercentageOrAddValue = 0,
                        PriceType = PriceOfferType.Vas,
                        CommissionType = PriceOfferCommissionType.CommissionValue
                    }
                    );
                }


                var input = new CreateOrEditPriceOfferInput
                {
                    ShippingRequestId = shippingRequest.Id,
                    ItemPrice = 0,
                    Channel = PriceOfferChannel.CarrierAsSaas,
                    ParentId = null,
                    PriceType = PriceOfferType.Trip,
                    //ignor vas's
                    ItemDetails = itemDetails,
                    CommissionPercentageOrAddValue = carrierAsSaasCommissionValue,
                    CommissionType = PriceOfferCommissionType.CommissionValue,
                    VasCommissionPercentageOrAddValue = 0,
                    VasCommissionType = PriceOfferCommissionType.CommissionValue,
                    SourceId = null
                };


                var offer = await _priceOfferManager.InitPriceOffer(input);
                _priceOfferManager.SetShippingRequestPricing(offer);

                offer.Status = PriceOfferStatus.Accepted;

                shippingRequest.Status = ShippingRequestStatus.PostPrice;

                await _priceOfferRepository.InsertAsync(offer);

            }
        }

        public async Task<ShippingRequest> GetDraftedShippingRequest(long id)
        {
            DisableDraftedFilter();
            if(await _featureChecker.IsEnabledAsync(AppFeatures.TachyonDealer))
            DisableTenancyFilters();
            ShippingRequest shippingRequest = await _shippingRequestRepository.GetAll()
                .Include(x => x.ShippingRequestDestinationCities)
                .ThenInclude(x=>x.CityFk)
                .Include(x=>x.ShippingRequestVases)
                .WhereIf(await _featureChecker.IsEnabledAsync(AppFeatures.TachyonDealer), x => x.IsTachyonDeal == true)
                .Where(x => x.Id == id && x.IsDrafted==true)
                .FirstOrDefaultAsync();
            return shippingRequest;
        }
        
        public async Task<ShippingRequest> GetShippingRequestForAssign(long id)
        {
            DisableTenancyFilters();
            ShippingRequest shippingRequest = await _shippingRequestRepository.GetAll()
                .Include(x=>x.DedicatedShippingRequestTrucks)
                .ThenInclude(x=>x.Truck)
                .Include(x => x.DedicatedShippingRequestDrivers)
                .ThenInclude(x=>x.DriverUser)
                .WhereIf(await _featureChecker.IsEnabledAsync(AppFeatures.TachyonDealer), x => x.Status == ShippingRequestStatus.PostPrice)
                .WhereIf(await _featureChecker.IsEnabledAsync(AppFeatures.Carrier), x=>x.CarrierTenantId == _abpSession.TenantId && x.Status==ShippingRequestStatus.PostPrice)
                .FirstOrDefaultAsync(x => x.Id == id);
            return shippingRequest;
        }

        public async Task<bool> CheckIfDriverWorkingOnAnotherTrip(long assignedDriverUserId)
        {
            return await _shippingRequestTripRepository.GetAll()
                .AnyAsync(x => x.AssignedDriverUserId == assignedDriverUserId
                            && x.Status == ShippingRequestTripStatus.InTransit
                            && x.DriverStatus == ShippingRequestTripDriverStatus.Accepted);
        }

        public async Task<bool> CheckIfDriversWorkingOnAnotherTrip(List<long> assignedDriverUserIds)
        {
            return await _shippingRequestTripRepository.GetAll()
                .AnyAsync(x => x.AssignedDriverUserId != null && assignedDriverUserIds.Contains(x.AssignedDriverUserId.Value)
                            && x.Status == ShippingRequestTripStatus.InTransit
                            && x.DriverStatus == ShippingRequestTripDriverStatus.Accepted);
        }

        public async Task<bool> CheckIfDriverIsRented(long assignedDriverUserId)
        {
            return await _dedicatedShippingRequestDriverRepository.GetAll()
                .AnyAsync(x => x.DriverUserId == assignedDriverUserId
                            && x.Status == WorkingStatus.Busy);
        }

        public async Task<bool> CheckIfTruckIsRented(long assignedTruckId)
        {
            return await _dedicatedShippingRequestTrucksRepository.GetAll()
                .AnyAsync(x => x.TruckId == assignedTruckId
                            && x.Status == WorkingStatus.Busy);
        }

        /// <summary>
        /// 
        /// <list type="bullet|number|table">
        /// <listheader>This Method Used For Validate</listheader>
        ///    <item>1-OtherGoodsCategoryName</item>
        ///    <item>2-OtherTransportTypeName</item>
        ///    <item>3-OtherTrucksTypeName</item>
        /// </list>
        /// See <see cref="CreateOrEditShippingRequestDto"/>
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task OthersNameValidation(IShippingRequestDtoHaveOthersName input)
        {
            #region Validate GoodCategory

            if (input.GoodCategoryId != null)
            {
                var goodCategory = await _lookup_goodCategoryRepository
                    .FirstOrDefaultAsync(input.GoodCategoryId.Value);

                if (goodCategory.Key.ToLower().Contains(TACHYONConsts.OthersDisplayName.ToLower()) &&
                    input.OtherGoodsCategoryName.IsNullOrEmpty())
                    throw new UserFriendlyException(L("GoodCategoryCanNotBeOtherAndEmptyAtSameTime"));
            }

            #endregion

            #region Validate TransportType

            if (input.TransportTypeId != null)
            {
                var transportType = await _transportTypeRepository
                    .FirstOrDefaultAsync(input.TransportTypeId.Value);

                if (transportType.DisplayName.ToLower().Contains(TACHYONConsts.OthersDisplayName.ToLower()) &&
                    input.OtherTransportTypeName.IsNullOrEmpty())
                    throw new UserFriendlyException(L("TransportTypeCanNotBeOtherAndEmptyAtSameTime"));
            }

            #endregion

            #region Validate TrucksType

            //? FYI TrucksTypeId Not Nullable 
            var trucksType = await _lookup_trucksTypeRepository
                .FirstOrDefaultAsync(input.TrucksTypeId);

            if (trucksType.DisplayName.ToLower().Contains(TACHYONConsts.OthersDisplayName.ToLower()) &&
                input.OtherTrucksTypeName.IsNullOrEmpty())
                throw new UserFriendlyException(L("TrucksTypeCanNotBeOtherAndEmptyAtSameTime"));

            #endregion

            #region Validate PackingType

            var packingType = await _packingTypeRepository
                .FirstOrDefaultAsync(input.PackingTypeId);

            if (packingType.DisplayName.ToLower().Contains(TACHYONConsts.OthersDisplayName.ToLower()) &&
                input.OtherPackingTypeName.IsNullOrEmpty())
                throw new UserFriendlyException(L("PackingTypeCanNotBeOtherAndEmptyAtSameTime"));

            #endregion

        }


        
    }
}