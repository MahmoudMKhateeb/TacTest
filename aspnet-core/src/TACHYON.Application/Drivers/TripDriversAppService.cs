using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.UI;
using AutoMapper.QueryableExtensions;
using DevExtreme.AspNet.Data.ResponseModel;
using System.Linq;
using System.Threading.Tasks;
using TACHYON.Shipping.ShippingRequestTrips;
using TACHYON.Shipping.Trips.Dto;

namespace TACHYON.Drivers
{


    [AbpAuthorize]
    public class TripDriversAppService : TACHYONAppServiceBase
    {
        private readonly IRepository<TripDriver, long> _tripDriverRepository;

        public TripDriversAppService(IRepository<TripDriver, long> tripDriverRepository)
        {
            _tripDriverRepository = tripDriverRepository;
        }


        /// <summary>
        /// Retrieves a list of trip drivers.
        /// The results include assigned driver and truck details.
        /// </summary>
        /// <param name="filter">A filter used to refine the queryable collection before returning the results.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a LoadResult with the filtered collection of drivers.</returns>
        public async Task<LoadResult> GetAllDX(string filter)
        {
            // Fetch and project to DTO using AutoMapper's ProjectTo
            var queryable = _tripDriverRepository
                .GetAll()
                .OrderByDescending(x => x.CreationTime)
                .ProjectTo<TripDriverForViewDto>(AutoMapperConfigurationProvider);

            return await LoadResultAsync(queryable, filter);
        }

        /// <summary>
        /// Updates the commission percentage for a trip driver.
        /// </summary>
        /// <param name="dto">A TripDriverForViewDto object containing the ID of the trip driver and the new commission percentage.</param>
        /// <exception cref="UserFriendlyException">If the trip driver does not exist.</exception>
        public async Task UpdateTripDriver(TripDriverForViewDto dto)
        {
            var tripDriver = await _tripDriverRepository
            .FirstOrDefaultAsync(x => x.Id == dto.Id);

            if (tripDriver == null) throw new UserFriendlyException(L("NoTripDriver"));

            tripDriver.Commission = dto.Commission;

        }

        /// <summary>
        /// Retrieves a list of assigned drivers to a trip.
        /// The results include assigned driver and truck details.
        /// </summary>
        /// <param name="tripId">The trip id to fetch and project to DTO.</param>
        /// <param name="filter">A filter used to refine the queryable collection before returning the results.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a LoadResult with the filtered collection of drivers.</returns>
        public async Task<LoadResult> GetTripDriversByTripIdDX(long tripId, string filter)
        {
            // Fetch and project to DTO using AutoMapper's ProjectTo
            var queryable = _tripDriverRepository
                .GetAllIncluding(td => td.Driver, td => td.TruckFk)
                .Where(td => td.ShippingRequestTripId == tripId)
                .OrderBy(x => x.CreationTime)
                .ProjectTo<TripDriverForViewDto>(AutoMapperConfigurationProvider);

            return await LoadResultAsync(queryable, filter);
        }

    }
}