using Abp;
using Abp.Application.Features;
using Abp.AspNetCore.SignalR.Hubs;
using Abp.Collections.Extensions;
using Abp.Configuration;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Abp.Runtime.Session;
using Abp.Timing;
using Abp.UI;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using TACHYON.Authorization.Users;
using TACHYON.Common;
using TACHYON.Dto;
using TACHYON.Features;
using TACHYON.Firebases;
using TACHYON.Invoices;
using TACHYON.Net.Sms;
using TACHYON.Notifications;
using TACHYON.PickingTypes;
using TACHYON.PriceOffers;
using TACHYON.Routs.RoutPoints;
using TACHYON.Shipping.Drivers.Dto;
using TACHYON.Shipping.RoutPoints;
using TACHYON.Shipping.ShippingRequests;
using TACHYON.Shipping.ShippingRequestTrips;
using TACHYON.Shipping.Trips.Dto;
using TACHYON.Tracking.Dto;
using PickingType = TACHYON.Routs.RoutPoints.PickingType;

namespace TACHYON.Shipping.Trips
{
    public class ShippingRequestsTripManager : TACHYONDomainServiceBase
    {
        private readonly IRepository<ShippingRequestTrip> _shippingRequestTrip;
        private readonly PriceOfferManager _priceOfferManager;
        private readonly IAppNotifier _appNotifier;
        private readonly IFeatureChecker _featureChecker;
        private readonly IAbpSession _abpSession;
        private readonly IHubContext<AbpCommonHub> _hubContext;
        private readonly IRepository<RoutPoint, long> _routPointRepository;
        private readonly IRepository<RoutPointDocument, long> _routPointDocumentRepository;
        private readonly IRepository<ShippingRequestTripTransition> _shippingRequestTripTransitionRepository;
        private readonly IRepository<RoutPointStatusTransition> _routPointStatusTransitionRepository;
        private readonly FirebaseNotifier _firebaseNotifier;
        private readonly ISmsSender _smsSender;
        private readonly InvoiceManager _invoiceManager;
        private readonly CommonManager _commonManager;
        private readonly UserManager UserManager;

        public ShippingRequestsTripManager(IRepository<ShippingRequestTrip> shippingRequestTrip, PriceOfferManager priceOfferManager, IAppNotifier appNotifier, IFeatureChecker featureChecker, IAbpSession abpSession, IHubContext<AbpCommonHub> hubContext, IRepository<RoutPoint, long> routPointRepository, IRepository<ShippingRequestTripTransition> shippingRequestTripTransitionRepository, IRepository<RoutPointStatusTransition> routPointStatusTransitionRepository, FirebaseNotifier firebaseNotifier, ISmsSender smsSender, InvoiceManager invoiceManager, CommonManager commonManager, IRepository<RoutPointDocument, long> routPointDocumentRepository, UserManager userManager)
        {
            _shippingRequestTrip = shippingRequestTrip;
            _priceOfferManager = priceOfferManager;
            _appNotifier = appNotifier;
            _featureChecker = featureChecker;
            _abpSession = abpSession;
            _hubContext = hubContext;
            _routPointRepository = routPointRepository;
            _shippingRequestTripTransitionRepository = shippingRequestTripTransitionRepository;
            _routPointStatusTransitionRepository = routPointStatusTransitionRepository;
            _firebaseNotifier = firebaseNotifier;
            _smsSender = smsSender;
            _invoiceManager = invoiceManager;
            _commonManager = commonManager;
            _routPointDocumentRepository = routPointDocumentRepository;
            UserManager = userManager;
        }

        /// <summary>
        /// Accept trip when assign to driver
        /// </summary>
        /// <param name="id"> trip id</param>
        /// <returns></returns>
        public async Task Accepted(int id)
        {
            DisableTenancyFilters();
            var currentUser = await GetCurrentUserAsync(_abpSession);
            var trip = await CheckIfCanAccepted(id, currentUser);
            trip.DriverStatus = ShippingRequestTripDriverStatus.Accepted;
            await GeneratePrices(trip);
            if (currentUser.IsDriver) await _appNotifier.DriverAcceptTrip(trip, currentUser.FullName);

            await _hubContext.Clients.Users(await GetUsersHubNotification(trip, currentUser)).SendAsync("tracking", TACHYONConsts.TriggerTrackingAccepted, ObjectMapper.Map<TrackingListDto>(trip));

        }

