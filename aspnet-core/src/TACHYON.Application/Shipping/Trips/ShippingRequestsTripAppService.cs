using Abp;
using Abp.Application.Features;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.EntityHistory;
using Abp.Extensions;
using Abp.Json;
using Abp.Linq.Extensions;
using Abp.Timing;
using Abp.UI;
using Abp.Webhooks;
using DevExtreme.AspNet.Data.ResponseModel;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using TACHYON.Authorization;
using TACHYON.Authorization.Users;
using TACHYON.Documents;
using TACHYON.Documents.DocumentFiles;
using TACHYON.Documents.DocumentFiles.Dtos;
using TACHYON.Documents.DocumentTypes;
using TACHYON.Documents.DocumentTypes.Dtos;
using TACHYON.Dto;
using TACHYON.Features;
using TACHYON.Goods.Dtos;
using TACHYON.Goods.GoodCategories;
using TACHYON.Goods.GoodsDetails;
using TACHYON.Integration.BayanIntegration.V3;
using TACHYON.Integration.BayanIntegration.V3.Jobs;
using TACHYON.MultiTenancy;
using TACHYON.Notifications;
using TACHYON.Rating;
using TACHYON.Penalties;
using TACHYON.Routs.RoutPoints;
using TACHYON.Routs.RoutPoints.Dtos;
using TACHYON.Routs.RoutPoints.RoutPointSmartEnum;
using TACHYON.Shipping.ShippingRequests;
using TACHYON.Shipping.ShippingRequestTrips;
using TACHYON.Shipping.Trips.Dto;
using TACHYON.Shipping.Trips.RejectReasons.Dtos;
using TACHYON.ShippingRequestTripVases;
using TACHYON.Storage;
using TACHYON.Shipping.ShippingRequestAndTripNotes;
using TACHYON.Shipping.Notes;
using TACHYON.ShippingRequestTripVases.Dtos;
using TACHYON.Tracking;
using TACHYON.Common;
using TACHYON.Commission;
using TACHYON.Shipping.ShippingRequests.Dtos;
using TACHYON.MultiTenancy.Dto;
using TACHYON.WebHooks;
using AutoMapper.QueryableExtensions;

namespace TACHYON.Shipping.Trips
{
    [AbpAuthorize(AppPermissions.Pages_ShippingRequestTrips)]
    public class ShippingRequestsTripAppService : TACHYONAppServiceBase, IShippingRequestsTripAppService
    {
        private readonly IRepository<ShippingRequestTrip> _shippingRequestTripRepository;
        private readonly IRepository<ShippingRequest, long> _shippingRequestRepository;
        private readonly IRepository<RoutPoint, long> _routPointRepository;
        private readonly IRepository<ShippingRequestTripVas, long> _shippingRequestTripVasRepository;
        private readonly IRepository<GoodsDetail, long> _goodsDetailRepository;
        private readonly IRepository<GoodCategory> _goodCategoryRepository;
        private readonly UserManager _userManager;
        private readonly IAppNotifier _appNotifier;
        private readonly ShippingRequestManager _shippingRequestManager;
        private readonly DocumentFilesAppService _documentFilesAppService;
        private readonly IRepository<DocumentFile, Guid> _documentFileRepository;
        private readonly DocumentFilesManager _documentFilesManager;
        private readonly IRepository<DocumentType, long> _documentTypeRepository;
        private readonly IBinaryObjectManager _binaryObjectManager;
        private readonly ITempFileCacheManager _tempFileCacheManager;
        private readonly RatingLogManager _ratingLogManager;
        private readonly IEntityChangeSetReasonProvider _reasonProvider;
        private readonly PenaltyManager _penaltyManager;
        private readonly ShippingRequestTripManager _shippingRequestTripManager;
        private readonly TenantManager _tenantManager;
        private readonly IRepository<ShippingRequestAndTripNote> _shippingRequestAndTripNoteRepository;
        private readonly BayanIntegrationManagerV3 _bayanIntegrationManagerV3;
        private readonly ShippingRequestPointWorkFlowProvider _shippingRequestPointWorkFlowProvider;
        private readonly PriceCommissionManager _priceCommissionManager;
        private readonly IFeatureChecker _featureChecker;
        private readonly IRepository<ShippingRequestDestinationCity> _shippingRequestDestinationCityRepository;
        private readonly AppWebhookPublisher _webhookPublisher;
         private readonly IRepository<TripDriver, long> _tripDriverRepository;


        public ShippingRequestsTripAppService(
            IRepository<ShippingRequestTrip> shippingRequestTripRepository,
            IRepository<ShippingRequest, long> shippingRequestRepository,
            IRepository<RoutPoint, long> routPointRepository,
            IRepository<ShippingRequestTripVas, long> shippingRequestTripVasRepository,
            IRepository<GoodsDetail, long> goodsDetailRepository,
            UserManager userManager,
            IAppNotifier appNotifier,
            ShippingRequestManager shippingRequestManager,
            DocumentFilesAppService documentFilesAppService,
            IRepository<GoodCategory> goodCategoryRepository,
            IRepository<DocumentFile, Guid> documentFileRepository,
            DocumentFilesManager documentFilesManager,
            IRepository<DocumentType, long> documentTypeRepository,
            IBinaryObjectManager binaryObjectManager,
            ITempFileCacheManager tempFileCacheManager,
            RatingLogManager ratingLogManager,
            IEntityChangeSetReasonProvider reasonProvider,
            PenaltyManager penaltyManager,
            ShippingRequestTripManager shippingRequestTripManager,
            TenantManager tenantManager,
            BayanIntegrationManagerV3 bayanIntegrationManagerV3,
            IRepository<ShippingRequestAndTripNote> shippingRequestAndTripNoteRepository,
            ShippingRequestPointWorkFlowProvider shippingRequestPointWorkFlowProvider,
            PriceCommissionManager priceCommissionManager,
            IFeatureChecker featureChecker,
            IRepository<ShippingRequestDestinationCity> shippingRequestDestinationCityRepository,
            AppWebhookPublisher webhookPublisher,
            IRepository<TripDriver, long> tripDriverRepository)
        {
            _shippingRequestTripRepository = shippingRequestTripRepository;
            _shippingRequestRepository = shippingRequestRepository;
            _routPointRepository = routPointRepository;
            _shippingRequestTripVasRepository = shippingRequestTripVasRepository;
            _goodsDetailRepository = goodsDetailRepository;
            _userManager = userManager;
            _appNotifier = appNotifier;
            _shippingRequestManager = shippingRequestManager;
            _documentFilesAppService = documentFilesAppService;
            _goodCategoryRepository = goodCategoryRepository;
            _documentFileRepository = documentFileRepository;
            _documentFilesManager = documentFilesManager;
            this._documentTypeRepository = documentTypeRepository;
            _binaryObjectManager = binaryObjectManager;
            _tempFileCacheManager = tempFileCacheManager;
            _ratingLogManager = ratingLogManager;
            _reasonProvider = reasonProvider;
            _penaltyManager = penaltyManager;
            _shippingRequestTripManager = shippingRequestTripManager;
            _tenantManager = tenantManager;
            _shippingRequestAndTripNoteRepository = shippingRequestAndTripNoteRepository;
            _bayanIntegrationManagerV3 = bayanIntegrationManagerV3;
            _shippingRequestPointWorkFlowProvider = shippingRequestPointWorkFlowProvider;
            _priceCommissionManager = priceCommissionManager;
            _featureChecker = featureChecker;
            _shippingRequestDestinationCityRepository = shippingRequestDestinationCityRepository;
            _webhookPublisher = webhookPublisher;
            _tripDriverRepository = tripDriverRepository;
        }

        /// <summary>
        /// Retrieves a list of shipping request trips that have no associated request ID and are associated with the current tenant.
        /// The results include detailed trip information such as trip dates, status, assigned driver, truck, origin and destination 
        /// facilities, waybill number, and various financial and reference details.
        /// </summary>
        /// <param name="filter">A filter used to refine the queryable collection before returning the results.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a LoadResult with the filtered collection of trips.</returns>
        public async Task<LoadResult> GetAllDx(string filter)
        {
            DisableTenancyFilters();
            var queryable = _shippingRequestTripRepository
                .GetAll()
                .Where(x => x.ShippingRequestId == null)
                .Where(x => x.CarrierTenantId == AbpSession.TenantId || x.ShipperTenantId == AbpSession.TenantId)
                .Select(x => new
                {
                    x.Id,
                    x.StartTripDate,
                    x.EndTripDate,
                    x.StartWorking,
                    x.EndWorking,
                    x.Status,
                    Driver = x.AssignedDriverUserFk.Name + " " + x.AssignedDriverUserFk.Surname,
                    Truck = x.AssignedTruckFk.PlateNumber,
                    Origin = x.OriginFacilityFk.Name,
                    Destination = x.DestinationFacilityFk.Name,
                    x.WaybillNumber,
                    x.SupposedPickupDateFrom,
                    x.SupposedPickupDateTo,
                    x.RouteType,
                    x.NumberOfDrops,
                    ShipperActorName = x.ShipperActorFk.CompanyName,
                    CarrierActorName = x.CarrierActorFk.CompanyName,
                    x.ContainerNumber,
                    x.BayanId,
                    x.SabOrderId,
                    x.ContainerReturnDate,
                    x.IsContainerReturned,
                    x.ShipperReference,
                    x.ShipperInvoiceNo,
                    ReplacesDriver = x.ReplacesDriverFk.Name + " " + x.ReplacesDriverFk.Surname,
                    ActorShipperSubTotalAmountWithCommission = x.ActorShipperPrice.SubTotalAmountWithCommission,
                    ActorShipperTotalAmountWithCommission = x.ActorShipperPrice.TotalAmountWithCommission,

                }
                );

            return await LoadResultAsync(queryable, filter);
        }

        /// <summary>
        /// Retrieves a list of shipping request trips that have no associated request ID and are associated with the current tenant.
        /// The results include detailed trip information such as trip dates, status, assigned driver, truck, origin and destination 
        /// facilities, waybill number, and various financial and reference details.
        /// </summary>
        /// <param name="filter">A filter used to refine the queryable collection before returning the results.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a LoadResult with the filtered collection of trips.</returns>
        public async Task<PagedResultDto<ShippingRequestsTripListDto>> GetAll(ShippingRequestTripFilterInput input)
        {
            DisableTenancyFilters();
            DisableShipperActorFilter();
            
            var isBroker = await FeatureChecker.IsEnabledAsync(AppFeatures.CarrierClients);

            var request = await _shippingRequestRepository.GetAll()
                .WhereIf(AbpSession.TenantId.HasValue && await IsEnabledAsync(AppFeatures.Carrier),
                    x => x.CarrierTenantId == AbpSession.TenantId)
                .WhereIf(AbpSession.TenantId.HasValue && await IsEnabledAsync(AppFeatures.Shipper) && !isBroker,
                    x => x.TenantId == AbpSession.TenantId)
                .WhereIf(AbpSession.TenantId.HasValue && isBroker, x => x.TenantId == AbpSession.TenantId || x.CarrierTenantId == AbpSession.TenantId)
                .FirstOrDefaultAsync(x => x.Id == input.RequestId);
            if (request == null)
                throw new UserFriendlyException(L("ShippingRequestIsNotFound"));

            var query = _shippingRequestTripRepository
                .GetAll()
                .AsNoTracking()
                .Include(x => x.OriginFacilityFk)
                .ThenInclude(x => x.CityFk).ThenInclude(x => x.Translations)
                .Include(x => x.DestinationFacilityFk)
                .ThenInclude(x => x.CityFk).ThenInclude(x => x.Translations)
                .Include(x => x.AssignedTruckFk)
                .Include(x => x.AssignedDriverUserFk)
                .Include(x => x.ShippingRequestTripRejectReason)
                .ThenInclude(t => t.Translations)
        .Where(x => x.ShippingRequestId == request.Id)
        .WhereIf(input.Status.HasValue, e => e.Status == input.Status)
        .OrderBy(!input.Sorting.IsNullOrEmpty() && !input.Sorting.Contains("Facility") ? input.Sorting : "Status asc");


            var resultPage = await query.PageBy(input).ToListAsync();
            resultPage.ForEach(r =>
            {
                if (r.ShippingRequestTripRejectReason != null)
                {
                    var reasone =
                        ObjectMapper.Map<ShippingRequestTripRejectReasonListDto>(r.ShippingRequestTripRejectReason);
                    if (reasone != null)
                    {
                        if (!string.IsNullOrEmpty(r.RejectedReason))
                        {
                            r.RejectedReason = $"{reasone.Name}-{r.RejectedReason}";
                        }
                        else
                        {
                            r.RejectedReason = reasone.Name;
                        }
                    }
                }
            });

            var pageResult = ObjectMapper.Map<List<ShippingRequestsTripListDto>>(resultPage);

            var canAssignTrucksAndDrivers = request.CarrierTenantId == AbpSession.TenantId || await IsTachyonDealer();
            foreach (var r in pageResult)
            {
                r.NotesCount = await GetTripNotesCount(r.Id);
                r.CanAssignTrucksAndDrivers = canAssignTrucksAndDrivers;
            }

            if (!input.Sorting.IsNullOrEmpty() && input.Sorting.Contains("Facility"))
                pageResult = SortByFacility(input.Sorting, pageResult);

            var totalCount = await query.CountAsync();

            var allRatingLogList = await _ratingLogManager.GetAllRatingByUserAsync(await IsShipper() ? RateType.CarrierByShipper : RateType.ShipperByCarrier,
                request.TenantId, request.CarrierTenantId);

            var canTripsCreateTemplate = (from trip in pageResult
                                          select (trip.Id, _routPointRepository.GetAllIncluding(x => x.GoodsDetails)
                                              .Where(x => x.ShippingRequestTripId == trip.Id && x.PickingType == PickingType.Dropoff)
                                              .All(x => x.GoodsDetails != null && x.GoodsDetails.Any()))).ToList();

            foreach (var x in pageResult)
            {

                x.IsTripRateBefore = allRatingLogList.Any(log => log.TripId == log.Id);
                x.CanCreateTemplate = canTripsCreateTemplate.FirstOrDefault(i => i.Id == x.Id).Item2;
                if (!x.BayanId.IsNullOrEmpty())
                {
                    dynamic b = JsonConvert.DeserializeObject(x.BayanId);
                    try
                    {
                        x.BayanId = b.tripId;
                    }
                    catch
                    {
                        x.BayanId = b.ToString();
                    }

                }
            }

            return new PagedResultDto<ShippingRequestsTripListDto>(
                totalCount,
                pageResult
            );
        }

