using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Abp.Runtime.Session;
using Abp.Timing;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using TACHYON.Authorization;
using TACHYON.Authorization.Users.Profile;
using TACHYON.Documents.DocumentFiles.Dtos;
using TACHYON.Dto;
using TACHYON.Features;
using TACHYON.Routs.RoutPoints;
using TACHYON.Shipping.Drivers.Dto;
using TACHYON.Shipping.ShippingRequests;
using TACHYON.Shipping.ShippingRequestTrips;
using TACHYON.Shipping.Trips;
using TACHYON.Tracking.Dto;
using TACHYON.Tracking.Dto.WorkFlow;
using AutoMapper.QueryableExtensions;
using TACHYON.Authorization.Users;
using TACHYON.Tracking.AdditionalSteps;
using Abp.Application.Features;
using System.Globalization;
using TACHYON.Invoices;
using TACHYON.Invoices.SubmitInvoices;
using DevExtreme.AspNet.Data.ResponseModel;
using Abp.Extensions;
using TACHYON.Common;

namespace TACHYON.Tracking
{
    [AbpAuthorize(AppPermissions.Pages_Tracking)]
    public class TrackingAppService : TACHYONAppServiceBase
    {
        private readonly IRepository<ShippingRequestTrip> _ShippingRequestTripRepository;
        private readonly IRepository<RoutPoint, long> _RoutPointRepository;
        private readonly IRepository<User, long> _userRepository;
        private readonly ShippingRequestPointWorkFlowProvider _workFlowProvider;
        private readonly ProfileAppService _ProfileAppService;
        private readonly ForceDeliverTripExcelExporter _deliverTripExcelExporter;
        private readonly IRepository<UserOrganizationUnit,long> _userOrganizationUnitRepository;
        private readonly IRepository<ShippingRequestTripAccident> _accidentRepository;
        private readonly AdditionalStepWorkflowProvider _stepWorkflowProvider;
        private readonly IRepository<InvoiceTrip, long> _InvoiceTripRepository;
        private readonly IRepository<SubmitInvoiceTrip, long> _SubmitInvoiceTripRepository;
        private readonly TachyonAppSession _tachyonAppSession;

