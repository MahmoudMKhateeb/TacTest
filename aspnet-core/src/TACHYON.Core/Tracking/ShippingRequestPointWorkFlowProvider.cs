using Abp;
using Abp.Application.Features;
using Abp.Authorization;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Linq.Extensions;
using Abp.Runtime.Session;
using Abp.Specifications;
using Abp.Timing;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TACHYON.Authorization;
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
using TACHYON.Tracking.Dto.WorkFlow;
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
        private readonly IRepository<ShippingRequestTrip> _shippingRequestTripRepository;
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
        private readonly IPermissionChecker _permissionChecker;


        #region Constractor
        public ShippingRequestPointWorkFlowProvider(IRepository<RoutPoint, long> routPointRepository, IRepository<ShippingRequestTrip> shippingRequestTrip, PriceOfferManager priceOfferManager, IAppNotifier appNotifier, IFeatureChecker featureChecker, IAbpSession abpSession, IRepository<RoutPointDocument, long> routPointDocumentRepository, IRepository<ShippingRequestTripTransition> shippingRequestTripTransitionRepository, IRepository<RoutPointStatusTransition> routPointStatusTransitionRepository, FirebaseNotifier firebaseNotifier, ISmsSender smsSender, InvoiceManager invoiceManager, CommonManager commonManager, UserManager userManager, IWebUrlService webUrlService, IPermissionChecker permissionChecker)
        {
            _routPointRepository = routPointRepository;
            _shippingRequestTripRepository = shippingRequestTrip;
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
            _permissionChecker = permissionChecker;
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
                            Name = L("StartedMovingToOfLoadingLocation"),
                            Permissions = new List<string>{},
                            Features = new List<string>{},
                        },
                        new WorkflowTransaction<PointTransactionArgs,RoutePointStatus>
                        {
                            Action =  WorkFlowActionConst.ArrivedToDestination,
                            FromStatus = RoutePointStatus.StartedMovingToOfLoadingLocation,
                            ToStatus = RoutePointStatus.ArrivedToDestination,
                            Func = ArrivedToDestination,
                            Name = L("ArrivedToDestination"),
                            Permissions = new List<string>{},
                            Features = new List<string>{},
                        },
                        new WorkflowTransaction<PointTransactionArgs,RoutePointStatus>
                        {
                            Action =  WorkFlowActionConst.StartOffloading,
                            FromStatus = RoutePointStatus.ArrivedToDestination,
                            ToStatus = RoutePointStatus.StartOffloading,
                            Func = StartOffloading,
                            Name = L("StartOffloading"),
                            Permissions = new List<string>{},
                            Features = new List<string>{},
                        },
                        new WorkflowTransaction<PointTransactionArgs,RoutePointStatus>
                        {
                            Action =  WorkFlowActionConst.FinishOffLoadShipment,
                            FromStatus = RoutePointStatus.StartOffloading,
                            ToStatus = RoutePointStatus.FinishOffLoadShipment,
                            Func = FinishOffLoadShipment,
                            Name = L("FinishOffLoadShipment"),
                            Permissions = new List<string>{},
                            Features = new List<string>{},
                        },
                        new WorkflowTransaction<PointTransactionArgs,RoutePointStatus>
                        {
                            Action =  WorkFlowActionConst.ReceiverConfirmed,
                            FromStatus = RoutePointStatus.FinishOffLoadShipment,
                            ToStatus = RoutePointStatus.ReceiverConfirmed,
                            Func = ReceiverConfirmed,
                            Name = L("ReceiverConfirmed"),
                            Permissions = new List<string>{},
                            Features = new List<string>{},
                        },
                        new WorkflowTransaction<PointTransactionArgs,RoutePointStatus>
                        {
                            Action =  WorkFlowActionConst.FinishOffLoadShipmentDeliveryConfirmation,
                            FromStatus = RoutePointStatus.FinishOffLoadShipment,
                            ToStatus = RoutePointStatus.DeliveryConfirmation,
                            Name = L("FinishOffLoadShipmentDeliveryConfirmation"),
                            Permissions = new List<string>{},
                            Features = new List<string>{},
                        },
                        new WorkflowTransaction<PointTransactionArgs,RoutePointStatus>
                        {
                            Action =  WorkFlowActionConst.DeliveryConfirmation,
                            FromStatus = RoutePointStatus.ReceiverConfirmed,
                            ToStatus = RoutePointStatus.DeliveryConfirmation,
                            Name = L("DeliveryConfirmation"),
                            Permissions = new List<string>{},
                            Features = new List<string>{},
                        },
                        new WorkflowTransaction<PointTransactionArgs,RoutePointStatus>
                        {
                            Action =  WorkFlowActionConst.DeliveryConfirmationReceiverConfirmed,
                            FromStatus = RoutePointStatus.DeliveryConfirmation,
                            ToStatus = RoutePointStatus.ReceiverConfirmed,
                            Func = ReceiverConfirmed,
                            Name = L("DeliveryConfirmationReceiverConfirmed"),
                            Permissions = new List<string>{},
                            Features = new List<string>{},
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
                            Name = L("StartedMovingToLoadingLocation"),
                            Permissions = new List<string>{},
                            Features = new List<string>{},
                        },
                        new WorkflowTransaction<PointTransactionArgs,RoutePointStatus>
                        {
                            Action =  WorkFlowActionConst.ArriveToLoadingLocation,
                            FromStatus = RoutePointStatus.StartedMovingToLoadingLocation,
                            ToStatus = RoutePointStatus.ArriveToLoadingLocation,
                            Func = ArriveToLoadingLocation,
                            Name = L("ArriveToLoadingLocation"),
                            Permissions = new List<string>{},
                            Features = new List<string>{},
                        },
                        new WorkflowTransaction<PointTransactionArgs,RoutePointStatus>
                        {
                            Action =  WorkFlowActionConst.StartLoading,
                            FromStatus = RoutePointStatus.ArriveToLoadingLocation,
                            ToStatus = RoutePointStatus.StartLoading,
                            Func = StartLoading,
                            Name = L("StartLoading"),
                            Permissions = new List<string>{},
                            Features = new List<string>{},
                        },
                        new WorkflowTransaction<PointTransactionArgs,RoutePointStatus>
                        {
                            Action =  WorkFlowActionConst.FinishLoading,
                            FromStatus = RoutePointStatus.StartLoading,
                            ToStatus = RoutePointStatus.FinishLoading,
                            Func =  FinishLoading,
                            Name = L("FinishLoading"),
                            Permissions = new List<string>{},
                            Features = new List<string>{},
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
                .Transactions.ToList();
        }
        public List<RoutPointTransactionDto> GetStatuses(ShippingRequestTripDriverRoutePointDto rout)
        {
            var resetTransaction = rout.RoutPointStatusTransitions.OrderByDescending(c => c.CreationTime)
                .FirstOrDefault(c => c.Status == RoutePointStatus.Reset);
            return GetTransactions(rout.WorkFlowVersion)
                     .GroupBy(c => c.ToStatus)
                     .Select(x =>
                     new RoutPointTransactionDto
                     {
                         Status = x.Key,
                         IsDone = rout.RoutPointStatusTransitions.Any(g => g.Status == x.Key && (resetTransaction == null || g.CreationTime > resetTransaction.CreationTime)),
                         Name = x.Key.ToString()
                     }).ToList();
        }
        public List<PointTransactionDto> GetTransactionsByStatus(int workFlowVersion, RoutePointStatus? status = null)
        {
            return Flows
                .FirstOrDefault(c => c.Version == workFlowVersion)?
                .Transactions.Where(x => !status.HasValue || x.FromStatus == status)
                .Select(x => new PointTransactionDto
                {
                    Action = x.Action,
                    FromStatus = x.FromStatus,
                    Name = x.Name,
                    ToStatus = x.ToStatus
                }).ToList();
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
            var trip = await _shippingRequestTripRepository
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
            if (!_shippingRequestTripRepository.GetAll().Any
                (x =>
                 x.Id != trip.Id &&
                 x.Status == ShippingRequestTripStatus.Intransit &&
                 x.AssignedDriverUserId == trip.AssignedDriverUserId
                 )
               )
            {
                /// Get the start pickup point
                var routeStart = await _routPointRepository.GetAll().Include(x => x.FacilityFk).SingleAsync(x => x.ShippingRequestTripId == trip.Id && x.PickingType == Routs.RoutPoints.PickingType.Pickup);

                routeStart.StartTime = Clock.Now;
                routeStart.IsActive = true;
                trip.Status = ShippingRequestTripStatus.Intransit;
                trip.StartTripDate = Clock.Now;
                await StartTransition(routeStart, new Point(Input.lat, Input.lng));
                if (!currentUser.IsDriver) await _firebaseNotifier.TripChanged(new Abp.UserIdentifier(trip.ShippingRequestFk.CarrierTenantId.Value, trip.AssignedDriverUserId.Value), trip.Id.ToString());
            }
            else
            {
                throw new UserFriendlyException(L("YouCanNotStartNewTripWhenYouHaveAnotherTripStillNotFinish"));
            }
        }
        [UnitOfWork]
        public async Task Invoke(PointTransactionArgs args, string action)
        {
            DisableTenancyFilters();
            var point = await _routPointRepository.GetAsync(args.PointId);
            if (point == null) throw new UserFriendlyException(L("YouCanNotChangeTheStatus"));

            var transaction = CheckIfTransactionIsExist(point, action);
            foreach (var item in transaction.Features)
            {
                if (!await _permissionChecker.IsGrantedAsync(false, transaction.Permissions?.ToArray()) || await _featureChecker.IsEnabledAsync(item))
                    throw new AbpAuthorizationException("You are not authorized to " + transaction.Name);
            }
            transaction.Func(args);

            await SetRoutStatusTransitionLog(point);
            await NotificationWhenPointChanged(point);
        }
        public async Task GoToNextLocation(long id)
        {
            DisableTenancyFilters();
            var currentUser = await GetCurrentUserAsync(_abpSession);

            var nextPoint = await GetNextRoutPoint(id, currentUser);
            if (nextPoint == null) throw new UserFriendlyException(L("TheLocationSelectedIsNotFound"));

            var activePointId = await GetLastTransitionInComplete(nextPoint.ShippingRequestTripId);

            var activePoint = await GetActivePointByTripAsyn(activePointId.ToPointId, currentUser);
            if (activePoint == null) throw new UserFriendlyException(L("NoActivePoint"));

            // deactivate active point 
            if (activePoint.IsComplete)
            {
                activePoint.IsActive = false;
                activePoint.EndTime = Clock.Now;
            }
            // activate new point 
            nextPoint.StartTime = Clock.Now;
            nextPoint.IsActive = true;

            await ChangeTransition(nextPoint);
            await NotificationWhenPointChanged(nextPoint);
        }

        /// <summary>
        /// driver confirm the trip has finished 
        /// </summary>
        /// <param name="document"> attachment</param>
        /// <param name="id">route point id</param>
        /// <returns></returns>
        public async Task ConfirmPointToDelivery(IHasDocument document, InvokeStatusInputDto input)
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

            currentPoint.Status = transction.ToStatus;
            currentPoint.ShippingRequestTripFk.RoutePointStatus = transction.ToStatus;

            await SetRoutStatusTransitionLog(currentPoint);
            var isComplete = await HandlePointDelivery(input.Id);

            if (!isComplete)
                await NotificationWhenPointChanged(currentPoint);
        }

        /// <summary>
        /// Get Currrent driver trip dto
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ShippingRequestTripDto> GetCurrentDriverTrip(long UserId)
        {
            var CurrentTrip = await _shippingRequestTripRepository.GetAll()
                    .Where(x => x.AssignedDriverUserId == UserId && x.Status == ShippingRequestTripStatus.Intransit).FirstOrDefaultAsync();
            return ObjectMapper.Map<ShippingRequestTripDto>(CurrentTrip);
        }
        public async Task<FileDto> GetPOD(long id)
        {
            DisableTenancyFilters();
            var currentUser = await GetCurrentUserAsync(_abpSession);

            var document = await _routPointDocumentRepository.GetAll()
                .Where(x => x.RoutPointId == id && x.RoutePointDocumentType == RoutePointDocumentType.POD)
                .WhereIf(!currentUser.TenantId.HasValue || await _featureChecker.IsEnabledAsync(AppFeatures.TachyonDealer), x => true)
                .WhereIf(currentUser.TenantId.HasValue && await _featureChecker.IsEnabledAsync(AppFeatures.Carrier), x => x.RoutPointFk.ShippingRequestTripFk.ShippingRequestFk.CarrierTenantId == currentUser.TenantId.Value)
                .WhereIf(currentUser.IsDriver, x => x.RoutPointFk.ShippingRequestTripFk.AssignedDriverUserId == currentUser.Id)
                .FirstOrDefaultAsync();

            if (document == null) throw new UserFriendlyException(L("TheRoutePointIsNotFound"));
            return await _commonManager.GetDocument(ObjectMapper.Map<IHasDocument>(document));
        }
        #endregion

        #region Transactions Functions

        //Driver => mobile
        //Carrier => web 
        private void StartedMovingToLoadingLocation(PointTransactionArgs args)
        {
            var status = RoutePointStatus.StartedMovingToLoadingLocation;
            var point = _routPointRepository.GetAllIncluding(x => x.ShippingRequestTripFk)
                .FirstOrDefault(x => x.Id == args.PointId);
            point.Status = status;
            point.ShippingRequestTripFk.RoutePointStatus = status;
        }
        private void ArriveToLoadingLocation(PointTransactionArgs args)
        {
            var status = RoutePointStatus.ArriveToLoadingLocation;
            var point = _routPointRepository.GetAllIncluding(x => x.ShippingRequestTripFk)
                .FirstOrDefault(x => x.Id == args.PointId);
            point.Status = status;
            point.ShippingRequestTripFk.RoutePointStatus = status;
        }
        private void StartLoading(PointTransactionArgs args)
        {
            var status = RoutePointStatus.StartLoading;
            var point = _routPointRepository.GetAllIncluding(x => x.ShippingRequestTripFk)
                .FirstOrDefault(x => x.Id == args.PointId);
            point.Status = status;
            point.ShippingRequestTripFk.RoutePointStatus = status;

        }
        private void FinishLoading(PointTransactionArgs args)
        {
            var status = RoutePointStatus.FinishLoading;
            var point = _routPointRepository.GetAllIncluding(x => x.ShippingRequestTripFk)
                .FirstOrDefault(x => x.Id == args.PointId);
            point.Status = status;
            point.ShippingRequestTripFk.RoutePointStatus = status;
            point.IsComplete = true;
            point.IsResolve = true;
            point.EndTime = Clock.Now;
            point.ActualPickupOrDeliveryDate = point.ShippingRequestTripFk.ActualPickupDate = Clock.Now;

            Task.Run(async () => await SendSmsToReceivers(point.ShippingRequestTripId)).Wait();
        }
        private void StartedMovingToOfLoadingLocation(PointTransactionArgs args)
        {
            var status = RoutePointStatus.StartedMovingToOfLoadingLocation;
            var point = _routPointRepository.GetAllIncluding(x => x.ShippingRequestTripFk)
                .FirstOrDefault(x => x.Id == args.PointId);
            point.Status = status;
            point.ShippingRequestTripFk.RoutePointStatus = status;

        }
        private void ArrivedToDestination(PointTransactionArgs args)
        {
            var status = RoutePointStatus.ArrivedToDestination;
            var point = _routPointRepository.GetAllIncluding(x => x.ShippingRequestTripFk)
                .FirstOrDefault(x => x.Id == args.PointId);
            point.Status = status;
            point.ShippingRequestTripFk.RoutePointStatus = status;

        }
        private void StartOffloading(PointTransactionArgs args)
        {
            var status = RoutePointStatus.StartOffloading;
            var point = _routPointRepository.GetAllIncluding(x => x.ShippingRequestTripFk)
                .FirstOrDefault(x => x.Id == args.PointId);
            point.Status = status;
            point.ShippingRequestTripFk.RoutePointStatus = status;

        }
        private void FinishOffLoadShipment(PointTransactionArgs args)
        {
            var status = RoutePointStatus.FinishOffLoadShipment;
            var point = _routPointRepository.GetAllIncluding(x => x.ShippingRequestTripFk)
                .FirstOrDefault(x => x.Id == args.PointId);
            point.Status = status;
            point.ShippingRequestTripFk.RoutePointStatus = status;
            point.ActualPickupOrDeliveryDate = Clock.Now;
            point.IsResolve = true;

            // todo trace this 
            var otherPoints = _routPointRepository.GetAll()
                .Where(x => x.ShippingRequestTripId == point.ShippingRequestTripId)
                .Where(x => x.Id != point.Id)
                .ToList();

            if (otherPoints.All(x => x.ActualPickupOrDeliveryDate.HasValue))
                point.ShippingRequestTripFk.ActualDeliveryDate = Clock.Now;
        }
        private void ReceiverConfirmed(PointTransactionArgs args)
        {
            var status = RoutePointStatus.ReceiverConfirmed;
            var point = _routPointRepository.GetAllIncluding(x => x.ShippingRequestTripFk)
                 .FirstOrDefault(x => x.Id == args.PointId);

            //validate Code
            if (string.IsNullOrEmpty(args.Code) || point.Code != args.Code)
                throw new UserFriendlyException(L("TheReceiverCodeIsIncorrect"));

            point.Status = status;
            point.ShippingRequestTripFk.RoutePointStatus = status;

            // check if point is complete .. and trip is complete 
            Task.Run(async () => await HandlePointDelivery(args.PointId)).Wait();
        }
        #endregion

        #region Helpers
        /// <summary>
        /// check if point is complete and mark it as complete & check if trip is complete and mark it as delivered
        /// </summary>
        /// <param name="point"></param>
        /// <param name="trip"></param>
        /// <returns></returns>
        private async Task<bool> HandlePointDelivery(long pointId)
        {
            var currentUser = await GetCurrentUserAsync(_abpSession);
            var point = await _routPointRepository.GetAsync(pointId);
            var isCompleted = CheckIfPointIsCompleted(point);
            if (isCompleted)
            {
                point.IsActive = false;
                point.IsComplete = true;
                point.EndTime = Clock.Now;
            }

            var allPointsCompleted = await _routPointRepository.GetAll()
                .Where(x => x.ShippingRequestTripId == point.ShippingRequestTripId && x.IsComplete == false && x.Id != point.Id)
                .CountAsync() == 0;

            if (isCompleted && allPointsCompleted)
            {
                var trip = await _shippingRequestTripRepository
                    .GetAllIncluding(d => d.ShippingRequestTripVases)
                    .Include(x => x.ShippingRequestFk)
                    .ThenInclude(c => c.Tenant)
                    .FirstOrDefaultAsync(t => t.Id == point.ShippingRequestTripId);

                trip.Status = ShippingRequestTripStatus.Delivered;
                trip.EndTripDate = Clock.Now;
                await Done(trip);
                await _invoiceManager.GenertateInvoiceWhenShipmintDelivery(trip);
                await NotificationWhenShipmentDelivered(point, currentUser);
            }
            return isCompleted;
        }
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
        private async Task<RoutPoint> GetNextRoutPoint(long id, User currentUser)
        {
            return await _routPointRepository
                .GetAll()
                .Include(c => c.ShippingRequestTripFk)
                .ThenInclude(s => s.ShippingRequestFk)
                .Include(x => x.FacilityFk)
                .Where(x =>
                x.Id == id
                && x.Status == RoutePointStatus.StandBy
                && !x.IsActive
                && !x.IsResolve
                && !x.IsComplete
                && x.ShippingRequestTripFk.Status == ShippingRequestTripStatus.Intransit
                && x.PickingType == Routs.RoutPoints.PickingType.Dropoff)
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
            return await _routPointRepository
                          .GetAll()
                          .Include(t => t.ShippingRequestTripFk)
                          .Where(x => x.Id == pointId && !x.IsComplete)
                          .WhereIf(!currentUser.TenantId.HasValue || await _featureChecker.IsEnabledAsync(AppFeatures.TachyonDealer), x => x.ShippingRequestTripFk.ShippingRequestFk.IsTachyonDeal)
                          .WhereIf(currentUser.TenantId.HasValue && await _featureChecker.IsEnabledAsync(AppFeatures.Carrier), x => x.ShippingRequestTripFk.ShippingRequestFk.CarrierTenantId == currentUser.TenantId.Value)
                          .WhereIf(currentUser.IsDriver, x => x.ShippingRequestTripFk.AssignedDriverUserId == currentUser.Id)
                          .FirstOrDefaultAsync(x => x.IsActive);
        }

        /// <summary>
        /// check if point is complete all workflow transuctions
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        private bool CheckIfPointIsCompleted(RoutPoint point)
        {
            var statuses = GetTransactions(point.WorkFlowVersion)
                .Where(x => x.ToStatus != point.Status)
                .GroupBy(c => c.ToStatus)
                .Select(x => x.Key);

            var resetTransition = _routPointStatusTransitionRepository.GetAll().OrderByDescending(p => p.Id)
                .FirstOrDefault(x => x.Status == RoutePointStatus.Reset && x.PointId == point.Id);

            var transitions = _routPointStatusTransitionRepository.GetAll()
                .Where(x =>
                x.PointId == point.Id &&
                (resetTransition == null || x.CreationTime > resetTransition.CreationTime))
                .Select(c => c.Status).ToList();
            return !statuses.Except(transitions).Any();
        }

        /// <summary>
        /// check if requested transaction is exsist in point workflow
        /// </summary>
        /// <param name="point"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        private WorkflowTransaction<PointTransactionArgs, RoutePointStatus> CheckIfTransactionIsExist(RoutPoint point, string action)
        {
            var workFlow = Flows.FirstOrDefault(w => w.Version == point.WorkFlowVersion);
            if (workFlow == null) throw new UserFriendlyException(L("WorkFlowNotExist"));
            // check if the transction is exist and is available for currnet point status
            var transaction = workFlow.Transactions.FirstOrDefault(c => c.Action.Equals(action) && c.FromStatus == point.Status);
            if (transaction == null) throw new UserFriendlyException(L("TransactionNotExist"));
            return transaction;
        }

        /// <summary>
        /// Transfer the prices from price offer to trip
        /// </summary>
        /// <param name="trip"></param>
        /// <returns></returns>
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
            var trip = await _shippingRequestTripRepository
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
            await SetRoutStatusTransitionLog(routPoint);
        }

        /// <summary>
        /// Change transition from old point to new next point
        /// </summary>
        /// <param name="routPoint"></param>
        /// <returns></returns>
        private async Task ChangeTransition(RoutPoint routPoint)
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
            //await SetRoutStatusTransition(routPoint);
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
                number = point.ReceiverFk.PhoneNumber;
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

        /// <summary>
        /// Mark shipping request as deliverd if all trips is done
        /// </summary>
        /// <param name="trip"></param>
        /// <returns></returns>
        private async Task ChangeShippingRequestStatusIfAllTripsDone(ShippingRequestTrip trip)
        {
            if (!_shippingRequestTripRepository.GetAll().Any(x => x.Status != ShippingRequestTripStatus.Delivered && x.ShippingRequestId == trip.ShippingRequestId))
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
            var lastTransition = await GetLastTransitionInComplete(tripId);
            if (lastTransition != null) lastTransition.IsComplete = true;
        }

        /// <summary>
        /// Set up route point transition
        /// </summary>
        /// <param name="routPoint"></param>
        /// <param name="Status"></param>
        /// <returns></returns>
        private async Task SetRoutStatusTransitionLog(RoutPoint routPoint)
        {
            await _routPointStatusTransitionRepository.InsertAsync(new RoutPointStatusTransition
            {
                PointId = routPoint.Id,
                Status = routPoint.Status
            });
        }

        /// <summary>
        /// Get Currnent user
        /// </summary>
        /// <param name="abpSession"></param>
        /// <returns></returns>
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
        public async Task NotificationWhenPointChanged(RoutPoint point)
        {
            var currentUser = await GetCurrentUserAsync(_abpSession);

            var trip = await _shippingRequestTripRepository
                .GetAllIncluding(x => x.ShippingRequestFk)
                .SingleAsync(x => x.Id == point.ShippingRequestTripId);

            if (!currentUser.IsDriver)
            {
                // todo use appNotifier
                await _firebaseNotifier.TripChanged(new Abp.UserIdentifier(trip.ShippingRequestFk.CarrierTenantId.Value, trip.AssignedDriverUserId.Value), trip.Id.ToString());
            }
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
            if (!currentUser.IsDriver) await _firebaseNotifier.TripChanged(new Abp.UserIdentifier(trip.ShippingRequestFk.CarrierTenantId.Value, trip.AssignedDriverUserId.Value), trip.Id.ToString());
        }
        #endregion
    }
}