        /// <summary>
        /// start new trip
        /// </summary>
        /// <param name="id"> trip id</param>
        /// <returns></returns>
        public async Task Start(ShippingRequestTripDriverStartInputDto Input)
        {
            DisableTenancyFilters();
            var currentUser = await GetCurrentUserAsync(_abpSession);
            var trip = await _shippingRequestTrip
                            .GetAll()
                            .Include(s => s.ShippingRequestFk)
                            .Where
                            (
                                x => x.Id == Input.Id &&
                                x.Status == ShippingRequestTripStatus.New &&
                                x.DriverStatus == ShippingRequestTripDriverStatus.Accepted &&
                                x.ShippingRequestFk.StartTripDate.Value.Date <= Clock.Now.Date
                            )
                            .WhereIf(!currentUser.TenantId.HasValue || await _featureChecker.IsEnabledAsync(AppFeatures.TachyonDealer), x => x.ShippingRequestFk.IsTachyonDeal)
                            .WhereIf(currentUser.TenantId.HasValue && await _featureChecker.IsEnabledAsync(AppFeatures.Carrier), x => x.ShippingRequestFk.CarrierTenantId == currentUser.TenantId.Value)
                            .WhereIf(currentUser.IsDriver, x => x.AssignedDriverUserId == currentUser.Id)
                            .FirstOrDefaultAsync();

            if (trip == null) throw new UserFriendlyException(L("YouCannotStartWithTheTripSelected"));
            /// Check if the driver already working on another trip
            if (!_shippingRequestTrip
                .GetAll()
                .Any
                (
                    x =>
                    x.Id != trip.Id &&
                    x.Status == ShippingRequestTripStatus.Intransit &&
                    x.AssignedDriverUserId == trip.AssignedDriverUserId
                 )
               )
            {
                /// Get the start pickup point
                var RouteStart = await _routPointRepository.GetAll().Include(x => x.FacilityFk).SingleAsync(x => x.ShippingRequestTripId == trip.Id && x.PickingType == Routs.RoutPoints.PickingType.Pickup);

                RouteStart.StartTime = Clock.Now;
                RouteStart.IsActive = true;
                RouteStart.Status = RoutePointStatus.StartedMovingToLoadingLocation;


                trip.Status = ShippingRequestTripStatus.Intransit;
                trip.RoutePointStatus = RoutePointStatus.StartedMovingToLoadingLocation;
                trip.StartTripDate = Clock.Now;
                await StartTransition(RouteStart, new Point(Input.lat, Input.lng));

                await _hubContext.Clients.Users(await GetUsersHubNotification(trip, currentUser)).SendAsync("tracking", TACHYONConsts.TriggerTrackingStarted, ObjectMapper.Map<TrackingListDto>(trip));

                if (!currentUser.IsDriver) await _firebaseNotifier.TripChanged(new Abp.UserIdentifier(trip.ShippingRequestFk.CarrierTenantId.Value, trip.AssignedDriverUserId.Value), trip.Id.ToString());
            }
            else
            {
                throw new UserFriendlyException(L("YouCanNotStartNewTripWhenYouHaveAnotherTripStillNotFinish"));
            }

        }