        private static List<ShippingRequestsTripListDto> SortByFacility(string sorting, List<ShippingRequestsTripListDto> pageResult)
        {

            if (sorting.Contains("DESC"))
                pageResult = pageResult
                    .OrderByDescending(x => sorting.Contains("origin") ? x.OriginCity : x.DestinationCity)
                    .ToList();

            else
                pageResult = pageResult
                    .OrderBy(x => sorting.Contains("origin") ? x.OriginCity : x.DestinationCity)
                    .ToList();
            return pageResult;
        }

        /// <summary>
        /// Retrieves the details of a shipping request trip for viewing purposes.
        /// </summary>
        /// <param name="id">The unique identifier of the shipping request trip.</param>
        /// <returns>A DTO containing the detailed view of the shipping request trip.</returns>
        /// <remarks>
        /// This method fetches the trip data, including route points with appointments and clearance VAS,
        /// trip manifest data, destination cities, document attachments, notes count, and assigns relevant
        /// permissions for assigning drivers and trucks. It checks if the current session is a Tachyon dealer 
        /// or matches the carrier tenant ID to determine permissions.
        /// </remarks>
        public async Task<ShippingRequestsTripForViewDto> GetShippingRequestTripForView(int id)
        {
            DisableTenancyFilters();
            DisableShipperActorFilter();
            
            //var trip = await _shippingRequestTripRepository.GetAllIncluding(x => x.ShippingRequestFk).Where(x=>x.Id==id).FirstOrDefaultAsync();
            var trip = await GetTrip(id);
            var shippingRequestTrip = await GetShippingRequestTripForMapper<ShippingRequestsTripForViewDto>(trip);

            var tripPoints = trip.RoutPoints.Where(x => x.NeedsAppointment && x.HasAppointmentVas || (x.NeedsClearance && x.HasClearanceVas));

            var pointHasManifest = await _shippingRequestPointWorkFlowProvider.GetPointHasManifest(id);
            if (pointHasManifest != null)
            {
                shippingRequestTrip.TripManifestDataDto = new TripManifestDataDto();
                var document = await _shippingRequestPointWorkFlowProvider.GetPointAttachment(pointHasManifest.Value, RoutePointDocumentType.Manifest);
                ObjectMapper.Map(document, shippingRequestTrip.TripManifestDataDto);

            }

            foreach (var point in tripPoints)
            {
                var pointDto = shippingRequestTrip.RoutPoints.FirstOrDefault(x => x.PointOrder == point.PointOrder);
                if (point.HasAppointmentVas)
                {
                    pointDto.AppointmentDataDto = new TripAppointmentDataDto();
                    pointDto.AppointmentDataDto.AppointmentDateTime = point.AppointmentDateTime;
                    pointDto.AppointmentDataDto.AppointmentNumber = point.AppointmentNumber;
                    pointDto.AppointmentDataDto.DocumentName = (await _shippingRequestPointWorkFlowProvider.GetPointAttachment(point.Id, RoutePointDocumentType.Appointment))?.DocumentName;
                    var appointmentVas = trip.ShippingRequestTripVases.FirstOrDefault(x => x.RoutePointId == point.Id && x.VasFk.Name.Equals(TACHYONConsts.AppointmentVasName));
                    ObjectMapper.Map(appointmentVas, pointDto.AppointmentDataDto);
                }

                if (point.HasClearanceVas)
                {
                    var clearanceVas = trip.ShippingRequestTripVases.FirstOrDefault(x => x.RoutePointId == point.Id && x.VasFk.Name.Equals(TACHYONConsts.ClearanceVasName));
                    pointDto.TripClearancePricesDto = new TripClearancePricesDto();
                    ObjectMapper.Map(clearanceVas, pointDto.TripClearancePricesDto);
                }

            }

            var index = 1;
            foreach (var destCity in trip.ShippingRequestDestinationCities)
            {
                var city = ObjectMapper.Map<TenantCityLookupTableDto>(destCity.CityFk).DisplayName;
                if (index == 1)
                {
                    shippingRequestTrip.DestinationCities = city;
                }
                else
                {
                    shippingRequestTrip.DestinationCities = shippingRequestTrip.DestinationCities + ", " + city;
                }
                index++;
            }

            if (shippingRequestTrip.HasAttachment)
            {
                var documentFile =
                    await _documentFileRepository.FirstOrDefaultAsync(x => x.ShippingRequestTripId == id);
                if (documentFile != null)
                {
                    shippingRequestTrip.DocumentFile = ObjectMapper.Map<DocumentFileDto>(documentFile);
                }
            }
            shippingRequestTrip.RoutPoints = shippingRequestTrip.RoutPoints.OrderBy(x => x.PickingType).ToList();
            shippingRequestTrip.NotesCount = await GetTripNotesCount(id);
            shippingRequestTrip.CanAssignDriversAndTrucks = await IsTachyonDealer() || AbpSession.TenantId == trip.ShippingRequestFk?.CarrierTenantId;
            if (trip.ShippingRequestFk != null)
            {
                shippingRequestTrip.isRequestOwner = AbpSession.TenantId == trip.ShippingRequestFk.TenantId && !trip.ShippingRequestFk.IsSaas();
            }

            return shippingRequestTrip;
        }

       
        /// <summary>
        /// Retrieves a list of assigned drivers to a trip.
        /// The results include assigned driver and truck details.
        /// </summary>
        /// <param name="tripId">The trip id to fetch and project to DTO.</param>
        /// <param name="filter">A filter used to refine the queryable collection before returning the results.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a LoadResult with the filtered collection of drivers.</returns>
        public async Task<LoadResult> GetTripDriversByTripIdDX(long tripId , string filter)
    {
        // Fetch and project to DTO using AutoMapper's ProjectTo
            var queryable =  _tripDriverRepository
                .GetAllIncluding(td => td.Driver, td => td.TruckFk)
                .Where(td => td.ShippingRequestTripId == tripId)
                .OrderBy(x => x.CreationTime)
                .ProjectTo<TripDriverForViewDto>(AutoMapperConfigurationProvider);


            return await LoadResultAsync(queryable, filter);
    }


        /// <summary>
        /// Retrieves a shipping request trip for editing by given trip id.
        /// It returns a <see cref="CreateOrEditShippingRequestTripDto"/> which includes trip information,
        /// rout points and their appointment and clearance details.
        /// </summary>
        /// <param name="input">The trip id to fetch and project to DTO.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="CreateOrEditShippingRequestTripDto"/>.</returns>
        public async Task<CreateOrEditShippingRequestTripDto> GetShippingRequestTripForEdit(EntityDto input)
        {
            DisableTenancyFilters();
            var trip = await GetTrip(input.Id);

            var shippingRequestTrip =
                await GetShippingRequestTripForMapper<CreateOrEditShippingRequestTripDto>(trip);

            var tripPoints = trip.RoutPoints.Where(x => x.NeedsAppointment && x.HasAppointmentVas || (x.NeedsClearance && x.HasClearanceVas));
            foreach (var point in tripPoints)
            {
                var pointDto = shippingRequestTrip.RoutPoints.FirstOrDefault(x => x.PointOrder == point.PointOrder);

                if (point.HasAppointmentVas)
                {
                    pointDto.AppointmentDataDto = new TripAppointmentDataDto
                    {
                        AppointmentDateTime = point.AppointmentDateTime,
                        AppointmentNumber = point.AppointmentNumber,
                        DocumentName = (await _shippingRequestPointWorkFlowProvider.GetPointAttachment(point.Id, RoutePointDocumentType.Appointment))?.DocumentName
                    };
                    var appointmentVas = trip.ShippingRequestTripVases.FirstOrDefault(x => x.RoutePointId == point.Id && x.VasFk.Name.Equals(TACHYONConsts.AppointmentVasName));
                    ObjectMapper.Map(appointmentVas, pointDto.AppointmentDataDto);
                }
                if (point.HasClearanceVas)
                {
                    var clearanceVas = trip.ShippingRequestTripVases.FirstOrDefault(x => x.RoutePointId == point.Id && x.VasFk.Name.Equals(TACHYONConsts.ClearanceVasName));
                    pointDto.TripClearancePricesDto = new TripClearancePricesDto();
                    ObjectMapper.Map(clearanceVas, pointDto.TripClearancePricesDto);
                }

            }
            // fill Attachment file
            if (shippingRequestTrip.HasAttachment)
            {
                var documentFile =
                    await _documentFileRepository.FirstOrDefaultAsync(x => x.ShippingRequestTripId == input.Id);
                if (documentFile != null)
                {
                    shippingRequestTrip.CreateOrEditDocumentFileDto =
                        ObjectMapper.Map<CreateOrEditDocumentFileDto>(documentFile);
                }
            }
            else
            {
                var documentType = await _documentTypeRepository.SingleAsync(x =>
                    x.SpecialConstant.Contains(TACHYONConsts.TripAttachmentDocumentTypeSpecialConstant));
                shippingRequestTrip.CreateOrEditDocumentFileDto = new CreateOrEditDocumentFileDto
                {
                    ShippingRequestTripId = shippingRequestTrip.Id
                };
                shippingRequestTrip.CreateOrEditDocumentFileDto.DocumentTypeDto =
                    ObjectMapper.Map<DocumentTypeDto>(documentType);
            }

            //Identify the prices objects if the shipment is entered from import, no prices saved in import process
            if (shippingRequestTrip.ShippingRequestId == null && shippingRequestTrip.ActorCarrierPrice == null) shippingRequestTrip.ActorCarrierPrice = new PriceOffers.Dto.CreateOrEditActorCarrierPrice();
            if (shippingRequestTrip.ShippingRequestId == null && shippingRequestTrip.ActorShipperPrice == null) shippingRequestTrip.ActorShipperPrice = new PriceOffers.Dto.CreateOrEditActorShipperPriceDto();

            return shippingRequestTrip;
        }

        /// <summary>
        /// Retrieves an empty shipping request trip for creation.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains a CreateOrEditShippingRequestTripDto object with a document type set.</returns>
        public async Task<CreateOrEditShippingRequestTripDto> GetShippingRequestTripForCreate()
        {
            var shippingRequestTrip =
                new CreateOrEditShippingRequestTripDto
                {
                    CreateOrEditDocumentFileDto = new CreateOrEditDocumentFileDto()
                };
            //Fill documentType  
            var documentType = await _documentTypeRepository.SingleAsync(x =>
                x.SpecialConstant.Contains(TACHYONConsts.TripAttachmentDocumentTypeSpecialConstant));
            shippingRequestTrip.CreateOrEditDocumentFileDto.DocumentTypeDto =
                ObjectMapper.Map<DocumentTypeDto>(documentType);

            var tenantId = AbpSession.TenantId;

            if (tenantId != null)
            {
                var maxWaybillsNo =await _shippingRequestTripManager.GetMaxNumberOfWaybills(tenantId.Value);
                if (maxWaybillsNo != null)
                {
                    var createdWaybillsNo = await _shippingRequestTripManager.GetWaybillsNo(tenantId.Value);
                    if (createdWaybillsNo >= maxWaybillsNo.Value)
                    {
                        shippingRequestTrip.IsExceedMaxWaybillsNumber = true;
                    }
                }
            }

            return shippingRequestTrip;
        }

