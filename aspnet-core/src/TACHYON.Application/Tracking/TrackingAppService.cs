using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
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
using TACHYON.Authorization.Users.Profile;
using TACHYON.Common;
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

namespace TACHYON.Tracking
{
    [AbpAuthorize()]
    public class TrackingAppService : TACHYONAppServiceBase, ITrackingAppService
    {
        private readonly IRepository<ShippingRequestTrip> _ShippingRequestTripRepository;
        private readonly IRepository<RoutPoint, long> _RoutPointRepository;
        private readonly ShippingRequestPointWorkFlowProvider _workFlowProvider;
        private readonly ProfileAppService _ProfileAppService;

        public TrackingAppService(ShippingRequestPointWorkFlowProvider workFlowProvider, IRepository<ShippingRequestTrip> shippingRequestTripRepository, ProfileAppService profileAppService, IRepository<RoutPoint, long> routPointRepository)
        {
            _ShippingRequestTripRepository = shippingRequestTripRepository;
            _workFlowProvider = workFlowProvider;
            _ProfileAppService = profileAppService;
            _RoutPointRepository = routPointRepository;
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
             .ThenInclude(s => s.Tenant)
            .Include(r => r.ShippingRequestFk)
             .ThenInclude(c => c.CarrierTenantFk)
                            //TAC-1509
                            //.Where(x => x.ShippingRequestFk.CarrierTenantId.HasValue)
                            .Where(x => x.ShippingRequestFk.Status == ShippingRequestStatus.PostPrice)
                            .WhereIf(AbpSession.TenantId.HasValue && await IsEnabledAsync(AppFeatures.Shipper), x => x.ShippingRequestFk.TenantId == AbpSession.TenantId)
                            .WhereIf(!AbpSession.TenantId.HasValue || await IsEnabledAsync(AppFeatures.TachyonDealer), x => true)
                            .WhereIf(AbpSession.TenantId.HasValue && await IsEnabledAsync(AppFeatures.Carrier), x => x.ShippingRequestFk.CarrierTenantId == AbpSession.TenantId)
                            .WhereIf(input.PickupFromDate.HasValue && input.PickupToDate.HasValue, x => x.ShippingRequestFk.StartTripDate >= input.PickupFromDate.Value && x.ShippingRequestFk.StartTripDate <= input.PickupToDate.Value)
                            .WhereIf(input.FromDate.HasValue && input.ToDate.HasValue, x => x.StartTripDate >= input.FromDate.Value && x.StartTripDate <= input.ToDate.Value)
                            .WhereIf(input.OriginId.HasValue, x => x.ShippingRequestFk.OriginCityId == input.OriginId)
                            .WhereIf(input.DestinationId.HasValue, x => x.ShippingRequestFk.DestinationCityId == input.DestinationId)
                            .WhereIf(input.RouteTypeId.HasValue, x => x.ShippingRequestFk.RouteTypeId == input.RouteTypeId)
                            .WhereIf(input.TruckTypeId.HasValue, x => x.ShippingRequestFk.TrucksTypeId == input.TruckTypeId)
                            .WhereIf(input.Status.HasValue, x => x.Status == input.Status)
                            .WhereIf(input.WaybillNumber.HasValue, x => x.WaybillNumber == input.WaybillNumber)
                            .WhereIf(!string.IsNullOrEmpty(input.Shipper), x => x.ShippingRequestFk.Tenant.Name.ToLower().Contains(input.Shipper) || x.ShippingRequestFk.Tenant.companyName.ToLower().Contains(input.Shipper) || x.ShippingRequestFk.Tenant.TenancyName.ToLower().Contains(input.Shipper))
                            .WhereIf(!string.IsNullOrEmpty(input.Carrier), x => x.ShippingRequestFk.CarrierTenantFk.Name.ToLower().Contains(input.Carrier) || x.ShippingRequestFk.CarrierTenantFk.companyName.ToLower().Contains(input.Carrier) || x.ShippingRequestFk.CarrierTenantFk.TenancyName.ToLower().Contains(input.Carrier))
                            .OrderBy(input.Sorting ?? "id desc")
                            .PageBy(input).ToList();

            List<TrackingListDto> trackingLists = new List<TrackingListDto>();
            query.ForEach(r =>
            {
                trackingLists.Add(GetMap(r));
            });

            return new PagedResultDto<TrackingListDto>(
                query.Count,
               trackingLists

            );
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
                    lat = a.FacilityFk.Location.X,
                    lng = a.FacilityFk.Location.Y,
                    StartTime = a.StartTime,
                    EndTime = a.EndTime,
                    IsActive = a.IsActive,
                    IsComplete = a.IsComplete,
                    IsResolve = a.IsResolve,
                    CanGoToNextLocation = a.CanGoToNextLocation,
                    IsDeliveryNoteUploaded = a.IsDeliveryNoteUploaded,
                    WaybillNumber = a.WaybillNumber,
                    IsPodUploaded = a.IsPodUploaded,
                    IsGoodPictureUploaded = a.IsGoodPictureUploaded,
                    FacilityRate = a.FacilityFk.Rate,
                    Statues = _workFlowProvider.GetStatuses(a.WorkFlowVersion, a.RoutPointStatusTransitions.Where(x => !x.IsReset).Select(x => x.Status).ToList()),
                    AvailableTransactions = !a.IsResolve ? new List<PointTransactionDto>() : _workFlowProvider.GetTransactionsByStatus(a.WorkFlowVersion, a.RoutPointStatusTransitions.Where(c => !c.IsReset).Select(v => v.Status).ToList(), a.Status),
                }).ToListAsync();