        public TrackingAppService(
            IRepository<ShippingRequestTrip> shippingRequestTripRepository,
            IRepository<RoutPoint, long> routPointRepository,
            IRepository<User, long> userRepository,
            ShippingRequestPointWorkFlowProvider workFlowProvider,
            ProfileAppService profileAppService,
            ForceDeliverTripExcelExporter deliverTripExcelExporter,
            IRepository<UserOrganizationUnit, long> userOrganizationUnitRepository,
            IRepository<ShippingRequestTripAccident> accidentRepository,
            AdditionalStepWorkflowProvider stepWorkflowProvider,
            IRepository<InvoiceTrip, long> invoiceTripRepository,
            IRepository<SubmitInvoiceTrip, long> submitInvoiceTripRepository,
            TachyonAppSession tachyonAppSession)
        {
            _ShippingRequestTripRepository = shippingRequestTripRepository;
            _RoutPointRepository = routPointRepository;
            _userRepository = userRepository;
            _workFlowProvider = workFlowProvider;
            _ProfileAppService = profileAppService;
            _deliverTripExcelExporter = deliverTripExcelExporter;
            _userOrganizationUnitRepository = userOrganizationUnitRepository;
            _accidentRepository = accidentRepository;
            _stepWorkflowProvider = stepWorkflowProvider;
            _InvoiceTripRepository = invoiceTripRepository;
            _SubmitInvoiceTripRepository = submitInvoiceTripRepository;
            _tachyonAppSession = tachyonAppSession;
        }
        [RequiresFeature(AppFeatures.TachyonDealer, AppFeatures.Carrier, AppFeatures.Shipper)]
        public async Task<PagedResultDto<TrackingListDto>> GetAll(TrackingSearchInputDto input)
        {
           // CheckIfCanAccessService(true, AppFeatures.TachyonDealer, AppFeatures.Carrier, AppFeatures.Shipper);

            bool isCmsEnabled = await FeatureChecker.IsEnabledAsync(AppFeatures.CMS);

            await CheckDirectShipmentTrackingPermission(input.TrackingMode);

            List<long> userOrganizationUnits = null;
            if (isCmsEnabled)
            {
                userOrganizationUnits = await _userOrganizationUnitRepository.GetAll()
                    .Where(x => x.UserId == AbpSession.UserId)
                    .Select(x => x.OrganizationUnitId).ToListAsync();
            }

            var isShipper = await IsEnabledAsync(AppFeatures.Shipper);
            var isCarrier = await IsEnabledAsync(AppFeatures.Carrier);
            var tenantId = AbpSession.TenantId;
            var isTMS = await IsTachyonDealer();

            bool hasCarrierClient = await FeatureChecker.IsEnabledAsync(AppFeatures.CarrierClients);
            
            DisableTenancyFilters();
            DisableShipperActorFilter();

            var query = await (await GetTrip(input))
                .Where(x => input.TrackingMode == ShipmentTrackingMode.Mixed ||
                            (input.TrackingMode == ShipmentTrackingMode.NormalShipment && x.ShippingRequestId.HasValue) ||
                            (input.TrackingMode == ShipmentTrackingMode.DirectShipment && !x.ShippingRequestId.HasValue))
                            .Where(x=> x.ShipperActorId == _tachyonAppSession.ActorShipperId || x.ShippingRequestFk.ShipperActorId == _tachyonAppSession.ActorShipperId || !_tachyonAppSession.ActorShipperId.HasValue) 
                .AsNoTracking()
                .OrderBy(input.Sorting ?? "id desc").PageBy(input).Select(src => new TrackingListDto
            {
                Origin = src.OriginFacilityFk != null ? src.OriginFacilityFk.Name : "",
                Driver = src.AssignedDriverUserFk != null ? src.AssignedDriverUserFk.FullName : "",
                DriverImageProfile = src.AssignedDriverUserFk != null ? src.AssignedDriverUserFk.ProfilePictureId : null,
                RouteTypeId = src.RouteType != null ? src.RouteType.Value : src.ShippingRequestFk.RouteTypeId.Value,
                Destination =  src.DestinationFacilityFk != null ? src.DestinationFacilityFk.Name : "",
                ReferenceNumber = src.ShippingRequestFk.ReferenceNumber,
                ShippingRequestFlag = src.ShippingRequestFk.ShippingRequestFlag,
                NumberOfTrucks = src.ShippingRequestFk.NumberOfTrucks,
                TenantId = src.ShippingRequestFk.TenantId,
                ShippingType = src.ShippingRequestFk.ShippingTypeId,
                RequestId = src.ShippingRequestId,
                DriverRate = src.AssignedDriverUserFk != null ? src.AssignedDriverUserFk.Rate : 0,
                shippingRequestStatus = src.ShippingRequestFk.Status,
                IsPrePayedShippingRequest = src.ShippingRequestFk.IsPrePayed,
                TripFlag = src.ShippingRequestTripFlag,
                IsSass = src.ShippingRequestFk != null ? src.ShippingRequestFk.IsSaas() : src.ShipperTenantId == src.CarrierTenantId,
                NumberOfDrops = src.ShippingRequestFk != null ? src.ShippingRequestFk.NumberOfDrops : src.NumberOfDrops,

                    TruckType = src.AssignedTruckFk.TrucksTypeFk.Translations.Where(x => x.Language == CultureInfo.CurrentCulture.Name).FirstOrDefault() != null
                ? src.AssignedTruckFk.TrucksTypeFk.Translations.Where(x => x.Language == CultureInfo.CurrentCulture.Name).FirstOrDefault().TranslatedDisplayName
                : src.AssignedTruckFk.TrucksTypeFk.Key,
                    GoodsCategory = src.ShippingRequestFk != null ? src.ShippingRequestFk.GoodCategoryFk.Translations.Where(x => x.Language == CultureInfo.CurrentCulture.Name).FirstOrDefault() != null
                ? src.ShippingRequestFk.GoodCategoryFk.Translations.Where(x => x.Language == CultureInfo.CurrentCulture.Name).FirstOrDefault().DisplayName
                : src.ShippingRequestFk.GoodCategoryFk.Key
                : src.GoodCategoryFk.Translations.Where(x => x.Language == CultureInfo.CurrentCulture.Name).FirstOrDefault() != null
                ? src.GoodCategoryFk.Translations.Where(x => x.Language == CultureInfo.CurrentCulture.Name).FirstOrDefault().DisplayName
                : src.GoodCategoryFk.Key,
                    Reason = src.ShippingRequestTripRejectReason != null
                ? src.ShippingRequestTripRejectReason.Translations.Where(x => x.Language == CultureInfo.CurrentCulture.Name).FirstOrDefault() != null
                ? src.ShippingRequestTripRejectReason.Translations.Where(x => x.Language == CultureInfo.CurrentCulture.Name).FirstOrDefault().Name
                : src.ShippingRequestTripRejectReason.Key
                : "",

                    //TruckType = src.AssignedTruckFk.TrucksTypeFk.Key,
                    //GoodsCategory = src.ShippingRequestFk != null ? src.ShippingRequestFk.GoodCategoryFk.Key : src.GoodCategoryFk.Key,
                    //Reason = src.ShippingRequestTripRejectReason != null
                    //? src.ShippingRequestTripRejectReason.Key
                    //: "",

                    CarrierTenantId = src.ShippingRequestFk != null ? src.ShippingRequestFk.CarrierTenantId : src.CarrierTenantId,
                CanDriveTrip = !tenantId.HasValue || tenantId == src.CarrierTenantId || isTMS || tenantId == src.ShippingRequestFk.CarrierTenantId,
                IsAssign = !tenantId.HasValue || (tenantId.HasValue && !isShipper) ? true : false,
                CanStartTrip = !tenantId.HasValue || (tenantId.HasValue && !isShipper) && (src.StartTripDate.Date <= Clock.Now.Date
                      && src.Status == ShippingRequestTripStatus.New
                      && src.DriverStatus == ShippingRequestTripDriverStatus.Accepted
                      ) ? true : false,
                Name = (!tenantId.HasValue || tenantId.HasValue && (isTMS || isCarrier) ) 
                ? src.ShippingRequestFk != null 
                    ?isCarrier 
                        ? src.ShippingRequestFk.Tenant.Name 
                        : $"{src.ShippingRequestFk.Tenant.Name}-{src.ShippingRequestFk.CarrierTenantFk.Name}"
                    : isCarrier
                        ? src.ShipperTenantFk.Name 
                        : src.ShipperTenantFk != null && src.CarrierTenantFk != null 
                            ? $"{src.ShipperTenantFk.Name}-{src.CarrierTenantFk.Name}" 
                            : ""
                 : src.ShippingRequestFk != null ?src.ShippingRequestFk.CarrierTenantFk.Name:src.CarrierTenantFk.Name,
                AssignedDriverUserId = src.AssignedDriverUserId,
                CanceledReason = src.CanceledReason,
                CancelStatus = src.CancelStatus,
                CreatorUserId = src.CreatorUserId,
                DriverStatus = src.DriverStatus,
                DriverStatusTitle = src.DriverStatus.GetEnumDescription(),
                HasAccident = src.HasAccident,
                Id = src.Id,
                isApproveCancledByCarrier = src.IsApproveCancledByCarrier,
                isApproveCancledByShipper = src.IsApproveCancledByShipper,
                IsApproveCancledByTachyonDealer = src.IsApproveCancledByTachyonDealer,
                IsForcedCanceledByTachyonDealer = src.IsForcedCanceledByTachyonDealer,
                NeedsDeliveryNote = src.NeedsDeliveryNote,
                RequestType = src.ShippingRequestFk.RequestType,
                RouteType = src.RouteType != null ? src.RouteType.GetEnumDescription() : src.ShippingRequestFk.RouteTypeId.GetEnumDescription(),
                StartTripDate = src.StartTripDate,
                Status = src.Status,
                StatusTitle = src.Status.GetEnumDescription(),
                WaybillNumber = src.WaybillNumber,
                ShippingRequestFlagTitle = src.ShippingRequestFk != null ? src.ShippingRequestFk.ShippingRequestFlag.GetEnumDescription() : "SAAS",
                ShippingTypeTitle = src.ShippingRequestFk != null ? src.ShippingRequestFk.ShippingTypeId.GetEnumDescription() : "",
                PlateNumber = src.AssignedTruckFk != null ? src.AssignedTruckFk.PlateNumber : "",
                BookingNumber = src.ShippingRequestFk != null ?src.ShippingRequestFk.ShipperInvoiceNo :"",
                SabOrderId = src.SabOrderId
                }).ToListAsync();


            var tripIds = query.Select(x => x.Id);
            var accidentList = await _accidentRepository.GetAll()
                .Where(x => !x.IsResolve && tripIds.Contains(x.RoutPointFK.ShippingRequestTripId))
                .Where(x => x.ReasoneId.HasValue && x.ResoneFK.IsTripImpactEnabled && !x.ForceContinueTripEnabled)
                .Select(x=>x.RoutPointFK.ShippingRequestTripId)
                .ToListAsync();

           

            var tripsNumber = query.Select(x => x.Id);
            var invoices = await _InvoiceTripRepository.GetAll().Select(x => new { invoiceNumber = x.InvoiceFK.InvoiceNumber, TripId = x.TripId }).Where(y => tripIds.Contains(y.TripId)).ToListAsync();

            var SubmitInvoices = await _SubmitInvoiceTripRepository.GetAll().Select(x => new { TripId = x.TripId, ReferencNumber = x.SubmitInvoicesFK.ReferencNumber }).Where(y => tripIds.Contains(y.TripId)).ToListAsync();

            foreach (var item in query)
            {
                item.IsTripImpactEnabled = accidentList.Contains(item.Id);

                if (item.Status == ShippingRequestTripStatus.DeliveredAndNeedsConfirmation || item.Status == ShippingRequestTripStatus.Delivered)
                {
                    item.ShipperInvoiceNumber = invoices.Where(x => x.TripId == item.Id).FirstOrDefault()?.invoiceNumber;
                    item.CarrierInvoiceNumber = SubmitInvoices.Where(x => x.TripId == item.Id).FirstOrDefault()?.ReferencNumber;
                }
            }


            return new PagedResultDto<TrackingListDto>(
                query.ToList().Count,
               query

            );
        }



