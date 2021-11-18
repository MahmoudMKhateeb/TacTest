using Abp;
using Abp.Application.Features;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Linq.Extensions;
using Abp.Runtime.Session;
using Abp.Timing;
using Abp.UI;
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
using TACHYON.PriceOffers;
using TACHYON.Routs.RoutPoints;
using TACHYON.Shipping.Drivers.Dto;
using TACHYON.Shipping.RoutPoints;
using TACHYON.Shipping.ShippingRequests;
using TACHYON.Shipping.ShippingRequestTrips;
using TACHYON.Shipping.Trips;
using TACHYON.Shipping.Trips.Dto;
using TACHYON.Tracking.Dto;
using TACHYON.Url;
using TACHYON.WorkFlows;

namespace TACHYON.Tracking
{
    /// <summary>
    /// </summary>
    /// <typeparam name="TArgs">transaction args object warper</typeparam>
    /// <typeparam name="TEnum">statuses Enum</typeparam>
    public class ShippingRequestPointWorkFlowProvider : TACHYONDomainServiceBase, IWorkFlow<PointTransactionArgs, RoutePointStatus>
    {
        public List<WorkFlow<PointTransactionArgs, RoutePointStatus>> Flows { get; set; }
        private readonly IRepository<RoutPoint, long> _routPointRepository;
        private readonly IRepository<ShippingRequestTrip> _shippingRequestTrip;
        private readonly PriceOfferManager _priceOfferManager;
        private readonly IAppNotifier _appNotifier;
        private readonly IFeatureChecker _featureChecker;
        private readonly IAbpSession _abpSession;
        private readonly IRepository<RoutPointDocument, long> _routPointDocumentRepository;
        private readonly IRepository<ShippingRequestTripTransition> _shippingRequestTripTransitionRepository;
        private readonly IRepository<RoutPointStatusTransition> _routPointStatusTransitionRepository;
        private readonly FirebaseNotifier _firebaseNotifier;
        private readonly ISmsSender _smsSender;
        private readonly InvoiceManager _invoiceManager;
        private readonly CommonManager _commonManager;
        private readonly UserManager UserManager;
        private readonly IWebUrlService _webUrlService;

