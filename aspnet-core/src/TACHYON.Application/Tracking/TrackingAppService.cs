using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Abp.Timing;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using NUglify.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using TACHYON.Authorization;
using TACHYON.Dto;
using TACHYON.Features;
using TACHYON.Goods.GoodCategories.Dtos;
using TACHYON.Routs.RoutPoints;
using TACHYON.Shipping.Drivers.Dto;
using TACHYON.Shipping.ShippingRequestTrips;
using TACHYON.Shipping.Trips;
using TACHYON.Shipping.Trips.RejectReasons.Dtos;
using TACHYON.Tracking.Dto;
using TACHYON.Trucks.TrucksTypes.Dtos;

namespace TACHYON.Tracking
{
    //to do add permission for best practice 
    [AbpAuthorize()]
    public class TrackingAppService : TACHYONAppServiceBase, ITrackingAppService
    {
        private readonly IRepository<ShippingRequestTrip> _ShippingRequestTripRepository;
        private readonly IRepository<RoutPoint, long> _routPointRepository;
        private readonly IRepository<ShippingRequestTrip> _shippingRequestTrip;

        private readonly ShippingRequestsTripManager _shippingRequestsTripManager;

        public TrackingAppService(IRepository<ShippingRequestTrip> shippingRequestTripRepository, IRepository<RoutPoint, long> routPointRepository, ShippingRequestsTripManager shippingRequestsTripManager, IRepository<ShippingRequestTrip> shippingRequestTrip)
        {
            _ShippingRequestTripRepository = shippingRequestTripRepository;
            _routPointRepository = routPointRepository;
            _shippingRequestsTripManager = shippingRequestsTripManager;
            _shippingRequestTrip = shippingRequestTrip;
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

        public async Task<ListResultDto<ShippingRequestTripDriverRoutePointDto>> GetForView(long id)
        {
            CheckIfCanAccessService(true, AppFeatures.TachyonDealer, AppFeatures.Carrier, AppFeatures.Shipper);
            DisableTenancyFilters();
            var routes = await _routPointRepository.GetAll()
            .Include(r => r.FacilityFk)
            .Include(r => r.GoodsDetails)
             .ThenInclude(c => c.GoodCategoryFk)
              .ThenInclude(t => t.Translations)
            .Include(g => g.GoodsDetails)
             .ThenInclude(u => u.UnitOfMeasureFk)
                            .Where(x => x.ShippingRequestTripFk.Id == id && x.ShippingRequestTripFk.ShippingRequestFk.CarrierTenantId.HasValue)
                            .WhereIf(AbpSession.TenantId.HasValue && await IsEnabledAsync(AppFeatures.Shipper), x => x.ShippingRequestTripFk.ShippingRequestFk.TenantId == AbpSession.TenantId)
                            .WhereIf(!AbpSession.TenantId.HasValue || await IsEnabledAsync(AppFeatures.TachyonDealer), x => true)
                            .WhereIf(AbpSession.TenantId.HasValue && await IsEnabledAsync(AppFeatures.Carrier), x => x.ShippingRequestTripFk.ShippingRequestFk.CarrierTenantId == AbpSession.TenantId)
            .ToListAsync();
            if (routes.IsNullOrEmpty()) throw new UserFriendlyException(L("TheTripIsNotFound"));

            if (AbpSession.TenantId.HasValue || !await IsGrantedAsync(AppPermissions.Pages_Tracking_ReceiverCode))
            {
                return new ListResultDto<ShippingRequestTripDriverRoutePointDto>()
                {
                    Items = ObjectMapper.Map<List<ShippingRequestTripDriverRoutePointDto>>(routes)
                };
            }

            var items = new List<ShippingRequestTripDriverRoutePointDto>();
            foreach (RoutPoint point in routes)
            {
                var dto = ObjectMapper.Map<ShippingRequestTripDriverRoutePointDto>(point);
                dto.ReceiverCode = point.Code;
                items.Add(dto);
            }
            return new ListResultDto<ShippingRequestTripDriverRoutePointDto>(items);
        }


        public async Task Accept(int id)
        {
            CheckIfCanAccessService(true, AppFeatures.TachyonDealer, AppFeatures.Carrier);
            await _shippingRequestsTripManager.Accepted(id);
        }

        public async Task Start(int id)
        {
            CheckIfCanAccessService(true, AppFeatures.TachyonDealer, AppFeatures.Carrier);
            await _shippingRequestsTripManager.Start(new ShippingRequestTripDriverStartInputDto { Id = id });
        }

        public async Task ChangeStatus(int id)
        {
            CheckIfCanAccessService(true, AppFeatures.TachyonDealer, AppFeatures.Carrier);
            await _shippingRequestsTripManager.ChangeStatus(id);
        }

        public async Task NextLocation(long id)
        {
            CheckIfCanAccessService(true, AppFeatures.TachyonDealer, AppFeatures.Carrier);
            await _shippingRequestsTripManager.GotoNextLocation(id);
        }

        public async Task ConfirmReceiverCode(ConfirmReceiverCodeInput input)
        {
            CheckIfCanAccessService(true, AppFeatures.TachyonDealer, AppFeatures.Carrier);
            await _shippingRequestsTripManager.ConfirmReceiverCode(input);
        }

        public async Task<FileDto> POD(long id)
        {
            CheckIfCanAccessService(true, AppFeatures.TachyonDealer, AppFeatures.Carrier, AppFeatures.Shipper);
            return await _shippingRequestsTripManager.GetPOD(id);
        }
        #region Helper
        private TrackingListDto GetMap(ShippingRequestTrip trip)
        {
            var dto = ObjectMapper.Map<TrackingListDto>(trip);
            if (trip.AssignedTruckFk != null) dto.TruckType = ObjectMapper.Map<TrucksTypeDto>(trip.AssignedTruckFk.TrucksTypeFk)?.TranslatedDisplayName ?? "";
            dto.GoodsCategory = ObjectMapper.Map<GoodCategoryDto>(trip.ShippingRequestFk.GoodCategoryFk)?.DisplayName;
            if (trip.ShippingRequestTripRejectReason != null)
            {
                dto.Reason = ObjectMapper.Map<ShippingRequestTripRejectReasonListDto>(trip.ShippingRequestTripRejectReason).Name ?? "";
            }
            if (AbpSession.TenantId.HasValue && !IsEnabled(AppFeatures.TachyonDealer))
            {
                if (!IsEnabled(AppFeatures.Shipper))
                {
                    dto.Name = trip.ShippingRequestFk.Tenant.Name;
                    dto.IsAssign = true;
                    dto.CanStartTrip = CanStartTrip(trip);
                }

            }
            else
            {
                if (trip.ShippingRequestFk.IsTachyonDeal)
                {
                    dto.CanStartTrip = CanStartTrip(trip);
                    dto.IsAssign = true;
                }
                dto.Name = $"{trip.ShippingRequestFk?.Tenant?.Name}-{trip.ShippingRequestFk?.CarrierTenantFk?.Name}";
            }
            return dto;
        }
        #endregion
        private bool CanStartTrip(ShippingRequestTrip trip)
        {
            if (trip.Status == ShippingRequestTripStatus.Intransit || !trip.AssignedDriverUserId.HasValue)
            {
                return false;
            }
            else if (trip.StartTripDate.Date <= Clock.Now.Date && trip.Status == ShippingRequestTripStatus.New)
            {

                //Check there any trip the driver still working on or not
                var Count = _shippingRequestTrip.GetAll()
                    .Where(x => x.AssignedDriverUserId == trip.AssignedDriverUserId && x.Status == ShippingRequestTripStatus.Intransit).Count();
                if (Count == 0)
                    return true;
            }

            return false;
        }
    }
}