        [RequiresFeature(AppFeatures.TachyonDealer, AppFeatures.Carrier, AppFeatures.Shipper)]
        public async Task<LoadResult> GetAllDx(string filter, TrackingSearchInputDto input)
        {

           await CheckDirectShipmentTrackingPermission(input.TrackingMode);

            var isShipper = await IsEnabledAsync(AppFeatures.Shipper);
            var isCarrier = await IsEnabledAsync(AppFeatures.Carrier);
            var isBroker = await IsBroker();
            var tenantId = AbpSession.TenantId;
            var isTMS = await IsTachyonDealer();

            DisableTenancyFilters();
            await DisableDraftedFilterIfTachyonDealerOrHost();

            var query =  (await GetTrip(input))
                .Where(x => input.TrackingMode == ShipmentTrackingMode.Mixed ||
                            (input.TrackingMode == ShipmentTrackingMode.NormalShipment && x.ShippingRequestId.HasValue) ||
                            (input.TrackingMode == ShipmentTrackingMode.DirectShipment && !x.ShippingRequestId.HasValue))
                .OrderByDescending(x=>x.Id)
                .ProjectTo<TrackingListDto>(AutoMapperConfigurationProvider).AsNoTracking();

            var result = query.ToList();

            var tripIds = result.Select(x => x.Id);
            var accidents = await _accidentRepository.GetAll()
                .Where(x => !x.IsResolve && tripIds.Contains(x.RoutPointFK.ShippingRequestTripId))
                .Where(x => x.ReasoneId.HasValue && x.ResoneFK.IsTripImpactEnabled && !x.ForceContinueTripEnabled)
                .Select(x => x.RoutPointFK.ShippingRequestTripId)
                .ToListAsync();

            var invoices = await _InvoiceTripRepository.GetAll().Select(x => new { invoiceNumber = x.InvoiceFK.InvoiceNumber, TripId = x.TripId }).Where(y => tripIds.Contains(y.TripId)).ToListAsync();

            var SubmitInvoices = await _SubmitInvoiceTripRepository.GetAll().Select(x => new {TripId =x.TripId, ReferencNumber = x.SubmitInvoicesFK.ReferencNumber }).Where(y => tripIds.Contains(y.TripId)).ToListAsync();

            foreach (var item in result)
            {
                item.IsTripImpactEnabled = accidents.Contains(item.Id);
                if (item.Status == ShippingRequestTripStatus.DeliveredAndNeedsConfirmation || item.Status == ShippingRequestTripStatus.Delivered)
                {
                    item.ShipperInvoiceNumber = invoices.FirstOrDefault(x => x.TripId == item.Id)?.invoiceNumber;
                    item.CarrierInvoiceNumber = SubmitInvoices.Where(x => x.TripId == item.Id).FirstOrDefault()?.ReferencNumber;
                }
                item.Name = (!tenantId.HasValue || tenantId.HasValue && (isTMS || isCarrier))
                ? isCarrier
                        ? item.ShipperName
                        : $"{item.ShipperName}-{item.CarrierName}"
                : item.CarrierName;


                item.CanDriveTrip = !tenantId.HasValue || tenantId == item.CarrierTenantId || isTMS;
                item.IsAssign = !tenantId.HasValue || (tenantId.HasValue && !isShipper);
                item.CanStartTrip = !tenantId.HasValue || (tenantId.HasValue && !isShipper) && (item.StartTripDate.Date <= Clock.Now.Date
                      && item.Status == ShippingRequestTripStatus.New
                      && item.DriverStatus == ShippingRequestTripDriverStatus.Accepted
                      );
                if (!isTMS)
                {
                    if (isShipper || isBroker)
                    {
                        item.IsInvoiceIssued = item.ShipperInvoiceNumber != null;
                    }
                    else
                    {
                        item.IsInvoiceIssued = isCarrier && item.CarrierInvoiceNumber != null;
                    }
                }
            }
            return LoadResult<TrackingListDto>(result, filter);

        }

        

