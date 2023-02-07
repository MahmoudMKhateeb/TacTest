using Abp;
using Abp.Domain.Repositories;
using Abp.EntityHistory;
using Abp.Runtime.Session;
using Abp.Timing;
using Abp.UI;
using Castle.Core.Internal;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TACHYON.Common;
using TACHYON.Documents;
using TACHYON.Invoices;
using TACHYON.Notifications;
using TACHYON.Routs.RoutPoints;
using TACHYON.Shipping.ShippingRequests;
using TACHYON.Shipping.ShippingRequestTrips;
using TACHYON.Shipping.Trips;
using TACHYON.Tracking.Dto.WorkFlow;
using TACHYON.WorkFlows;

namespace TACHYON.Tracking.AdditionalSteps
{
    public class AdditionalStepWorkflowProvider : TACHYONDomainServiceBase, IWorkFlowProvider<
        AdditionalStepTransaction<AdditionalStepArgs, AdditionalStepType>, AdditionalStepArgs, AdditionalStepType>
    {
        public List<WorkFlow<AdditionalStepTransaction<AdditionalStepArgs, AdditionalStepType>>> Flows { get; set; }
       
        private readonly IRepository<RoutPoint, long> _routePointRepository;
        private readonly IEntityChangeSetReasonProvider _reasonProvider;
        private readonly IRepository<RoutPointDocument,long> _routePointDocumentRepository;
        private readonly IRepository<AdditionalStepTransition,long> _additionalStepTransition;
        private readonly IRepository<ShippingRequestTrip> _tripRepository;
        private readonly InvoiceManager _invoiceManager;
        private readonly IAppNotifier _appNotifier;
        private readonly DocumentFilesManager _documentFilesManager;
        public IAbpSession AbpSession { set; get; }



        public AdditionalStepWorkflowProvider(
            IRepository<RoutPoint, long> routePointRepository,
            IEntityChangeSetReasonProvider reasonProvider,
            IRepository<RoutPointDocument, long> routePointDocumentRepository,
            IRepository<AdditionalStepTransition, long> additionalStepTransition,
            IRepository<ShippingRequestTrip> tripRepository,
            InvoiceManager invoiceManager,
            IAppNotifier appNotifier,
            DocumentFilesManager documentFilesManager,
            IAbpSession abpSession)
        {
            _routePointRepository = routePointRepository;
            _reasonProvider = reasonProvider;
            _routePointDocumentRepository = routePointDocumentRepository;
            _additionalStepTransition = additionalStepTransition;
            _tripRepository = tripRepository;
            _invoiceManager = invoiceManager;
            _appNotifier = appNotifier;
            Flows = new List<WorkFlow<AdditionalStepTransaction<AdditionalStepArgs, AdditionalStepType>>>
            {
                // Ports Movement - Import -> With/out Return Trip -> `First Trip`
                new()
                {
                    Version = AdditionalStepWorkflowVersionConst.PortsMovementImportFirstTripVersion,
                    Transactions = new List<AdditionalStepTransaction<AdditionalStepArgs, AdditionalStepType>>
                    {
                        new()
                        {
                            Action = AdditionalStepWorkflowActionConst.ReceiverConfirmation,
                            AdditionalStepType = AdditionalStepType.ReceiverCode,
                            Name = "ReceiverCode",
                            IsRequired = false,
                            Func = ReceiverConfirmation
                        },
                        new()
                        {
                            Action = AdditionalStepWorkflowActionConst.DeliveryConfirmation,
                            AdditionalStepType = AdditionalStepType.Pod,
                            RoutePointDocumentType = RoutePointDocumentType.POD,
                            Name = "DeliveryConfirmation",
                            IsRequired = true,
                            Func = DeliveryConfirmation
                        }
                    },
                },
                // Ports Movement - Import -> Without Return Trip -> `Return Trip`
                new()
                {
                    Version = AdditionalStepWorkflowVersionConst.PortsMovementImportReturnTripVersion,
                    Transactions = new List<AdditionalStepTransaction<AdditionalStepArgs, AdditionalStepType>>
                    {
                        new()
                        {
                            Action = AdditionalStepWorkflowActionConst.ReceiverConfirmation,
                            AdditionalStepType = AdditionalStepType.ReceiverCode,
                            Name = "ReceiverCode",
                            IsRequired = false,
                            Func = ReceiverConfirmation
                        },
                        new()
                        {
                            Action = AdditionalStepWorkflowActionConst.UploadEirFile,
                            AdditionalStepType = AdditionalStepType.Eir,
                            RoutePointDocumentType = RoutePointDocumentType.Eir,
                            Name = "UploadEirFile",
                            IsRequired = true,
                            Func = UploadEirFile
                        }
                    }
                },
                // Ports Movement - Export -> Two way Route with/out port shuttling -> `First Trip`
                new()
                {
                    Version = AdditionalStepWorkflowVersionConst.PortsMovementExportFirstTripVersion,
                    Transactions = new List<AdditionalStepTransaction<AdditionalStepArgs, AdditionalStepType>>
                    {
                        new()
                        {
                            Action = AdditionalStepWorkflowActionConst.ReceiverConfirmation,
                            AdditionalStepType = AdditionalStepType.ReceiverCode,
                            Name = "ReceiverCode",
                            IsRequired = false,
                            Func = ReceiverConfirmation
                        },
                        new()
                        {
                            Action = AdditionalStepWorkflowActionConst.UploadManifestFile,
                            AdditionalStepType = AdditionalStepType.Manifest,
                            RoutePointDocumentType = RoutePointDocumentType.Manifest,
                            Name = "UploadManifestFile",
                            IsRequired = true,
                            Func = UploadManifestFile
                        }
                    }
                },
                // Ports Movement - Export -> Two way Route with/out port shuttling -> `Second Trip`
                new()
                {
                    Version = AdditionalStepWorkflowVersionConst.PortsMovementExportSecondTripVersion,
                    Transactions = new List<AdditionalStepTransaction<AdditionalStepArgs, AdditionalStepType>>
                    {
                        new()
                        {
                            Action = AdditionalStepWorkflowActionConst.ReceiverConfirmation,
                            AdditionalStepType = AdditionalStepType.ReceiverCode,
                            Name = "ReceiverCode",
                            IsRequired = true,
                            Func = ReceiverConfirmation
                        },
                        new()
                        {
                            Action = AdditionalStepWorkflowActionConst.DeliveryConfirmation,
                            AdditionalStepType = AdditionalStepType.Pod,
                            RoutePointDocumentType = RoutePointDocumentType.POD,
                            Name = "DeliveryConfirmation",
                            IsRequired = true,
                            Func = DeliveryConfirmation
                        }
                    }
                },
                // Ports Movement - Export -> Two way Route with port shuttling  -> `One Trip Only, Third Trip`
                new()
                {
                    Version = AdditionalStepWorkflowVersionConst.PortsMovementExportThirdTripVersion,
                    Transactions = new List<AdditionalStepTransaction<AdditionalStepArgs, AdditionalStepType>>
                    {
                        new()
                        {
                            Action = AdditionalStepWorkflowActionConst.ReceiverConfirmation,
                            AdditionalStepType = AdditionalStepType.ReceiverCode,
                            Name = "ReceiverCode",
                            IsRequired = false,
                            Func = ReceiverConfirmation
                        },
                        new()
                        {
                            Action = AdditionalStepWorkflowActionConst.DeliveryConfirmation,
                            AdditionalStepType = AdditionalStepType.Pod,
                            RoutePointDocumentType = RoutePointDocumentType.POD,
                            Name = "DeliveryConfirmation",
                            IsRequired = true,
                            Func = DeliveryConfirmation
                        }
                    }
                },
                // Ports Movement - Export -> One way Route with port shuttling  -> `One Trip Only`
                new()
                {
                    Version = AdditionalStepWorkflowVersionConst.PortsMovementExportOneWayFirstTripVersion,
                    Transactions = new List<AdditionalStepTransaction<AdditionalStepArgs, AdditionalStepType>>
                    {
                        new()
                        {
                            Action = AdditionalStepWorkflowActionConst.ReceiverConfirmation,
                            AdditionalStepType = AdditionalStepType.ReceiverCode,
                            Name = "ReceiverCode",
                            IsRequired = false,
                            Func = ReceiverConfirmation
                        },
                        new()
                        {
                            Action = AdditionalStepWorkflowActionConst.UploadConfirmationDocument,
                            AdditionalStepType = AdditionalStepType.ConfirmationDocument,
                            RoutePointDocumentType = RoutePointDocumentType.ConfirmationDocuments,
                            Name = "ConfirmationDocument",
                            IsRequired = true,
                            Func = UploadConfirmationDocument
                        }
                    }
                }
            };
            _documentFilesManager = documentFilesManager;
            AbpSession = abpSession;
        }


        public async Task Invoke(AdditionalStepArgs args, string action)
        {

            RoutPoint point = await _routePointRepository.FirstOrDefaultAsync(args.PointId);

            if (point is not { AdditionalStepWorkFlowVersion: {} }) 
                throw new UserFriendlyException(L("PointHasNotAdditionalSteps"));
            
            var transaction = GetTransaction(point.AdditionalStepWorkFlowVersion.Value, action);

           string reason = await transaction.Func(args);

           await LogAdditionalStepTransition(args.PointId, transaction.AdditionalStepType, transaction.RoutePointDocumentType);
           if (transaction.IsRequired)
           {
               await HandlePointDelivery(point, args.CurrentUser);
           }
           
            _reasonProvider.Use(reason);
            await CurrentUnitOfWork.SaveChangesAsync(); // to save reason
        }

        public List<AdditionalStepTransaction<AdditionalStepArgs, AdditionalStepType>> GetTransactions(int workflowVersion)
        {
            var workFlow = GetWorkflow(workflowVersion);
            return workFlow.Transactions;
        }

        public List<AdditionalStepTransaction<AdditionalStepArgs, AdditionalStepType>> GetAvailableTransactions(
            int workflowVersion, AdditionalStepType statusesEnum)
        {
            // Note: we will not use this method 
            // if you wanna get available transactions please use `GetPointAdditionalSteps` method
            return new List<AdditionalStepTransaction<AdditionalStepArgs, AdditionalStepType>>();
        }

        private async Task<List<AdditionalStepTransaction<AdditionalStepArgs, AdditionalStepType>>> GetPointAdditionalSteps(
            RoutPoint point)
        {
            return point.AdditionalStepWorkFlowVersion is null
                ? new List<AdditionalStepTransaction<AdditionalStepArgs, AdditionalStepType>>()
                : await GetPointAdditionalSteps(point.AdditionalStepWorkFlowVersion.Value, point.Id);
        }

        // TODO: test this please
        public async Task<List<AdditionalStepTransaction<AdditionalStepArgs, AdditionalStepType>>> GetPointAdditionalSteps(
            int workflowVersion, long pointId)
        {
            var transactions = GetTransactions(workflowVersion).ToList();

            var transactionsStepTypes = GetTransactions(workflowVersion)
                .Select(x => x.AdditionalStepType).ToList();

            var transitions = await _additionalStepTransition.GetAll()
                .Where(x => x.RoutePointId == pointId && !x.IsReset)
                .Select(c => c.AdditionalStepType).ToListAsync();

            var availableAdditionalStepTypes = transactionsStepTypes.Except(transitions).ToList();

            return availableAdditionalStepTypes.IsNullOrEmpty()
                ? new List<AdditionalStepTransaction<AdditionalStepArgs, AdditionalStepType>>()
                : transactions.Where(x => availableAdditionalStepTypes.Contains(x.AdditionalStepType)).ToList();
        }
        /// <summary>
        /// This API returns all point transitions, steps that done in tracking including receiver code, files
        /// </summary>
        /// <param name="workflowVersion"></param>
        /// <param name="pointId"></param>
        /// <returns></returns>
        public async Task<List<AdditionalStepTransitionDto>> GetAllPointAdditionalFilesTransitions(long pointId)
        {
            var point= await _routePointRepository.FirstOrDefaultAsync(pointId);

            var transitions = await _additionalStepTransition.GetAll()
                .Where(x => x.RoutePointId == pointId && !x.IsReset && x.IsFile).ToListAsync();
            return ObjectMapper.Map<List<AdditionalStepTransitionDto>>(transitions);
        }

        #region Actions

        private async Task<string> ReceiverConfirmation(AdditionalStepArgs args)
        {
            var point = await _routePointRepository.GetAll().Include(x=>x.ShippingRequestTripFk).Where(x => x.Id == args.PointId)
                .SingleAsync();
            
            if (string.IsNullOrEmpty(args.Code) || !point.Code.Equals(args.Code))
                throw new UserFriendlyException(L("TheReceiverCodeIsIncorrect"));

            //_routePointRepository.Update(args.PointId,
            //    routPoint => routPoint.ShippingRequestTripFk.EndWorking = Clock.Now);
            point.ShippingRequestTripFk.EndWorking = Clock.Now;

            return AdditionalStepWorkflowActionConst.ReceiverConfirmation;
        }
        
        private async Task<string> DeliveryConfirmation(AdditionalStepArgs args)
        {
            bool isExist = await _routePointRepository.GetAll().AnyAsync(x => x.Id == args.PointId);

            if (!isExist) throw new UserFriendlyException(L("PointIsNotFound"));
            args.DocumentId = await _documentFilesManager.SaveDocumentFileBinaryObject(args.DocumentId.ToString(), AbpSession.TenantId);
            var document = ObjectMapper.Map<IHasDocument>(args);
            await UploadFile(document, args.PointId, RoutePointDocumentType.POD);
            
            _routePointRepository.Update(args.PointId, point => point.IsPodUploaded = true);
            
            return AdditionalStepWorkflowActionConst.DeliveryConfirmation;
        }
        
        private async Task<string> UploadEirFile(AdditionalStepArgs args)
        {
            await CheckIfPointExist(args.PointId);
            args.DocumentId = await _documentFilesManager.SaveDocumentFileBinaryObject(args.DocumentId.ToString(), AbpSession.TenantId);
            var document = ObjectMapper.Map<IHasDocument>(args);
            await UploadFile(document, args.PointId, RoutePointDocumentType.Eir);
            
            return AdditionalStepWorkflowActionConst.UploadEirFile;
        }

        private async Task<string> UploadManifestFile(AdditionalStepArgs args)
        {
            await CheckIfPointExist(args.PointId);
            args.DocumentId = await _documentFilesManager.SaveDocumentFileBinaryObject(args.DocumentId.ToString(), AbpSession.TenantId);
            var document = ObjectMapper.Map<IHasDocument>(args);
            await UploadFile(document, args.PointId, RoutePointDocumentType.Manifest);

            return AdditionalStepWorkflowActionConst.UploadManifestFile;
        }



        private async Task<string> UploadConfirmationDocument(AdditionalStepArgs args)
        {
            await CheckIfPointExist(args.PointId);
            args.DocumentId = await _documentFilesManager.SaveDocumentFileBinaryObject(args.DocumentId.ToString(), AbpSession.TenantId);
            var document = ObjectMapper.Map<IHasDocument>(args);
            await UploadFile(document, args.PointId, RoutePointDocumentType.ConfirmationDocuments);

            return AdditionalStepWorkflowActionConst.UploadConfirmationDocument;
        }

        #endregion

        #region Helpers

        private AdditionalStepTransaction<AdditionalStepArgs, AdditionalStepType> GetTransaction(int workflowVersion,
            string action)
        {
            var workFlow = GetWorkflow(workflowVersion);

            var transaction = workFlow.Transactions.FirstOrDefault(c => c.Action.Equals(action));
            if (transaction == null) throw new UserFriendlyException(L("TransactionNotExist"));
            return transaction;
        }

        private WorkFlow<AdditionalStepTransaction<AdditionalStepArgs, AdditionalStepType>> GetWorkflow(int workflowVersion)
        {
            var workFlow = Flows.FirstOrDefault(w => w.Version == workflowVersion);
            if (workFlow == null) throw new UserFriendlyException(L("WorkFlowNotExist"));
            return workFlow;
        }

        private async Task UploadFile(IHasDocument document, long pointId, RoutePointDocumentType documentType)
        {
            if (document is not {}) throw new UserFriendlyException(L("File_Empty_Error"));
            
            await _routePointDocumentRepository.InsertAsync(new RoutPointDocument
            {
                RoutPointId = pointId,
                DocumentContentType = document.DocumentContentType,
                DocumentName = document.DocumentName,
                DocumentId = document.DocumentId,
                RoutePointDocumentType = documentType
            });
        }

        private async Task CheckIfPointExist(long pointId)
        {
            bool isExist = await _routePointRepository.GetAll().AnyAsync(x => x.Id == pointId);

            if (!isExist) throw new UserFriendlyException(L("PointIsNotFound"));
        }

        private async Task LogAdditionalStepTransition(long pointId, AdditionalStepType type, RoutePointDocumentType? routePointDocumentType)
        {
            var transition = new AdditionalStepTransition() { AdditionalStepType = type, RoutePointId = pointId, 
                RoutePointDocumentType = routePointDocumentType,
                IsFile = type != AdditionalStepType.ReceiverCode};

            await _additionalStepTransition.InsertAsync(transition);
        }
        
        private async Task HandlePointDelivery(RoutPoint point, UserIdentifier user)
        {

            // todo check if the current point and other points is at least have `FinishOffLoadingShipment` status

           var trip = await _tripRepository.GetAll().Where(x => x.Id == point.ShippingRequestTripId)
               .Select(x => new
               {
                   x.Id, x.ShippingRequestId, RequestRouteType = x.ShippingRequestFk.RouteTypeId, x.RouteType,
                   ShipperUser = new UserIdentifier(x.ShippingRequestFk.TenantId,x.ShippingRequestFk.CreatorUserId.Value)
               })
               .FirstOrDefaultAsync();
           // todo test this please 
           var availableSteps = await GetPointAdditionalSteps(point);
           bool isCompleted = availableSteps.IsNullOrEmpty();
           
            if (isCompleted)
            {
                point.IsActive = false;
                point.IsComplete = true;
                point.EndTime = Clock.Now;

                bool isOtherPointsCompleted = await _routePointRepository.GetAll()
                    .Where(x => x.ShippingRequestTripId == point.ShippingRequestTripId && x.Id != point.Id).AllAsync(x => x.IsComplete);

                //if current point is completed and all another point in trip is completed the trip statues should be Delivered
                if (isOtherPointsCompleted) await HandleDeliveredTrip(trip.ShippingRequestId,trip.Id,trip.ShipperUser);
                
                return;
            }
             // if point is not completed execute the below
             

             bool isAllRequiredStepsCompleted = await IsAllRequiredStepsCompleted(point.AdditionalStepWorkFlowVersion.Value, point.Id);
           
             // don't mark trip as can be invoiced until all required transactions is completed
             if (!isAllRequiredStepsCompleted) return; 
             
             
             bool isSingleDrop = trip.RouteType == ShippingRequestRouteType.SingleDrop ||
                                 trip.RequestRouteType == ShippingRequestRouteType.SingleDrop;
             
             bool isMultipleDrop = trip.RouteType == ShippingRequestRouteType.MultipleDrops ||
                                   trip.RequestRouteType == ShippingRequestRouteType.MultipleDrops;
             
             
                if (isSingleDrop)
                {
                    _tripRepository.Update(trip.Id, x => x.InvoiceStatus = InvoiceTripStatus.CanBeInvoiced);
                    //we will create invoice in this case if shipper period is PayInAdvance or PayUponDelivery
                    await _invoiceManager.GenertateInvoiceWhenShipmintDelivery(point.ShippingRequestTripId);
                }
                
                else if (isMultipleDrop)
                {
                    var otherPoints = await _routePointRepository.GetAll()
                        .Where(c => c.Id != point.Id && c.PickingType == PickingType.Dropoff && c.ShippingRequestTripId == trip.Id && c.AdditionalStepWorkFlowVersion.HasValue)
                        .Select(x=> new {x.Id, StepWorkflowVerion = x.AdditionalStepWorkFlowVersion.Value}).ToListAsync();

                    foreach (var otherPoint in otherPoints)
                    {
                        bool isOtherPointCompletedRequiredSteps =
                            await IsAllRequiredStepsCompleted(otherPoint.StepWorkflowVerion, otherPoint.Id);
                        
                        if (!isOtherPointCompletedRequiredSteps) return;
                    }
                    
                    _tripRepository.Update(trip.Id, x => x.InvoiceStatus = InvoiceTripStatus.CanBeInvoiced);
                    await _invoiceManager.GenertateInvoiceWhenShipmintDelivery(trip.Id);//we will create invoice in this case if shipper period is PayInAdvance
                    
                }

            
        }
        
        
        /// <summary>
        /// Mark shipping request as delivered if all trips is done
        /// </summary>
        /// <param name="srId"></param>
        /// <param name="tripId"></param>
        /// <param name="shipper"></param>
        private async Task HandleDeliveredTrip(long srId, int tripId, UserIdentifier shipper)
        {
            bool isOtherTripsDelivered = await _tripRepository.GetAll().AllAsync(x =>
                x.Status == ShippingRequestTripStatus.Delivered && x.ShippingRequestId == srId);
            if (isOtherTripsDelivered)
            {
                _tripRepository.Update(tripId, x =>
                {
                    x.ShippingRequestFk.Status = ShippingRequestStatus.Completed;
                    x.Status = ShippingRequestTripStatus.Delivered;
                });
                await _appNotifier.ShipperShippingRequestFinish(shipper, srId);
            }
        }

        private async Task<bool> IsAllRequiredStepsCompleted(int workflowVersion, long pointId)
        {
            var availableSteps = await GetPointAdditionalSteps(workflowVersion, pointId);
            bool isCompleted = availableSteps.IsNullOrEmpty() || availableSteps.All(x => !x.IsRequired);
            
            return isCompleted;
        }
        
        #endregion
        
    }
}