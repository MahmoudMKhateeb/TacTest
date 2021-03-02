using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Abp.Timing;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using TACHYON.Shipping.Drivers.Dto;
using TACHYON.Shipping.ShippingRequestTrips;
using TACHYON.Shipping.TripStatuses;
using TACHYON.Storage;

namespace TACHYON.Shipping.Drivers
{
    public class ShippingRequestDriverAppService : TACHYONAppServiceBase, IShippingRequestDriverAppService
    {
        private readonly IRepository<ShippingRequestTrip> _ShippingRequestTrip;
        private readonly IBinaryObjectManager _BinaryObjectManager;
        public ShippingRequestDriverAppService(
            IRepository<ShippingRequestTrip> ShippingRequestTrip,
            IBinaryObjectManager BinaryObjectManager)
        {
            _ShippingRequestTrip = ShippingRequestTrip;
            _BinaryObjectManager = BinaryObjectManager;
        }
        public async Task<PagedResultDto<ShippingRequestTripDriverListDto>> GetAll(ShippingRequestTripDriverFilterInput input)
        {
            var query = _ShippingRequestTrip
                .GetAll()
                .Include(i => i.ShippingRequestFk)
                   .ThenInclude(r => r.RouteFk)
                   .ThenInclude(r => r.DestinationCityFk)
               .Include(i => i.ShippingRequestFk)
                   .ThenInclude(r => r.RouteFk)
                   .ThenInclude(r => r.OriginCityFk)
               .Include(i => i.ShippingRequestFk)
                   .ThenInclude(r => r.RouteFk)
                   .ThenInclude(r => r.DestinationFacilityFk)
               .Include(i => i.ShippingRequestFk)
                   .ThenInclude(r => r.RouteFk)
                   .ThenInclude(r => r.OriginFacilityFk)
                   .Where(t => t.AssignedDriverUserId == AbpSession.UserId)
                .WhereIf(input.Status.HasValue, e => (ShippingRequestTripStatus)e.TripStatusId == input.Status)
                .AsNoTracking()
                .OrderBy(input.Sorting ?? "TripStatusId asc")
                .PageBy(input);

            var totalCount = await query.CountAsync();

            return new PagedResultDto<ShippingRequestTripDriverListDto>(
                totalCount,
                ObjectMapper.Map<List<ShippingRequestTripDriverListDto>>(query)
            );
        }

        //public  async Task<ShippingRequestTripDriverListDto> Get(long TripId)
        //{
        //    var query = await _ShippingRequestTrip.GetAll()
        //        .Include(s=>s.ShippingRequestFk)
        //        .ThenInclude(rt=>rt.RoutPoints)
        //        .SingleAsync(t => t.Id == TripId && t.AssignedDriverUserId == AbpSession.UserId);
        //    if (query !=null)
        //    {

        //    }
        //   // return Task.CompletedTask;
        //}
        public async void StartTrip(long TripId)
        {
            var trip = await _ShippingRequestTrip
                .SingleAsync(t => t.Id == TripId && t.AssignedDriverUserId == AbpSession.UserId && (ShippingRequestTripStatus) t.TripStatusId!= ShippingRequestTripStatus.offloading && t.ShippingRequestFk.StartTripDate <= Clock.Now);
            if (trip !=null)
            {
               if (_ShippingRequestTrip.GetAll().Any(x=>x.Id != trip.Id && (ShippingRequestTripStatus)x.TripStatusId != ShippingRequestTripStatus.offloading  && x.AssignedDriverUserId == AbpSession.UserId))
                {
                    trip.TripStatusId =(int)ShippingRequestTripStatus.Startloading;
                    trip.StartTripDate = Clock.Now;
                }

            }
        }

        public async Task<bool> ConfirmReceiving(string Code)
        {
            return await GetActiveTrip() !=null;
        }

        public async Task<bool> UploadDeliveryDocument(ShippingRequestTripDriverDocumentDto Input)
        {
            if (Input == null || string.IsNullOrEmpty(Input.DocumentBase64)) return false;      
            var trip = await GetActiveTrip();
            if (trip == null) return false;

            var fileBytes = Convert.FromBase64String(Input.DocumentBase64.Split(',')[1]);
            var fileObject = new BinaryObject(AbpSession.TenantId, fileBytes);
            await _BinaryObjectManager.SaveAsync(fileObject);

            trip.EndTripDate = Clock.Now;
            trip.TripStatusId = (int)ShippingRequestTripStatus.offloading;
            trip.DocumentContentType = Input.DocumentContentType;
            trip.DocumentName = Input.DocumentName;
            trip.DocumentId = fileObject.Id;
            return true;
        }


        #region Heleper
        private async Task<ShippingRequestTrip> GetActiveTrip()
        {
            return await _ShippingRequestTrip
                            .SingleAsync(t => t.AssignedDriverUserId == AbpSession.UserId && (ShippingRequestTripStatus)t.TripStatusId != ShippingRequestTripStatus.offloading);
        }
        #endregion
    }
}