        [AbpAllowAnonymous]
        public async Task<PagedResultDto<TrackingByWaybillDto>> GetDropsOffByMasterWaybill(long waybillNumber)
        {
            DisableTenancyFilters();
            var query = await _RoutPointRepository
             .GetAll().AsNoTracking()
             .Where(x => x.ShippingRequestTripFk.WaybillNumber == waybillNumber ||
             (x.WaybillNumber == waybillNumber &&
             x.ShippingRequestTripFk.ShippingRequestFk.RouteTypeId == ShippingRequestRouteType.MultipleDrops &&
             x.PickingType == PickingType.Dropoff))
             .ProjectTo<TrackingByWaybillDto>(AutoMapperConfigurationProvider).ToListAsync();
            if (!query.Any()) throw new UserFriendlyException(L("InCorrectWaybillNumber"));

            return new PagedResultDto<TrackingByWaybillDto>(query.Count, query);

        }

        [AbpAllowAnonymous]
        public async Task<TrackingByWaybillRoutPointDto> GetDropOffBySubWaybill(string waybillNumber)
        {
            DisableTenancyFilters();
            var dropOff = await _RoutPointRepository
             .GetAll().AsNoTracking()
             .ProjectTo<TrackingByWaybillRoutPointDto>(AutoMapperConfigurationProvider)
             .FirstOrDefaultAsync(x => x.WaybillNumber.Equals(waybillNumber));

            if (dropOff == null) throw new UserFriendlyException(L("InCorrectWaybillNumber"));
            return dropOff;
        }
        public async Task<TrackingShippingRequestTripDto> GetForView(long id)
        {
            CheckIfCanAccessService(true, AppFeatures.TachyonDealer, AppFeatures.Carrier, AppFeatures.Shipper);
            DisableTenancyFilters();
            DisableShipperActorFilter(); // todo throw exception if shipper actor is not the current user actor shipper.

            var hasCarrierClient = await IsEnabledAsync(AppFeatures.CarrierClients);

            var trip =
                 await _ShippingRequestTripRepository.GetAll()
                            .Where(x => x.Id == id)
                            .Where(x => x.ShippingRequestFk.CarrierTenantId.HasValue || x.CarrierTenantId.HasValue)
                            .WhereIf(AbpSession.TenantId.HasValue && !hasCarrierClient && await IsEnabledAsync(AppFeatures.Shipper), x => x.ShippingRequestFk.TenantId == AbpSession.TenantId)
                            .WhereIf(!AbpSession.TenantId.HasValue || await IsEnabledAsync(AppFeatures.TachyonDealer), x => true)
                            .WhereIf(AbpSession.TenantId.HasValue && !hasCarrierClient && await IsEnabledAsync(AppFeatures.Carrier), x => x.ShippingRequestFk.CarrierTenantId == AbpSession.TenantId || x.CarrierTenantId == AbpSession.TenantId)
                            .WhereIf(AbpSession.TenantId.HasValue && hasCarrierClient,
                                            x => x.ShippingRequestFk.TenantId == AbpSession.TenantId ||
                                                 x.ShippingRequestFk.CarrierTenantId == AbpSession.TenantId ||
                                                 x.CarrierTenantId == AbpSession.TenantId)
                            .ProjectTo<TrackingShippingRequestTripDto>(AutoMapperConfigurationProvider)
                            .FirstOrDefaultAsync();

            if (trip == null) throw new UserFriendlyException(L("TheTripIsNotFound"));
            

            trip.RoutPoints = await _RoutPointRepository.GetAll()
                .Where(x => x.ShippingRequestTripId == id)
                .Select(a => new TrackingRoutePointDto
                {
                    Id = a.Id,
                    ShippingRequestTripId = a.ShippingRequestTripId,
                    PickingType = a.PickingType,
                    Status = a.Status,
                    ReceiverFullName = a.ReceiverFk != null ? a.ReceiverFk.FullName : a.ReceiverFullName,
                    Address = a.FacilityFk.Address,
                    lat = a.FacilityFk.Location.Y,
                    lng = a.FacilityFk.Location.X,
                    StartTime = a.StartTime,
                    EndTime = a.EndTime,
                    IsActive = a.IsActive,
                    IsComplete = a.IsComplete,
                    IsResolve = a.IsResolve,
                    CanGoToNextLocation = a.CanGoToNextLocation,
                    IsDeliveryNoteUploaded = a.IsDeliveryNoteUploaded,
                    WaybillNumber = a.WaybillNumber,
                    IsPodUploaded = a.IsPodUploaded,
                    FacilityRate = a.FacilityFk.Rate,
                    ReceiverCode = AbpSession.TenantId.HasValue ? null : a.Code,
                    PointOrder = a.PointOrder,
                    IsHasAdditionalSteps = a.AdditionalStepWorkFlowVersion.HasValue,
                    AdditionalStepWorkFlowVersion = a.AdditionalStepWorkFlowVersion,
                    Statues = _workFlowProvider.GetStatuses(a.WorkFlowVersion,
                        a.RoutPointStatusTransitions.Where(x => !x.IsReset)
                            .Select(x =>
                                new RoutPointTransactionArgDto { Status = x.Status, CreationTime = x.CreationTime })
                            .ToList()),
                    AvailableTransactions =
                        !a.IsResolve || trip.DriverStatus != ShippingRequestTripDriverStatus.Accepted
                            ? new List<PointTransactionDto>()
                            : _workFlowProvider.GetTransactionsByStatus(a.WorkFlowVersion,
                                a.RoutPointStatusTransitions.Where(c => !c.IsReset).Select(v => v.Status).ToList(),
                                a.Status),
                }).ToListAsync();
            trip.RoutPoints =(trip.ShippingType == ShippingTypeEnum.ImportPortMovements || trip.ShippingType == ShippingTypeEnum.ExportPortMovements)
                ? trip.RoutPoints.OrderBy(x => x.PointOrder).ToList()
                : trip.RoutPoints.OrderBy(x => x.PickingType).ToList();

            foreach (var routePoint in trip.RoutPoints.Where(routePoint => routePoint.AdditionalStepWorkFlowVersion is { }))
            {
                var steps =
                    await _stepWorkflowProvider.GetPointAdditionalSteps(routePoint.AdditionalStepWorkFlowVersion.Value,
                        routePoint.Id);
                routePoint.AvailableSteps = steps.Select(x =>
                        new AdditionalStepDto { Action = x.Action, Name = x.Name, StepType = x.AdditionalStepType })
                    .ToList();
            }
            return trip;
        }

