using Abp.Application.Services.Dto;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using TACHYON.Cities.Dtos;
using TACHYON.Features;
using TACHYON.Goods.GoodCategories.Dtos;
using TACHYON.Shipping.ShippingRequestTrips;
using TACHYON.Shipping.Trips.RejectReasons.Dtos;
using TACHYON.Tracking.Dto;
using TACHYON.Trucks.TrucksTypes.Dtos;

namespace TACHYON.Tracking
{
    public class TrackingAppService : TACHYONAppServiceBase, ITrackingAppService
    {
        private readonly IRepository<ShippingRequestTrip> _ShippingRequestTripRepository;

        public TrackingAppService(IRepository<ShippingRequestTrip> shippingRequestTripRepository)
        {
            _ShippingRequestTripRepository = shippingRequestTripRepository;
        }

        public async Task<PagedResultDto<TrackingListDto>> GetAll(TrackingSearchInputDto input)
        {
            DisableTenancyFilters();
            var query = _ShippingRequestTripRepository
        .GetAll()
        .AsNoTracking()
            .Include(x => x.OriginFacilityFk)
             .ThenInclude(c=>c.CityFk)
            .Include(x => x.DestinationFacilityFk)
            .Include(x => x.AssignedTruckFk)
             .ThenInclude(t=>t.TrucksTypeFk)
               .ThenInclude(t => t.Translations)
            .Include(x => x.AssignedDriverUserFk)
            .Include(x => x.ShippingRequestTripRejectReason)
                .ThenInclude(t => t.Translations)
            .Include(r=>r.ShippingRequestFk)
              .ThenInclude(c=>c.GoodCategoryFk)
                .ThenInclude(t=>t.Translations)
            .Include(r => r.ShippingRequestFk)
             .ThenInclude(s=>s.Tenant)
            .Include(r => r.ShippingRequestFk)
             .ThenInclude(c => c.CarrierTenantFk)
                            .Where(x => x.ShippingRequestFk.Status == Shipping.ShippingRequests.ShippingRequestStatus.PostPrice || x.ShippingRequestFk.Status == Shipping.ShippingRequests.ShippingRequestStatus.Completed || x.ShippingRequestFk.Status == Shipping.ShippingRequests.ShippingRequestStatus.Cancled)
                            .WhereIf(AbpSession.TenantId.HasValue && await IsEnabledAsync(AppFeatures.Shipper), x => x.ShippingRequestFk.TenantId == AbpSession.TenantId)
                            .WhereIf(!AbpSession.TenantId.HasValue || await IsEnabledAsync(AppFeatures.TachyonDealer), x => true)
                            .WhereIf(AbpSession.TenantId.HasValue && await IsEnabledAsync(AppFeatures.Carrier), x => x.ShippingRequestFk.CarrierTenantId == AbpSession.TenantId)
                            .WhereIf(input.PickupFromDate.HasValue && input.PickupToDate.HasValue, x => x.ShippingRequestFk.StartTripDate >= input.PickupFromDate.Value && x.ShippingRequestFk.StartTripDate <= input.PickupToDate.Value)
                            .WhereIf(input.FromDate.HasValue && input.ToDate.HasValue, x =>  x.StartTripDate >= input.FromDate.Value && x.StartTripDate <= input.ToDate.Value)
                            .WhereIf(input.OriginId.HasValue, x => x.ShippingRequestFk.OriginCityId == input.OriginId)
                            .WhereIf(input.DestinationId.HasValue, x => x.ShippingRequestFk.DestinationCityId == input.DestinationId)
                            .WhereIf(input.RouteTypeId.HasValue, x => x.ShippingRequestFk.RouteTypeId == input.RouteTypeId)
                            .WhereIf(input.TruckTypeId.HasValue, x => x.ShippingRequestFk.TrucksTypeId == input.TruckTypeId)
                            .WhereIf(input.Status.HasValue, x => x.Status ==  input.Status)
                            .WhereIf(!string.IsNullOrEmpty(input.Shipper), x => x.ShippingRequestFk.Tenant.Name.ToLower().Contains(input.Shipper) || x.ShippingRequestFk.Tenant.companyName.ToLower().Contains(input.Shipper) || x.ShippingRequestFk.Tenant.TenancyName.ToLower().Contains(input.Shipper))
                            .WhereIf(!string.IsNullOrEmpty(input.Carrier), x => x.ShippingRequestFk.CarrierTenantFk.Name.ToLower().Contains(input.Carrier) || x.ShippingRequestFk.CarrierTenantFk.companyName.ToLower().Contains(input.Carrier) || x.ShippingRequestFk.CarrierTenantFk.TenancyName.ToLower().Contains(input.Carrier))
                            .OrderBy(input.Sorting ?? "Status desc,id asc")
                            .PageBy(input).ToList();

            List<TrackingListDto> trackingLists = new List<TrackingListDto>();
            query.ForEach(r =>
            {
                var dto = ObjectMapper.Map<TrackingListDto>(r);
                  if (r.AssignedTruckFk !=null)  dto.TruckType = ObjectMapper.Map<TrucksTypeDto>(r.AssignedTruckFk.TrucksTypeFk)?.TranslatedDisplayName ?? "" ;
                    dto.GoodsCategory = ObjectMapper.Map<GoodCategoryDto>(r.ShippingRequestFk.GoodCategoryFk)?.DisplayName;
                if (r.ShippingRequestTripRejectReason != null)
                {
                    dto.Reason = ObjectMapper.Map<ShippingRequestTripRejectReasonListDto>(r.ShippingRequestTripRejectReason).Name ?? "";
                }
                if (AbpSession.TenantId.HasValue && !  IsEnabled(AppFeatures.TachyonDealer))
                {
                    if (!IsEnabled(AppFeatures.Shipper))
                    {
                        dto.Name = r.ShippingRequestFk.Tenant.Name;
                    }

                }
                else
                {
                    dto.Name = $"{r.ShippingRequestFk.Tenant.Name}-{r.ShippingRequestFk.CarrierTenantFk.Name}";
                }
                trackingLists.Add(dto);
            });

            return new PagedResultDto<TrackingListDto>(
                query.Count,
               trackingLists

            );
        }
    }
}