        /// <summary>
        /// Change trip status per point
        /// </summary>
        /// <param name="id"> trip id</param>
        /// <returns></returns>
        public async Task ChangeStatus(int id)
        {
            DisableTenancyFilters();
            var currentUser = await GetCurrentUserAsync(_abpSession);
            var activePoint = await _routPointRepository
                            .GetAll()
                             .Include(t => t.ShippingRequestTripFk)
                                 .ThenInclude(s => s.ShippingRequestFk)
                            .Include(t => t.ShippingRequestTripFk)
                                .ThenInclude(s => s.RoutPoints)
                            .Where
                            (
                                x => x.ShippingRequestTripId == id &&
                                x.ShippingRequestTripFk.ShippingRequestFk.Status == ShippingRequests.ShippingRequestStatus.PostPrice &&
                               !x.IsComplete &&
                                x.Status != RoutePointStatus.StandBy
                            )
                            .WhereIf(!currentUser.TenantId.HasValue || await _featureChecker.IsEnabledAsync(AppFeatures.TachyonDealer), x => x.ShippingRequestTripFk.ShippingRequestFk.IsTachyonDeal)
                            .WhereIf(currentUser.TenantId.HasValue && await _featureChecker.IsEnabledAsync(AppFeatures.Carrier), x => x.ShippingRequestTripFk.ShippingRequestFk.CarrierTenantId == currentUser.TenantId.Value)
                            .WhereIf(currentUser.IsDriver, x => x.ShippingRequestTripFk.AssignedDriverUserId == currentUser.Id)
                            .FirstOrDefaultAsync(x => x.IsActive &&
                            x.ShippingRequestTripFk.Status == ShippingRequestTripStatus.Intransit);

            if (activePoint == null) throw new UserFriendlyException(L("YouCanNotChangeTheStatus"));
            var trip = activePoint.ShippingRequestTripFk;

            switch (trip.RoutePointStatus)
            {
                case RoutePointStatus.StartedMovingToLoadingLocation:
                    trip.RoutePointStatus = RoutePointStatus.ArriveToLoadingLocation;
                    activePoint.Status = RoutePointStatus.ArriveToLoadingLocation;
                    break;
                case RoutePointStatus.ArriveToLoadingLocation:
                    trip.RoutePointStatus = RoutePointStatus.StartLoading;
                    activePoint.Status = RoutePointStatus.StartLoading;
                    break;
                case RoutePointStatus.StartLoading:
                    trip.RoutePointStatus = RoutePointStatus.FinishLoading;
                    //point.IsActive = false;
                    activePoint.IsComplete = true;
                    activePoint.EndTime = Clock.Now;
                    activePoint.Status = RoutePointStatus.FinishLoading;
                    activePoint.ActualPickupOrDeliveryDate = trip.ActualPickupDate = Clock.Now;
                    await SendSmsToReceivers(trip.Id);
                    break;
                case RoutePointStatus.StartedMovingToOfLoadingLocation:
                    trip.RoutePointStatus = RoutePointStatus.ArrivedToDestination;
                    activePoint.Status = RoutePointStatus.ArrivedToDestination;
                    break;
                case RoutePointStatus.ArrivedToDestination:
                    trip.RoutePointStatus = RoutePointStatus.StartOffloading;
                    activePoint.Status = RoutePointStatus.StartOffloading;

                    break;
                case RoutePointStatus.StartOffloading:
                    trip.RoutePointStatus = RoutePointStatus.FinishOffLoadShipment;
                    activePoint.Status = RoutePointStatus.FinishOffLoadShipment;
                    activePoint.ActualPickupOrDeliveryDate = Clock.Now;
                    if (!trip.RoutPoints.Any(x => x.ActualPickupOrDeliveryDate == null && x.Id != activePoint.Id))
                    {
                        trip.ActualDeliveryDate = Clock.Now;
                    }

                    activePoint.CompletedStatus = RoutePointCompletedStatus.CompletedAndMissingReceiverCode;
                    //check if all trip points completed, change the trip status from intransit to delivered and needs confirmation
                    if (trip.RoutPoints.Where(x => x.Id != activePoint.Id).All(x => x.CompletedStatus > RoutePointCompletedStatus.NotCompleted))
                    {
                        trip.Status = ShippingRequestTripStatus.DeliveredAndNeedsConfirmation;
                    }
                    break;
                default:
                    throw new UserFriendlyException(L("YouCanNotChangeStatus"));

            }
            await SetRoutStatusTransition(activePoint);

            await NotificationWhenPointChanged(activePoint, currentUser);
        }

        /// <summary>
        /// Set new active dropoff point for trip
        /// </summary>
        /// <param name="PointId">Dropoff point id</param>

        public async Task GotoNextLocation(long id)
        {
            DisableTenancyFilters();
            var currentUser = await GetCurrentUserAsync(_abpSession);

            var currentpoint = await _routPointRepository
                .GetAll()
                .Include(x => x.ShippingRequestTripFk)
                     .ThenInclude(x => x.ShippingRequestFk)
                .Include(x => x.FacilityFk)
                .Where(x => x.Id == id && x.Status == RoutePointStatus.StandBy && x.ShippingRequestTripFk.Status == ShippingRequestTripStatus.Intransit && x.PickingType == Routs.RoutPoints.PickingType.Dropoff)
                .WhereIf(!currentUser.TenantId.HasValue || await _featureChecker.IsEnabledAsync(AppFeatures.TachyonDealer), x => x.ShippingRequestTripFk.ShippingRequestFk.IsTachyonDeal)
                .WhereIf(currentUser.TenantId.HasValue && await _featureChecker.IsEnabledAsync(AppFeatures.Carrier), x => x.ShippingRequestTripFk.ShippingRequestFk.CarrierTenantId == currentUser.TenantId.Value)
                .WhereIf(currentUser.IsDriver, x => x.ShippingRequestTripFk.AssignedDriverUserId == currentUser.Id)
                .FirstOrDefaultAsync();
            var activePoint = await GetActivePointByTripAsyn(currentpoint.ShippingRequestTripId, currentUser);
            if (currentpoint == null) throw new UserFriendlyException(L("TheLocationSelectedIsNotFound"));
            if (activePoint == null) throw new UserFriendlyException(L("NoActivePoint"));

            // deactivate active point

            var trip = activePoint.ShippingRequestTripFk;

            if (activePoint.PickingType == PickingType.Pickup && trip.RoutePointStatus != RoutePointStatus.FinishLoading) throw new UserFriendlyException(L("TheTripIsNotFound"));

            if (trip.RoutePointStatus == RoutePointStatus.FinishLoading || trip.RoutePointStatus == RoutePointStatus.DeliveryConfirmation)
            {
                activePoint.IsActive = false;
                activePoint.IsComplete = true;
                activePoint.EndTime = Clock.Now;

            }

            if (activePoint.PickingType == PickingType.Dropoff &&
                (trip.RoutePointStatus == RoutePointStatus.FinishOffLoadShipment ||
                trip.RoutePointStatus == RoutePointStatus.ReceiverConfirmed))
            {
                //can go to next point if not confirming receiverd code nor uploading POD
                activePoint.IsActive = false;
            }
            //finish active point
            await CanGoToNextLocation(currentpoint);

            currentpoint.StartTime = Clock.Now;
            currentpoint.IsActive = true;
            currentpoint.Status = RoutePointStatus.StartedMovingToOfLoadingLocation;
            currentpoint.ShippingRequestTripFk.RoutePointStatus = RoutePointStatus.StartedMovingToOfLoadingLocation;

            await ChangeTransition(currentpoint);
            await NotificationWhenPointChanged(currentpoint, currentUser);

        }