        public async Task<FileDto> GetForceDeliverTripExcelFile(int tripId)
        {
            DisableTenancyFilters();
            var tripPoints = await _RoutPointRepository.GetAll()
                .Where(x => x.ShippingRequestTripId == tripId)
                .Select(x => new ExportPointExcelDto() { Id = x.Id, PickingType = x.PickingType, WorkFlowVersion = x.WorkFlowVersion })
                .ToListAsync();

            tripPoints.ForEach(point =>
            {
                point.Transactions = _workFlowProvider.Flows.Where(x => x.Version == point.WorkFlowVersion)
                    .SelectMany(x => x.Transactions).GroupBy(x => x.ToStatus)
                    .Select(x => L(x.Key.ToString())).ToList();
            });

            return _deliverTripExcelExporter.ExportToFile(tripPoints);
        }

        public async Task Accept(int id)
        {
            CheckIfCanAccessService(true, AppFeatures.TachyonDealer, AppFeatures.Carrier, AppFeatures.CarrierClients);
            await _workFlowProvider.Accepted(id);
        }
        public async Task Start(int id)
        {
            CheckIfCanAccessService(true, AppFeatures.TachyonDealer, AppFeatures.Carrier, AppFeatures.CarrierClients);
            await _workFlowProvider.Start(new ShippingRequestTripDriverStartInputDto { Id = id });
        }
        public async Task InvokeStatus(InvokeStatusInputDto input)
        {
            CheckIfCanAccessService(true, AppFeatures.TachyonDealer, AppFeatures.Carrier, AppFeatures.CarrierClients);
            var args = new PointTransactionArgs
            {
                PointId = input.Id,
                Code = input.Code
            };
            await _workFlowProvider.Invoke(args, input.Action);
        }

