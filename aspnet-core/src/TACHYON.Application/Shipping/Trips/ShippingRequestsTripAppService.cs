using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq.Dynamic.Core;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TACHYON.Features;
using TACHYON.Shipping.ShippingRequests;
using TACHYON.Shipping.ShippingRequestTrips;
using TACHYON.Shipping.Trips.Dto;
using Abp.Collections.Extensions;
using Abp.Linq.Extensions;

namespace TACHYON.Shipping.Trips
{
    public class ShippingRequestsTripAppService : TACHYONAppServiceBase, IShippingRequestsTripAppService
    {
        private readonly IRepository<ShippingRequestTrip> _ShippingRequestTripRepository;
        private readonly IRepository<ShippingRequest, long> _ShippingRequestRepository;


        public ShippingRequestsTripAppService(
            IRepository<ShippingRequestTrip> ShippingRequestTripRepository,
            IRepository<ShippingRequest, long> ShippingRequestRepository)
        {
            _ShippingRequestTripRepository = ShippingRequestTripRepository;
            _ShippingRequestRepository = ShippingRequestRepository;
        }

        public async Task<PagedResultDto<ShippingRequestsTripListDto>> GetAll(ShippingRequestTripFilterInput Input)
        {
            var request = await GetShippingRequestByPermission(Input.RequestId);
            var query = _ShippingRequestTripRepository
    .GetAll()
    .AsNoTracking()
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
                ObjectMapper.Map<List<ShippingRequestsTripListDto>>(query)

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
            if (input.Id == 0)
            {
                Create(input);
            }
            else
            {

                Update(input);
            }

        }


        private async void Create(CreateOrEditShippingRequestTripDto input)
        {
            ShippingRequestTrip trip = ObjectMapper.Map<ShippingRequestTrip>(input);
            TripValidationForEditOrDelete(trip);
            await _ShippingRequestTripRepository.InsertAsync(trip);
        }
        private async void Update(CreateOrEditShippingRequestTripDto input)
        {
            var trip = await GetTrip((int)input.Id, input.ShippingRequestId);

            ObjectMapper.Map(input, trip);         
        }
        public async Task Delete(EntityDto input)
        {
            var trip = await _ShippingRequestTripRepository.
                FirstOrDefaultAsync(
                x => x.Id == input.Id &&
                x.Status != ShippingRequestTripStatus.StandBy && x.ShippingRequestFk.TenantId == AbpSession.TenantId);

            if (trip != null)
            {
                TripValidationForEditOrDelete(trip);
                await _ShippingRequestTripRepository.DeleteAsync(input.Id);
            }
        }


        #region Heleper
        private void TripValidationForEditOrDelete(ShippingRequestTrip trip)
        {
            // When Edit Or Delete
            if (trip.Status != ShippingRequestTripStatus.StandBy)
            {
                throw new UserFriendlyException(L("CanNotEditOrDeleteTrip"));

            }
        }
        private async Task<ShippingRequestTrip> GetTrip(int tripid,long? RequestId=null)
        {
            var trip = await _ShippingRequestTripRepository
                .GetAll()
                .AsNoTracking()
                .Include(x => x.AssignedTruckFk)
                .Include(x => x.RoutPoints)
                   .ThenInclude(r => r.FacilityFk)
                .Include(x => x.RoutPoints)
                   .ThenInclude(r => r.GoodsDetails)
                .Include(x => x.ShippingRequestTripVases)
                .WhereIf(RequestId.HasValue,x=>x.ShippingRequestId== RequestId)
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
                    return await GetShippingReques(ShippingRequestId);
                }
            }

            return await GetShippingReques(ShippingRequestId);
        }


        private async Task<ShippingRequest> GetShippingReques(long ShippingRequestId)
        {

            var request = await _ShippingRequestRepository.FirstOrDefaultAsync(x => x.Id == ShippingRequestId && (!IsEnabled(AppFeatures.Carrier) || (IsEnabled(AppFeatures.Carrier) && x.CarrierTenantId == AbpSession.TenantId)));
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
                ShippingRequest shippingRequest = await GetShippingRequestByPermission(trip.ShippingRequestId);
            }

            return ObjectMapper.Map<T>(trip);
        }
        #endregion
    }
}
