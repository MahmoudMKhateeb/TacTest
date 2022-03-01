using Abp;
using Abp.Application.Features;
using Abp.Authorization;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.EntityHistory;
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
using TACHYON.Documents.DocumentFiles;
using TACHYON.Documents.DocumentFiles.Dtos;
using TACHYON.Dto;
using TACHYON.Features;
using TACHYON.Firebases;
using TACHYON.Invoices;
using TACHYON.Net.Sms;
using TACHYON.Notifications;
using TACHYON.PriceOffers;
using TACHYON.Routs.RoutPoints;
using TACHYON.Routs.RoutPoints.RoutPointSmartEnum;
using TACHYON.Shipping.Drivers.Dto;
using TACHYON.Shipping.RoutPoints;
using TACHYON.Shipping.ShippingRequests;
using TACHYON.Shipping.ShippingRequestTrips;
using TACHYON.Shipping.Trips;
using TACHYON.Shipping.Trips.Dto;
using TACHYON.Storage;
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
        private readonly IEntityChangeSetReasonProvider _reasonProvider;
        private readonly ITempFileCacheManager _tempFileCacheManager;


        #region Constractor
        public ShippingRequestPointWorkFlowProvider(IRepository<RoutPoint, long> routPointRepository, IRepository<ShippingRequestTrip> shippingRequestTrip, PriceOfferManager priceOfferManager, IAppNotifier appNotifier, IFeatureChecker featureChecker, IAbpSession abpSession, IRepository<RoutPointDocument, long> routPointDocumentRepository, IRepository<ShippingRequestTripTransition> shippingRequestTripTransitionRepository, IRepository<RoutPointStatusTransition> routPointStatusTransitionRepository, FirebaseNotifier firebaseNotifier, ISmsSender smsSender, InvoiceManager invoiceManager, CommonManager commonManager, UserManager userManager, IWebUrlService webUrlService, IPermissionChecker permissionChecker, IEntityChangeSetReasonProvider reasonProvider, ITempFileCacheManager tempFileCacheManager)
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
            _reasonProvider = reasonProvider;
            _tempFileCacheManager = tempFileCacheManager;
            Flows = new List<WorkFlow<PointTransactionArgs, RoutePointStatus>>
            {
            // Pick up workflow 
            new WorkFlow<PointTransactionArgs,RoutePointStatus>
                {
                    Version = 0,
                    Transactions = new List<WorkflowTransaction<PointTransactionArgs,RoutePointStatus>>
                    {
                        new WorkflowTransaction<PointTransactionArgs,RoutePointStatus>
                        {
                            Action =  WorkFlowActionConst.StartedMovingToLoadingLocation,
                            FromStatus = RoutePointStatus.StandBy,
                            ToStatus = RoutePointStatus.StartedMovingToLoadingLocation,
                            Func = StartedMovingToLoadingLocation,
                            Name = "StartedMovingToLoadingLocation",
                            Permissions = new List<string>{},
                            Features = new List<string>{},
                        },
                        new WorkflowTransaction<PointTransactionArgs,RoutePointStatus>
                        {
                            Action =  WorkFlowActionConst.ArriveToLoadingLocation,
                            FromStatus = RoutePointStatus.StartedMovingToLoadingLocation,
                            ToStatus = RoutePointStatus.ArriveToLoadingLocation,
                            Func = ArriveToLoadingLocation,
                            Name = "ArriveToLoadingLocation",
                            Permissions = new List<string>{},
                            Features = new List<string>{},
                        },
                        new WorkflowTransaction<PointTransactionArgs,RoutePointStatus>
                        {
                            Action =  WorkFlowActionConst.StartLoading,
                            FromStatus = RoutePointStatus.ArriveToLoadingLocation,
                            ToStatus = RoutePointStatus.StartLoading,
                            Func =  StartLoading,
                            Name = "StartLoading",
                            Permissions = new List<string>{},
                            Features = new List<string>{},
                        },
                        new WorkflowTransaction<PointTransactionArgs,RoutePointStatus>
                        {
                            Action =  WorkFlowActionConst.FinishLoading,
                            FromStatus = RoutePointStatus.StartLoading,
                            ToStatus = RoutePointStatus.FinishLoading,
                            Func =  FinishLoading,
                            Name = "FinishLoading",
                            Permissions = new List<string>{},
                            Features = new List<string>{},
                        },
                    },
                },
            // Drop Off workflow without uplode delivery note
            new WorkFlow<PointTransactionArgs,RoutePointStatus>
                {
                    Version = 1,
                    Transactions = new List<WorkflowTransaction<PointTransactionArgs,RoutePointStatus>>
                    {
                        new WorkflowTransaction<PointTransactionArgs,RoutePointStatus>
                        {
                            Action =  WorkFlowActionConst.StartedMovingToOfLoadingLocation,
                            FromStatus = RoutePointStatus.StandBy,
                            ToStatus = RoutePointStatus.StartedMovingToOffLoadingLocation,
                            Func = StartedMovingToOfLoadingLocation,
                            Name = "StartedMovingToOfLoadingLocation",
                            Permissions = new List<string>{},
                            Features = new List<string>{},
                        },
                        new WorkflowTransaction<PointTransactionArgs,RoutePointStatus>
                        {
                            Action =  WorkFlowActionConst.ArrivedToDestination,
                            FromStatus = RoutePointStatus.StartedMovingToOffLoadingLocation,
                            ToStatus = RoutePointStatus.ArrivedToDestination,
                            Func = ArrivedToDestination,
                            Name = "ArrivedToDestination",
                            Permissions = new List<string>{},
                            Features = new List<string>{},
                        },
                        new WorkflowTransaction<PointTransactionArgs,RoutePointStatus>
                        {
                            Action =  WorkFlowActionConst.StartOffloading,
                            FromStatus = RoutePointStatus.ArrivedToDestination,
                            ToStatus = RoutePointStatus.StartOffloading,
                            Func = StartOffloading,
                            Name = "StartOffloading",
                            Permissions = new List<string>{},
                            Features = new List<string>{},
                        },
                        new WorkflowTransaction<PointTransactionArgs,RoutePointStatus>
                        {
                            Action =  WorkFlowActionConst.FinishOffLoadShipment,
                            FromStatus = RoutePointStatus.StartOffloading,
                            ToStatus = RoutePointStatus.FinishOffLoadShipment,
                            Func = FinishOffLoadShipment,
                            Name = "FinishOffLoadShipment",
                            Permissions = new List<string>{},
                            Features = new List<string>{},
                        },
                        new WorkflowTransaction<PointTransactionArgs,RoutePointStatus>
                        {
                            Action =  WorkFlowActionConst.ReceiverConfirmed,
                            FromStatus = RoutePointStatus.FinishOffLoadShipment,
                            ToStatus = RoutePointStatus.ReceiverConfirmed,
                            Func = ReceiverConfirmed,
                            Name = "ReceiverConfirmed",
                            Permissions = new List<string>{},
                            Features = new List<string>{},
                        },
                        new WorkflowTransaction<PointTransactionArgs,RoutePointStatus>
                        {
                            Action =  WorkFlowActionConst.FinishOffLoadShipmentDeliveryConfirmation,
                            FromStatus = RoutePointStatus.FinishOffLoadShipment,
                            ToStatus = RoutePointStatus.DeliveryConfirmation,
                            Func = DeliveryConfirmation,
                            Name = "FinishOffLoadShipmentDeliveryConfirmation",
                            Permissions = new List<string>{},
                            Features = new List<string>{},
                        },
                        new WorkflowTransaction<PointTransactionArgs,RoutePointStatus>
                        {
                            Action =  WorkFlowActionConst.DeliveryConfirmation,
                            FromStatus = RoutePointStatus.ReceiverConfirmed,
                            ToStatus = RoutePointStatus.DeliveryConfirmation,
                            Func = DeliveryConfirmation,
                            Name = "DeliveryConfirmation",
                            Permissions = new List<string>{},
                            Features = new List<string>{},
                        },
                        new WorkflowTransaction<PointTransactionArgs,RoutePointStatus>
                        {
                            Action =  WorkFlowActionConst.DeliveryConfirmationReceiverConfirmed,
                            FromStatus = RoutePointStatus.DeliveryConfirmation,
                            ToStatus = RoutePointStatus.ReceiverConfirmed,
                            Func = ReceiverConfirmed,
                            Name = "DeliveryConfirmationReceiverConfirmed",
                            Permissions = new List<string>{},
                            Features = new List<string>{},
                        },
                    },
                },
            // Drop Off workflow with uplode delivery note
            new WorkFlow<PointTransactionArgs,RoutePointStatus>
                {
                    Version = 2,
                    Transactions = new List<WorkflowTransaction<PointTransactionArgs,RoutePointStatus>>
                    {
                        new WorkflowTransaction<PointTransactionArgs,RoutePointStatus>
                        {
                            Action =  WorkFlowActionConst.StartedMovingToOfLoadingLocation,
                            FromStatus = RoutePointStatus.StandBy,
                            ToStatus = RoutePointStatus.StartedMovingToOffLoadingLocation,
                            Func = StartedMovingToOfLoadingLocation,
                            Name = "StartedMovingToOfLoadingLocation",
                            Permissions = new List<string>{},
                            Features = new List<string>{},
                        },
                        new WorkflowTransaction<PointTransactionArgs,RoutePointStatus>
                        {
                            Action =  WorkFlowActionConst.ArrivedToDestination,
                            FromStatus = RoutePointStatus.StartedMovingToOffLoadingLocation,
                            ToStatus = RoutePointStatus.ArrivedToDestination,
                            Func = ArrivedToDestination,
                            Name = "ArrivedToDestination",
                            Permissions = new List<string>{},
                            Features = new List<string>{},
                        },
                        new WorkflowTransaction<PointTransactionArgs,RoutePointStatus>
                        {
                            Action =  WorkFlowActionConst.StartOffloading,
                            FromStatus = RoutePointStatus.ArrivedToDestination,
                            ToStatus = RoutePointStatus.StartOffloading,
                            Func = StartOffloading,
                            Name = "StartOffloading",
                            Permissions = new List<string>{},
                            Features = new List<string>{},
                        },
                        new WorkflowTransaction<PointTransactionArgs,RoutePointStatus>
                        {
                            Action =  WorkFlowActionConst.FinishOffLoadShipment,
                            FromStatus = RoutePointStatus.StartOffloading,
                            ToStatus = RoutePointStatus.FinishOffLoadShipment,
                            Func = FinishOffLoadShipment,
                            Name = "FinishOffLoadShipment",
                            Permissions = new List<string>{},
                            Features = new List<string>{},
                        },
                        new WorkflowTransaction<PointTransactionArgs,RoutePointStatus>
                        {
                            Action =  WorkFlowActionConst.UplodeDeliveryNote,
                            FromStatus = RoutePointStatus.FinishOffLoadShipment,
                            ToStatus = RoutePointStatus.DeliveryNoteUploded,
                            Func = DeliveryNoteUploded,
                            Name = "UplodeDeliveryNote",
                            Permissions = new List<string>{},
                            Features = new List<string>{},
                        },
                        new WorkflowTransaction<PointTransactionArgs,RoutePointStatus>
                        {
                            Action =  WorkFlowActionConst.ReceiverConfirmed,
                            FromStatus = RoutePointStatus.DeliveryNoteUploded,
                            ToStatus = RoutePointStatus.ReceiverConfirmed,
                            Func = ReceiverConfirmed,
                            Name = "ReceiverConfirmed",
                            Permissions = new List<string>{},
                            Features = new List<string>{},
                        },
                        new WorkflowTransaction<PointTransactionArgs,RoutePointStatus>
                        {
                            Action =  WorkFlowActionConst.UplodeDeliveryNoteDeliveryConfirmation,
                            FromStatus = RoutePointStatus.DeliveryNoteUploded,
                            ToStatus = RoutePointStatus.DeliveryConfirmation,
                            Func = DeliveryConfirmation,
                            Name = "UplodeDeliveryNoteDeliveryConfirmation",
                            Permissions = new List<string>{},
                            Features = new List<string>{},
                        },
                        new WorkflowTransaction<PointTransactionArgs,RoutePointStatus>
                        {
                            Action =  WorkFlowActionConst.DeliveryConfirmation,
                            FromStatus = RoutePointStatus.ReceiverConfirmed,
                            ToStatus = RoutePointStatus.DeliveryConfirmation,
                            Func = DeliveryConfirmation,
                            Name = "DeliveryConfirmation",
                            Permissions = new List<string>{},
                            Features = new List<string>{},
                        },
                        new WorkflowTransaction<PointTransactionArgs,RoutePointStatus>
                        {
                            Action =  WorkFlowActionConst.DeliveryConfirmationReceiverConfirmed,
                            FromStatus = RoutePointStatus.DeliveryConfirmation,
                            ToStatus = RoutePointStatus.ReceiverConfirmed,
                            Func = ReceiverConfirmed,
                            Name = "DeliveryConfirmationReceiverConfirmed",
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
            return Flows.FirstOrDefault(c => c.Version == workFlowVersion)?.Transactions.ToList();
        }
        public List<RoutPointTransactionDto> GetStatuses(int workflowVersion, List<RoutePointStatus> statuses)
        {
            return GetTransactions(workflowVersion).GroupBy(c => c.ToStatus)
                     .Select(x =>
                     new RoutPointTransactionDto
                     {
                         Status = x.Key,
                         IsDone = statuses.Any(g => g == x.Key),
                         Name = L(x.Key.ToString())
                     }).ToList();
        }
        public List<PointTransactionDto> GetTransactionsByStatus(int workFlowVersion, List<RoutePointStatus> statuses, RoutePointStatus status)
        {
            return GetTransactions(workFlowVersion).Where(x => !statuses.Any(c => c == x.ToStatus) && x.FromStatus == status)
                .Select(x => new PointTransactionDto
                {
                    Action = x.Action,
                    Name = L(x.Name),
                    FromStatus = x.FromStatus,
                    ToStatus = x.ToStatus
                }).ToList();
        }
        public List<WorkflowTransaction<PointTransactionArgs, RoutePointStatus>> GetAvailableTransactions(int workFlowVersion, RoutePointStatus status)
        {
            return GetTransactions(workFlowVersion).Where(t => t.FromStatus == status).ToList();
        }
        public async Task Accepted(int id)
        {
            DisableTenancyFilters();
            var trip = await CheckIfCanAccepted(id);
            trip.DriverStatus = ShippingRequestTripDriverStatus.Accepted;
            await TransferPricesToTrip(trip);
            var currentUser = await GetCurrentUserAsync();
            if (currentUser.IsDriver) await _appNotifier.DriverAcceptTrip(trip, currentUser.FullName);
        }
        public async Task Start(ShippingRequestTripDriverStartInputDto Input)
        {
            DisableTenancyFilters();
            var currentUser = await GetCurrentUserAsync();

            var trip = await CheckIfCanStartTrip(Input.Id);
            if (trip == null) throw new UserFriendlyException(L("YouCannotStartWithTheTripSelected"));
            //Get PickUp Point
            var routeStart = await GetPickUpPointToStart(trip.Id);
            routeStart.StartTime = Clock.Now;
            routeStart.IsActive = true;
            routeStart.IsResolve = true;
            trip.Status = ShippingRequestTripStatus.InTransit;
            trip.StartTripDate = Clock.Now;

            await StartTransition(routeStart, new Point(Input.lat, Input.lng));
            // if (!currentUser.IsDriver) await _firebaseNotifier.TripChanged(new Abp.UserIdentifier(trip.ShippingRequestFk.CarrierTenantId.Value, trip.AssignedDriverUserId.Value), trip.Id.ToString());
        }
        public async Task Invoke(PointTransactionArgs args, string action)
        {
            DisableTenancyFilters();

            var point = await GetPointForInvoke(args.PointId);
            if (point == null) throw new UserFriendlyException(L("YouCanNotChangeTheStatus"));

            var transaction = CheckIfTransactionIsExist(point.WorkFlowVersion, point.Status, action);

            if (!_permissionChecker.IsGranted(false, transaction.Permissions?.ToArray()) || (transaction.Features.Any() && transaction.Features.Any(x => _featureChecker.IsEnabled(x))))
                throw new AbpAuthorizationException("You are not authorized to " + transaction.Name);

            var reason = await transaction.Func(args);
            _reasonProvider.Use(reason);

            await SetRoutStatusTransitionLog(point);
            await NotificationWhenPointChanged(point);
            //we need to use save changes in invoke to save the reason in entity change set its does not take it when complete uow transuction
            await CurrentUnitOfWork.SaveChangesAsync(); // need to get alternative 
        }
        public async Task GoToNextLocation(long nextPointId)
        {
            DisableTenancyFilters();

            var nextPoint = await GetNextLocationInTrip(nextPointId);
            if (nextPoint == null) throw new UserFriendlyException(L("TheLocationSelectedIsNotFound"));

            var activePoint = await GetResolvedPointInTrip(nextPoint.ShippingRequestTripId);
            if (activePoint == null) throw new UserFriendlyException(L("NoActivePoint"));

            if (activePoint.IsComplete)
                activePoint.EndTime = Clock.Now;

            // deactivate active point if Completed
            activePoint.IsActive = false;
            activePoint.CanGoToNextLocation = false;
            // activate new point 
            nextPoint.IsActive = true;
            nextPoint.IsResolve = true;
            nextPoint.StartTime = Clock.Now;

            await ChangeTransition(nextPoint);
            await NotificationWhenPointChanged(nextPoint);
        }
        public async Task<ShippingRequestTripDto> GetCurrentDriverTrip(long userId)
        {
            var currentTrip = await _shippingRequestTripRepository.GetAll()
                    .Where(
                    x =>
                    x.AssignedDriverUserId == userId &&
                    x.Status == ShippingRequestTripStatus.InTransit)
                    .FirstOrDefaultAsync();
            return ObjectMapper.Map<ShippingRequestTripDto>(currentTrip);
        }
        public async Task<List<GetAllUploadedFileDto>> GetPOD(long id)
        {
            DisableTenancyFilters();
            var currentUser = await GetCurrentUserAsync();
            var documents = await _routPointDocumentRepository.GetAll()
                .Where(x => x.RoutPointId == id && x.RoutePointDocumentType == RoutePointDocumentType.POD)
                .WhereIf(!currentUser.TenantId.HasValue || await _featureChecker.IsEnabledAsync(AppFeatures.TachyonDealer), x => true)
                .WhereIf(currentUser.TenantId.HasValue && await _featureChecker.IsEnabledAsync(AppFeatures.Carrier), x => x.RoutPointFk.ShippingRequestTripFk.ShippingRequestFk.CarrierTenantId == currentUser.TenantId.Value)
                .WhereIf(currentUser.IsDriver, x => x.RoutPointFk.ShippingRequestTripFk.AssignedDriverUserId == currentUser.Id)
                .ToListAsync();
            if (!documents.Any()) throw new UserFriendlyException(L("TheRoutePointIsNotFound"));
            return await _commonManager.GetDocuments(ObjectMapper.Map<List<IHasDocument>>(documents), currentUser);
        }

        #endregion

        #region Transactions Functions
        //work flows functions
        private async Task<string> StartedMovingToLoadingLocation(PointTransactionArgs args)
        {
            var status = RoutePointStatus.StartedMovingToLoadingLocation;
            var point = await _routPointRepository.GetAllIncluding(x => x.ShippingRequestTripFk)
                .FirstOrDefaultAsync(x => x.Id == args.PointId);
            point.Status = status;
            point.ShippingRequestTripFk.RoutePointStatus = status;
            return nameof(RoutPointPickUpStep1);
        }
        private async Task<string> ArriveToLoadingLocation(PointTransactionArgs args)
        {
            var status = RoutePointStatus.ArriveToLoadingLocation;
            var point = await _routPointRepository.GetAllIncluding(x => x.ShippingRequestTripFk)
                .FirstOrDefaultAsync(x => x.Id == args.PointId);
            point.Status = status;
            point.ShippingRequestTripFk.RoutePointStatus = status;
            return nameof(RoutPointPickUpStep2);
        }
        private async Task<string> StartLoading(PointTransactionArgs args)
        {
            var status = RoutePointStatus.StartLoading;
            var point = await _routPointRepository.GetAllIncluding(x => x.ShippingRequestTripFk)
                .FirstOrDefaultAsync(x => x.Id == args.PointId);
            point.Status = status;
            point.ShippingRequestTripFk.RoutePointStatus = status;
            return nameof(RoutPointPickUpStep3);
        }
        private async Task<string> FinishLoading(PointTransactionArgs args)
        {
            var status = RoutePointStatus.FinishLoading;
            var point = await _routPointRepository.GetAllIncluding(x => x.ShippingRequestTripFk)
                .FirstOrDefaultAsync(x => x.Id == args.PointId);
            point.Status = status;
            point.ShippingRequestTripFk.RoutePointStatus = status;
            point.IsComplete = true;
            point.EndTime = Clock.Now;
            point.ActualPickupOrDeliveryDate = point.ShippingRequestTripFk.ActualPickupDate = Clock.Now;
            point.CanGoToNextLocation = true;
            await SendSmsToReceivers(point.ShippingRequestTripId);
            return nameof(RoutPointPickUpStep4);
        }
        private async Task<string> StartedMovingToOfLoadingLocation(PointTransactionArgs args)
        {
            var status = RoutePointStatus.StartedMovingToOffLoadingLocation;
            var point = await _routPointRepository.GetAllIncluding(x => x.ShippingRequestTripFk)
                .FirstOrDefaultAsync(x => x.Id == args.PointId);
            point.Status = status;
            point.ShippingRequestTripFk.RoutePointStatus = status;
            return nameof(RoutPointDropOffStep1);

        }
        private async Task<string> ArrivedToDestination(PointTransactionArgs args)
        {
            var status = RoutePointStatus.ArrivedToDestination;
            var point = await _routPointRepository.GetAllIncluding(x => x.ShippingRequestTripFk)
                .FirstOrDefaultAsync(x => x.Id == args.PointId);
            point.Status = status;
            point.ShippingRequestTripFk.RoutePointStatus = status;
            return nameof(RoutPointDropOffStep2);
        }
        private async Task<string> StartOffloading(PointTransactionArgs args)
        {
            var status = RoutePointStatus.StartOffloading;
            var point = await _routPointRepository.GetAllIncluding(x => x.ShippingRequestTripFk)
                .FirstOrDefaultAsync(x => x.Id == args.PointId);
            point.Status = status;
            point.ShippingRequestTripFk.RoutePointStatus = status;
            return nameof(RoutPointDropOffStep3);
        }
        private async Task<string> FinishOffLoadShipment(PointTransactionArgs args)
        {
            var status = RoutePointStatus.FinishOffLoadShipment;
            var point = await _routPointRepository.GetAllIncluding(x => x.ShippingRequestTripFk)
                .FirstOrDefaultAsync(x => x.Id == args.PointId);
            point.Status = status;
            point.ShippingRequestTripFk.RoutePointStatus = status;
            point.ActualPickupOrDeliveryDate = Clock.Now;

            point.CanGoToNextLocation = await _routPointRepository.GetAll().AnyAsync(x => !x.IsComplete && !x.IsResolve && x.ShippingRequestTripId == point.ShippingRequestTripId);

            // todo trace this 
            var otherPoints = await _routPointRepository.GetAll()
                .Where(x => x.ShippingRequestTripId == point.ShippingRequestTripId)
                .Where(x => x.Id != point.Id)
                .ToListAsync();

            if (otherPoints.All(x => x.ActualPickupOrDeliveryDate.HasValue))
                point.ShippingRequestTripFk.ActualDeliveryDate = Clock.Now;
            return nameof(RoutPointDropOffStep4);

        }
        private async Task<string> ReceiverConfirmed(PointTransactionArgs args)
        {
            var status = RoutePointStatus.ReceiverConfirmed;
            var point = await _routPointRepository.GetAllIncluding(x => x.ShippingRequestTripFk)
                 .FirstOrDefaultAsync(x => x.Id == args.PointId);

            //validate Code
            if (string.IsNullOrEmpty(args.Code) || point.Code != args.Code)
                throw new UserFriendlyException(L("TheReceiverCodeIsIncorrect"));

            point.Status = status;
            point.ShippingRequestTripFk.RoutePointStatus = status;

            // check if point is complete .. and trip is complete 
            await HandlePointDelivery(args.PointId);
            return nameof(RoutPointDropOffStep5);

        }
        private async Task<string> DeliveryConfirmation(PointTransactionArgs args)
        {
            var status = RoutePointStatus.DeliveryConfirmation;
            var point = await _routPointRepository.GetAllIncluding(x => x.ShippingRequestTripFk)
                 .FirstOrDefaultAsync(x => x.Id == args.PointId);

            if (args.Documents != null && !args.Documents.Any())
                throw new UserFriendlyException(L("File_Empty_Error"));


            foreach (var document in args.Documents)
            {
                _routPointDocumentRepository.Insert(new RoutPointDocument
                {
                    RoutPointId = point.Id,
                    DocumentContentType = document.DocumentContentType,
                    DocumentName = document.DocumentName,
                    DocumentId = document.DocumentId,
                    RoutePointDocumentType = RoutePointDocumentType.POD
                });
            }

            point.Status = status;
            point.ShippingRequestTripFk.RoutePointStatus = status;
            point.IsPodUploaded = true;
            await HandlePointDelivery(args.PointId);
            return nameof(RoutPointDropOffStep6);
        }
        private async Task<string> DeliveryNoteUploded(PointTransactionArgs args)
        {
            var status = RoutePointStatus.DeliveryNoteUploded;
            var point = await _routPointRepository.GetAllIncluding(x => x.ShippingRequestTripFk)
                 .FirstOrDefaultAsync(x => x.Id == args.PointId);

            if (args.Documents != null && !args.Documents.Any())
                throw new UserFriendlyException(L("File_Empty_Error"));

            foreach (var document in args.Documents)
            {
                _routPointDocumentRepository.Insert(new RoutPointDocument
                {
                    RoutPointId = point.Id,
                    DocumentContentType = "image/jpeg",
                    DocumentName = document.DocumentName,
                    DocumentId = document.DocumentId,
                    RoutePointDocumentType = RoutePointDocumentType.DeliveryNote
                });
            }

            point.IsDeliveryNoteUploaded = true;
            point.Status = status;
            point.ShippingRequestTripFk.RoutePointStatus = status;

            return nameof(RoutPointDropOffStep7);
        }
        #endregion

        #region Helpers
        /// <summary>
        /// Check the trip can accpted or not by status
        /// </summary>
        private async Task<ShippingRequestTrip> CheckIfCanAccepted(int tripId)
        {
            var currentUser = await GetCurrentUserAsync();
            var trip = await _shippingRequestTripRepository
                            .GetAllIncluding(o => o.OriginFacilityFk, d => d.DestinationFacilityFk, x => x.ShippingRequestFk, x => x.ShippingRequestTripVases)
                            .Where(x => x.Id == tripId && x.ShippingRequestFk.CarrierTenantId.HasValue && x.ShippingRequestFk.Status != ShippingRequestStatus.Cancled && x.AssignedDriverUserId.HasValue)
                            .WhereIf(!currentUser.TenantId.HasValue || await _featureChecker.IsEnabledAsync(AppFeatures.TachyonDealer), x => x.ShippingRequestFk.IsTachyonDeal)
                            .WhereIf(currentUser.TenantId.HasValue && await _featureChecker.IsEnabledAsync(AppFeatures.Carrier), x => x.ShippingRequestFk.CarrierTenantId == currentUser.TenantId.Value)
                            .WhereIf(currentUser.IsDriver, x => x.AssignedDriverUserId == currentUser.Id)
                            .FirstOrDefaultAsync(t => t.DriverStatus == ShippingRequestTripDriverStatus.None && (t.Status == ShippingRequestTripStatus.New ||
                            // new driver "changed" can accept trip
                            t.Status == ShippingRequestTripStatus.InTransit));
            if (trip == null) throw new UserFriendlyException(L("TheTripIsNotFound"));
            return trip;
        }
        /// <summary>
        /// Transfer the prices from price offer to trip
        /// </summary>
        private async Task TransferPricesToTrip(ShippingRequestTrip trip)
        {
            DisableTenancyFilters();
            var offer = await _priceOfferManager.GetOfferAcceptedByShippingRequestId(trip.ShippingRequestId);

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
        /// Check If user Can Start the trip
        /// </summary>
        private async Task<ShippingRequestTrip> CheckIfCanStartTrip(int tripId)
        {
            var currentUser = await GetCurrentUserAsync();
            return await _shippingRequestTripRepository
                            .GetAll().Include(s => s.ShippingRequestFk).Where(x => x.Id == tripId &&
                                x.Status == ShippingRequestTripStatus.New &&
                                x.DriverStatus == ShippingRequestTripDriverStatus.Accepted &&
                                x.ShippingRequestFk.StartTripDate.Value.Date <= Clock.Now.Date)
                            .WhereIf(!currentUser.TenantId.HasValue || await _featureChecker.IsEnabledAsync(AppFeatures.TachyonDealer), x => x.ShippingRequestFk.IsTachyonDeal)
                            .WhereIf(currentUser.TenantId.HasValue && await _featureChecker.IsEnabledAsync(AppFeatures.Carrier), x => x.ShippingRequestFk.CarrierTenantId == currentUser.TenantId.Value)
                            .WhereIf(currentUser.IsDriver, x => x.AssignedDriverUserId == currentUser.Id)
                            .FirstOrDefaultAsync();
        }
        /// <summary>
        /// Get PickUp Point to Activate it when start the trip
        /// </summary>
        private async Task<RoutPoint> GetPickUpPointToStart(int tripId)
        {
            return await _routPointRepository.GetAll().Include(x => x.FacilityFk)
                .SingleAsync(x => x.ShippingRequestTripId == tripId && x.PickingType == Routs.RoutPoints.PickingType.Pickup);
        }
        /// <summary>
        /// Get Point and check if it active to invoke it
        /// </summary>
        private async Task<RoutPoint> GetPointForInvoke(long pointId)
        {
            var currentUser = await GetCurrentUserAsync();
            return await _routPointRepository
                .GetAll().Where(x => x.IsResolve && !x.IsComplete && x.Id == pointId)
                .WhereIf(!currentUser.TenantId.HasValue || await _featureChecker.IsEnabledAsync(AppFeatures.TachyonDealer), x => x.ShippingRequestTripFk.ShippingRequestFk.IsTachyonDeal)
                .WhereIf(currentUser.TenantId.HasValue && await _featureChecker.IsEnabledAsync(AppFeatures.Carrier), x => x.ShippingRequestTripFk.ShippingRequestFk.CarrierTenantId == currentUser.TenantId.Value)
                .WhereIf(currentUser.IsDriver, x => x.ShippingRequestTripFk.AssignedDriverUserId == currentUser.Id)
                .FirstOrDefaultAsync();
        }
        /// <summary>
        /// check if requested transaction is exsist in point workflow
        /// </summary>
        private WorkflowTransaction<PointTransactionArgs, RoutePointStatus> CheckIfTransactionIsExist(int workFlowVersion, RoutePointStatus status, string action)
        {
            var workFlow = Flows.FirstOrDefault(w => w.Version == workFlowVersion);
            if (workFlow == null) throw new UserFriendlyException(L("WorkFlowNotExist"));
            // check if the transction is exist and is available for currnet point status
            var transaction = workFlow.Transactions.FirstOrDefault(c => c.Action.Equals(action) && c.FromStatus == status);
            if (transaction == null) throw new UserFriendlyException(L("TransactionNotExist"));
            return transaction;
        }
        /// <summary>
        /// Get next Point and check if it Available to Open
        /// </summary>
        private async Task<RoutPoint> GetNextLocationInTrip(long pointId)
        {
            var currentUser = await GetCurrentUserAsync();
            return await _routPointRepository
                .GetAll().Include(c => c.ShippingRequestTripFk)
                .ThenInclude(s => s.ShippingRequestFk).Include(x => x.FacilityFk)
                .Where(x => x.Id == pointId
                         && x.Status == RoutePointStatus.StandBy && !x.IsActive && !x.IsResolve && !x.IsComplete
                         && x.ShippingRequestTripFk.Status == ShippingRequestTripStatus.InTransit
                         && x.PickingType == Routs.RoutPoints.PickingType.Dropoff)
                .WhereIf(!currentUser.TenantId.HasValue || await _featureChecker.IsEnabledAsync(AppFeatures.TachyonDealer), x => x.ShippingRequestTripFk.ShippingRequestFk.IsTachyonDeal)
                .WhereIf(currentUser.TenantId.HasValue && await _featureChecker.IsEnabledAsync(AppFeatures.Carrier), x => x.ShippingRequestTripFk.ShippingRequestFk.CarrierTenantId == currentUser.TenantId.Value)
                .WhereIf(currentUser.IsDriver, x => x.ShippingRequestTripFk.AssignedDriverUserId == currentUser.Id)
                .FirstOrDefaultAsync();
        }
        /// <summary>
        /// Get Resolved Point and in trip
        /// </summary>
        private async Task<RoutPoint> GetResolvedPointInTrip(int tripId)
        {
            var currentUser = await GetCurrentUserAsync();
            return await _routPointRepository
                .GetAll().Where(x => x.ShippingRequestTripId == tripId &&
                x.ShippingRequestTripFk.Status == ShippingRequestTripStatus.InTransit && x.CanGoToNextLocation)
                .WhereIf(!currentUser.TenantId.HasValue || await _featureChecker.IsEnabledAsync(AppFeatures.TachyonDealer), x => x.ShippingRequestTripFk.ShippingRequestFk.IsTachyonDeal)
                .WhereIf(currentUser.TenantId.HasValue && await _featureChecker.IsEnabledAsync(AppFeatures.Carrier), x => x.ShippingRequestTripFk.ShippingRequestFk.CarrierTenantId == currentUser.TenantId.Value)
                .WhereIf(currentUser.IsDriver, x => x.ShippingRequestTripFk.AssignedDriverUserId == currentUser.Id)
                .FirstOrDefaultAsync();
        }
        /// <summary>
        /// check if point is complete and mark it as complete & check if trip is complete and mark it as delivered
        /// </summary>
        private async Task HandlePointDelivery(long pointId)
        {
            var currentUser = await GetCurrentUserAsync();
            var point = await _routPointRepository.GetAsync(pointId);
            var isCompleted = CheckIfPointIsCompleted(point);
            if (isCompleted)
            {
                point.IsActive = false;
                point.IsComplete = true;
                point.EndTime = Clock.Now;

                var allPointsCompleted = !await _routPointRepository.GetAll()
               .AnyAsync(x => x.ShippingRequestTripId == point.ShippingRequestTripId && !x.IsComplete && x.Id != point.Id);
                var allPointsResolved = !await _routPointRepository.GetAll()
                    .AnyAsync(x => x.ShippingRequestTripId == point.ShippingRequestTripId && !x.IsResolve && x.Id != point.Id);

                var trip = await _shippingRequestTripRepository
                    .GetAllIncluding(d => d.ShippingRequestTripVases)
                    .Include(x => x.ShippingRequestFk).ThenInclude(c => c.Tenant)
                    .FirstOrDefaultAsync(t => t.Id == point.ShippingRequestTripId);

                //if current point is completed and all another point in trip is completed the trip statues should be Delivered
                if (allPointsCompleted)
                {

                    trip.Status = ShippingRequestTripStatus.Delivered;
                    trip.EndWorking = Clock.Now;
                    await ChangeShippingRequestStatusIfAllTripsDone(trip);
                    await CloseLastTransitionInComplete(trip.Id);
                    await _invoiceManager.GenertateInvoiceWhenShipmintDelivery(trip);
                    await NotificationWhenShipmentDelivered(point, currentUser);
                }
                else if (allPointsResolved && trip.Status == ShippingRequestTripStatus.InTransit)
                {
                    trip.Status = ShippingRequestTripStatus.DeliveredAndNeedsConfirmation;
                }
            }
        }
        /// <summary>
        /// check if point is complete all workflow transuctions
        /// </summary>
        private bool CheckIfPointIsCompleted(RoutPoint point)
        {
            var statuses = GetTransactions(point.WorkFlowVersion)
                .Where(x => x.ToStatus != point.Status)
                .GroupBy(c => c.ToStatus)
                .Select(x => x.Key);

            var transitions = _routPointStatusTransitionRepository.GetAll()
                .Where(x =>
                x.PointId == point.Id && !x.IsReset)
                .Select(c => c.Status).ToList();
            return !statuses.Except(transitions).Any();
        }
        /// <summary>
        /// When start pikup way
        /// </summary>
        private async Task StartTransition(RoutPoint routPoint, Point fromLocation)
        {
            fromLocation.SRID = 4326;
            ShippingRequestTripTransition tripTransition = new ShippingRequestTripTransition
            {
                FromLocation = fromLocation,
                ToPointId = routPoint.Id,
                ToLocation = routPoint.FacilityFk.Location
            };
            await _shippingRequestTripTransitionRepository.InsertAsync(tripTransition);
            await SetRoutStatusTransitionLog(routPoint);
        }
        /// <summary>
        /// Change transition from old point to new next point
        /// </summary>
        private async Task ChangeTransition(RoutPoint routPoint)
        {
            var oldPointTransition = await GetLastTransitionInComplete(routPoint.ShippingRequestTripId);
            if (oldPointTransition != null)
            {
                oldPointTransition.IsComplete = true;
                ShippingRequestTripTransition tripTransition = new ShippingRequestTripTransition
                {
                    FromLocation = oldPointTransition.ToLocation,
                    FromPointId = oldPointTransition.ToPointId,
                    ToPointId = routPoint.Id,
                    ToLocation = routPoint.FacilityFk.Location
                };
                await _shippingRequestTripTransitionRepository.InsertAsync(tripTransition);
            }
        }
        /// <summary>
        /// Get the last transition for trip is not complete
        /// </summary>
        private async Task<ShippingRequestTripTransition> GetLastTransitionInComplete(int tripId)
        {
            return await _shippingRequestTripTransitionRepository
                .FirstOrDefaultAsync(x => x.ToPoint.ShippingRequestTripId == tripId && !x.IsComplete);
        }
        /// <summary>
        /// Send shipment code to receivers
        /// </summary>
        private async Task SendSmsToReceivers(int tripId)
        {
            var dropOffPoints = await _routPointRepository.GetAll().Include(x => x.ReceiverFk)
                .Where(x => x.ShippingRequestTripId == tripId && x.PickingType == Routs.RoutPoints.PickingType.Dropoff)
                .ToListAsync();
            foreach (var point in dropOffPoints)
                await SendSmsToReceiver(point);

        }
        /// <summary>
        /// Send shipment code to receiver
        /// </summary>
        private async Task SendSmsToReceiver(RoutPoint point)
        {
            string number = point.ReceiverPhoneNumber;
            var ratingLink = $"{L("ClickToRate")} {_webUrlService.WebSiteRootAddressFormat}account/RatingPage/{point.Code}";
            string message = L(TACHYONConsts.SMSShippingRequestReceiverCode, point.WaybillNumber, point.Code, ratingLink);
            if (point.ReceiverFk != null)
                number = point.ReceiverFk.PhoneNumber;
            await _smsSender.SendAsync(number, message);

        }
        ///<summary>
        /// Mark shipping request as deliverd if all trips is done
        /// </summary>
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
        private async Task CloseLastTransitionInComplete(int tripId)
        {
            var lastTransition = await GetLastTransitionInComplete(tripId);
            if (lastTransition != null) lastTransition.IsComplete = true;
        }
        /// <summary>
        /// Set up route point transition
        /// </summary>
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
        protected virtual async Task<User> GetCurrentUserAsync()
        {
            var user = await UserManager.FindByIdAsync(_abpSession.GetUserId().ToString());
            if (user == null)
                throw new Exception("There is no current user!");
            return user;
        }
        #endregion
        #region Notfications
        /// <summary>
        /// Singlar notifcation when the route point status changed
        /// </summary>
        public async Task NotificationWhenPointChanged(RoutPoint point)
        {
            var currentUser = await GetCurrentUserAsync();

            var trip = await _shippingRequestTripRepository
                .GetAllIncluding(x => x.ShippingRequestFk)
                .SingleAsync(x => x.Id == point.ShippingRequestTripId);

            // if (!currentUser.IsDriver) await _firebaseNotifier.TripChanged(new Abp.UserIdentifier(trip.ShippingRequestFk.CarrierTenantId.Value, trip.AssignedDriverUserId.Value), trip.Id.ToString());
        }
        /// <summary>
        /// Singlar notifcation when the shipment delivered
        /// </summary>
        public async Task NotificationWhenShipmentDelivered(RoutPoint point, User currentUser)
        {
            var trip = point.ShippingRequestTripFk;
            // if (!currentUser.IsDriver) await _firebaseNotifier.TripChanged(new Abp.UserIdentifier(trip.ShippingRequestFk.CarrierTenantId.Value, trip.AssignedDriverUserId.Value), trip.Id.ToString());
        }
        #endregion
    }
}