        /// <summary>
        /// Creates or edits a shipping request trip.
        /// </summary>
        /// <param name="input">A CreateOrEditShippingRequestTripDto object containing the trip information.</param>
        /// <exception cref="UserFriendlyException">If the user does not have permission to create a home delivery trip.</exception>
        /// <exception cref="UserFriendlyException">If the goods category is not valid for the given shipping type.</exception>
        /// <exception cref="UserFriendlyException">If the receiver is not provided.</exception>
        /// <exception cref="UserFriendlyException">If the goods description or quantity is not provided.</exception>
        /// <exception cref="UserFriendlyException">If the trip dates are not valid for the given shipping request.</exception>
        /// <exception cref="UserFriendlyException">If the number of drops is not valid for the given shipping request.</exception>
        /// <exception cref="UserFriendlyException">If the total weight of the goods is not valid for the given shipping request.</exception>
        /// <exception cref="UserFriendlyException">If the dedicated request trip dates are not valid.</exception>
        /// <exception cref="UserFriendlyException">If the truck and driver are not valid for the given shipping request.</exception>
        /// <exception cref="UserFriendlyException">If the port movement request is not valid.</exception>
        public async Task CreateOrEdit(CreateOrEditShippingRequestTripDto input)
        {
            await DisableTenancyFiltersIfTachyonDealer();

            if(input.ShippingRequestTripFlag == ShippingRequestTripFlag.HomeDelivery && !_featureChecker.IsEnabled(AbpSession.TenantId.Value, AppFeatures.HomeDelivery))
            {
                throw new UserFriendlyException(L("YounDon'tHavePermissionToHomeDelivery"));
            }

            ShippingRequest request = null;

            if (input.ShippingRequestId.HasValue)
            {
                request = await GetShippingRequestByPermission(input.ShippingRequestId.Value);
                if (request.ShippingTypeId == ShippingTypeEnum.ImportPortMovements || request.ShippingTypeId == ShippingTypeEnum.ExportPortMovements)
                {
                    _shippingRequestManager.OverridePortMovementRoutInputsForTrip(input, request);
                }
                else
                {
                    // validate goods category if shipping type not port movements, bcz port movements has sometimes empty container category that breaks normal goods category validation
                    await ValidateGoodsCategory(input.RoutPoints, request.GoodCategoryId);

                    //additional receiver must provided
                    ValidateReceiver(input);

                    if (input.ShippingRequestTripFlag != ShippingRequestTripFlag.HomeDelivery && input.RoutPoints.Where(x => x.PickingType == PickingType.Dropoff).SelectMany(x => x.GoodsDetailListDto).Any(x => x.Description == null))
                    {
                        throw new UserFriendlyException("GoodsDescriptionIsRequired");
                    }

                    if (input.ShippingRequestTripFlag != ShippingRequestTripFlag.HomeDelivery && input.RoutPoints.Where(x => x.PickingType == PickingType.Dropoff).SelectMany(x => x.GoodsDetailListDto).Any(x => x.Amount == null))
                    {
                        throw new UserFriendlyException("GoodsQuantityIsRequired");
                    }
                }

                if (request.ShippingRequestFlag == ShippingRequestFlag.Normal)
                {
                    request = await GetShippingRequestByPermission(input.ShippingRequestId.Value);

                    _shippingRequestTripManager.ValidateTripDates(input, request);
                    _shippingRequestTripManager.ValidateNumberOfDrops(input.RoutPoints.Count(x => x.PickingType == PickingType.Dropoff), request);
                    //if (request.ShippingTypeId != ShippingTypeEnum.ImportPortMovements && request.ShippingTypeId != ShippingTypeEnum.ExportPortMovements)
                    _shippingRequestTripManager.ValidateTotalweight(input.RoutPoints.Where(x => x.PickingType == PickingType.Dropoff).SelectMany(x => x.GoodsDetailListDto).ToList<ICreateOrEditGoodsDetailDtoBase>(), request);

                }

                if (request != null && request.ShippingRequestFlag == ShippingRequestFlag.Dedicated)
                {
                    if (input.RouteType == ShippingRequestRouteType.SingleDrop) input.NumberOfDrops = 1;
                    _shippingRequestTripManager.ValidateDedicatedRequestTripDates(input, request);

                    if (input.ShippingRequestTripFlag == ShippingRequestTripFlag.HomeDelivery)
                    {
                        input.NumberOfDrops = input.RoutPoints.Count(x => x.PickingType == PickingType.Dropoff);
                    }
                    else  //validate number of drops if normal trip
                    {
                        _shippingRequestTripManager.ValidateDedicatedNumberOfDrops(input.RoutPoints.Count(x => x.PickingType == PickingType.Dropoff), input.NumberOfDrops);
                    }
                    await _shippingRequestTripManager.ValidateTruckAndDriver(input);
                }

                await ValidatePortMovementRequest(input, request);
            }

            else // shipping type validation
            {
                if (input.ShippingTypeId == ShippingTypeEnum.ImportPortMovements || input.ShippingTypeId == ShippingTypeEnum.ExportPortMovements)
                    await _shippingRequestManager.ValidatePortMovementInputs(input.OriginFacilityId, input.RouteType.Value, input.NumberOfDrops, input.ShippingTypeId.Value, input.RoundTripType);

                _shippingRequestManager.ValidateDestinationCities(input.RouteType.Value, input.ShippingRequestDestinationCities, input.ShippingTypeId.Value);

            }

            if (!input.Id.HasValue)
            {
                

                if (request != null)
                {
                    if (request.ShippingRequestFlag == ShippingRequestFlag.Normal)
                    {
                        await _shippingRequestTripManager.ValidateNumberOfTrips(request, 1);
                    }

                    request.TotalsTripsAddByShippier += 1;
                }

                await Create(input, request);
                if (request != null && request.ShippingRequestFlag == ShippingRequestFlag.Dedicated)
                {
                    //add all vases automatic in dedicated trip
                    AddAllRequestVasesToDedicatedTrip(input, request);

                }
            }
            else

            {
                await Update(input, request);
            }
        }

        /// <summary>
        /// Sets the appointment data for given route point.
        /// </summary>
        /// <param name="input">The appointment data to set.</param>
        /// <param name="point">The route point to set the appointment data for.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        /// <exception cref="UserFriendlyException">If the route point does not need an appointment.</exception>
        public async Task SetAppointmentData(TripAppointmentDataDto input, RoutPoint point)
        {
            if (!point.NeedsAppointment) { throw new UserFriendlyException(L("DropDoesNotNeedAppointment")); }
            _priceCommissionManager.Calculate(input);

            if (point.HasAppointmentVas) 
            {
                if (!await IsTachyonDealer() && point.ShippingRequestTripFk.ShippingRequestId != null) return;
                //appointment vas is exists for this point
                //here will update appointment vas
                var appointmentTripVas = point.ShippingRequestTripFk.ShippingRequestTripVases.FirstOrDefault(x => x.RoutePointId == point.Id && x.VasFk.Name.Equals(TACHYONConsts.AppointmentVasName));

                ObjectMapper.Map(input, appointmentTripVas);
            }
            else // add appointment data and vas for first time
            {
                // Add shipping request vas if not exits
                var srVasId = await _shippingRequestManager.AddPortMovementShippingRequestVases(input.ShippingRequestId, true, false);

                //Add vas to trip vas
                var tripVas = ObjectMapper.Map<ShippingRequestTripVas>(input);

                if (input.ShippingRequestId != null)
                {
                    tripVas.ShippingRequestVasId = srVasId ?? throw new UserFriendlyException(L("VasMissing"));
                }
                
                tripVas.VasId = await _shippingRequestManager.GetPortMovementVasId(true, false);
                
                tripVas.ShippingRequestTripId = point.ShippingRequestTripId;
                tripVas.RoutePointId = point.Id;
                
                point.ShippingRequestTripFk.ShippingRequestTripVases.Add(tripVas);
                
                point.HasAppointmentVas = true;  
            }

            //Set appointment data
            point.AppointmentDateTime = input.AppointmentDateTime;
            point.AppointmentNumber = input.AppointmentNumber;

            // appointment attachment
            if (input.DocumentId != null)
            {
                input.DocumentId = await _documentFilesManager.SaveDocumentFileBinaryObject(input.DocumentId.ToString(), input.DocumentContentType, AbpSession.TenantId);
                var document = ObjectMapper.Map<IHasDocument>(input);
                await _shippingRequestPointWorkFlowProvider.UploadFiles(new List<IHasDocument> { document }, point.Id, RoutePointDocumentType.Appointment);
            }
        }

        /// <summary>
        /// Sets the clearance prices for given route point.
        /// </summary>
        /// <param name="input">The clearance prices to set.</param>
        /// <param name="point">The route point to set the clearance prices for.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        /// <exception cref="UserFriendlyException">If the route point does not need clearance.</exception>
        public async Task SetClearanceData(TripClearancePricesDto input, RoutPoint point)
        {
            if (!point.NeedsClearance) { throw new UserFriendlyException(L("DropDoesNotNeedClearance")); }
            _priceCommissionManager.Calculate(input);

            if (point.HasClearanceVas)
            {
                if (!await IsTachyonDealer() && point.ShippingRequestTripFk.ShippingRequestId != null) return;
                //appointment vas is exists for this point
                //here will update appointment vas
                var appointmentTripVas = point.ShippingRequestTripFk.ShippingRequestTripVases.FirstOrDefault(x => x.RoutePointId == point.Id && x.VasFk.Name.Equals(TACHYONConsts.ClearanceVasName));

                ObjectMapper.Map(input, appointmentTripVas);
            }
            else // add appointment data and vas for first time
            {
                // Add shipping request vas if not exits
                var srVasId = await _shippingRequestManager.AddPortMovementShippingRequestVases(input.ShippingRequestId, false, true);

                    //Add vas to trip vas
                    var tripVas = ObjectMapper.Map<ShippingRequestTripVas>(input);

                if (input.ShippingRequestId != null) { tripVas.ShippingRequestVasId = srVasId ?? throw new UserFriendlyException(L("VasMissing")); }
                
                    tripVas.VasId = await _shippingRequestManager.GetPortMovementVasId(false, true);
                
                    tripVas.ShippingRequestTripId = point.ShippingRequestTripId;
                    tripVas.RoutePointId = point.Id;

                    point.ShippingRequestTripFk.ShippingRequestTripVases.Add(tripVas);
                
                point.HasClearanceVas = true;
            }
        }

        /// <summary>
        /// Sets the appointment data for a specified route point on behalf of a carrier or TMS.
        /// </summary>
        /// <param name="input">The appointment data to set.</param>
        /// <param name="routePointId">The ID of the route point to set the appointment data for.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task CarrierSetAppointmentData(TripAppointmentDataDto input, long routePointId)
        {
            //carrier or TMS set appointment prices
            RoutPoint point = await GetDropPoint(routePointId);
            input.ShippingRequestId = point.ShippingRequestTripFk.ShippingRequestId.Value;
            await SetAppointmentData(input, point);
        }

        private async Task<RoutPoint> GetDropPoint(long pointId)
        {
            DisableTenancyFilters();
            var point = await _routPointRepository.GetAll().Include(x => x.ShippingRequestTripFk)
                .ThenInclude(x => x.ShippingRequestFk).ThenInclude(x => x.ShippingRequestVases).ThenInclude(x => x.VasFk)
                .Include(x => x.ShippingRequestTripFk).ThenInclude(x => x.ShippingRequestTripVases)
                .ThenInclude(x => x.ShippingRequestVasFk).ThenInclude(x => x.VasFk)
                .WhereIf(await IsTachyonDealer(), x => true)
                .WhereIf(!await IsTachyonDealer(), x => x.ShippingRequestTripFk.ShippingRequestFk.CarrierTenantId == AbpSession.TenantId)
                .FirstOrDefaultAsync(x => x.Id == pointId);

            if (point == null) { throw new UserFriendlyException(L("PointNotFound")); }

            return point;
        }

        /// <summary>
        /// Sets the clearance data for a specified route point on behalf of a carrier or TMS.
        /// </summary>
        /// <param name="input">The clearance prices data to set.</param>
        /// <param name="routePointId">The ID of the route point to set the clearance data for.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task CarrierSetClearanceData(TripClearancePricesDto input, long routePointId)
        {
            RoutPoint point = await GetDropPoint(routePointId);
            input.ShippingRequestId = point.ShippingRequestTripFk.ShippingRequestId.Value;
            await SetClearanceData(input, point);
        }

        /// <summary>
        /// Retrieves the appointment file associated with a specific route point.
        /// </summary>
        /// <param name="pointId">The ID of the route point for which to retrieve the appointment file.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of uploaded file DTOs related to the appointment.</returns>
        public async Task<List<GetAllUploadedFileDto>> GetAppointmentFile(long pointId)
        {
            DisableTenancyFilters();
            return await _shippingRequestPointWorkFlowProvider.GetPointFile(pointId, RoutePointDocumentType.Appointment);
        }


