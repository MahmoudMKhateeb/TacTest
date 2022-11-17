using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.EntityHistory;
using Abp.Linq.Extensions;
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
using TACHYON.Common;
using TACHYON.Documents.DocumentFiles.Dtos;
using TACHYON.Dto;
using TACHYON.Features;
using TACHYON.Goods.GoodCategories.Dtos;
using TACHYON.Routs.RoutPoints;
using TACHYON.Shipping.Drivers.Dto;
using TACHYON.Shipping.ShippingRequests;
using TACHYON.Shipping.ShippingRequestTrips;
using TACHYON.Shipping.Trips;
using TACHYON.Shipping.Trips.RejectReasons.Dtos;
using TACHYON.Tracking.Dto;
using TACHYON.Tracking.Dto.WorkFlow;
using TACHYON.Trucks.TrucksTypes.Dtos;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Nito.AsyncEx;
using System.Threading;

namespace TACHYON.Tracking
{
    [AbpAuthorize()]
    public class TrackingAppService : TACHYONAppServiceBase
    {
        private readonly IRepository<ShippingRequestTrip> _ShippingRequestTripRepository;
        private readonly IRepository<RoutPoint, long> _RoutPointRepository;
        private readonly IRepository<ShippingRequest, long> _shippingRequestRepository;
        private readonly ShippingRequestPointWorkFlowProvider _workFlowProvider;
        private readonly ProfileAppService _ProfileAppService;
        private readonly ForceDeliverTripExcelExporter _deliverTripExcelExporter;