        // todo add permission here
        public async Task InvokeAdditionalStep(InvokeStepInputDto input)
        {
            var args = new AdditionalStepArgs
            {
                CurrentUser = AbpSession.ToUserIdentifier(),
                PointId = input.Id,
                Code = input.Code,
                DocumentId = input.DocumentId,
                DocumentContentType = input.DocumentContentType,
                DocumentName = input.DocumentName
            };
            
            await _stepWorkflowProvider.Invoke(args, input.Action);
        }
        public async Task NextLocation(long id)
        {
            CheckIfCanAccessService(true, AppFeatures.TachyonDealer, AppFeatures.Carrier, AppFeatures.CarrierClients);
            await _workFlowProvider.GoToNextLocation(id);
        }
        public async Task<List<GetAllUploadedFileDto>> POD(long id)
        {
            CheckIfCanAccessService(true, AppFeatures.TachyonDealer, AppFeatures.Carrier, AppFeatures.Shipper, AppFeatures.CarrierClients);
            return await _workFlowProvider.GetPOD(id);
        }

        public async Task<List<GetAllUploadedFileDto>> GetPointFile(long pointId, RoutePointDocumentType type)
        {
            DisableTenancyFilters();
            return await _workFlowProvider.GetPointFile(pointId, type);
        }

        public async Task<List<AdditionalStepTransitionDto>> GetAllPointAdditionalFilesTransitions(long pointId)
        {
            return await _stepWorkflowProvider.GetAllPointAdditionalFilesTransitions(pointId);
        }

            [AbpAuthorize(AppPermissions.Pages_Tracking_ResetPointReceiverCode)]
        public string ResetPointReceiverCode(long pointId)
        {

            var randomCode = new Random().Next(100000, 999999).ToString();
            _RoutPointRepository.Update(pointId, point => point.Code = randomCode);

            return randomCode;
        }
        public async Task<List<ShippingRequestFilterListDto>> GetRequestReferancesList()
        {
            return await _workFlowProvider.GetRequestReferancesList();
        }


        #region Helper
        //private async Task GetMap(TrackingListDto trip)
        //{
            //var dto = ObjectMapper.Map<TrackingListDto>(trip);


            //using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MayHaveTenant))
            //{
            //    var isTripCreatorUserExist = await _userRepository.GetAll().AnyAsync(x => x.Id == trip.CreatorUserId);
            //    if (trip.CreatorUserId.HasValue && isTripCreatorUserExist)
            //        trip.TenantPhoto = (await _ProfileAppService.GetProfilePictureByUser((long)trip.CreatorUserId))
            //            .ProfilePicture;
            //}

            //if (trip.ShippingRequestFk != null)
            //{
            //    dto.NumberOfDrops = trip.ShippingRequestFk.NumberOfDrops;
            //}
            //else
            //{
            //    dto.NumberOfDrops = trip.NumberOfDrops;
            //}

            //if (trip.AssignedTruckFk != null) dto.TruckType = ObjectMapper.Map<TrucksTypeDto>(trip.AssignedTruckFk.TrucksTypeFk)?.TranslatedDisplayName ?? "";
            //dto.GoodsCategory = ObjectMapper.Map<GoodCategoryDto>(trip.ShippingRequestFk?.GoodCategoryFk ?? trip.GoodCategoryFk)?.DisplayName;
            //if (trip.ShippingRequestTripRejectReason != null)
            //{
            //    dto.Reason = ObjectMapper.Map<ShippingRequestTripRejectReasonListDto>(trip.ShippingRequestTripRejectReason).Name ?? "";
            //}
            //var tenantId = AbpSession.TenantId;
            //if (!tenantId.HasValue || (tenantId.HasValue && !await IsEnabledAsync(AppFeatures.Shipper)))
            //{
            //    if (trip.ShippingRequestFk != null)
            //    {
            //        dto.Name = tenantId.HasValue && IsEnabled(AppFeatures.Carrier)
            //            ? trip.ShippingRequestFk.Tenant.Name
            //            : $"{trip.ShippingRequestFk?.Tenant?.Name}-{trip.ShippingRequestFk?.CarrierTenantFk?.Name}";
            //    }
            //    else
            //    {
            //        dto.Name = tenantId.HasValue && IsEnabled(AppFeatures.Carrier)
            //            ? trip.ShipperTenantFk.Name
            //            : $"{trip.ShipperTenantFk?.Name}-{trip.CarrierTenantFk?.Name}";
            //    }

                //trip.IsAssign = true;
                //trip.CanStartTrip = CanStartTrip(trip);
                // dto.CanAcceptTrip = CanAcceptTrip(trip, workingOnAnotherTrip);
                //if (!dto.CanAcceptTrip)
                //    dto.NoActionReason = CanNotAcceptReason(trip, workingOnAnotherTrip);
                //if (trip.Status == ShippingRequestTripStatus.New && trip.DriverStatus == ShippingRequestTripDriverStatus.Accepted && !dto.CanStartTrip)
                //    dto.NoActionReason = CanNotStartReason(trip, workingOnAnotherTrip);
            //}
            //trip.CanDriveTrip = !tenantId.HasValue || tenantId== trip?.CarrierTenantId || await IsTachyonDealer();
            //trip.IsTripImpactEnabled = await _accidentRepository.GetAll()
            //    .Where(x => !x.IsResolve && x.RoutPointFK.ShippingRequestTripId == trip.Id)
            //    .AnyAsync(x => x.ReasoneId.HasValue && x.ResoneFK.IsTripImpactEnabled && !x.ForceContinueTripEnabled);
            //return dto;
        //}
        private bool CanStartTrip(TrackingListDto trip)
        {
            if (trip.StartTripDate.Date <= Clock.Now.Date
               && trip.Status == ShippingRequestTripStatus.New
               && trip.DriverStatus == ShippingRequestTripDriverStatus.Accepted
               )
                return true;

            return false;
        }

