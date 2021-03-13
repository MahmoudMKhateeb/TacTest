using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Linq.Extensions;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using TACHYON.Authorization;
using TACHYON.Features;
using TACHYON.Goods.GoodsDetails;
using TACHYON.Routs.RoutPoints;
using TACHYON.Shipping.ShippingRequests;
using TACHYON.Shipping.ShippingRequestTrips;
using TACHYON.Shipping.Trips.Dto;
using TACHYON.ShippingRequestTripVases;

namespace TACHYON.Shipping.Trips
{
    [AbpAuthorize(AppPermissions.Pages_ShippingRequestTrips)]
    public class ShippingRequestsTripAppService : TACHYONAppServiceBase, IShippingRequestsTripAppService
    {
        private readonly IRepository<ShippingRequestTrip> _ShippingRequestTripRepository;
        private readonly IRepository<ShippingRequest, long> _ShippingRequestRepository;
        private readonly IRepository<RoutPoint, long> _RoutPointRepository;
        private readonly IRepository<ShippingRequestTripVas, long> _ShippingRequestTripVasRepository;
        private readonly IRepository<GoodsDetail,long> _GoodsDetailRepository;
        
        public ShippingRequestsTripAppService(
            IRepository<ShippingRequestTrip> ShippingRequestTripRepository,
            IRepository<ShippingRequest, long> ShippingRequestRepository,
            IRepository<RoutPoint, long> RoutPointRepository,
            IRepository<ShippingRequestTripVas, long> ShippingRequestTripVasRepository,
            IRepository<GoodsDetail, long> GoodsDetailRepository)
        {
            _ShippingRequestTripRepository = ShippingRequestTripRepository;
            _ShippingRequestRepository = ShippingRequestRepository;
            _RoutPointRepository = RoutPointRepository;
            _ShippingRequestTripVasRepository = ShippingRequestTripVasRepository;
            _GoodsDetailRepository = GoodsDetailRepository;
        }

        public async Task<PagedResultDto<ShippingRequestsTripListDto>> GetAll(ShippingRequestTripFilterInput Input)
        {
            var request = await GetShippingRequestByPermission(Input.RequestId);
            var query = _ShippingRequestTripRepository
    .GetAll()
    .AsNoTracking()
                .Include(x=>x.OriginFacilityFk)
                .Include(x=>x.DestinationFacilityFk)    
                .Include(x => x.AssignedTruckFk)
                .Include(x => x.RoutPoints)
                   .ThenInclude(r => r.FacilityFk)
                .Include(x => x.RoutPoints)
                   .ThenInclude(r => r.GoodsDetails)
                .Include(x => x.ShippingRequestTripVases)
                .Where(x => x.ShippingRequestId == request.Id)
    .WhereIf(Input.Status.HasValue, e => e.Status == Input.Status)
    .OrderBy(Input.Sorting ?? "Status asc")
    .PageBy(Input);

            var totalCount = await query.CountAsync();
            return new PagedResultDto<ShippingRequestsTripListDto>(
                totalCount,
                ObjectMapper.Map<List<ShippingRequestsTripListDto>>(await query.ToListAsync())

            );
        }

        public async Task<ShippingRequestsTripForViewDto> GetShippingRequestTripForView(int id)
        {

            return await GetShippingRequestTripForMapper<ShippingRequestsTripForViewDto>(id);
        }


        public async Task<CreateOrEditShippingRequestTripDto> GetShippingRequestTripForEdit(EntityDto input)
        {
            return await GetShippingRequestTripForMapper<CreateOrEditShippingRequestTripDto>(input.Id);

        }

        public async Task CreateOrEdit(CreateOrEditShippingRequestTripDto input)
        {
            var request = await GetShippingRequestByPermission(input.ShippingRequestId);

            if (
                input.StartTripDate.Date > request.EndTripDate?.Date ||
                input.StartTripDate.Date < request.StartTripDate?.Date ||
                input.EndTripDate.Date > request.EndTripDate?.Date ||
                input.EndTripDate.Date < request.StartTripDate?.Date
                )
            {

                throw new UserFriendlyException(L("The trip date range must between shipping request range date"));

            }

            int requestNumberOfDrops = request.NumberOfDrops;

            if (input.RoutPoints.Count(x => x.PickingType == PickingType.Dropoff) != requestNumberOfDrops)
            {
                throw new UserFriendlyException(L("The number of drop points must be" + requestNumberOfDrops));
            }

            if (!input.Id.HasValue)
            {
               await Create(input);
            }
            else
            {

               await Update(input);
            }

        }