        public TrackingAppService(ShippingRequestPointWorkFlowProvider workFlowProvider, IRepository<ShippingRequestTrip> shippingRequestTripRepository, ProfileAppService profileAppService, IRepository<RoutPoint, long> routPointRepository, ForceDeliverTripExcelExporter deliverTripExcelExporter)
        {
            _ShippingRequestTripRepository = shippingRequestTripRepository;
            _workFlowProvider = workFlowProvider;
            _ProfileAppService = profileAppService;
            _RoutPointRepository = routPointRepository;
            _deliverTripExcelExporter = deliverTripExcelExporter;
        }
        public async Task<PagedResultDto<TrackingListDto>> GetAll(TrackingSearchInputDto input)
        {
            CheckIfCanAccessService(true, AppFeatures.TachyonDealer, AppFeatures.Carrier, AppFeatures.Shipper);

            DisableTenancyFilters();
            var query = _ShippingRequestTripRepository
            .GetAll()
            .AsNoTracking()
            .Include(x => x.OriginFacilityFk)
            .Include(x => x.DestinationFacilityFk)
            .Include(x => x.AssignedTruckFk)
             .ThenInclude(t => t.TrucksTypeFk)
               .ThenInclude(t => t.Translations)
            .Include(x => x.AssignedDriverUserFk)
            .Include(x => x.ShippingRequestTripRejectReason)
                .ThenInclude(t => t.Translations)
            .Include(r => r.ShippingRequestFk)
              .ThenInclude(c => c.GoodCategoryFk)
                .ThenInclude(t => t.Translations)
                .Include(r => r.ShippingRequestFk)
                .ThenInclude(c => c.GoodCategoryFk)
                .ThenInclude(t => t.Translations)
                .Include(r => r.ShippingRequestFk)
                .ThenInclude(s => s.Tenant)
                .Include(r => r.ShippingRequestFk)
                .ThenInclude(c => c.CarrierTenantFk)
                //TAC-1509
                //.Where(x => x.ShippingRequestFk.CarrierTenantId.HasValue)
                .Where(x=>x.ShippingRequestFk.ShippingRequestFlag==ShippingRequestFlag.Normal)
                .Where(x => x.ShippingRequestFk.Status == ShippingRequestStatus.PostPrice || x.ShippingRequestFk.CarrierTenantId.HasValue)
                .WhereIf(AbpSession.TenantId.HasValue && await IsEnabledAsync(AppFeatures.Shipper),
                    x => x.ShippingRequestFk.TenantId == AbpSession.TenantId)
                .WhereIf(!AbpSession.TenantId.HasValue || await IsEnabledAsync(AppFeatures.TachyonDealer), x => true)
                .WhereIf(AbpSession.TenantId.HasValue && await IsEnabledAsync(AppFeatures.Carrier),
                    x => x.ShippingRequestFk.CarrierTenantId == AbpSession.TenantId)
                .WhereIf(input.PickupFromDate.HasValue && input.PickupToDate.HasValue,
                    x => x.ShippingRequestFk.StartTripDate >= input.PickupFromDate.Value &&
                         x.ShippingRequestFk.StartTripDate <= input.PickupToDate.Value)
                .WhereIf(input.FromDate.HasValue && input.ToDate.HasValue,
                    x => x.StartTripDate >= input.FromDate.Value && x.StartTripDate <= input.ToDate.Value)
                .WhereIf(input.OriginId.HasValue, x => x.ShippingRequestFk.OriginCityId == input.OriginId)
                .WhereIf(input.DestinationId.HasValue,
                    x => x.ShippingRequestFk.ShippingRequestDestinationCities.Any(y=>y.CityId == input.DestinationId))
                .WhereIf(input.RouteTypeId.HasValue, x => x.ShippingRequestFk.RouteTypeId == input.RouteTypeId)
                .WhereIf(input.TransportTypeId.HasValue, x=>x.ShippingRequestFk.TransportTypeId == input.TransportTypeId)
                .WhereIf(input.TruckTypeId.HasValue, x => x.ShippingRequestFk.TrucksTypeId == input.TruckTypeId)
                .WhereIf(input.TruckCapacityId.HasValue, x => x.ShippingRequestFk.CapacityId == input.TruckCapacityId)
                .WhereIf(input.Status.HasValue, x => x.Status == input.Status)
                .WhereIf(input.WaybillNumber.HasValue, x => x.WaybillNumber == input.WaybillNumber ||
                x.RoutPoints.Any(y=>y.WaybillNumber == input.WaybillNumber))
                .WhereIf(!string.IsNullOrEmpty(input.Shipper),
                    x => x.ShippingRequestFk.Tenant.Name.ToLower().Contains(input.Shipper) ||
                         x.ShippingRequestFk.Tenant.companyName.ToLower().Contains(input.Shipper) ||
                         x.ShippingRequestFk.Tenant.TenancyName.ToLower().Contains(input.Shipper))
                .WhereIf(!string.IsNullOrEmpty(input.ReferenceNumber), x => x.ShippingRequestFk.ReferenceNumber.Contains(input.ReferenceNumber))
                .WhereIf(!string.IsNullOrEmpty(input.Carrier),
                    x => x.ShippingRequestFk.CarrierTenantFk.Name.ToLower().Contains(input.Carrier) ||
                         x.ShippingRequestFk.CarrierTenantFk.companyName.ToLower().Contains(input.Carrier) ||
                         x.ShippingRequestFk.CarrierTenantFk.TenancyName.ToLower().Contains(input.Carrier))
                .WhereIf(input.PackingTypeId.HasValue, x => x.ShippingRequestFk.PackingTypeId == input.PackingTypeId)
                .WhereIf(input.GoodsOrSubGoodsCategoryId.HasValue, x => x.ShippingRequestFk.GoodCategoryId == input.GoodsOrSubGoodsCategoryId)
                .WhereIf(!string.IsNullOrEmpty(input.PlateNumberId), x=>x.ShippingRequestFk.AssignedTruckFk.PlateNumber == input.PlateNumberId)
                .WhereIf(!string.IsNullOrEmpty(input.DriverNameOrMobile), x => x.ShippingRequestFk.AssignedDriverUserFk.PhoneNumber == input.DriverNameOrMobile ||
                (x.ShippingRequestFk.AssignedDriverUserFk!=null && 
                (x.ShippingRequestFk.AssignedDriverUserFk.Name.ToLower().Contains(input.DriverNameOrMobile) ||
                x.ShippingRequestFk.AssignedDriverUserFk.Surname.ToLower().Contains(input.DriverNameOrMobile))))
                .WhereIf(input.DeliveryFromDate.HasValue && input.DeliveryToDate.HasValue, x=>x.ShippingRequestFk.ShippingRequestTrips.Any(y=> y.ActualDeliveryDate>= input.DeliveryFromDate &&
                y.ActualDeliveryDate<= input.DeliveryToDate))
                .WhereIf(!string.IsNullOrEmpty(input.ContainerNumber), x => x.ShippingRequestFk.ShippingRequestTrips.Any(y=>y.ContainerNumber == input.ContainerNumber))
                .WhereIf(input.IsInvoiceIssued !=null, x => x.ShippingRequestFk.ShippingRequestTrips.Any(y => y.IsShipperHaveInvoice == input.IsInvoiceIssued))
                .WhereIf(input.IsSubmittedPOD !=null, x => x.ShippingRequestFk.ShippingRequestTrips.Any(y => y.RoutPoints.Any(x=>x.IsPodUploaded == input.IsSubmittedPOD)))
                //todo for tasneem will update request type after dediced
                .WhereIf(input.RequestTypeId ==1, x=>x.ShippingRequestFk.TenantId != x.ShippingRequestFk.CarrierTenantId)
                .WhereIf(input.RequestTypeId == 2, x => x.ShippingRequestFk.TenantId == x.ShippingRequestFk.CarrierTenantId)
                .OrderBy(input.Sorting ?? "id desc")
                .PageBy(input).ToList();

            List<TrackingListDto> trackingLists = new List<TrackingListDto>();
            query.ForEach(async r =>
            {
                trackingLists.Add(await GetMap(r));
            });

            return new PagedResultDto<TrackingListDto>(
                query.Count,
               trackingLists

            );
        }
        [AbpAllowAnonymous]
        public async Task<PagedResultDto<TrackingByWaybillDto>> GetDropsOffByMasterWaybill(long waybillNumber)
        {
            DisableTenancyFilters();
            var query =  await _RoutPointRepository
             .GetAll().AsNoTracking()
             .Where(x => x.ShippingRequestTripFk.WaybillNumber == waybillNumber)
             .ProjectTo<TrackingByWaybillDto>(AutoMapperConfigurationProvider).ToListAsync();
            if (!query.Any()) throw new UserFriendlyException(L("InCorrectWaybillNumber"));

             return new PagedResultDto<TrackingByWaybillDto>(query.Count, query);

        }