        #region Constractor
        public ShippingRequestPointWorkFlowProvider(IRepository<RoutPoint, long> routPointRepository, IRepository<ShippingRequestTrip> shippingRequestTrip, PriceOfferManager priceOfferManager, IAppNotifier appNotifier, IFeatureChecker featureChecker, IAbpSession abpSession, IRepository<RoutPointDocument, long> routPointDocumentRepository, IRepository<ShippingRequestTripTransition> shippingRequestTripTransitionRepository, IRepository<RoutPointStatusTransition> routPointStatusTransitionRepository, FirebaseNotifier firebaseNotifier, ISmsSender smsSender, InvoiceManager invoiceManager, CommonManager commonManager, UserManager userManager, IWebUrlService webUrlService)
        {
            _routPointRepository = routPointRepository;
            _shippingRequestTrip = shippingRequestTrip;
            _priceOfferManager = priceOfferManager;
            _appNotifier = appNotifier;
            _featureChecker = featureChecker;
            _abpSession = abpSession;
            _routPointDocumentRepository = routPointDocumentRepository;
            _shippingRequestTripTransitionRepository = shippingRequestTripTransitionRepository;
            _routPointStatusTransitionRepository = routPointStatusTransitionRepository;
            _firebaseNotifier = firebaseNotifier;
            _smsSender = smsSender;
            _invoiceManager = invoiceManager;
            _commonManager = commonManager;
            UserManager = userManager;
            _webUrlService = webUrlService;
            Flows = new List<WorkFlow<PointTransactionArgs, RoutePointStatus>>
            {
            new WorkFlow<PointTransactionArgs,RoutePointStatus>
                {
                    Version = 0,
                    Transactions = new List<WorkflowTransaction<PointTransactionArgs,RoutePointStatus>>
                    {
                        new WorkflowTransaction<PointTransactionArgs,RoutePointStatus>
                        {
                            Action =  WorkFlowActionConst.StartedMovingToOfLoadingLocation,
                            FromStatus = RoutePointStatus.StandBy,
                            ToStatus = RoutePointStatus.StartedMovingToOfLoadingLocation,
                            Func = StartedMovingToOfLoadingLocation,
                        },
                        new WorkflowTransaction<PointTransactionArgs,RoutePointStatus>
                        {
                            Action =  WorkFlowActionConst.ArrivedToDestination,
                            FromStatus = RoutePointStatus.StartedMovingToOfLoadingLocation,
                            ToStatus = RoutePointStatus.ArrivedToDestination,
                            Func = ArrivedToDestination,
                        },
                        new WorkflowTransaction<PointTransactionArgs,RoutePointStatus>
                        {
                            Action =  WorkFlowActionConst.StartOffloading,
                            FromStatus = RoutePointStatus.ArrivedToDestination,
                            ToStatus = RoutePointStatus.StartOffloading,
                            Func = StartOffloading,
                        },
                        new WorkflowTransaction<PointTransactionArgs,RoutePointStatus>
                        {
                            Action =  WorkFlowActionConst.FinishOffLoadShipment,
                            FromStatus = RoutePointStatus.StartOffloading,
                            ToStatus = RoutePointStatus.FinishOffLoadShipment,
                            Func = FinishOffLoadShipment,
                        },
                        new WorkflowTransaction<PointTransactionArgs,RoutePointStatus>
                        {
                            Action =  WorkFlowActionConst.ReceiverConfirmed,
                            FromStatus = RoutePointStatus.FinishOffLoadShipment,
                            ToStatus = RoutePointStatus.ReceiverConfirmed,
                            Func = ReceiverConfirmed,
                        },
                        new WorkflowTransaction<PointTransactionArgs,RoutePointStatus>
                        {
                            Action =  WorkFlowActionConst.DeliveryConfirmation,
                            FromStatus = RoutePointStatus.FinishOffLoadShipment,
                            ToStatus = RoutePointStatus.DeliveryConfirmation,
                            Func = ReceiverConfirmed,
                        },
                        new WorkflowTransaction<PointTransactionArgs,RoutePointStatus>
                        {
                            Action =  WorkFlowActionConst.DeliveryConfirmation,
                            FromStatus = RoutePointStatus.ReceiverConfirmed,
                            ToStatus = RoutePointStatus.DeliveryConfirmation,
                            Func = DeliveryConfirmation,
                        },
                        new WorkflowTransaction<PointTransactionArgs,RoutePointStatus>
                        {
                            Action =  WorkFlowActionConst.DeliveryConfirmation,
                            FromStatus = RoutePointStatus.ReceiverConfirmed,
                            ToStatus = RoutePointStatus.DeliveryConfirmation,
                            Func = DeliveryConfirmation,
                        },
                    },
                },
            new WorkFlow<PointTransactionArgs,RoutePointStatus>
                {
                    Version = 1,
                    Transactions = new List<WorkflowTransaction<PointTransactionArgs,RoutePointStatus>>
                    {
                        new WorkflowTransaction<PointTransactionArgs,RoutePointStatus>
                        {
                            Action =  WorkFlowActionConst.StartedMovingToLoadingLocation,
                            FromStatus = RoutePointStatus.StandBy,
                            ToStatus = RoutePointStatus.StartedMovingToLoadingLocation,
                            Func = StartedMovingToLoadingLocation,
                        },
                        new WorkflowTransaction<PointTransactionArgs,RoutePointStatus>
                        {
                            Action =  WorkFlowActionConst.ArriveToLoadingLocation,
                            FromStatus = RoutePointStatus.StartedMovingToLoadingLocation,
                            ToStatus = RoutePointStatus.ArriveToLoadingLocation,
                            Func = ArriveToLoadingLocation,
                        },
                        new WorkflowTransaction<PointTransactionArgs,RoutePointStatus>
                        {
                            Action =  WorkFlowActionConst.StartLoading,
                            FromStatus = RoutePointStatus.ArriveToLoadingLocation,
                            ToStatus = RoutePointStatus.StartLoading,
                            Func = StartLoading,
                        },
                        new WorkflowTransaction<PointTransactionArgs,RoutePointStatus>
                        {
                            Action =  WorkFlowActionConst.FinishLoading,
                            FromStatus = RoutePointStatus.StartLoading,
                            ToStatus = RoutePointStatus.FinishLoading,
                            Func =  FinishLoading,
                        },
                    },
                },
            };
        }

