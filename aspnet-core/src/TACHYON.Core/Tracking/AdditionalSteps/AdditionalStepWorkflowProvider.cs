using Abp;
using Abp.Application.Features;
using Abp.Domain.Repositories;
using Abp.EntityHistory;
using Abp.Runtime.Session;
using Abp.Timing;
using Abp.UI;
using Castle.Core.Internal;
using Microsoft.EntityFrameworkCore;
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
using TACHYON.Shipping.Trips.Dto;
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
        private readonly IRepository<ShippingRequest, long> _shippingRequestRepository;
        private readonly CommonManager _commonManager;
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
            IAbpSession abpSession,
            IRepository<ShippingRequest, long> shippingRequestRepository,
            CommonManager commonManager)
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
                // Ports Movement - Export -> Two way Route with port shuttling, One way route  -> `One Trip Only, Third Trip`
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
            _shippingRequestRepository = shippingRequestRepository;
            _commonManager = commonManager;
        }


        public async Task Invoke(AdditionalStepArgs args, string action)
        {

            RoutPoint point = await _routePointRepository.FirstOrDefaultAsync(args.PointId);

            if (point is not { AdditionalStepWorkFlowVersion: {} }) 
                throw new UserFriendlyException(L("PointHasNotAdditionalSteps"));
            
            var transaction = GetTransaction(point.AdditionalStepWorkFlowVersion.Value, action);

           string reason = await transaction.Func(args);

           await LogAdditionalStepTransition(args.PointId, transaction.AdditionalStepType, transaction.RoutePointDocumentType);

               await HandlePointDelivery(point, transaction.AdditionalStepType);

           
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

        public bool IsPointContainReceiverCodeTransition(
            int workflowVersion, long pointId)
        {
            var transactions = GetTransactions(workflowVersion).ToList();

            var transactionsStepTypes = GetTransactions(workflowVersion)
                .Select(x => x.AdditionalStepType).ToList();


            return transactionsStepTypes.Contains(AdditionalStepType.ReceiverCode);
        }
        /// <summary>
        /// This API returns all point transitions, steps that done in tracking including receiver code, files
        /// </summary>
        /// <param name="workflowVersion"></param>
        /// <param name="pointId"></param>
        /// <returns></returns>
        public async Task<List<AdditionalStepTransitionDto>> GetAllPointAdditionalFilesTransitions(long pointId)
        {
            var point= await _routePointRepository.GetAllIncluding(x=> x.RoutPointDocuments).FirstOrDefaultAsync(x=> x.Id == pointId);

            var transitions = await _additionalStepTransition.GetAll()
                .Where(x => x.RoutePointId == pointId && !x.IsReset && x.IsFile).ToListAsync();
            var dtos= ObjectMapper.Map<List<AdditionalStepTransitionDto>>(transitions);
            foreach(var dto in dtos)
            {
                dto.FileContentType = point.RoutPointDocuments.FirstOrDefault(x => x.RoutePointDocumentType == dto.RoutePointDocumentType)?.DocumentContentType;
            }
            return dtos;
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

            args.DocumentId = await _documentFilesManager.SaveDocumentFileBinaryObject(args.DocumentId.ToString(), args.DocumentContentType, AbpSession.TenantId);

            var document = ObjectMapper.Map<IHasDocument>(args);
            await UploadFile(document, args.PointId, RoutePointDocumentType.POD);
            
            _routePointRepository.Update(args.PointId, point => point.IsPodUploaded = true);
            
            return AdditionalStepWorkflowActionConst.DeliveryConfirmation;
        }
        
        private async Task<string> UploadEirFile(AdditionalStepArgs args)
        {
            await CheckIfPointExist(args.PointId);
            args.DocumentId = await _documentFilesManager.SaveDocumentFileBinaryObject(args.DocumentId.ToString(), args.DocumentContentType, AbpSession.TenantId);
            var document = ObjectMapper.Map<IHasDocument>(args);
            await UploadFile(document, args.PointId, RoutePointDocumentType.Eir);
            
            return AdditionalStepWorkflowActionConst.UploadEirFile;
        }

        private async Task<string> UploadManifestFile(AdditionalStepArgs args)
        {
            await CheckIfPointExist(args.PointId);
            args.DocumentId = await _documentFilesManager.SaveDocumentFileBinaryObject(args.DocumentId.ToString(), args.DocumentContentType, AbpSession.TenantId);
            var document = ObjectMapper.Map<IHasDocument>(args);
            await UploadFile(document, args.PointId, RoutePointDocumentType.Manifest);

            return AdditionalStepWorkflowActionConst.UploadManifestFile;
        }



        private async Task<string> UploadConfirmationDocument(AdditionalStepArgs args)
        {
            await CheckIfPointExist(args.PointId);
            args.DocumentId = await _documentFilesManager.SaveDocumentFileBinaryObject(args.DocumentId.ToString(), args.DocumentContentType, AbpSession.TenantId);
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

            await _additionalStepTransition.InsertAndGetIdAsync(transition);
        }
        
        private async Task HandlePointDelivery(RoutPoint point, AdditionalStepType additionalStepType)
        {
            DisableTenancyFilters();
           var trip = await _tripRepository.GetAll().Where(x => x.Id == point.ShippingRequestTripId)
               .Select(x => new DeliveredTripDto
               {
                   Id = x.Id,
                   ShippingRequestId = x.ShippingRequestId,
                   RequestRouteType = x.ShippingRequestFk.RouteTypeId,
                   RouteType = x.RouteType,
                   Status = x.Status,
                   ShipperUser = x.ShippingRequestId.HasValue ? new UserIdentifier(x.ShippingRequestFk.TenantId, x.ShippingRequestFk.CreatorUserId.Value) : new UserIdentifier(x.ShipperTenantId, x.CreatorUserId.Value),
                   TripShipperTenantId = x.ShippingRequestFk != null && x.ShippingRequestFk.TenantId !=  x.ShippingRequestFk.CarrierTenantId 
                   ?(int?) x.ShippingRequestFk.TenantId 
                   :null,
                   TripCarrierTenantId = x.ShippingRequestFk != null && x.ShippingRequestFk.TenantId != x.ShippingRequestFk.CarrierTenantId
                   ? (int?)x.ShippingRequestFk.CarrierTenantId
                   : null,
                   CarrierTenantId = x.ShippingRequestId != null
                   ? (int)x.ShippingRequestFk.CarrierTenantId
                   : x.CarrierTenantId.Value,
                   InvoiceTripStatus = x.InvoiceStatus,
                   CarrierInvoiceTripStatus = x.CarrierInvoiceStatus
               })
               .FirstOrDefaultAsync();

            bool isCompleted = await CheckIfPointIsCompleted(point);
            if (isCompleted)
            {
                point.IsActive = false;
                point.IsComplete = true;
                point.EndTime = Clock.Now;

                bool isOtherPointsCompleted = await _routePointRepository.GetAll()
                    .Where(x => x.ShippingRequestTripId == point.ShippingRequestTripId && x.Id != point.Id).AllAsync(x => x.IsComplete);

                //if current point is completed and all another point in trip is completed the trip statues should be Delivered
                if (isOtherPointsCompleted)
                {
                    _tripRepository.Update(point.ShippingRequestTripId, x =>
                    {
                        x.Status = ShippingRequestTripStatus.Delivered;
                    });
                    //send notification about rating
                    if(trip.ShippingRequestId != null)
                    {
                        if(trip.TripShipperTenantId != null)
                        await _appNotifier.NotifyTenantWithRating(trip.ShippingRequestId, trip.Id, trip.TripShipperTenantId.Value, trip.TripCarrierTenantId.Value);
                        if(trip.TripCarrierTenantId != null)
                        await _appNotifier.NotifyTenantWithRating(trip.ShippingRequestId, trip.Id, trip.TripCarrierTenantId.Value, trip.TripShipperTenantId.Value);
                    }
                     
                    if (trip.ShippingRequestId != null)
                    {
                        await HandleDeliveredTrip(trip.ShippingRequestId.Value, trip.Id, trip.ShipperUser);
                    }
                }
                
                return;
            }
             // if point is not completed execute the below
             

             //bool isAllRequiredStepsCompleted = await IsAllRequiredStepsCompleted(point.AdditionalStepWorkFlowVersion.Value, point.Id);
           
             // don't mark trip as can be invoiced until all required transactions is completed
             //if (!isAllRequiredStepsCompleted) return; 
             
             
             bool isSingleDrop = trip.RouteType == ShippingRequestRouteType.SingleDrop ||
                                 trip.RequestRouteType == ShippingRequestRouteType.SingleDrop;
             
             bool isMultipleDrop = trip.RouteType == ShippingRequestRouteType.MultipleDrops ||
                                   trip.RequestRouteType == ShippingRequestRouteType.MultipleDrops;

            await _invoiceManager.HandleInvoiceStatus(point.Id, isSingleDrop, additionalStepType == AdditionalStepType.ReceiverCode, trip.InvoiceTripStatus, trip.CarrierInvoiceTripStatus, trip.Id, trip.ShipperUser.TenantId.Value, trip.CarrierTenantId);

        }

        private async Task<bool> CheckIfPointIsCompleted(RoutPoint point)
        {
            var pointRemainingSteps = await GetPointAdditionalSteps(point.AdditionalStepWorkFlowVersion.Value, point.Id);
            if (pointRemainingSteps.Any()) return false;
            return true;
        }


        /// <summary>
        /// Mark shipping request as delivered if all trips is done
        /// </summary>
        /// <param name="srId"></param>
        /// <param name="tripId"></param>
        /// <param name="shipper"></param>
        private async Task HandleDeliveredTrip(long srId, int tripId, UserIdentifier shipper)
        {
            // when set All asyn ==> delivered it get false ? question why ?
            bool isOtherTripsDelivered = await _tripRepository.GetAll().Where(x => x.ShippingRequestId == srId)
                 .AllAsync(x => x.ShippingRequestFk.NumberOfTrips == x.ShippingRequestFk.TotalsTripsAddByShippier
                 && x.Status == ShippingRequestTripStatus.Delivered);
            if (isOtherTripsDelivered)
            {
                _shippingRequestRepository.Update(srId, x =>
                {
                    x.Status = ShippingRequestStatus.Completed;
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