        [AbpAllowAnonymous]
        public async Task<TrackingByWaybillRoutPointDto> GetDropOffBySubWaybill(string waybillNumber)
        {
            DisableTenancyFilters();
            var dropOff =  await _RoutPointRepository
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

            var trip =
                 await _ShippingRequestTripRepository.GetAll()
                            .Where(x => x.Id == id && x.ShippingRequestFk.CarrierTenantId.HasValue)
                            .WhereIf(AbpSession.TenantId.HasValue && await IsEnabledAsync(AppFeatures.Shipper), x => x.ShippingRequestFk.TenantId == AbpSession.TenantId)
                            .WhereIf(!AbpSession.TenantId.HasValue || await IsEnabledAsync(AppFeatures.TachyonDealer), x => true)
                            .WhereIf(AbpSession.TenantId.HasValue && await IsEnabledAsync(AppFeatures.Carrier), x => x.ShippingRequestFk.CarrierTenantId == AbpSession.TenantId)
                            .FirstOrDefaultAsync();

            if (trip == null) throw new UserFriendlyException(L("TheTripIsNotFound"));
            var mappedTrip = ObjectMapper.Map<TrackingShippingRequestTripDto>(trip);

            mappedTrip.RoutPoints = await _RoutPointRepository.GetAll()
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
                    Statues = _workFlowProvider.GetStatuses(a.WorkFlowVersion,
                            a.RoutPointStatusTransitions.Where(x => !x.IsReset)
                            .Select(x => new RoutPointTransactionArgDto { Status = x.Status, CreationTime = x.CreationTime }).ToList()),
                    AvailableTransactions = !a.IsResolve || mappedTrip.DriverStatus != ShippingRequestTripDriverStatus.Accepted
                        ? new List<PointTransactionDto>()
                        : _workFlowProvider.GetTransactionsByStatus(a.WorkFlowVersion,
                            a.RoutPointStatusTransitions.Where(c => !c.IsReset).Select(v => v.Status).ToList(),
                            a.Status),
                }).ToListAsync();
            mappedTrip.RoutPoints = mappedTrip.RoutPoints.OrderBy(x => x.PickingType).ToList();
            return mappedTrip;
        }

        public async Task<FileDto> GetForceDeliverTripExcelFile(int tripId)
        {
            DisableTenancyFilters();
            var tripPoints = await _RoutPointRepository.GetAll()
                .Where(x=> x.ShippingRequestTripId == tripId)
                .Select(x => new ExportPointExcelDto() {Id = x.Id, PickingType = x.PickingType, WorkFlowVersion = x.WorkFlowVersion})
                .ToListAsync();
            
            tripPoints.ForEach(point =>
            {
                point.Transactions = _workFlowProvider.Flows.Where(x => x.Version == point.WorkFlowVersion)
                    .SelectMany(x => x.Transactions).GroupBy(x=> x.ToStatus)
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
            CheckIfCanAccessService(true, AppFeatures.TachyonDealer, AppFeatures.Carrier,  AppFeatures.CarrierClients);
            var args = new PointTransactionArgs
            {
                PointId = input.Id,
                Code = input.Code
            };
            await _workFlowProvider.Invoke(args, input.Action);
        }
        public async Task NextLocation(long id)
        {
            CheckIfCanAccessService(true, AppFeatures.TachyonDealer, AppFeatures.Carrier,  AppFeatures.CarrierClients);
            await _workFlowProvider.GoToNextLocation(id);
        }
        public async Task<List<GetAllUploadedFileDto>> POD(long id)
        {
            CheckIfCanAccessService(true, AppFeatures.TachyonDealer, AppFeatures.Carrier, AppFeatures.Shipper, AppFeatures.CarrierClients);
            return await _workFlowProvider.GetPOD(id);
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
        private async Task<TrackingListDto> GetMap(ShippingRequestTrip trip)
        {
            var dto = ObjectMapper.Map<TrackingListDto>(trip);

            var date64 = _ProfileAppService.GetProfilePictureByUser((long)trip.CreatorUserId).Result.ProfilePicture;
            dto.TenantPhoto = String.IsNullOrEmpty(date64) ? null : date64;
            dto.NumberOfDrops = trip.ShippingRequestFk.NumberOfDrops;
            if (trip.AssignedTruckFk != null) dto.TruckType = ObjectMapper.Map<TrucksTypeDto>(trip.AssignedTruckFk.TrucksTypeFk)?.TranslatedDisplayName ?? "";
            dto.GoodsCategory = ObjectMapper.Map<GoodCategoryDto>(trip.ShippingRequestFk.GoodCategoryFk)?.DisplayName;
            if (trip.ShippingRequestTripRejectReason != null)
            {
                dto.Reason = ObjectMapper.Map<ShippingRequestTripRejectReasonListDto>(trip.ShippingRequestTripRejectReason).Name ?? "";
            }
            var tenantId = AbpSession.TenantId;
            if (!tenantId.HasValue || (tenantId.HasValue && !IsEnabled(AppFeatures.Shipper)))
            {
                dto.Name = tenantId.HasValue && IsEnabled(AppFeatures.Carrier) ? trip.ShippingRequestFk.Tenant.Name : dto.Name = $"{trip.ShippingRequestFk?.Tenant?.Name}-{trip.ShippingRequestFk?.CarrierTenantFk?.Name}";
                dto.IsAssign = true;
                dto.CanStartTrip = CanStartTrip(trip);
                // dto.CanAcceptTrip = CanAcceptTrip(trip, workingOnAnotherTrip);
                //if (!dto.CanAcceptTrip)
                //    dto.NoActionReason = CanNotAcceptReason(trip, workingOnAnotherTrip);
                //if (trip.Status == ShippingRequestTripStatus.New && trip.DriverStatus == ShippingRequestTripDriverStatus.Accepted && !dto.CanStartTrip)
                //    dto.NoActionReason = CanNotStartReason(trip, workingOnAnotherTrip);
            }
            dto.CanDriveTrip = false;
            if (tenantId.HasValue && (tenantId==trip.ShippingRequestFk.CarrierTenantId || await IsTachyonDealer()))
            {
                dto.CanDriveTrip = true;
            }
            return dto;
        }
        private bool CanStartTrip(ShippingRequestTrip trip)
        {
            if (trip.StartTripDate.Date <= Clock.Now.Date
               && trip.Status == ShippingRequestTripStatus.New
               && trip.DriverStatus == ShippingRequestTripDriverStatus.Accepted)
                return true;

            return false;
        }

        #endregion
    }
}