        #endregion

        #region Main Functions
        public List<WorkflowTransaction<PointTransactionArgs, RoutePointStatus>> GetTransactions(int workFlowVersion)
        {
            return Flows
                .FirstOrDefault(c => c.Version == workFlowVersion)?
                .Transactions
                .ToList();
        }
        public List<WorkflowTransaction<PointTransactionArgs, RoutePointStatus>> GetAvailableTransactions(int workFlowVersion, RoutePointStatus status)
        {
            return Flows
                .FirstOrDefault(c => c.Version == workFlowVersion)?
                .Transactions.Where(t => t.FromStatus == status).ToList();
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
            //await _hubContext.Clients.Users(await GetUsersHubNotification(trip, currentUser)).SendAsync("tracking", TACHYONConsts.TriggerTrackingAccepted, ObjectMapper.Map<TrackingListDto>(trip));
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
                (x =>
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
                trip.Status = ShippingRequestTripStatus.Intransit;
                trip.StartTripDate = Clock.Now;
                await StartTransition(RouteStart, new Point(Input.lat, Input.lng));
                //await _hubContext.Clients.Users(await GetUsersHubNotification(trip, currentUser)).SendAsync("tracking", TACHYONConsts.TriggerTrackingStarted, ObjectMapper.Map<TrackingListDto>(trip));
                if (!currentUser.IsDriver) await _firebaseNotifier.TripChanged(new Abp.UserIdentifier(trip.ShippingRequestFk.CarrierTenantId.Value, trip.AssignedDriverUserId.Value), trip.Id.ToString());
            }
            else
            {
                throw new UserFriendlyException(L("YouCanNotStartNewTripWhenYouHaveAnotherTripStillNotFinish"));
            }
        }

        [UnitOfWork]
        public async Task Invoke(InvokeStatusInputDto input)
        {

            var currentUser = await GetCurrentUserAsync(_abpSession);
            var point = await GetRoutPointForAction(input.Id, currentUser);
            if (point == null) throw new UserFriendlyException(L("YouCanNotChangeTheStatus"));
            var transaction = CheckIfTransactionIsExist(point, input.Action);

            if (!string.IsNullOrEmpty(input.Code) && point.Code != input.Code)
                throw new UserFriendlyException(L("TheReceiverCodeIsIncorrect"));

            var trip = point.ShippingRequestTripFk;
            var args = new PointTransactionArgs
            {
                Point = point,
                Trip = trip,
            };

            transaction.Func(args);

            await SetRoutStatusTransition(point);
            await NotificationWhenPointChanged(point, currentUser);
        }
        public async Task GotoNextLocation(long id)
        {
            DisableTenancyFilters();
            var currentUser = await GetCurrentUserAsync(_abpSession);
            var currentpoint = await _routPointRepository
                .GetAll()
                .Include(c => c.ShippingRequestTripFk)
                .ThenInclude(s => s.ShippingRequestFk)
                .Include(x => x.FacilityFk)
                .Where(x => x.Status == RoutePointStatus.StandBy
                && x.ShippingRequestTripFk.Status == ShippingRequestTripStatus.Intransit
                && x.PickingType == Routs.RoutPoints.PickingType.Dropoff)
                .WhereIf(!currentUser.TenantId.HasValue || await _featureChecker.IsEnabledAsync(AppFeatures.TachyonDealer), x => x.ShippingRequestTripFk.ShippingRequestFk.IsTachyonDeal)
                .WhereIf(currentUser.TenantId.HasValue && await _featureChecker.IsEnabledAsync(AppFeatures.Carrier), x => x.ShippingRequestTripFk.ShippingRequestFk.CarrierTenantId == currentUser.TenantId.Value)
                .WhereIf(currentUser.IsDriver, x => x.ShippingRequestTripFk.AssignedDriverUserId == currentUser.Id)
                .FirstOrDefaultAsync();
            var activePoint = await GetActivePointByTripAsyn(id, currentUser);
            if (currentpoint == null) throw new UserFriendlyException(L("TheLocationSelectedIsNotFound"));
            if (activePoint == null) throw new UserFriendlyException(L("NoActivePoint"));
            // deactivate active point
            if (activePoint.IsComplete)
            {
                activePoint.IsActive = false;
                activePoint.EndTime = Clock.Now;
            }
            currentpoint.StartTime = Clock.Now;
            currentpoint.IsActive = true;

            await ChangeTransition(currentpoint);
            await NotificationWhenPointChanged(currentpoint, currentUser);
        }