        //[RequiresFeature(AppFeatures.Shipper)]
        //public async Task ChangeAddTripsByTmsFeature()
        //{
        //    if (!AbpSession.TenantId.HasValue) return;
        //    await TenantManager.SetFeatureValueAsync(AbpSession.TenantId.Value, AppFeatures.AddTripsByTachyonDeal,
        //        await IsEnabledAsync(AppFeatures.AddTripsByTachyonDeal) ? "false" : "true");
        //}
        public async Task<FileDto> GetTripAttachmentFileDto(int id)
        {
            DisableTenancyFilters();
            var documentFile = new DocumentFile();
            using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MustHaveTenant, AbpDataFilters.MayHaveTenant))
            {
                var user = await _userManager.FindByIdAsync(AbpSession.UserId.ToString());
                documentFile = await _documentFileRepository.GetAll()
                    .WhereIf(IsEnabled(AppFeatures.Shipper),
                        x => x.ShippingRequestTripFk.ShippingRequestFk.TenantId == AbpSession.TenantId)
                    .WhereIf(IsEnabled(AppFeatures.Carrier),
                        x => x.ShippingRequestTripFk.ShippingRequestFk.CarrierTenantId == AbpSession.TenantId)
                    .WhereIf(user.IsDriver, x => x.ShippingRequestTripFk.AssignedDriverUserId == AbpSession.UserId)
                    .FirstOrDefaultAsync(x => x.ShippingRequestTripId == id && x.ShippingRequestTripFk.HasAttachment);
            }

            if (documentFile == null)
            {
                throw new UserFriendlyException(L("TheFileIsNotFound"));
            }

            var binaryObject = await _binaryObjectManager.GetOrNullAsync(documentFile.BinaryObjectId.Value);
            var file = new FileDto(documentFile.Name, documentFile.Extn);

            _tempFileCacheManager.SetFile(file.FileToken, binaryObject.Bytes);