        [AbpAuthorize(AppPermissions.Pages_ShippingRequestTrips_Create)]
        private async Task Create(CreateOrEditShippingRequestTripDto input)
        {
            var RoutePoint = input.RoutPoints.OrderBy(x=>x.PickingType);
            ShippingRequestTrip trip = ObjectMapper.Map<ShippingRequestTrip>(input);
         
            await _ShippingRequestTripRepository.InsertAsync(trip);
        }

        [AbpAuthorize(AppPermissions.Pages_ShippingRequestTrips_Edit)]
        private async Task Update(CreateOrEditShippingRequestTripDto input)
        {
            var trip = await GetTrip((int)input.Id, input.ShippingRequestId);
            TripCanEditOrDelete(trip);



            foreach (var point in trip.RoutPoints)
            {
                if (!input.RoutPoints.Any(x => x.Id == point.Id))
                {
                    await _RoutPointRepository.DeleteAsync(point);
                }
                foreach (var g in point.GoodsDetails.Where(x => x.Id != 0))
                {
                    if (!input.RoutPoints.Any(x => x.GoodsDetailListDto.Any(d => d.Id == g.Id)))
                    {
                        await _GoodsDetailRepository.DeleteAsync(g);
                    }
                }
            }

            foreach (var vas in trip.ShippingRequestTripVases)
            {
                if (!input.ShippingRequestTripVases.Any(x => x.Id == vas.Id))
                {
                    await _ShippingRequestTripVasRepository.DeleteAsync(vas);
                }
            }

            ObjectMapper.Map(input, trip);




        }

        [AbpAuthorize(AppPermissions.Pages_ShippingRequestTrips_Delete)]
        public async Task Delete(EntityDto input)
        {
            
            var trip = await _ShippingRequestTripRepository.
                FirstOrDefaultAsync(
                x => x.Id == input.Id &&
                x.Status == ShippingRequestTripStatus.StandBy);

         

            if (trip != null)
            {
                var Request = await GetShippingRequestByPermission(trip.ShippingRequestId);
                TripCanEditOrDelete(trip);
                await _ShippingRequestTripRepository.DeleteAsync(trip);
            }
        }


        #region Heleper
        private void TripCanEditOrDelete(ShippingRequestTrip trip)
        {
            // When Edit Or Delete
            if (trip.Status != ShippingRequestTripStatus.StandBy)
            {
                throw new UserFriendlyException(L("CanNotEditOrDeleteTrip"));

            }

            
        }
        private async Task<ShippingRequestTrip> GetTrip(int tripid, long? RequestId = null)
        {

            var trip = await _ShippingRequestTripRepository
                .GetAll()
                .Include(x=>x.OriginFacilityFk)
                .Include(x => x.DestinationFacilityFk)
                .Include(x => x.AssignedTruckFk)
                .Include(x => x.RoutPoints)
                   .ThenInclude(r => r.FacilityFk)
                .Include(x => x.RoutPoints)
                   .ThenInclude(r => r.GoodsDetails)
                .Include(x => x.ShippingRequestTripVases)
                .WhereIf(RequestId.HasValue, x => x.ShippingRequestId == RequestId)
                 .FirstOrDefaultAsync(x => x.Id == tripid);
            if (trip == null) throw new UserFriendlyException(L("ShippingRequestTripIsNotFound"));
            return trip;
        }


        private async Task<ShippingRequest> GetShippingRequestByPermission(long ShippingRequestId)
        {
            if (await IsEnabledAsync(AppFeatures.TachyonDealer) || await IsEnabledAsync(AppFeatures.Carrier))
            {
                using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MustHaveTenant))
                {
                    return await GetShippingRequest(ShippingRequestId);
                }
            }

            return await GetShippingRequest(ShippingRequestId);
        }


        private async Task<ShippingRequest> GetShippingRequest(long ShippingRequestId)
        {

            var request = await _ShippingRequestRepository
                .FirstOrDefaultAsync(x => x.Id == ShippingRequestId && (!IsEnabled(AppFeatures.Carrier) || (IsEnabled(AppFeatures.Carrier) && x.CarrierTenantId == AbpSession.TenantId)));
            if (request == null)
            {
                throw new UserFriendlyException(L("ShippingRequestIsNotFound"));
            }
            return request;
        }



        private async Task<T> GetShippingRequestTripForMapper<T>(int id)
        {
            var trip = await GetTrip(id);
            if (trip != null)
            {
                 await GetShippingRequestByPermission(trip.ShippingRequestId);
            }

            return ObjectMapper.Map<T>(trip);
        }
        #endregion
    }
}