        /// <summary>
        /// driver confirm the trip has finished 
        /// </summary>
        /// <param name="document"> attachment</param>
        /// <param name="id">route point id</param>
        /// <returns></returns>
        public async Task<bool> ConfirmPointToDelivery(IHasDocument document, InvokeStatusInputDto input)
        {
            DisableTenancyFilters();
            var currentUser = await GetCurrentUserAsync(_abpSession);

            var currentPoint = await GetRoutPointForAction(input.Id, currentUser);
            if (currentPoint == null) throw new UserFriendlyException(L("TheRoutePointIsNotFound"));
            var transction = CheckIfTransactionIsExist(currentPoint, input.Action);

            var routePointDocument = new RoutPointDocument();
            routePointDocument.RoutPointId = currentPoint.Id;
            routePointDocument.DocumentContentType = "image/jpeg";
            routePointDocument.DocumentName = document.DocumentName;
            routePointDocument.DocumentId = document.DocumentId;
            routePointDocument.RoutePointDocumentType = RoutePointDocumentType.POD;
            await _routPointDocumentRepository.InsertAsync(routePointDocument);
            var isCompleted = CheckIfPointIsCompleted(currentPoint);
            if (isCompleted)
            {
                currentPoint.IsActive = false;
                currentPoint.IsComplete = true;
                currentPoint.EndTime = Clock.Now;
            }
            currentPoint.Status = transction.ToStatus;
            await SetRoutStatusTransition(currentPoint);

            var trip = currentPoint.ShippingRequestTripFk;
            var allPointsCompleted = await _routPointRepository.GetAll()
                .Where(x => x.ShippingRequestTripId == trip.Id && x.IsComplete == false && x.Id != currentPoint.Id)
                .CountAsync() == 0;
            if (isCompleted && allPointsCompleted)
            {
                trip.Status = ShippingRequestTripStatus.Delivered;
                trip.RoutePointStatus = RoutePointStatus.Delivered;
                trip.EndTripDate = Clock.Now;

                await Done(trip);
                await _invoiceManager.GenertateInvoiceWhenShipmintDelivery(trip);
                await NotificationWhenShipmentDelivered(currentPoint, currentUser);
            }
            else
            {
                trip.RoutePointStatus = transction.ToStatus;
                await NotificationWhenPointChanged(currentPoint, currentUser);
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

            if (document == null) throw new UserFriendlyException(L("TheRoutePointIsNotFound"));
            return await _commonManager.GetDocument(ObjectMapper.Map<IHasDocument>(document));
        }
        #endregion

        #region Transactions Functions
        private void StartedMovingToLoadingLocation(PointTransactionArgs args)
        {
            var status = RoutePointStatus.StartedMovingToLoadingLocation;
            args.Point.Status = status;
            args.Trip.RoutePointStatus = status;
        }
        private void ArriveToLoadingLocation(PointTransactionArgs args)
        {
            var status = RoutePointStatus.ArriveToLoadingLocation;
            args.Point.Status = status;
            args.Trip.RoutePointStatus = status;
        }
        private void StartLoading(PointTransactionArgs args)
        {
            var status = RoutePointStatus.StartLoading;
            args.Point.Status = status;
            args.Trip.RoutePointStatus = status;
        }
        private void FinishLoading(PointTransactionArgs args)
        {
            var status = RoutePointStatus.FinishLoading;
            args.Point.Status = status;
            args.Trip.RoutePointStatus = status;
            args.Point.IsComplete = true;
            args.Point.IsResolve = true;
            args.Point.EndTime = Clock.Now;
            args.Point.ActualPickupOrDeliveryDate = args.Trip.ActualPickupDate = Clock.Now;
            Task.Run(async () => await SendSmsToReceivers(args.Trip.Id));
        }
        private void StartedMovingToOfLoadingLocation(PointTransactionArgs args)
        {
            var status = RoutePointStatus.StartedMovingToOfLoadingLocation;
            args.Point.Status = status;
            args.Trip.RoutePointStatus = status;
        }
        private void ArrivedToDestination(PointTransactionArgs args)
        {
            var status = RoutePointStatus.ArrivedToDestination;
            args.Point.Status = status;
            args.Trip.RoutePointStatus = status;
        }
        private void StartOffloading(PointTransactionArgs args)
        {
            var status = RoutePointStatus.StartOffloading;
            args.Point.Status = status;
            args.Trip.RoutePointStatus = status;
        }
        private void FinishOffLoadShipment(PointTransactionArgs args)
        {
            var status = RoutePointStatus.FinishOffLoadShipment;
            args.Point.Status = status;
            args.Trip.RoutePointStatus = status;
            args.Point.ActualPickupOrDeliveryDate = Clock.Now;
            if (!args.Trip.RoutPoints.Any(x => x.ActualPickupOrDeliveryDate == null && x.Id != args.Point.Id))
                args.Trip.ActualDeliveryDate = Clock.Now;
        }
        private void ReceiverConfirmed(PointTransactionArgs args)
        {
            var code = args.ConfirmationCode;
            var status = RoutePointStatus.ReceiverConfirmed;
            args.Point.Status = status;
            args.Trip.RoutePointStatus = status;
        }
        private void DeliveryConfirmation(PointTransactionArgs args)
        {
            var status = RoutePointStatus.DeliveryConfirmation;
            args.Point.Status = status;
            args.Trip.RoutePointStatus = status;
        }
        #endregion

        #region Helpers
        private async Task<RoutPoint> GetActivePointByTripAsyn(long pointId, User currentUser)
        {
            return await _routPointRepository
                .GetAll()
                .Where(x => x.IsActive && x.IsResolve &&
                x.ShippingRequestTripFk.Status == ShippingRequestTripStatus.Intransit &&
                x.Id == pointId)
                .WhereIf(!currentUser.TenantId.HasValue || await _featureChecker.IsEnabledAsync(AppFeatures.TachyonDealer), x => x.ShippingRequestTripFk.ShippingRequestFk.IsTachyonDeal)
                .WhereIf(currentUser.TenantId.HasValue && await _featureChecker.IsEnabledAsync(AppFeatures.Carrier), x => x.ShippingRequestTripFk.ShippingRequestFk.CarrierTenantId == currentUser.TenantId.Value)
                .WhereIf(currentUser.IsDriver, x => x.ShippingRequestTripFk.AssignedDriverUserId == currentUser.Id)
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// get Rout Point to take action
        /// </summary>
        /// <param name="pointId"> point id</param>
        /// <param name="currentUser">current User login</param>
        private async Task<RoutPoint> GetRoutPointForAction(long pointId, User currentUser)
        {
            DisableTenancyFilters();
            return await _routPointRepository
                          .GetAll()
                          .Include(t => t.ShippingRequestTripFk)
                          .ThenInclude(s => s.ShippingRequestFk)
                          .Where(
                              x => x.Id == pointId &&
                              x.ShippingRequestTripFk.ShippingRequestFk.Status == ShippingRequestStatus.PostPrice &&
                             !x.IsComplete)
                          .WhereIf(!currentUser.TenantId.HasValue || await _featureChecker.IsEnabledAsync(AppFeatures.TachyonDealer), x => x.ShippingRequestTripFk.ShippingRequestFk.IsTachyonDeal)
                          .WhereIf(currentUser.TenantId.HasValue && await _featureChecker.IsEnabledAsync(AppFeatures.Carrier), x => x.ShippingRequestTripFk.ShippingRequestFk.CarrierTenantId == currentUser.TenantId.Value)
                          .WhereIf(currentUser.IsDriver, x => x.ShippingRequestTripFk.AssignedDriverUserId == currentUser.Id)
                          .FirstOrDefaultAsync(x => x.IsActive &&
                          x.ShippingRequestTripFk.Status == ShippingRequestTripStatus.Intransit);
        }
        private bool CheckIfPointIsCompleted(RoutPoint point)
        {
            var statuses = GetTransactions(point.WorkFlowVersion)
                .Select(x => x.ToStatus);
            var transitions = _routPointStatusTransitionRepository.GetAll().Where(x => x.PointId == point.Id)
                .Select(c => c.Status).ToList();
            return !statuses.Except(transitions).Any();
        }

        private WorkflowTransaction<PointTransactionArgs, RoutePointStatus> CheckIfTransactionIsExist(RoutPoint point, string action)
        {
            var workFlow = Flows.FirstOrDefault(w => w.Version == point.WorkFlowVersion);
            if (workFlow == null) throw new UserFriendlyException(L("YouCanNotChangeTheStatus"));
            // check if the transction is exist and is available for currnet point status
            var transaction = workFlow.Transactions.FirstOrDefault(c => c.Action.Equals(action) && c.FromStatus == point.Status);
            if (transaction == null) throw new UserFriendlyException(L("YouCanNotChangeTheStatus"));
            return transaction;
        }

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

        private async Task GeneratePrices(ShippingRequestTrip trip)
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
                            .Where(x => x.Id == id && x.ShippingRequestFk.CarrierTenantId.HasValue && x.ShippingRequestFk.Status != ShippingRequestStatus.Cancled && x.AssignedDriverUserId.HasValue)
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
        private async Task SendSmsToReceivers(int tripId)
        {
            var RoutePoints = await _routPointRepository.GetAll().Include(x => x.ReceiverFk)
                .Where(x => x.ShippingRequestTripId == tripId && x.PickingType == Routs.RoutPoints.PickingType.Dropoff)
                .ToListAsync();
            foreach (var RoutePoint in RoutePoints)
            {
                await SendSmsToReceiver(RoutePoint);
            }

        }

        /// <summary>
        /// Send shipment code to receiver
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        private async Task SendSmsToReceiver(RoutPoint point)
        {
            string number = point.ReceiverPhoneNumber;
            var ratingLink = $"{L("ClickToRate")} {_webUrlService.WebSiteRootAddressFormat}account/RatingPage/{point.Code}";
            string message = L(TACHYONConsts.SMSShippingRequestReceiverCode, point.WaybillNumber, point.Code, ratingLink);
            if (point.ReceiverFk != null)
            {
                number = point.ReceiverFk.PhoneNumber;
            }
            await _smsSender.SendAsync(number, message);

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
        private async Task CloseLastTransitionInComplete(int tripId)
        {
            var Last = await GetLastTransitionInComplete(tripId);
            if (Last != null) Last.IsComplete = true;
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
        protected virtual async Task<User> GetCurrentUserAsync(IAbpSession abpSession)
        {
            var user = await UserManager.FindByIdAsync(abpSession.GetUserId().ToString());
            if (user == null)
            {
                throw new Exception("There is no current user!");
            }

            return user;
        }
        #endregion

        #region Notfications

        /// <summary>
        /// Singlar notifcation when the route point status changed
        /// </summary>
        /// <param name="point"></param>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        public async Task NotificationWhenPointChanged(RoutPoint point, User currentUser)
        {
            var trip = point.ShippingRequestTripFk;
            //await _hubContext.Clients.Users(await GetUsersHubNotification(trip, currentUser)).SendAsync("tracking", TACHYONConsts.TriggerTrackingChanged, ObjectMapper.Map<ShippingRequestTripDriverRoutePointDto>(point));
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
            //await _hubContext.Clients.Users(await GetUsersHubNotification(trip, currentUser)).SendAsync("tracking", TACHYONConsts.TriggerTrackingShipmentDelivered, ObjectMapper.Map<ShippingRequestTripDriverRoutePointDto>(point));
            if (!currentUser.IsDriver) await _firebaseNotifier.TripChanged(new Abp.UserIdentifier(trip.ShippingRequestFk.CarrierTenantId.Value, trip.AssignedDriverUserId.Value), trip.Id.ToString());
        }
        #endregion
    }
}