            mappedTrip.CanStartTrip = CanStartTrip(trip);
            return mappedTrip;
        }
        public async Task Accept(int id)
        {
            CheckIfCanAccessService(true, AppFeatures.TachyonDealer, AppFeatures.Carrier);
            await _workFlowProvider.Accepted(id);
        }
        public async Task Start(int id)
        {
            CheckIfCanAccessService(true, AppFeatures.TachyonDealer, AppFeatures.Carrier);
            await _workFlowProvider.Start(new ShippingRequestTripDriverStartInputDto { Id = id });
        }
        public async Task InvokeStatus(InvokeStatusInputDto input)
        {
            CheckIfCanAccessService(true, AppFeatures.TachyonDealer, AppFeatures.Carrier);
            var args = new PointTransactionArgs
            {
                PointId = input.Id,
                Code = input.Code
            };
            await _workFlowProvider.Invoke(args, input.Action);
        }
        public async Task NextLocation(long id)
        {
            CheckIfCanAccessService(true, AppFeatures.TachyonDealer, AppFeatures.Carrier);
            await _workFlowProvider.GoToNextLocation(id);
        }
        public async Task<List<FileDto>> POD(long id)
        {
            CheckIfCanAccessService(true, AppFeatures.TachyonDealer, AppFeatures.Carrier, AppFeatures.Shipper);
            return await _workFlowProvider.GetPOD(id);
        }
        public async Task<IHasDocument> GetDeliveryGoodPicture(long id)
        {
            CheckIfCanAccessService(true, AppFeatures.TachyonDealer, AppFeatures.Carrier, AppFeatures.Shipper);
            return await _workFlowProvider.GetDeliveryGoodPicture(id);
        }

        #region Helper
        private TrackingListDto GetMap(ShippingRequestTrip trip)
        {
            var workingOnAnotherTrip = WorkingInAnotherTrip(trip);
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
        private bool WorkingInAnotherTrip(ShippingRequestTrip trip)
        {
            return _ShippingRequestTripRepository.GetAll().Any(x => x.AssignedDriverUserId == trip.AssignedDriverUserId && x.DriverStatus == ShippingRequestTripDriverStatus.Accepted && x.Status == ShippingRequestTripStatus.Intransit);
        }
        #endregion
    }
}