        private async Task<RoutPoint> GetActivePointByTripAsyn(int tripId, User currentUser)
        {
            return await _routPointRepository
                .GetAll()
                .Include(x => x.ShippingRequestTripFk)
                     .ThenInclude(x => x.ShippingRequestFk)
                .Include(x => x.FacilityFk)
                .Where(x => x.IsActive &&
                x.ShippingRequestTripFk.Status == ShippingRequestTripStatus.Intransit &&
                x.ShippingRequestTripId == tripId)
                .WhereIf(!currentUser.TenantId.HasValue || await _featureChecker.IsEnabledAsync(AppFeatures.TachyonDealer), x => x.ShippingRequestTripFk.ShippingRequestFk.IsTachyonDeal)
                .WhereIf(currentUser.TenantId.HasValue && await _featureChecker.IsEnabledAsync(AppFeatures.Carrier), x => x.ShippingRequestTripFk.ShippingRequestFk.CarrierTenantId == currentUser.TenantId.Value)
                .WhereIf(currentUser.IsDriver, x => x.ShippingRequestTripFk.AssignedDriverUserId == currentUser.Id)
                .FirstOrDefaultAsync();


        }

        /// <summary>
        /// Driver confirm the receiver code 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="Code"></param>
        /// <returns></returns>
        public async Task ConfirmReceiverCode(ConfirmReceiverCodeInput input)
        {
            DisableTenancyFilters();
            var currentUser = await GetCurrentUserAsync(_abpSession);
            var CurrentPoint = await _routPointRepository
                            .GetAll()
                             .Include(t => t.ShippingRequestTripFk)
                                 .ThenInclude(s => s.ShippingRequestFk)
                            .Where
                                    (
                                        x =>
                                            x.Id == input.Id &&
                                            x.Status == RoutePointStatus.FinishOffLoadShipment &&
                                            x.Code == input.Code &&
                                            (x.ShippingRequestTripFk.Status == ShippingRequestTripStatus.Intransit ||
                                            x.ShippingRequestTripFk.Status == ShippingRequestTripStatus.DeliveredAndNeedsConfirmation) &&
                                            x.ShippingRequestTripFk.ShippingRequestFk.Status == ShippingRequestStatus.PostPrice

                                    )
                            .WhereIf(!currentUser.TenantId.HasValue || await _featureChecker.IsEnabledAsync(AppFeatures.TachyonDealer), x => x.ShippingRequestTripFk.ShippingRequestFk.IsTachyonDeal)
                            .WhereIf(currentUser.TenantId.HasValue && await _featureChecker.IsEnabledAsync(AppFeatures.Carrier), x => x.ShippingRequestTripFk.ShippingRequestFk.CarrierTenantId == currentUser.TenantId.Value)
                            .WhereIf(currentUser.IsDriver, x => x.ShippingRequestTripFk.AssignedDriverUserId == currentUser.Id)
                            .FirstOrDefaultAsync();

            if (CurrentPoint == null) throw new UserFriendlyException(L("TheReceiverCodeIsIncorrect"));

            CurrentPoint.ShippingRequestTripFk.RoutePointStatus = RoutePointStatus.ReceiverConfirmed;
            CurrentPoint.Status = RoutePointStatus.ReceiverConfirmed;
            await SetRoutStatusTransition(CurrentPoint);
            await NotificationWhenPointChanged(CurrentPoint, currentUser);
        }