            return file;
        }

       
        /// <summary>
        /// Updates the expected delivery time for a trip.
        /// </summary>
        /// <param name="input">An UpdateExpectedDeliveryTimeInput containing the trip ID and expected delivery time.</param>
        /// <exception cref="UserFriendlyException">If the trip does not exist.</exception>
        /// <exception cref="UserFriendlyException">If the trip is already started.</exception>
        /// <exception cref="UserFriendlyException">If the expected delivery time is not valid for the given trip.</exception>
        public async Task UpdateExpectedDeliveryTimeForTrip(UpdateExpectedDeliveryTimeInput input)
        {
            DisableTenancyFilters();
            var trip = await _shippingRequestTripRepository.GetAllIncluding(x => x.ShippingRequestFk)
                .SingleAsync(x => x.Id == input.Id);

            await ValidateExpectedDeliveryTime(input.ExpectedDeliveryTime, trip);
            trip.ExpectedDeliveryTime = input.ExpectedDeliveryTime;
        }

        #region AssignDriverAndTruck
        /// <summary>
        /// Assigns a driver and truck to a shipping request trip by the carrier.
        /// </summary>
        /// <param name="input">The input containing the trip ID, assigned driver user ID, and assigned truck ID.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task AssignDriverAndTruckToShippmentByCarrier(AssignDriverAndTruckToShippmentByCarrierInput input)
        {
            ShippingRequestTrip trip = await GetShippingRequestTrip(input.Id);
            using (UnitOfWorkManager.Current.SetTenantId(trip.ShippingRequestFk.CarrierTenantId))
            {
                await ValidateTripAndDriverAssignment(trip, input.AssignedDriverUserId, input.AssignedTruckId);

                long? oldAssignedDriverUserId = trip.AssignedDriverUserId;
                long? oldAssignedTruckId = trip.AssignedTruckId;

                AssignDriverAndTruckToTrip(trip, input);

                ResetDriverStatusIfNeeded(trip);

                bool isDriverChanged = oldAssignedDriverUserId != trip.AssignedDriverUserId;
                bool isTruckChanged = oldAssignedTruckId != input.AssignedTruckId;

                if (isDriverChanged)
                {
                    await HandleDriverChangeNotifications(trip, oldAssignedDriverUserId);
                }

                await UserManager.UpdateUserDriverStatus(oldAssignedDriverUserId.Value, UserDriverStatus.Available);

                SetUpdateReason(isDriverChanged, isTruckChanged);

                await UpdateDriverStatus(input.AssignedDriverUserId, UserDriverStatus.NotAvailable);

                if (ShouldNotifyCarrier(oldAssignedTruckId, oldAssignedDriverUserId, trip))
                {
                    await NotifyCarrierOfTripUpdate(trip);
                }

                if (!oldAssignedTruckId.HasValue && !oldAssignedDriverUserId.HasValue)
                {
                    await ApplyNotAssigningPenalty(trip);
                }

                await NotifyDriverOfAssignment(trip);

                await CurrentUnitOfWork.SaveChangesAsync();
            }

            await QueueBayanIntegrationUpdate(input.Id, input.AssignedDriverUserId, input.AssignedTruckId);
        }

        /// <summary>
        /// Retrieves a shipping request trip with its related shipping request and assigned driver.
        /// If the trip does not exist, a user-friendly exception is thrown.
        /// </summary>
        /// <param name="tripId">The ID of the trip to retrieve.</param>
        /// <returns>The retrieved shipping request trip.</returns>
        private async Task<ShippingRequestTrip> GetShippingRequestTrip(long tripId)
        {
            using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MayHaveTenant, AbpDataFilters.MustHaveTenant))
            {
                var trip = await _shippingRequestTripRepository.GetAll()
                    .Include(e => e.ShippingRequestFk)
                    .Include(d => d.AssignedDriverUserFk)
                    .Include(x=> x.TripDrivers)
                    .FirstOrDefaultAsync(e => e.Id == tripId);

                if (trip == null)
                {
                    throw new UserFriendlyException(L("NoTripToAssignDriver"));
                }
                return trip;
            }
        }

        /// <summary>
        /// Validates a driver and truck assignment to a trip.
        /// 
        /// This method checks that the driver is not already working on another trip and that the truck is not already rented during the trip duration.
        /// </summary>
        /// <param name="trip">The trip to assign the driver and truck to.</param>
        /// <param name="driverId">The id of the driver to assign.</param>
        /// <param name="truckId">The id of the truck to assign.</param>
        private async Task ValidateTripAndDriverAssignment(ShippingRequestTrip trip, long driverId, long truckId)
        {
            if (trip.Status == ShippingRequestTripStatus.InTransit && await CheckIfDriverWorkingOnAnotherTrip(driverId))
            {
                throw new UserFriendlyException(L("TheDriverAreadyWorkingOnAnotherTrip"));
            }
            if (await _shippingRequestManager.IsTruckBusyDuringTripDuration(truckId, trip))
            {
                throw new UserFriendlyException(L("TruckIsAlreadyRented"));
            }
            if (await _shippingRequestManager.IsDriverBusyDuringTripDuration(driverId, trip))
            {
                throw new UserFriendlyException(L("DriverIsAlreadyRented"));
            }
        }

        
        /// <summary>
        /// Assigns a driver and truck to a trip.
        /// 
        /// This method updates the trip's driver and truck with the given ids and sets the assigned time to the current time.
        /// It also sets the active trip driver to the given driver and truck.
        /// </summary>
        /// <param name="trip">The trip to assign the driver and truck to.</param>
        /// <param name="input">The input containing the driver and truck ids.</param>
        private void AssignDriverAndTruckToTrip(ShippingRequestTrip trip, AssignDriverAndTruckToShippmentByCarrierInput input)
        {
            trip.ContainerNumber = input.ContainerNumber;
            trip.SealNumber = input.SealNumber;
            trip.AssignedDriverUserId = input.AssignedDriverUserId;
            trip.AssignedTruckId = input.AssignedTruckId;
            trip.AssignedDriverTime = Clock.Now;

            // implement TripDriver 
            foreach (var tripDriver in trip.TripDrivers){
                tripDriver.IsActive = false;
            }        
            trip.TripDrivers.Add(new TripDriver(){
                DriverId = input.AssignedDriverUserId,
                ShippingRequestTripId = trip.Id,
                TruckId = input.AssignedTruckId
            });

        }

        /// <summary>
        /// Resets the driver status of a trip to its initial state.
        /// </summary>
        /// <param name="trip">The shipping request trip to reset the driver status of.</param>
        /// <remarks>
        /// The driver status is reset to <see cref="ShippingRequestTripDriverStatus.None"/>,
        /// the rejected reason is cleared and the rejected reason id is set to null.
        /// </remarks>
        private void ResetDriverStatusIfNeeded(ShippingRequestTrip trip)
        {
            if (trip.DriverStatus != ShippingRequestTripDriverStatus.None)
            {
                trip.DriverStatus = ShippingRequestTripDriverStatus.None;
                trip.RejectedReason = string.Empty;
                trip.RejectReasonId = default(int?);
            }
        }

        /// <summary>
        /// Handles notifications related to driver changes in a shipping request trip.
        /// </summary>
        /// <param name="trip">The shipping request trip for which the driver change notification is being handled.</param>
        /// <param name="oldAssignedDriverUserId">The ID of the previously assigned driver, if any.</param>
        /// <remarks>
        /// If an old driver is assigned, this method sends a notification to inform them they have been unassigned from the trip.
        /// </remarks>
        private async Task HandleDriverChangeNotifications(ShippingRequestTrip trip, long? oldAssignedDriverUserId)
        {

            if (oldAssignedDriverUserId.HasValue)
            {
                await _appNotifier.NotifyDriverWhenUnassignedTrip(
                    trip.Id,
                    trip.WaybillNumber.ToString(),
                    new UserIdentifier(AbpSession.TenantId, oldAssignedDriverUserId.Value)
                );
            }
        }

        /// <summary>
        /// Sets the update reason for a trip update, based on whether the driver and/or truck were changed.
        /// </summary>
        /// <param name="isDriverChanged">True if the driver was changed, false otherwise.</param>
        /// <param name="isTruckChanged">True if the truck was changed, false otherwise.</param>
        /// <remarks>
        /// The update reason is set to one of the following values, depending on the parameters:
        /// <list type="bullet">
        /// <item><see cref="RoutPointAction1"/> if only the driver was changed.</item>
        /// <item><see cref="RoutPointAction2"/> if only the truck was changed.</item>
        /// <item><see cref="RoutPointAction4"/> if both the driver and truck were changed.</item>
        /// </list>
        /// </remarks>
        private void SetUpdateReason(bool isDriverChanged, bool isTruckChanged)
        {
            string reason = isDriverChanged switch
            {
                true when isTruckChanged => nameof(RoutPointAction4),
                true => nameof(RoutPointAction1),
                false when isTruckChanged => nameof(RoutPointAction2),
                _ => null
            };

            if (reason != null)
            {
                _reasonProvider.Use(reason);
            }
        }

        /// <summary>
        /// Updates the driver status of a user.
        /// </summary>
        /// <param name="driverId">The ID of the user to update the driver status for.</param>
        /// <param name="status">The new driver status for the user.</param>
        private async Task UpdateDriverStatus(long driverId, UserDriverStatus status)
        {
            await UserManager.UpdateUserDriverStatus(driverId, status);
        }

        /// <summary>
        /// Checks if the carrier should be notified of a trip update.
        /// A notification is sent if the truck assigned to the trip has changed and the trip is related to a carrier.
        /// </summary>
        /// <param name="oldAssignedTruckId">The ID of the truck previously assigned to the trip, if any.</param>
        /// <param name="oldAssignedDriverUserId">The ID of the driver previously assigned to the trip, if any.</param>
        /// <param name="trip">The shipping request trip to check for notification.</param>
        /// <returns>True if the carrier should be notified, false otherwise.</returns>
        private bool ShouldNotifyCarrier(long? oldAssignedTruckId, long? oldAssignedDriverUserId, ShippingRequestTrip trip)
        {
            return oldAssignedTruckId != trip.AssignedTruckId && trip.ShippingRequestFk.CarrierTenantId != null;
        }

        /// <summary>
        /// Notifies the carrier of a trip update.
        /// </summary>
        /// <param name="trip">The shipping request trip that has been updated.</param>
        /// <remarks>
        /// This method retrieves the assigned driver and prepares a notification input object
        /// with relevant trip details. It then sends a notification to the carrier
        /// associated with the trip using the application notifier.
        /// </remarks>

        private async Task NotifyCarrierOfTripUpdate(ShippingRequestTrip trip)
        {
            var driver = await _userManager.GetUserByIdAsync(trip.AssignedDriverUserId.Value);
            var notifyTripInput = new NotifyTripUpdatedInput
            {
                CarrierTenantId = trip.ShippingRequestFk.CarrierTenantId.Value,
                TripId = trip.Id,
                WaybillNumber = trip.WaybillNumber.ToString(),
                DriverIdentifier = new UserIdentifier(driver.TenantId, trip.AssignedDriverUserId.Value)
            };

            await _appNotifier.NotifyCarrierWhenTripUpdated(notifyTripInput);
        }

        /// <summary>
        /// Applies a penalty to the carrier for not assigning a truck and driver to the trip.
        /// </summary>
        /// <param name="trip">The shipping request trip that had no truck and driver assigned.</param>
        /// <remarks>
        /// This method calls the penalty manager to apply a penalty to the carrier for not assigning a truck and driver to the trip.
        /// The penalty is applied based on the carrier's tenant ID, the shipper's tenant ID, the start date of the trip and the trip ID.
        /// </remarks>
        private async Task ApplyNotAssigningPenalty(ShippingRequestTrip trip)
        {
            await _penaltyManager.ApplyNotAssigningTruckAndDriverPenalty(
                trip.ShippingRequestFk.CarrierTenantId.Value,
                trip.ShippingRequestFk.TenantId,
                trip.StartTripDate,
                trip.Id
            );
        }

        /// <summary>
        /// Notifies the driver of a trip assignment.
        /// </summary>
        /// <param name="trip">The shipping request trip that has been assigned to the driver.</param>
        /// <remarks>
        /// This method calls the application notifier to send a notification to the driver
        /// that the trip has been assigned to them.
        /// </remarks>
        private async Task NotifyDriverOfAssignment(ShippingRequestTrip trip)
        {
            await _appNotifier.NotifyDriverWhenAssignTrip(
                trip.Id,
                new UserIdentifier(trip.ShippingRequestFk.CarrierTenantId, trip.AssignedDriverUserId.Value)
            );
        }

        /// <summary>
        /// Queues an update of the specified trip, driver and truck to the Bayan integration.
        /// </summary>
        /// <param name="tripId">The ID of the trip to update.</param>
        /// <param name="driverId">The ID of the driver to update.</param>
        /// <param name="truckId">The ID of the truck to update.</param>
        /// <remarks>
        /// This method is called after a trip has been assigned to a driver and truck.
        /// It sends a message to the Bayan integration to update the trip, driver and truck with the latest information.
        /// </remarks>
        private async Task QueueBayanIntegrationUpdate(long tripId, long driverId, long truckId)
        {
            await _bayanIntegrationManagerV3.QueueUpdateVehicleOrDriver(
                new UpdateVehicleOrDriverJobArgs { TripId = (int)tripId, DriverId = driverId, TruckId = truckId }
            );
        }
        #endregion
        // public async Task AssignDriverAndTruckToShippmentByCarrier(AssignDriverAndTruckToShippmentByCarrierInput input)
        // {
        //     ShippingRequestTrip trip;
        //     bool isDriverChanged = false;

        //     using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MayHaveTenant, AbpDataFilters.MustHaveTenant))
        //     {
        //         trip = await _shippingRequestTripRepository.GetAll().Include(e => e.ShippingRequestFk)
        //             .Include(d => d.AssignedDriverUserFk)
        //             .Where(e => e.Id == input.Id)
        //             .FirstOrDefaultAsync();

        //         if (trip == null) throw new UserFriendlyException(L("NoTripToAssignDriver"));
        //     }
        //     trip.ContainerNumber = input.ContainerNumber;
        //     trip.SealNumber = input.SealNumber;
        //     var carrierTenantId = trip.ShippingRequestFk.CarrierTenantId;

        //     using (UnitOfWorkManager.Current.SetTenantId(carrierTenantId))
        //     {




        //         if (trip.Status == ShippingRequestTripStatus.InTransit && await CheckIfDriverWorkingOnAnotherTrip(input.AssignedDriverUserId))
        //             throw new UserFriendlyException(L("TheDriverAreadyWorkingOnAnotherTrip"));
        //         //check if truck or driver rented
        //         if (await _shippingRequestManager.IsTruckBusyDuringTripDuration(input.AssignedTruckId, trip))
        //         {
        //             throw new UserFriendlyException(L("TruckIsAlreadyRented"));
        //         }
        //         if (await _shippingRequestManager.IsDriverBusyDuringTripDuration(input.AssignedDriverUserId, trip))
        //         {
        //             throw new UserFriendlyException(L("DriverIsAlreadyRented"));
        //         }

        //         long? oldAssignedDriverUserId = trip.AssignedDriverUserId;
        //         long? oldAssignedTruckId = trip.AssignedTruckId;
        //         trip.AssignedDriverUserId = input.AssignedDriverUserId;
        //         trip.AssignedTruckId = input.AssignedTruckId;
        //         bool isTruckChanged = oldAssignedTruckId != input.AssignedTruckId;

        //         //reset driver status when change 
        //         if (trip.DriverStatus != ShippingRequestTripDriverStatus.None)
        //         {
        //             trip.DriverStatus = ShippingRequestTripDriverStatus.None;
        //             trip.RejectedReason = string.Empty;
        //             trip.RejectReasonId = default(int?);
        //         }

        //         if (oldAssignedDriverUserId != trip.AssignedDriverUserId)
        //         {
        //             trip.AssignedDriverTime = Clock.Now;
        //             // Send Notification To New Driver
        //             if (oldAssignedDriverUserId.HasValue)
        //             {
        //                 await _appNotifier.NotifyDriverWhenUnassignedTrip
        //                 (
        //                     trip.Id,
        //                     trip.WaybillNumber.ToString(),
        //                     new UserIdentifier(AbpSession.TenantId, oldAssignedDriverUserId.Value)
        //                 );

        //                 await UserManager.UpdateUserDriverStatus(oldAssignedDriverUserId.Value, UserDriverStatus.Available);
        //                 isDriverChanged = true;
        //             }
        //         }

        //         #region SetUpdateReason

        //         string reason;

        //         switch (isDriverChanged)
        //         {
        //             case true when isTruckChanged:
        //                 reason = nameof(RoutPointAction4);
        //                 break;
        //             case true:
        //                 reason = nameof(RoutPointAction1);
        //                 break;
        //             case false when isTruckChanged:
        //                 reason = nameof(RoutPointAction2);
        //                 break;
        //             default: return;
        //         }

        //         _reasonProvider.Use(reason);

        //         #endregion

        //         await UserManager.UpdateUserDriverStatus(input.AssignedDriverUserId, UserDriverStatus.NotAvailable);

        //         if (oldAssignedTruckId != trip.AssignedTruckId && trip.ShippingRequestFk.CarrierTenantId != null)
        //         {
        //             var driver = await _userManager.GetUserByIdAsync(trip.AssignedDriverUserId.Value);
        //             var notifyTripInput = new NotifyTripUpdatedInput()
        //             {
        //                 CarrierTenantId = trip.ShippingRequestFk.CarrierTenantId.Value,
        //                 TripId = trip.Id,
        //                 WaybillNumber = trip.WaybillNumber.ToString(),
        //                 DriverIdentifier = new UserIdentifier(driver.TenantId, trip.AssignedDriverUserId.Value)
        //             };

        //             await _appNotifier.NotifyCarrierWhenTripUpdated(notifyTripInput);
        //         }

        //         if (!oldAssignedTruckId.HasValue && !oldAssignedDriverUserId.HasValue)
        //             await _penaltyManager.ApplyNotAssigningTruckAndDriverPenalty
        //             (
        //                 trip.ShippingRequestFk.CarrierTenantId.Value,
        //                 trip.ShippingRequestFk.TenantId,
        //                 trip.StartTripDate,
        //                 trip.Id
        //             );

        //         // Send Notification To New Driver
        //         await _appNotifier.NotifyDriverWhenAssignTrip
        //         (
        //             trip.Id,
        //             new UserIdentifier(trip.ShippingRequestFk.CarrierTenantId, trip.AssignedDriverUserId.Value)
        //         );



        //         await CurrentUnitOfWork.SaveChangesAsync();

        //     }

        //     //Bayan integration job
        //     await _bayanIntegrationManagerV3.QueueUpdateVehicleOrDriver
        //     (
        //         new UpdateVehicleOrDriverJobArgs { TripId = input.Id, DriverId = input.AssignedDriverUserId, TruckId = input.AssignedTruckId }
        //     );
        // }


        /// <summary>
        /// Replaces the driver or truck assigned to a shipping request trip.
        /// </summary>
        /// <param name="newDriverId">The ID of the new driver to be assigned to the trip, if any.</param>
        /// <param name="newTruckId">The ID of the new truck to be assigned to the trip, if any.</param>
        /// <param name="tripId">The ID of the trip for which the driver or truck is being replaced.</param>
        /// <remarks>
        /// If a new driver is provided, it updates the trip's assigned driver and deactivates previous trip drivers.
        /// If a new truck is provided without a new driver, it updates the trip's assigned truck.
        /// Sends a notification to the new driver if a driver change occurs.
        /// </remarks>
        public async Task ReplaceDriverOrTruckAsync(long? newDriverId, long? newTruckId, int tripId)
        {

            var trip = await _shippingRequestTripRepository
           .GetAllIncluding(x => x.TripDrivers)
           .FirstAsync(x => x.Id == tripId);



            if (newDriverId != null)
            {
                var oldDriverId = trip.AssignedDriverUserId;

                if (oldDriverId != newDriverId)
                {
                    trip.ReplacesDriverId = oldDriverId;
                    trip.AssignedDriverUserId = newDriverId;

                    trip.ReplacedDriverWorkingHour = trip.DriverWorkingHour;
                    trip.ReplacedDriverDistance = trip.Distance;

                    //trip.DriverWorkingHour = 0;
                    //trip.Distance = 0;

                    // implement TripDriver 
                    foreach (var tripDriver in trip.TripDrivers)
                    {
                        tripDriver.IsActive = false;
                    }
                    trip.TripDrivers.Add(new TripDriver()
                    {
                        DriverId = newDriverId.Value,
                        ShippingRequestTripId = trip.Id,
                        TruckId = newTruckId != null ? newTruckId.Value : trip.AssignedTruckId,
                        IsActive = true

                    });

                    // Send Notification To New Driver
                    await _appNotifier.NotifyDriverWhenAssignTrip
                    (
                        trip.Id,
                        new UserIdentifier(trip.CarrierTenantId, trip.AssignedDriverUserId.Value)
                    );

                }
            }


            if (newTruckId != null && newDriverId == null)
            {
                var oldTruckId = trip.AssignedTruckId;
                if (oldTruckId != newTruckId)
                {
                    trip.ReplacedTruckId = oldTruckId;
                    trip.AssignedTruckId = newTruckId;

                    var tripDriver = trip.TripDrivers.FirstOrDefault(x => x.IsActive);
                    if (tripDriver == null) throw new UserFriendlyException(L("NoTripDriver"));
                    tripDriver.TruckId = newTruckId;

                }
            }



        }

        
        
        /// <summary>
        /// Updates the commission percentage for a trip driver.
        /// </summary>
        /// <param name="dto">A TripDriverForViewDto object containing the ID of the trip driver and the new commission percentage.</param>
        /// <exception cref="UserFriendlyException">If the trip driver does not exist.</exception>
        public async Task UpdateTripDriver(TripDriverForViewDto dto)
        {
            var tripDriver = await _tripDriverRepository
            .FirstOrDefaultAsync(x=> x.Id == dto.Id);
            
            if (tripDriver == null ) throw new UserFriendlyException(L("NoTripDriver"));
            
            tripDriver.Commission = dto.Commission;
    
        }

        /// <summary>
        /// Changes the container return date and/or sets if the container is returned of a trip.
        /// </summary>
        /// <param name="tripId">The ID of the trip to change the container return date and/or set if the container is returned.</param>
        /// <param name="containerReturnDate">The new container return date, if any.</param>
        /// <param name="isContainerReturned">A value indicating if the container is returned, if any.</param>
        public async Task ChangeContainerReturnDate(int tripId , DateTime? containerReturnDate , bool? isContainerReturned )
        {
            await DisableTenancyFilterIfTachyonDealerOrHost();
            var trip = await _shippingRequestTripRepository.GetAsync(tripId);
            trip.ContainerReturnDate = containerReturnDate;
            trip.IsContainerReturned = isContainerReturned;
        }

        [AbpAuthorize(AppPermissions.Pages_ShippingRequestTrips_Create)]
        private async Task Create(CreateOrEditShippingRequestTripDto input, [CanBeNull] ShippingRequest request)
        {
           
            ShippingRequestTrip trip = ObjectMapper.Map<ShippingRequestTrip>(input);

           

            if (request == null) // direct shipment 
            {
                trip.CarrierTenantId = AbpSession.TenantId;
                trip.ShipperTenantId = AbpSession.TenantId;

                await AddOrRemoveDestinationCities(input.ShippingRequestDestinationCities, trip);

            }

            // if dedicated or direct trip 
            if (request?.ShippingRequestFlag == ShippingRequestFlag.Dedicated || trip.ShippingRequestId == null)
            {
                trip.AssignedDriverUserId = input.DriverUserId;
                trip.AssignedTruckId = input.TruckId;


            }
            //AssignWorkFlowVersionToRoutPoints(trip);
            var shippingType = request != null ? request.ShippingTypeId : trip.ShippingTypeId;
            var roundTrip = request!= null ? request.RoundTripType :trip.RoundTripType;
            

            _shippingRequestTripManager.AssignWorkFlowVersionToRoutPoints(trip.NeedsDeliveryNote, trip.ShippingRequestTripFlag,shippingType,roundTrip, trip.RoutPoints.ToArray());
            //insert trip 
            var shippingRequestTripId = await _shippingRequestTripRepository.InsertAndGetIdAsync(trip);

            // port movement set appointment and clearance
            await SetNeedsAppointmentAndClearance(input, trip);

            //appointment data
            if (true)
            {
                await BindAppointmentAndClearance(input, request, trip);
            }


            //accept trip if trip is home delivery
            if (trip.ShippingRequestTripFlag == ShippingRequestTripFlag.HomeDelivery)
                await _shippingRequestTripManager.DriverAcceptTrip(trip);
            // add document file
            var docFileDto = input.CreateOrEditDocumentFileDto;

            if (trip.HasAttachment)
            {
                docFileDto.Name = input.CreateOrEditDocumentFileDto.DocumentTypeDto.DisplayName + "_" +
                                  shippingRequestTripId;
                docFileDto.ShippingRequestTripId = shippingRequestTripId;
                await _documentFilesAppService.CreateOrEdit(docFileDto);
            }

            // store saas invoicing feature on, off
            trip.SaasInvoicingActivation = await _shippingRequestTripManager.GetSaasInvoicingActivation(trip.ShippingRequestId != null ? request.TenantId : trip.ShipperTenantId.Value);

            //Notify Carrier with trip details
            if (request?.ShippingRequestFlag == ShippingRequestFlag.Normal)
                await _shippingRequestTripManager.NotifyCarrierWithTripDetails(trip, request.CarrierTenantId, true, true, true);

            //notify TMS if tenant reach max number of waybills
            var tenant = request == null ? trip.ShipperTenantId.Value : trip.ShippingRequestFk.TenantId;
            var maxNumberOfWaybills = await _shippingRequestTripManager.GetMaxNumberOfWaybills(tenant);
            if(maxNumberOfWaybills != null)
            {
                var createdTrips =await _shippingRequestTripManager.GetWaybillsNo(tenant);
                if(maxNumberOfWaybills.Value == createdTrips)
                {
                    await _appNotifier.NotifyTMSWithMaxWaybillsExceeds(tenant);
                }
            }

            // publish trip created webHook
            {
                input.Id = shippingRequestTripId;
                await _webhookPublisher.PublishNewTripCreatedWebhook(input);
            }
            
        }

        /// <summary>
        /// Adds SAB order id to the trip
        /// </summary>
        /// <param name="tripId">The id of the trip</param>
        /// <param name="sabOrderId">The SAB order id</param>
        /// <returns>A Task that represents the asynchronous operation</returns>
        public async Task AddSabOrderId(int tripId , string sabOrderId)
        {
            var trip = await _shippingRequestTripRepository.GetAll()
                .FirstOrDefaultAsync(x=> x.Id == tripId);
            trip.SabOrderId = sabOrderId;
            
        }
        /// <summary>
        /// Adds remarks to a trip.
        /// </summary>
        /// <param name="input">A RemarksInputDto object containing the remarks information.</param>
        /// <exception cref="UserFriendlyException">If the trip is not found.</exception>
        public async Task AddRemarks(RemarksInputDto input)
        {
            DisableTenancyFilters();
            var tenantId = AbpSession.TenantId;
            var shippginRequstTrip = await _shippingRequestTripRepository.GetAll()
                .WhereIf(tenantId.HasValue && await IsEnabledAsync(AppFeatures.Carrier), x => x.ShippingRequestFk.CarrierTenantId == tenantId)
                .WhereIf(tenantId.HasValue && await IsEnabledAsync(AppFeatures.Shipper), x => x.ShippingRequestFk.TenantId == tenantId)
                .WhereIf(tenantId.HasValue && await IsEnabledAsync(AppFeatures.TachyonDealer), x => x.ShippingRequestFk.IsTachyonDeal)
                .Include(x => x.ShippingRequestFk) // include shipping request because we need it in trip update event when send Notfication
                .FirstOrDefaultAsync(x => x.Id == input.Id);

            if (shippginRequstTrip == null) throw new UserFriendlyException(L("YouCannotAddRemarksForThisTrip"));

            shippginRequstTrip.RoundTrip = input.RoundTrip;
            shippginRequstTrip.CanBePrinted = input.CanBePrinted;
            shippginRequstTrip.ContainerNumber = input.ContainerNumber;
        }
        
        /// <summary>
        /// Retrieves the remarks of a shipping request trip by the given trip ID.
        /// </summary>
        /// <param name="tripId">The ID of the trip for which to retrieve remarks.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a RemarksInputDto object
        /// with the trip remarks information such as round trip status, container number, and printability.</returns>
        public async Task<RemarksInputDto> GetRemarks(int tripId)
        {
            return await _shippingRequestTripRepository.GetAll()
                  .Select(y => new RemarksInputDto()
                  {
                      Id = y.Id,
                      RoundTrip = y.RoundTrip,
                      ContainerNumber = y.ContainerNumber,
                      CanBePrinted = y.CanBePrinted
                  }).FirstOrDefaultAsync(x => x.Id == tripId);
        }

        private async Task<int> GetTripNotesCount(long tripId)
        {
            DisableTenancyFilters();
            return await _shippingRequestAndTripNoteRepository
                .GetAll()
                .Include(r => r.TripFK)
                .ThenInclude(r => r.ShippingRequestFk)
                 //.WhereIf(AbpSession.TenantId.HasValue && await IsEnabledAsync(AppFeatures.Shipper), x => x.TenantId == AbpSession.TenantId)
                 //.WhereIf(AbpSession.TenantId.HasValue && await IsEnabledAsync(AppFeatures.Carrier), x => x.TenantId == AbpSession.TenantId)
                 //.WhereIf(AbpSession.TenantId.HasValue && await IsEnabledAsync(AppFeatures.TachyonDealer), x => x.TenantId == AbpSession.TenantId)
                 .WhereIf(AbpSession.TenantId.HasValue && await IsEnabledAsync(AppFeatures.Shipper),
                    x => x.TenantId == AbpSession.TenantId ||
                    (x.TenantId != AbpSession.TenantId && (x.Visibility == VisibilityNotes.ShipperOnly || x.Visibility == VisibilityNotes.Internal)))
                .WhereIf(AbpSession.TenantId.HasValue && IsEnabled(AppFeatures.Carrier),
                    x => ((x.TripFK.ShippingRequestFk.CarrierTenantId == AbpSession.TenantId || x.TripFK.ShippingRequestFk.CarrierTenantIdForDirectRequest == AbpSession.TenantId) &&
                    (x.Visibility == VisibilityNotes.Internal ||
                    x.Visibility == VisibilityNotes.CarrierOnly ||
                    x.Visibility == VisibilityNotes.TMSAndCarrier)) ||
                    (x.TenantId == AbpSession.TenantId)
                    )
              .WhereIf(AbpSession.TenantId.HasValue && IsEnabled(AppFeatures.TachyonDealer),
                    x => (x.TripFK.ShippingRequestFk.IsTachyonDeal &&
                   (x.Visibility == VisibilityNotes.TMSOnly
                   || x.Visibility == VisibilityNotes.TMSAndCarrier)) ||
                   (x.TenantId == AbpSession.TenantId)
                   )
                .CountAsync(x => x.TripId == tripId);
        }

        [AbpAuthorize(AppPermissions.Pages_ShippingRequestTrips_Edit)]
        private async Task Update(CreateOrEditShippingRequestTripDto input, ShippingRequest request)
        {
            var trip = await GetTrip((int)input.Id, input.ShippingRequestId);
            TripCanEditOrDelete(trip);
            if (trip.ShippingRequestFk is { ShippingRequestFlag: ShippingRequestFlag.Dedicated } || trip.ShippingRequestFk == null)
            {
                trip.AssignedDriverUserId = input.DriverUserId;
                trip.AssignedTruckId = input.TruckId;
            }

            //await ValidateGoodsCategory(input.RoutPoints, request.GoodCategoryId);
            await RemoveDeletedTripPoints(input, trip);
            await RemoveDeletedTripVases(input, trip);
            await AddOrRemoveDestinationCities(input.ShippingRequestDestinationCities, trip);

            await ValidateExpectedDeliveryTime(input.ExpectedDeliveryTime, trip);

            if (input.HasAttachment)
            {
                if (input.CreateOrEditDocumentFileDto.UpdateDocumentFileInput != null)
                {
                    await _documentFilesAppService.CreateOrEdit(input.CreateOrEditDocumentFileDto);
                }
            }
            else
            {
                //remove file if exists
                var documentFile =
                    await _documentFileRepository.FirstOrDefaultAsync(x => x.ShippingRequestTripId == trip.Id);
                if (documentFile != null)
                {
                    await _documentFilesManager.DeleteDocumentFile(documentFile);
                }
            }

            ObjectMapper.Map(input, trip);
            await SetNeedsAppointmentAndClearance(input, trip);
            if (true )
            {
                await BindAppointmentAndClearance(input, request, trip);
            }


            if (trip.ShippingRequestTripFlag == ShippingRequestTripFlag.HomeDelivery)
            {
                // Note: if the trip is normal and changed to Home delivery 
                // the driver status must updated
                trip.DriverStatus = ShippingRequestTripDriverStatus.Accepted;

                var pointHasAbilityToChangeWorkflow =
                    trip.RoutPoints?.Where(x => x.Status == RoutePointStatus.StandBy).ToList();

                    var shippingType = request != null ? request.ShippingTypeId : trip.ShippingTypeId;
                var roundTrip = request != null ? request.RoundTripType : trip.RoundTripType;

                if (pointHasAbilityToChangeWorkflow != null && pointHasAbilityToChangeWorkflow.Count > 0)
                {
                         
                    _shippingRequestTripManager.AssignWorkFlowVersionToRoutPoints(trip.NeedsDeliveryNote, trip.ShippingRequestTripFlag, shippingType,roundTrip, pointHasAbilityToChangeWorkflow.ToArray());
                }
            }

            if (!trip.BayanId.IsNullOrEmpty())
            {

                var points = await _routPointRepository.GetAll()
                    .Where(x => x.PickingType == PickingType.Dropoff)
                    .ToListAsync();
                foreach (var point in points)
                {
                    if (!point.BayanId.IsNullOrEmpty())
                    {
                        await _bayanIntegrationManagerV3.QueueUpdateWaybill(point.Id);
                    }
                }

            }
        }

        private async Task BindAppointmentAndClearance(CreateOrEditShippingRequestTripDto input, ShippingRequest request, ShippingRequestTrip trip)
        {

            var points = trip.RoutPoints.Where(x => x.NeedsAppointment && input.RoutPoints.First(y => y.PointOrder == x.PointOrder).AppointmentDataDto != null);
            foreach (var point in points)
            {
                if (request != null && (request.Status != ShippingRequestStatus.PostPrice && request.Status != ShippingRequestStatus.Completed))
                {
                    throw new UserFriendlyException(L("RequestMustBeConfirmedToAddAppointmentAndClearanceVases"));
                }
                var inputPoint = input.RoutPoints.First(x => x.PointOrder == point.PointOrder);
                if (inputPoint.AppointmentDataDto == null) inputPoint.AppointmentDataDto = new TripAppointmentDataDto();
                inputPoint.AppointmentDataDto.ShippingRequestId = request?.Id;

                await SetAppointmentData(inputPoint.AppointmentDataDto, point);
            }
            var clearancePoints = trip.RoutPoints.Where(x => x.NeedsClearance && input.RoutPoints.First(y => y.PointOrder == x.PointOrder).TripClearancePricesDto != null);
            foreach(var point in clearancePoints)
            {
                if (request!= null && (request.Status != ShippingRequestStatus.PostPrice && request.Status != ShippingRequestStatus.Completed))
                {
                    throw new UserFriendlyException(L("RequestMustBeConfirmedToAddAppointmentAndClearanceVases"));
                }
                var inputPoint = input.RoutPoints.First(x => x.PointOrder == point.PointOrder);
                if (inputPoint.TripClearancePricesDto == null) inputPoint.TripClearancePricesDto = new TripClearancePricesDto();
                inputPoint.TripClearancePricesDto.ShippingRequestId = request?.Id;
                await SetClearanceData(inputPoint.TripClearancePricesDto, point);
            }
            
        }

        private async Task SetNeedsAppointmentAndClearance(CreateOrEditShippingRequestTripDto input, ShippingRequestTrip trip)
        {
            if (await IsTachyonDealer())
            {
                foreach (var point in input.RoutPoints)
                {
                    if (point.DropNeedsAppointment)
                        trip.RoutPoints.FirstOrDefault(x => x.PointOrder == point.PointOrder).NeedsAppointment = true;
                    if (point.DropNeedsClearance)
                        trip.RoutPoints.FirstOrDefault(x => x.PointOrder == point.PointOrder).NeedsClearance = true;
                }
            }
        }

        private async Task ValidateExpectedDeliveryTime(DateTime? expectedDeliveryTime, ShippingRequestTrip trip)
        {
            if (!expectedDeliveryTime.Equals(trip.ExpectedDeliveryTime))
            {
                var isPickupPointCompleted = await _routPointRepository.GetAll()
                    .AnyAsync(x => x.PickingType == PickingType.Pickup &&
                                   x.ShippingRequestTripId == trip.Id && x.Status == RoutePointStatus.FinishLoading);
                if (isPickupPointCompleted)
                    throw new UserFriendlyException(L("YouCanNotEditExpectedDeliveryTime"));
            }
        }


        /// <summary>
        /// Deletes a shipping request trip by given trip id.
        /// The user must have <see cref="AppPermissions.Pages_ShippingRequestTrips_Delete"/> permission to be able to delete a trip.
        /// </summary>
        /// <param name="input">The trip id to delete.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        [AbpAuthorize(AppPermissions.Pages_ShippingRequestTrips_Delete)]
        public async Task Delete(EntityDto input)
        {
            await DisableTenancyFilterIfTachyonDealerOrHost();
            var trip = await _shippingRequestTripRepository.FirstOrDefaultAsync(
                x => x.Id == input.Id &&
                     x.Status == ShippingRequestTripStatus.New
            );


            if (trip.ShippingRequestId != null)
            {
                var request = await GetShippingRequestByPermission(trip.ShippingRequestId.Value);
                request.TotalsTripsAddByShippier -= 1; // todo remove this
            }

            TripCanEditOrDelete(trip);

            await _shippingRequestTripRepository.DeleteAsync(trip);

        }



        /// <summary>
        /// Cancels a shipping request trip based on the provided input.
        /// The trip must be in a "New" or "InTransit" status and the associated shipping request must be in a "PrePrice" status.
        /// The cancellation is only allowed if the trip's cancel status is "None" and certain tenant-specific conditions are met.
        /// </summary>
        /// <param name="input">A CancelTripInput object containing the trip cancellation details.</param>
        /// <exception cref="UserFriendlyException">Thrown if the trip is not found.</exception>
        [AbpAuthorize(AppPermissions.Pages_ShippingRequestTrips_Cancel)]
        public async Task CancelTrip(CancelTripInput input)
        {
            DisableTenancyFilters();
            var trip = await _shippingRequestTripRepository.GetAll().Include(x => x.ShippingRequestFk)
                     //add preprice  & !payInadvance condition now, then in future post price cancelation will be added
                     .Where(x => x.ShippingRequestFk.Status == ShippingRequestStatus.PrePrice)
                     .Where(x => x.Status == ShippingRequestTripStatus.New ||
                     x.Status == ShippingRequestTripStatus.InTransit)
                     .WhereIf(IsEnabled(AppFeatures.Carrier), x => x.ShippingRequestFk.CarrierTenantId == AbpSession.TenantId && x.CancelStatus == ShippingRequestTripCancelStatus.None)
                     .WhereIf(IsEnabled(AppFeatures.Shipper), x => x.ShippingRequestFk.TenantId == AbpSession.TenantId && x.CancelStatus == ShippingRequestTripCancelStatus.None)
                .FirstOrDefaultAsync(x => x.Id == input.id);
            if (trip != null)
            {
                var carrierIdent = default(UserIdentifier);
                //if the request is in post price
                if (trip.ShippingRequestFk.CarrierTenantId != null)
                {
                    carrierIdent = await GetAdminTenant((int)trip.ShippingRequestFk.CarrierTenantId);
                }
                //check Invoice type, skip if pay in advance
                await CancelTripAsync(input, trip, carrierIdent);
            }
            else
            {
                throw new UserFriendlyException(L("TripNotFound"));
            }
        }

        /// <summary>
        /// Updates the invoice flag for a shipping request trip by given trip id.
        /// The invoice flag determines whether the trip can be invoiced separately or not.
        /// </summary>
        /// <param name="shippingRequesTripId">The trip id to update.</param>
        /// <param name="invoiceFlag">The invoice flag to set.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task UpdateTripInvoiceFlag(int shippingRequesTripId, string invoiceFlag)
        {
            DisableTenancyFilters();
            var trip = await _shippingRequestTripRepository.GetAsync(shippingRequesTripId);
            trip.SplitInvoiceFlag = invoiceFlag?.Trim();

        }


        /// <summary>
        /// Initiates the creation of a trip in the Bayan integration system.
        /// </summary>
        /// <param name="tripId">The ID of the trip to be created in the Bayan integration.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task CreateBayanIntegrationTrip(int tripId)
        {
            await _bayanIntegrationManagerV3.CreateTrip(tripId);
        }

        /// <summary>
        /// Prints the details of a trip from the Bayan integration system.
        /// </summary>
        /// <param name="tripId">The ID of the trip to print.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a byte array of the printed trip details.</returns>
        public async Task<byte[]> PrintBayanIntegrationTrip(int tripId)
        {
            return await _bayanIntegrationManagerV3.PrintTrip(tripId);

        }
        
        
       


        #region Heleper

        /// <summary>
        /// Check can delete or edit trip if trip on stand by status
        /// </summary>
        /// <param name="trip"></param>
        private void TripCanEditOrDelete(ShippingRequestTrip trip)
        {
            // When Edit Or Delete, Allow Home delivery to edit trip even if it is intransit
            if (trip.ShippingRequestFk != null && trip.ShippingRequestTripFlag == ShippingRequestTripFlag.Normal && trip.ShippingRequestFk.ShippingRequestFlag == ShippingRequestFlag.Normal && trip.Status != ShippingRequestTripStatus.New)
            {
                throw new UserFriendlyException(L("CanNotEditOrDeleteTrip"));
            }
            else if (trip.ShippingRequestFk == null && trip.Status != ShippingRequestTripStatus.New)
            {
                throw new UserFriendlyException(L("CanNotEditOrDeleteTrip"));
            }
        }

        private async Task<ShippingRequestTrip> GetTrip(int tripid, long? requestId = null)
        {
            using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MustHaveTenant, AbpDataFilters.MayHaveTenant))
            {
                var trip = await _shippingRequestTripRepository
                    .GetAll()
                    .Include(x => x.ShippingRequestFk)
                    .Include(x => x.OriginFacilityFk)
                    .Include(x => x.DestinationFacilityFk)
                    .Include(x => x.AssignedTruckFk)
                    .Include(x => x.AssignedDriverUserFk)
                    .Include(x => x.ShippingRequestTripRejectReason)
                    .Include(x => x.RoutPoints)
                    .ThenInclude(r => r.FacilityFk)
                    .Include(x => x.RoutPoints)
                    .ThenInclude(r => r.GoodsDetails)
                    .ThenInclude(c => c.GoodCategoryFk)
                    .ThenInclude(r => r.Translations)
                    .Include(x => x.RoutPoints)
                    .ThenInclude(r => r.GoodsDetails)
                    .ThenInclude(c => c.UnitOfMeasureFk)
                    .ThenInclude(x => x.Translations)
                .Include(x => x.RoutPoints)
                    .ThenInclude(c => c.ReceiverFk)
                    .Include(x => x.ShippingRequestTripVases)
                    .ThenInclude(v => v.ShippingRequestVasFk)
                    .ThenInclude(v => v.VasFk)
                    .Include(x=>x.ShippingRequestTripVases)
                    .ThenInclude(x=>x.VasFk)
                    .Include(x => x.ActorCarrierPrice)
                    .Include(x => x.ActorShipperPrice)
                    .Include(x=>x.ShippingRequestDestinationCities)
                    .ThenInclude(x=>x.CityFk)
                     .Include(x => x.OriginCityFk)
                    .WhereIf(requestId.HasValue, x => x.ShippingRequestId == requestId)
                    .FirstOrDefaultAsync(x => x.Id == tripid);
                trip.RoutPoints = trip.RoutPoints.OrderBy(x => x.PickingType).ToList();
                if (trip == null) throw new UserFriendlyException(L("ShippingRequestTripIsNotFound"));
                return trip;
            }
        }

        /// <summary>
        /// Return Request when the user loging as shipper or host or carrier 
        /// </summary>
        /// <param name="shippingRequestId"></param>
        /// <returns></returns>
        private async Task<ShippingRequest> GetShippingRequestByPermission(long shippingRequestId)
        {

            var request = await _shippingRequestRepository.GetAll()
                .Include(x => x.ShippingRequestVases)
                .WhereIf(AbpSession.TenantId != null && await IsEnabledAsync(AppFeatures.Carrier), x => x.CarrierTenantId == AbpSession.TenantId)
                .WhereIf(AbpSession.TenantId != null && await IsEnabledAsync(AppFeatures.Shipper), x => x.TenantId == AbpSession.TenantId)
                .WhereIf(AbpSession.TenantId != null && await IsEnabledAsync(AppFeatures.TachyonDealer), x => true)
                .WhereIf(AbpSession.TenantId == null, x => true)
                .FirstOrDefaultAsync(x => x.Id == shippingRequestId);
            if (request == null)
            {
                throw new UserFriendlyException(L("ShippingRequestIsNotFound"));
            }

            return request;
        }


        /// <summary>
        /// Generic  auto mapper fro trip when view or edit 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        private async Task<T> GetShippingRequestTripForMapper<T>(ShippingRequestTrip trip)
        {
            var hasCarrierClients = await IsEnabledAsync(AppFeatures.CarrierClients); // that's mean he is broker
            var hasSaasShipments = await IsEnabledAsync(AppFeatures.CarrierAsASaas); // that's mean he is broker

            var userHasAccess = await _shippingRequestRepository.GetAll()
                .Where(x => x.Id == trip.ShippingRequestId)
                .WhereIf(AbpSession.TenantId.HasValue && await IsEnabledAsync(AppFeatures.Shipper) && !hasCarrierClients, x => x.TenantId == AbpSession.TenantId)
                .WhereIf(AbpSession.TenantId.HasValue && await IsEnabledAsync(AppFeatures.Carrier) && !hasCarrierClients, x => x.CarrierTenantId == AbpSession.TenantId)
                .WhereIf(AbpSession.TenantId.HasValue && hasCarrierClients, x => x.CarrierTenantId == AbpSession.TenantId || x.TenantId == AbpSession.TenantId)
                .AnyAsync();


            if (trip.CarrierTenantId == AbpSession.TenantId && trip.ShipperTenantId == AbpSession.TenantId)
            {

            }
            else if (!userHasAccess)
            {
                throw new UserFriendlyException(L("YouDoNotHaveAccess"));
            }

            return ObjectMapper.Map<T>(trip);
        }

        private async Task<UserIdentifier> GetAdminTenant(int tenantId)
        {
            return new UserIdentifier(tenantId, (await _userManager.GetAdminByTenantIdAsync(tenantId)).Id);
        }

        private async Task ValidateGoodsCategory(IEnumerable<CreateOrEditRoutPointDto> routPoints, int? shippingRequestGoodCategoryId)
        {
            var goodsCategories = await _goodCategoryRepository.GetAllListAsync();

            // todo Add Localized String Here
            foreach (var goodsDetail in routPoints.Where(x => x.GoodsDetailListDto != null)
                         .SelectMany(routPoint => routPoint.GoodsDetailListDto))
            {
                if (goodsDetail.GoodCategoryId != null)
                {
                    if (shippingRequestGoodCategoryId == null)
                        throw new UserFriendlyException(L("ErrorWhenCreateTrip")); // Need Review

                    var goodCategory = goodsCategories
                        .FirstOrDefault(x => x.Id == goodsDetail.GoodCategoryId.Value);

                    if (goodCategory.FatherId == null)
                        throw new UserFriendlyException(L("GoodsCategoryMustBeSubCategoryNotMainCategory"));

                    if (goodCategory.FatherId != shippingRequestGoodCategoryId)
                        throw new UserFriendlyException(L("GoodsCategoryMustBeSubOfShippingRequestGoodCategory"));
                }
                else throw new UserFriendlyException(L("GoodsCategoryIsRequired"));
            }
        }


        private async Task RemoveDeletedTripVases(CreateOrEditShippingRequestTripDto input, ShippingRequestTrip trip)
        {
            //delete vases except appointment and clearance, that added manual from back when add appointment and clearance prices
            foreach (var vas in trip.ShippingRequestTripVases.Where(x=> !x.VasFk.Name.Equals(TACHYONConsts.AppointmentVasName) &&
            !x.VasFk.Name.Equals(TACHYONConsts.ClearanceVasName)))
            {
                if (!input.ShippingRequestTripVases.Any(x => x.Id == vas.Id))
                {
                    await _shippingRequestTripVasRepository.DeleteAsync(vas);
                }
            }
        }

        private async Task RemoveDeletedTripPoints(CreateOrEditShippingRequestTripDto input, ShippingRequestTrip trip)
        {
            foreach (var point in trip.RoutPoints)
            {
                if (!input.RoutPoints.Any(x => x.Id == point.Id))
                {
                    await _routPointRepository.DeleteAsync(point);
                }

                foreach (var g in point.GoodsDetails.Where(x => x.Id != 0))
                {
                    if (!input.RoutPoints.Any(x => (x.GoodsDetailListDto != null && x.GoodsDetailListDto.Count > 0) && x.GoodsDetailListDto.Any(d => d.Id == g.Id)))
                    {
                        await _goodsDetailRepository.DeleteAsync(g);
                    }
                }
            }
        }


        private async Task<bool> CheckIfDriverWorkingOnAnotherTrip(long assignedDriverUserId)
        {
            return await _shippingRequestTripRepository.GetAll()
                .AnyAsync(x => x.AssignedDriverUserId == assignedDriverUserId
                            && x.Status == ShippingRequestTripStatus.InTransit
                            && x.DriverStatus == ShippingRequestTripDriverStatus.Accepted);
        }


        private async Task CancelTripAsync(CancelTripInput input, ShippingRequestTrip trip, UserIdentifier carrierIdent)
        {
            List<UserIdentifier> userIdentifiers = new List<UserIdentifier>();

            ValidateCanceledReason(input, trip);

            if (IsEnabled(AppFeatures.Shipper))
            {
                //add carrier when request in post price
                if (trip.ShippingRequestFk.CarrierTenantId != null)
                {
                    userIdentifiers.Add(carrierIdent);
                }
                userIdentifiers.Add(await _userManager.GetTachyonDealerUserIdentifierAsync());
                var shipperTenant = await _tenantManager.GetByIdAsync(trip.ShippingRequestFk.TenantId);

                if (!trip.ShippingRequestFk.IsTachyonDeal)
                {
                    trip.Status = ShippingRequestTripStatus.Canceled;
                    trip.CancelStatus = ShippingRequestTripCancelStatus.Canceled;
                    await _appNotifier.ShippingRequestTripCanceled(userIdentifiers, trip, shipperTenant.TenancyName);
                }
                else
                {
                    trip.CancelStatus = ShippingRequestTripCancelStatus.WaitingForTMSApproval;
                    if (trip.ShippingRequestId != null)
                    {
                        await _appNotifier.NotifyTmsWhenCancellationRequestedByShipper
                        (
                            trip.ShippingRequestFk.ReferenceNumber,
                            trip.WaybillNumber.ToString(),
                            shipperTenant.TenancyName,
                            trip.ShippingRequestId.Value
                        );
                    }
                }
            }
            else if (IsEnabled(AppFeatures.Carrier))
            {
                trip.IsApproveCancledByCarrier = true;
                userIdentifiers.Add(new UserIdentifier(trip.ShippingRequestFk.TenantId, (long)trip.ShippingRequestFk.CreatorUserId));
                userIdentifiers.Add(await _userManager.GetTachyonDealerUserIdentifierAsync());

                if (!trip.ShippingRequestFk.IsTachyonDeal)
                {
                    trip.Status = ShippingRequestTripStatus.Canceled;
                    trip.CancelStatus = ShippingRequestTripCancelStatus.Canceled;
                    await _appNotifier.ShippingRequestTripCanceled(userIdentifiers, trip, (await _tenantManager.GetByIdAsync(carrierIdent.TenantId.Value)).TenancyName);
                }
                else
                {
                    trip.CancelStatus = ShippingRequestTripCancelStatus.WaitingForTMSApproval;
                }
            }
            else if (IsEnabled(AppFeatures.TachyonDealer))
            {
                var tmsIdent = await _userManager.GetTachyonDealerUserIdentifierAsync();
                trip.IsApproveCancledByTachyonDealer = input.IsApproved;
                if (trip.IsApproveCancledByTachyonDealer)
                {
                    trip.Status = ShippingRequestTripStatus.Canceled;
                    trip.CancelStatus = ShippingRequestTripCancelStatus.Canceled;
                    if (trip.ShippingRequestFk.CarrierTenantId != null)
                    {
                        userIdentifiers.Add(await GetAdminTenant((int)trip.ShippingRequestFk.CarrierTenantId));
                    }

                    var shipperAdmin = await UserManager.GetAdminByTenantIdAsync(trip.ShippingRequestFk.TenantId);
                    userIdentifiers.Add(shipperAdmin.ToUserIdentifier());
                    await _appNotifier.ShippingRequestTripCanceled(userIdentifiers, trip, (await _tenantManager.GetByIdAsync(tmsIdent.TenantId.Value)).TenancyName);
                }
                else
                {
                    ValidateRejectedReason(input, trip);
                    trip.RejectedCancelingReason = input.RejectedCancelingReason;
                    trip.CancelStatus = ShippingRequestTripCancelStatus.Rejected;
                    await _appNotifier.ShippingRequestTripRejectCancelByTachyonDealer(userIdentifiers, trip.ShippingRequestFk);
                }
            }

        }

        private void ValidateRejectedReason(CancelTripInput input, ShippingRequestTrip trip)
        {
            if (string.IsNullOrWhiteSpace(input.RejectedCancelingReason) &&
                                    trip.CancelStatus == ShippingRequestTripCancelStatus.WaitingForTMSApproval)
            {
                throw new UserFriendlyException(L("YouMustEnterRejectedReason"));
            }
        }

        private void ValidateCanceledReason(CancelTripInput input, ShippingRequestTrip trip)
        {
            if (trip.CancelStatus == ShippingRequestTripCancelStatus.None)
            {
                if (string.IsNullOrWhiteSpace(input.CanceledReason))
                {
                    throw new UserFriendlyException(L("CanceledReasonIsRequired"));
                }
                trip.CanceledReason = input.CanceledReason;
            }
        }

        private async Task ValidatePortMovementRequest(CreateOrEditShippingRequestTripDto input, ShippingRequest request)
        {
            if (request.ShippingTypeId == ShippingTypeEnum.ImportPortMovements || request.ShippingTypeId == ShippingTypeEnum.ExportPortMovements)
            {
                _shippingRequestTripManager.ValidateDedicatedNumberOfPickups(input.RoutPoints.Count(x => x.PickingType == PickingType.Pickup), input.NumberOfDrops);

                if (string.IsNullOrEmpty(input.ContainerNumber))
                {
                    throw new UserFriendlyException(L("ContainerNumberIsRequired"));
                }

                if (input.RoutPoints.Any(x => x.PointOrder == null || x.PointOrder <= 0))
                {
                    throw new UserFriendlyException(L("PointOrderIsMandatory"));
                }

                var facilities = _shippingRequestTripManager.GetAllFacilitiesByIds(input.RoutPoints.Select(x => x.FacilityId).ToList());

                //validate sender, receiver, weight, Qty, description
                var firstStep = input.RoutPoints.OrderBy(x => x.PointOrder).Take(2).ToList();
                var secondStep = input.RoutPoints.OrderBy(x => x.PointOrder).Skip(2)?.Take(2).ToList();
                var thirdStep = input.RoutPoints.OrderBy(x => x.PointOrder).Skip(4)?.Take(2).ToList();

                if (request.ShippingTypeId == ShippingTypeEnum.ImportPortMovements)
                {
                    if (firstStep[0].FacilityId != request.OriginFacilityId && request.ShippingRequestFlag == ShippingRequestFlag.Normal)
                    {
                        throw new UserFriendlyException(L("OriginPortMustBeSameAsOriginRequestPort"));
                    }

                    if (!firstStep[1].ReceiverId.HasValue && firstStep[1].ReceiverPhoneNumber.IsNullOrEmpty())
                    {
                        throw new UserFriendlyException(L("ReceiverIsRequiredForFirstTripDrop"));
                    }
                    if (firstStep[1].GoodsDetailListDto.Any(x => string.IsNullOrEmpty(x.Description)))
                    {
                        throw new UserFriendlyException(L("GoodsDescriptionForFirstTripIsRequired"));
                    }

                    if (facilities.First(x => x.Id == firstStep[1].FacilityId).FacilityType != AddressBook.FacilityType.Facility)
                    {
                        throw new UserFriendlyException(L("DropFacilityTypeForSecondTripMustNotBePort"));
                    }
                    await ValidateGoodsCategory(firstStep, request.GoodCategoryId);

                    //second trip
                    if (request.RoundTripType == RoundTripType.WithReturnTrip)
                    {
                        if (secondStep[0].FacilityId != firstStep[1].FacilityId)
                        {
                            throw new UserFriendlyException(L("InvalidFacilityPickupForSecondTrip"));
                        }
                        if (!secondStep[0].ReceiverId.HasValue && secondStep[0].ReceiverPhoneNumber.IsNullOrEmpty())
                        {
                            throw new UserFriendlyException(L("SenderIsRequiredForSecondTrip"));
                        }

                        if (facilities.First(x => x.Id == secondStep[1].FacilityId).FacilityType == AddressBook.FacilityType.Facility &&
                            !secondStep[1].ReceiverId.HasValue && secondStep[1].ReceiverPhoneNumber.IsNullOrEmpty())
                        {
                            throw new UserFriendlyException(L("ReceiverIsRequiredForSecondTrip"));
                        }
                    }

                }

                else if (request.ShippingTypeId == ShippingTypeEnum.ExportPortMovements)
                {
                    if (request.RoundTripType == RoundTripType.TwoWayRoutsWithPortShuttling)
                    {
                        if (firstStep[0].ReceiverId == null || thirdStep[0].ReceiverId == null)
                        {
                            throw new UserFriendlyException(L("SenderIsRequired"));
                        }
                        if (secondStep[1].ReceiverId == null)
                        {
                            throw new UserFriendlyException(L("ReceiverIsRequiredForSecondTrip"));
                        }
                        if (facilities.FirstOrDefault(x => x.Id == thirdStep[1].FacilityId).FacilityType != AddressBook.FacilityType.Port)
                        {
                            throw new UserFriendlyException(L("FinalDropFacilityMustBePort"));
                        }
                        await ValidateGoodsCategory(secondStep.Union(thirdStep), request.GoodCategoryId);
                    }
                    else if (request.RoundTripType == RoundTripType.TwoWayRoutsWithoutPortShuttling)
                    {
                        await ValidateGoodsCategory(secondStep, request.GoodCategoryId);
                        if (firstStep[0].ReceiverId == null)
                        {
                            throw new UserFriendlyException(L("SenderIsRequired"));
                        }
                        if (secondStep[1].ReceiverId == null)
                        {
                            throw new UserFriendlyException(L("ReceiverIsRequiredForSecondTrip"));
                        }
                    }
                    else if (request.RoundTripType == RoundTripType.OneWayRoutWithoutPortShuttling)
                    {
                        if (facilities.FirstOrDefault(x => x.Id == firstStep[1].FacilityId).FacilityType != AddressBook.FacilityType.Port)
                        {
                            throw new UserFriendlyException(L("DropFacilityMustBePort"));
                        }
                    }

                    if (input.RoutPoints.Where(x => x.PickingType == PickingType.Dropoff).SelectMany(x => x.GoodsDetailListDto).Any(x => x.Amount == null || string.IsNullOrEmpty(x.Description)))
                    {
                        throw new UserFriendlyException(L("GoodsQtyAndDescriptionIsRequired"));
                    }
                }
            }
        }

        private static void ValidateReceiver(CreateOrEditShippingRequestTripDto input)
        {
            foreach (var drop in input.RoutPoints)
            {
                if (drop.ReceiverId == null &&
                    (string.IsNullOrWhiteSpace(drop.ReceiverFullName) ||
                     string.IsNullOrWhiteSpace(drop.ReceiverPhoneNumber)))
                {
                    throw new UserFriendlyException("YouMustEnterReceiver");
                }
            }
        }

        private async Task<int?> GetGeneralGoodsCategoryId()
        {
            return await _goodCategoryRepository.GetAll().Where(x => x.Flag.Equals(TACHYONConsts.GeneralGoods)).Select(x => x.Id).FirstOrDefaultAsync();
        }

        private static void AddAllRequestVasesToDedicatedTrip(CreateOrEditShippingRequestTripDto input, ShippingRequest request)
        {
            var vasList = new List<CreateOrEditShippingRequestTripVasDto>();
            foreach (var requestVas in request.ShippingRequestVases)
            {
                var tripVas = new CreateOrEditShippingRequestTripVasDto
                {
                    ShippingRequestVasId = requestVas.VasId
                };
                vasList.Add(tripVas);
            }
            input.ShippingRequestTripVases = vasList;
        }

        private async Task AddOrRemoveDestinationCities(List<ShippingRequestDestinationCitiesDto> destinationCitiesDtos, ShippingRequestTrip trip)
        {
            foreach (var destinationCity in destinationCitiesDtos)
            {
                DisableDraftedFilter();
                //destinationCity.ShippingRequestId = shippingRequest.Id;
                var exists = await _shippingRequestDestinationCityRepository.GetAll().AnyAsync(c => c.CityId == destinationCity.CityId &&
                c.ShippingRequestTripId == trip.Id);

                if (!exists)
                {
                    if (trip.ShippingRequestDestinationCities == null) trip.ShippingRequestDestinationCities = new List<ShippingRequestDestinationCity>();
                    trip.ShippingRequestDestinationCities.Add(ObjectMapper.Map<ShippingRequestDestinationCity>(destinationCity));
                }
            }
            //remove uncoming destination cities
            foreach (var destinationCity in trip.ShippingRequestDestinationCities)
            {
                var cityId = destinationCity.CityId;
                if (!destinationCitiesDtos.Any(x => x.CityId == cityId))
                {
                    await _shippingRequestDestinationCityRepository.DeleteAsync(destinationCity);
                }
            }
        }
        #endregion
    }
}