        private async Task<IQueryable<ShippingRequestTrip>> GetTrip(TrackingSearchInputDto input)
        {
            bool isCmsEnabled = await FeatureChecker.IsEnabledAsync(AppFeatures.CMS);

            List<long> userOrganizationUnits = null;
            if (isCmsEnabled)
            {
                userOrganizationUnits = await _userOrganizationUnitRepository.GetAll()
                    .Where(x => x.UserId == AbpSession.UserId)
                    .Select(x => x.OrganizationUnitId).ToListAsync();
            }

            var isShipper = await IsEnabledAsync(AppFeatures.Shipper);
            var isCarrier = await IsEnabledAsync(AppFeatures.Carrier);
            var tenantId = AbpSession.TenantId;
            var isTMS = await IsTachyonDealer();

            bool hasCarrierClient = await FeatureChecker.IsEnabledAsync(AppFeatures.CarrierClients);
            DisableTenancyFilters();
            return _ShippingRequestTripRepository
               .GetAll()
                .Where
               (
                   x => (x.ShippingRequestFk != null &&
                         (x.ShippingRequestFk.Status == ShippingRequestStatus.PostPrice || x.ShippingRequestFk.CarrierTenantId.HasValue)) ||
                        x.CarrierTenantId.HasValue
               )
               .WhereIf
               (
                   !isTMS,
                   x => x.ShippingRequestFk.ShippingRequestFlag == ShippingRequestFlag.Normal ||
                        (x.ShippingRequestFk.ShippingRequestFlag == ShippingRequestFlag.Dedicated && AbpSession.TenantId == x.ShippingRequestFk.TenantId) ||
                        AbpSession.TenantId == x.CarrierTenantId
               )

               .WhereIf
               (
                   AbpSession.TenantId.HasValue && isShipper && !hasCarrierClient,
                   x => x.ShippingRequestFk.TenantId == AbpSession.TenantId
               )
               .WhereIf(!AbpSession.TenantId.HasValue || isTMS, x => true)
               .WhereIf
               (
                   AbpSession.TenantId.HasValue && isCarrier && !hasCarrierClient,
                   x => x.ShippingRequestFk.CarrierTenantId == AbpSession.TenantId || x.CarrierTenantId == AbpSession.TenantId
               )
               .WhereIf
               (
                   AbpSession.TenantId.HasValue && hasCarrierClient,
                   x => x.ShippingRequestFk.CarrierTenantId == AbpSession.TenantId ||
                        x.ShippingRequestFk.TenantId == AbpSession.TenantId ||
                        x.CarrierTenantId == AbpSession.TenantId
               )
               .WhereIf
               (
                   input.PickupFromDate.HasValue && input.PickupToDate.HasValue,
                   x => x.ShippingRequestFk.StartTripDate >= input.PickupFromDate.Value &&
                        x.ShippingRequestFk.StartTripDate <= input.PickupToDate.Value
               )
               .WhereIf
               (
                   input.FromDate.HasValue && input.ToDate.HasValue,
                   x => x.StartTripDate >= input.FromDate.Value && x.StartTripDate <= input.ToDate.Value
               )
                .WhereIf(input.OriginId.HasValue, x => x.ShippingRequestFk.OriginCityId == input.OriginId)
               .WhereIf
               (
                   input.DestinationId.HasValue,
                   x => x.ShippingRequestFk.ShippingRequestDestinationCities.Any(y => y.CityId == input.DestinationId)
               )
               .WhereIf(input.RouteTypeId.HasValue, x => x.ShippingRequestFk.RouteTypeId == input.RouteTypeId)
               .WhereIf(input.TransportTypeId.HasValue, x => x.ShippingRequestFk.TransportTypeId == input.TransportTypeId)
               .WhereIf(input.TruckTypeId.HasValue, x => x.ShippingRequestFk.TrucksTypeId == input.TruckTypeId)
               .WhereIf(input.TruckCapacityId.HasValue, x => x.ShippingRequestFk.CapacityId == input.TruckCapacityId)
               .WhereIf(input.Status.HasValue, x => x.Status == input.Status)
               .WhereIf
               (
                   input.WaybillNumber.HasValue,
                   x => x.WaybillNumber == input.WaybillNumber ||
                        x.RoutPoints.Any(y => y.WaybillNumber == input.WaybillNumber)
               )
               .WhereIf
               (
                   !string.IsNullOrEmpty(input.Shipper),
                   x => x.ShippingRequestFk.Tenant.Name.ToLower().Contains(input.Shipper) ||
                        x.ShippingRequestFk.Tenant.companyName.ToLower().Contains(input.Shipper) ||
                        x.ShippingRequestFk.Tenant.TenancyName.ToLower().Contains(input.Shipper)
               )
               .WhereIf(!string.IsNullOrEmpty(input.ReferenceNumber), x => x.ShippingRequestFk.ReferenceNumber.Contains(input.ReferenceNumber))
               .WhereIf
               (
                   !string.IsNullOrEmpty(input.Carrier),
                   x => x.ShippingRequestFk.CarrierTenantFk.Name.ToLower().Contains(input.Carrier) ||
                        x.ShippingRequestFk.CarrierTenantFk.companyName.ToLower().Contains(input.Carrier) ||
                        x.ShippingRequestFk.CarrierTenantFk.TenancyName.ToLower().Contains(input.Carrier)
               )
               .WhereIf(input.PackingTypeId.HasValue, x => x.ShippingRequestFk.PackingTypeId == input.PackingTypeId)
               .WhereIf(input.GoodsOrSubGoodsCategoryId.HasValue, x => x.ShippingRequestFk.GoodCategoryId == input.GoodsOrSubGoodsCategoryId)
               //.WhereIf(!string.IsNullOrEmpty(input.PlateNumberId), x => x.ShippingRequestFk.AssignedTruckFk.PlateNumber == input.PlateNumberId)
               .WhereIf
               (
                   !string.IsNullOrEmpty(input.DriverNameOrMobile),
                   x => (x.ShippingRequestFk.AssignedDriverUserFk.PhoneNumber == input.DriverNameOrMobile ||
                        (x.ShippingRequestFk.AssignedDriverUserFk != null &&
                         (x.ShippingRequestFk.AssignedDriverUserFk.Name.ToLower().Contains(input.DriverNameOrMobile) ||
                          x.ShippingRequestFk.AssignedDriverUserFk.Surname.ToLower().Contains(input.DriverNameOrMobile))))

                          ||

                         ( x.AssignedDriverUserFk.PhoneNumber == input.DriverNameOrMobile ||
                        (x.AssignedDriverUserFk != null &&
                         (x.AssignedDriverUserFk.Name.ToLower().Contains(input.DriverNameOrMobile) ||
                          x.AssignedDriverUserFk.Surname.ToLower().Contains(input.DriverNameOrMobile))))
                          ||

                          ( x.ReplacesDriverFk.PhoneNumber == input.DriverNameOrMobile ||
                        (x.ReplacesDriverFk != null &&
                         (x.ReplacesDriverFk.Name.ToLower().Contains(input.DriverNameOrMobile) ||
                          x.ReplacesDriverFk.Surname.ToLower().Contains(input.DriverNameOrMobile))))
                          
               )
               .WhereIf
               (
                   input.DeliveryFromDate.HasValue && input.DeliveryToDate.HasValue,
                   x => x.ShippingRequestFk.ShippingRequestTrips.Any
                   (
                       y => y.ActualDeliveryDate >= input.DeliveryFromDate &&
                            y.ActualDeliveryDate <= input.DeliveryToDate
                   )
               )
               .WhereIf
               (
                   !string.IsNullOrEmpty(input.ContainerNumber),
                   x => x.ShippingRequestFk.ShippingRequestTrips.Any(y => y.ContainerNumber == input.ContainerNumber)
               )
               .WhereIf(input.IsInvoiceIssued != null, x => x.ShippingRequestFk.ShippingRequestTrips.Any(y => y.IsShipperHaveInvoice == input.IsInvoiceIssued))
               .WhereIf
               (
                   input.IsSubmittedPOD != null,
                   y => y.RoutPoints.Any(x => x.IsPodUploaded == input.IsSubmittedPOD)
               )
               //todo for tasneem will update request type after dediced
               .WhereIf(input.RequestTypeId == 1, x => x.ShippingRequestFk.TenantId != x.ShippingRequestFk.CarrierTenantId)
               .WhereIf(input.RequestTypeId == 2, x => x.ShippingRequestFk.TenantId == x.ShippingRequestFk.CarrierTenantId)
               .WhereIf
               (
                   isCmsEnabled && !userOrganizationUnits.IsNullOrEmpty(),
                   x =>
                       (x.ShippingRequestFk.CarrierActorId.HasValue && userOrganizationUnits.Contains
                           (x.ShippingRequestFk.CarrierActorFk.OrganizationUnitId)) || (x.ShippingRequestFk.ShipperActorId.HasValue &&
                                                                                        userOrganizationUnits.Contains
                                                                                            (x.ShippingRequestFk.ShipperActorFk.OrganizationUnitId))
               )
               .WhereIf
               (!input.ActorName.IsNullOrEmpty(),
                 x => x.ShipperActorFk.CompanyName.ToLower().Contains(input.ActorName.ToLower())
                 || x.CarrierActorFk.CompanyName.ToLower().Contains(input.ActorName.ToLower())
                 )
                .WhereIf
                (input.ActorType == Actors.ActorTypesEnum.Shipper, x => x.ShipperActorFk.ActorType == Actors.ActorTypesEnum.Shipper)
                .WhereIf
                (input.ActorType == Actors.ActorTypesEnum.Carrier, x => x.CarrierActorFk.ActorType == Actors.ActorTypesEnum.Carrier)
                .WhereIf
                (input.ActorType == Actors.ActorTypesEnum.MySelf, x => x.CarrierActorFk.ActorType == Actors.ActorTypesEnum.MySelf || x.ShipperActorFk.ActorType == Actors.ActorTypesEnum.MySelf )
                
                 ;

        }


        private async Task CheckDirectShipmentTrackingPermission(ShipmentTrackingMode trackingMode)
        {
            if (trackingMode is ShipmentTrackingMode.DirectShipment)
            {
                if (!await PermissionChecker.IsGrantedAsync(AppPermissions.Pages_Tracking_DirectShipmentTracking))
                    throw new AbpAuthorizationException(L("YouMustHavePermissionToAccessDirectShipment"));
            }
        }

        #endregion
    }
}