        /// <summary>
        /// driver confirm the trip has finished 
        /// </summary>
        /// <param name="document"> attachment</param>
        /// <param name="id">route point id</param>
        /// <returns></returns>

        public async Task<bool> ConfirmPointToDelivery(IHasDocument document, long id)
        {
            DisableTenancyFilters();
            var currentUser = await GetCurrentUserAsync(_abpSession);

            var CurrentPoint = await _routPointRepository.GetAll().
                Include(x => x.ShippingRequestTripFk)
                    .ThenInclude(x => x.ShippingRequestTripVases)
                .Include(x => x.ShippingRequestTripFk)
                    .ThenInclude(x => x.ShippingRequestFk)
                     .ThenInclude(x => x.Tenant)
                .Where
                                    (
                                        x =>
                                            x.Id == id &&
                                            x.Status == RoutePointStatus.ReceiverConfirmed &&
                                            (x.ShippingRequestTripFk.Status == ShippingRequestTripStatus.Intransit ||
                                            x.ShippingRequestTripFk.Status == ShippingRequestTripStatus.DeliveredAndNeedsConfirmation) &&
                                            x.ShippingRequestTripFk.ShippingRequestFk.Status == ShippingRequests.ShippingRequestStatus.PostPrice
                                    )
                .WhereIf(!currentUser.TenantId.HasValue || await _featureChecker.IsEnabledAsync(AppFeatures.TachyonDealer), x => x.ShippingRequestTripFk.ShippingRequestFk.IsTachyonDeal)
                .WhereIf(currentUser.TenantId.HasValue && await _featureChecker.IsEnabledAsync(AppFeatures.Carrier), x => x.ShippingRequestTripFk.ShippingRequestFk.CarrierTenantId == currentUser.TenantId.Value)
                .WhereIf(currentUser.IsDriver, x => x.ShippingRequestTripFk.AssignedDriverUserId == currentUser.Id)
                .FirstOrDefaultAsync();
            if (CurrentPoint == null) throw new UserFriendlyException(L("TheRoutePointIsNotFound"));

            CurrentPoint.EndTime = Clock.Now;

            //CurrentPoint.DocumentContentType = "image/jpeg";
            //CurrentPoint.DocumentName = document.DocumentName;
            //CurrentPoint.DocumentId = document.DocumentId;
            var routePointDocument = new RoutPointDocument();
            routePointDocument.RoutPointId = CurrentPoint.Id;
            routePointDocument.DocumentContentType = "image/jpeg";
            routePointDocument.DocumentName = document.DocumentName;
            routePointDocument.DocumentId = document.DocumentId;
            routePointDocument.RoutePointDocumentType = RoutePointDocumentType.POD;
            await _routPointDocumentRepository.InsertAsync(routePointDocument);

            CurrentPoint.IsActive = false;
            CurrentPoint.IsComplete = true;
            CurrentPoint.Status = RoutePointStatus.DeliveryConfirmation;
            await SetRoutStatusTransition(CurrentPoint);
            var trip = CurrentPoint.ShippingRequestTripFk;
            if (await _routPointRepository.GetAll().Where(x => x.ShippingRequestTripId == trip.Id && x.IsComplete == false && x.Id != CurrentPoint.Id).CountAsync() == 0)
            {
                trip.Status = ShippingRequestTripStatus.Delivered;
                trip.RoutePointStatus = RoutePointStatus.Delivered;
                trip.EndTripDate = Clock.Now;

                await Done(trip);
                await _invoiceManager.GenertateInvoiceWhenShipmintDelivery(trip);
                await NotificationWhenShipmentDelivered(CurrentPoint, currentUser);
            }
            else
            {
                trip.RoutePointStatus = RoutePointStatus.DeliveryConfirmation;
                await NotificationWhenPointChanged(CurrentPoint, currentUser);
            }


            return true;


        }

        /// <summary>
        /// Get Currrent driver trip dto
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ShippingRequestTripDto> GetCurrentDriverTrip(long UserId)
        {
            var CurrentTrip = await _shippingRequestTrip.GetAll()
                    .Where(x => x.AssignedDriverUserId == UserId && x.Status == ShippingRequestTripStatus.Intransit).FirstOrDefaultAsync();
            return ObjectMapper.Map<ShippingRequestTripDto>(CurrentTrip);

        }
        public async Task<FileDto> GetPOD(long id)
        {
            DisableTenancyFilters();
            var currentUser = await GetCurrentUserAsync(_abpSession);

            var document = await _routPointDocumentRepository.GetAll()
                .Where(x => x.RoutPointId == id && x.RoutPointFk.Status == RoutePointStatus.DeliveryConfirmation && x.RoutePointDocumentType == RoutePointDocumentType.POD)
                .WhereIf(!currentUser.TenantId.HasValue || await _featureChecker.IsEnabledAsync(AppFeatures.TachyonDealer), x => true)
                .WhereIf(currentUser.TenantId.HasValue && await _featureChecker.IsEnabledAsync(AppFeatures.Carrier), x => x.RoutPointFk.ShippingRequestTripFk.ShippingRequestFk.CarrierTenantId == currentUser.TenantId.Value)
                .WhereIf(currentUser.IsDriver, x => x.RoutPointFk.ShippingRequestTripFk.AssignedDriverUserId == currentUser.Id)
                .FirstOrDefaultAsync();


            //var CurrentPoint = await _routPointRepository.GetAll()
            //    .Where(x =>x.Id == id && x.Status == RoutePointStatus.DeliveryConfirmation)
            //    .WhereIf(!currentUser.TenantId.HasValue || await _featureChecker.IsEnabledAsync(AppFeatures.TachyonDealer), x => true)
            //    .WhereIf(currentUser.TenantId.HasValue && await _featureChecker.IsEnabledAsync(AppFeatures.Carrier), x => x.ShippingRequestTripFk.ShippingRequestFk.CarrierTenantId == currentUser.TenantId.Value)
            //    .WhereIf(currentUser.IsDriver, x => x.ShippingRequestTripFk.AssignedDriverUserId == currentUser.Id)
            //    .FirstOrDefaultAsync();
            if (document == null) throw new UserFriendlyException(L("TheRoutePointIsNotFound"));



            return await _commonManager.GetDocument(ObjectMapper.Map<IHasDocument>(document));


        }
        #region Helper
        /// <summary>
        /// Get list of users can send notification by SignalR to update the data if any of users tracking the trip
        /// </summary>
        /// <param name="trip">trip info</param>
        /// <param name="user">current user</param>
        /// <returns></returns>
        private async Task<List<string>> GetUsersHubNotification(ShippingRequestTrip trip, User user)
        {
            List<string> userIds = new List<string>();
            userIds.Add(trip.ShippingRequestFk.CreatorUserId.ToString());
            userIds.Add((await UserManager.GetAdminByTenantIdAsync(trip.ShippingRequestFk.CarrierTenantId.Value)).Id.ToString());
            if (trip.ShippingRequestFk.IsTachyonDeal)
            {
                userIds.Add((await UserManager.GetAdminTachyonDealerAsync()).Id.ToString());
                userIds.Add((await UserManager.GetAdminHostAsync()).Id.ToString());
            }

            return userIds;
        }

        public async Task GeneratePrices(ShippingRequestTrip trip)
        {
            DisableTenancyFilters();
            var offer = await _priceOfferManager.GetOffercceptedByShippingRequestId(trip.ShippingRequestId);

            trip.CommissionType = offer.CommissionType;
            trip.SubTotalAmount = offer.ItemPrice;
            trip.VatAmount = offer.ItemVatAmount;
            trip.TotalAmount = offer.ItemTotalAmount;
            trip.SubTotalAmountWithCommission = offer.ItemSubTotalAmountWithCommission;
            trip.VatAmountWithCommission = offer.ItemVatAmountWithCommission;
            trip.TotalAmountWithCommission = offer.ItemTotalAmountWithCommission;
            trip.CommissionAmount = offer.ItemCommissionAmount;
            trip.CommissionPercentageOrAddValue = offer.CommissionPercentageOrAddValue;
            trip.TaxVat = offer.TaxVat;
            if (trip.ShippingRequestTripVases == null || trip.ShippingRequestTripVases.Count == 0) return;
            foreach (var vas in trip.ShippingRequestTripVases)
            {
                var item = offer.PriceOfferDetails.FirstOrDefault(x => x.SourceId == vas.ShippingRequestVasId && x.PriceType == PriceOfferType.Vas);
                vas.CommissionType = item.CommissionType;
                vas.SubTotalAmount = item.ItemPrice;
                vas.VatAmount = item.ItemVatAmount;
                vas.TotalAmount = item.ItemTotalAmount;
                vas.SubTotalAmountWithCommission = item.ItemSubTotalAmountWithCommission;
                vas.VatAmountWithCommission = item.ItemVatAmountWithCommission;
                vas.TotalAmountWithCommission = item.ItemTotalAmountWithCommission;
                vas.CommissionPercentageOrAddValue = item.CommissionPercentageOrAddValue;
                vas.CommissionAmount = item.ItemCommissionAmount;
                vas.Quantity = 1;
            }

        }


        /// <summary>
        /// Check the trip can accpted or not by status
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        private async Task<ShippingRequestTrip> CheckIfCanAccepted(int id, User user)
        {
            var trip = await _shippingRequestTrip
                            .GetAllIncluding(o => o.OriginFacilityFk, d => d.DestinationFacilityFk, x => x.ShippingRequestFk, x => x.ShippingRequestTripVases)
                            .Where(x => x.Id == id && x.ShippingRequestFk.CarrierTenantId.HasValue && x.ShippingRequestFk.Status != ShippingRequests.ShippingRequestStatus.Cancled && x.AssignedDriverUserId.HasValue)
                            .WhereIf(!user.TenantId.HasValue || await _featureChecker.IsEnabledAsync(AppFeatures.TachyonDealer), x => x.ShippingRequestFk.IsTachyonDeal)
                            .WhereIf(user.TenantId.HasValue && await _featureChecker.IsEnabledAsync(AppFeatures.Carrier), x => x.ShippingRequestFk.CarrierTenantId == user.TenantId.Value)
                            .WhereIf(user.IsDriver, x => x.AssignedDriverUserId == user.Id)
                            .FirstOrDefaultAsync(t => t.DriverStatus == ShippingRequestTripDriverStatus.None && (t.Status == ShippingRequestTripStatus.New ||
                            // new driver "changed" can accept trip
                            t.Status == ShippingRequestTripStatus.Intransit));
            if (trip == null) throw new UserFriendlyException(L("TheTripIsNotFound"));
            return trip;
        }


        /// <summary>
        /// When start pikup way
        /// </summary>
        /// <param name="routPoint">first location</param>
        /// <param name="FromLocation">location map address from start rout</param>
        /// <returns></returns>
        private async Task StartTransition(RoutPoint routPoint, Point FromLocation)
        {
            FromLocation.SRID = 4326;
            ShippingRequestTripTransition tripTransition = new ShippingRequestTripTransition();
            tripTransition.FromLocation = FromLocation;
            tripTransition.ToPointId = routPoint.Id;
            tripTransition.ToLocation = routPoint.FacilityFk.Location;
            await _shippingRequestTripTransitionRepository.InsertAsync(tripTransition);
            await SetRoutStatusTransition(routPoint);

        }

        /// <summary>
        /// Change transition from old point to new next point
        /// </summary>
        /// <param name="routPoint"></param>
        /// <returns></returns>
        public async Task ChangeTransition(RoutPoint routPoint)
        {
            ShippingRequestTripTransition tripTransition = new ShippingRequestTripTransition();

            var oldPointTransition = await GetLastTransitionInComplete(routPoint.ShippingRequestTripId);
            if (oldPointTransition != null)
            {
                oldPointTransition.IsComplete = true;

                tripTransition.FromLocation = oldPointTransition.ToLocation;
                tripTransition.FromPointId = oldPointTransition.ToPointId;
                tripTransition.ToPointId = routPoint.Id;
                tripTransition.ToLocation = routPoint.FacilityFk.Location;

                await _shippingRequestTripTransitionRepository.InsertAsync(tripTransition);
            }
            await SetRoutStatusTransition(routPoint);

        }
        /// <summary>
        /// Set up route point transition
        /// </summary>
        /// <param name="routPoint"></param>
        /// <param name="Status"></param>
        /// <returns></returns>
        private async Task SetRoutStatusTransition(RoutPoint routPoint)
        {
            await _routPointStatusTransitionRepository.InsertAsync(new RoutPointStatusTransition
            {
                PointId = routPoint.Id,
                Status = routPoint.Status
            });
        }


        /// <summary>
        /// Get the last transition for trip is not complete
        /// </summary>
        /// <param name="TripId"></param>
        /// <returns></returns>
        private async Task<ShippingRequestTripTransition> GetLastTransitionInComplete(int TripId)
        {
            return await _shippingRequestTripTransitionRepository.FirstOrDefaultAsync(x => x.ToPoint.ShippingRequestTripId == TripId && !x.IsComplete);

        }


        /// <summary>
        /// Send shipment code to receivers
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public async Task SendSmsToReceivers(int tripId)
        {
            var RoutePoints = await _routPointRepository.GetAll().Include(x => x.ReceiverFk).Where(x => x.ShippingRequestTripId == tripId && x.PickingType == Routs.RoutPoints.PickingType.Dropoff).ToListAsync();
            RoutePoints.ForEach(async p =>
            {
                await SendSmsToReceiver(p);
            });

        }


        /// <summary>
        /// Send shipment code to receiver
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public async Task SendSmsToReceiver(RoutPoint point)
        {
            string number = point.ReceiverPhoneNumber;
            string formattedDate = point.EndTime?.ToString("dd/MM/yyyy");
            string message = L(TACHYONConsts.SMSShippingRequestReceiverCode, point.WaybillNumber, formattedDate ?? L("NotSet"), point.Code);
            if (point.ReceiverFk != null)
            {
                number = point.ReceiverFk.PhoneNumber;
            }
            await _smsSender.SendAsync(number, message);

        }
        /// <summary>
        /// Check if the new point location can be started if not because there another poing still not completed
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>

        private async Task CanGoToNextLocation(RoutPoint point)
        {
            var Count = await _routPointRepository
                .GetAll()
                .Where
                (
                    x => x.ShippingRequestTripId == point.ShippingRequestTripId &&
                       //x.Status != RoutePointStatus.StandBy 
                       //check if another point in trip is active
                       (x.Id != point.Id && x.IsActive && x.PickingType == PickingType.Dropoff ||
             //check if the selected point is completed or active, wether it pickup or drop
             (x.Id == point.Id && (x.IsComplete || x.IsActive)))
                //!x.IsComplete
                //x.IsActive
                )
                .CountAsync();

            if (Count > 0) throw new UserFriendlyException(L("ThereIsAnotherActivePointStillNotClose"));
        }

        /// <summary>
        /// Singlar notifcation when the route point status changed
        /// </summary>
        /// <param name="point"></param>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        public async Task NotificationWhenPointChanged(RoutPoint point, User currentUser)
        {
            var trip = point.ShippingRequestTripFk;
            await _hubContext.Clients.Users(await GetUsersHubNotification(trip, currentUser)).SendAsync("tracking", TACHYONConsts.TriggerTrackingChanged, ObjectMapper.Map<ShippingRequestTripDriverRoutePointDto>(point));
            if (!currentUser.IsDriver) await _firebaseNotifier.TripChanged(new Abp.UserIdentifier(trip.ShippingRequestFk.CarrierTenantId.Value, trip.AssignedDriverUserId.Value), trip.Id.ToString());
        }

        /// <summary>
        /// Singlar notifcation when the shipment delivered
        /// </summary>
        /// <param name="point"></param>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        public async Task NotificationWhenShipmentDelivered(RoutPoint point, User currentUser)
        {
            var trip = point.ShippingRequestTripFk;
            await _hubContext.Clients.Users(await GetUsersHubNotification(trip, currentUser)).SendAsync("tracking", TACHYONConsts.TriggerTrackingShipmentDelivered, ObjectMapper.Map<ShippingRequestTripDriverRoutePointDto>(point));
            if (!currentUser.IsDriver) await _firebaseNotifier.TripChanged(new Abp.UserIdentifier(trip.ShippingRequestFk.CarrierTenantId.Value, trip.AssignedDriverUserId.Value), trip.Id.ToString());
        }



        /// <summary>
        /// Call when the trip is finish
        /// </summary>
        /// <param name="RequestId"></param>
        /// <param name="TripId"></param>
        /// <returns></returns>
        private async Task Done(ShippingRequestTrip trip)
        {
            await ChangeShippingRequestStatusIfAllTripsDone(trip);
            await CloseLastTransitionInComplete(trip.Id);
        }

        public async Task ChangeShippingRequestStatusIfAllTripsDone(ShippingRequestTrip trip)
        {
            if (!_shippingRequestTrip.GetAll().Any(x => x.Status != ShippingRequestTripStatus.Delivered && x.ShippingRequestId == trip.ShippingRequestId))
            {
                trip.ShippingRequestFk.Status = ShippingRequestStatus.Completed;
                await _appNotifier.ShipperShippingRequestFinish(new UserIdentifier(trip.ShippingRequestFk.TenantId, trip.ShippingRequestFk.CreatorUserId.Value), trip.ShippingRequestFk);
            }
        }

        /// <summary>
        /// Close the last transition for trip by set field IsComplete to value true
        /// </summary>
        /// <param name="TripId"></param>
        /// <returns></returns>
        private async Task CloseLastTransitionInComplete(int TripId)
        {
            var Last = await GetLastTransitionInComplete(TripId);
            if (Last != null) Last.IsComplete = true;

        }
        #endregion

        protected virtual async Task<User> GetCurrentUserAsync(IAbpSession abpSession)
        {
            var user = await UserManager.FindByIdAsync(abpSession.GetUserId().ToString());
            if (user == null)
            {
                throw new Exception("There is no current user!");
            }

            return user;
        }
    }
}