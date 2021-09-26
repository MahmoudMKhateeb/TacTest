using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TACHYON.Rating
{
    public class RatingLogManager : TACHYONDomainServiceBase
    {
        private readonly IRepository<RatingLog, long> _ratingLogRepository;

        public RatingLogManager(IRepository<RatingLog, long> ratingLogRepository)
        {
            _ratingLogRepository = ratingLogRepository;
        }

        public async Task<List<RatingLog>> GetAllCarriersRatingAsync(int? CarrierId)
        {
            return await _ratingLogRepository
                .GetAll().Where(x => x.RateType == RateType.CarrierTripBySystem)
                .WhereIf(CarrierId!=null, x=>x.CarrierId==CarrierId)
                .ToListAsync();
        }

        public async Task<List<RatingLog>> GetAllShippersRatingAsync(int? ShipperId)
        {
            return await _ratingLogRepository
                .GetAll().Where(x => x.RateType == RateType.ShipperTripBySystem)
                .WhereIf(ShipperId != null, x => x.ShipperId == ShipperId)
                .ToListAsync();
        }

        public async Task<int> GetShipperRatingCountAsync(int ShipperId)
        {
            return await _ratingLogRepository
                .GetAll().Where(x => x.RateType == RateType.ShipperTripBySystem)
                .Where(x => x.ShipperId == ShipperId)
                .CountAsync();
        }

        public async Task<int> GetFacilityRatingCountAsync(long facilityId)
        {
            return await _ratingLogRepository
                .GetAll().Where(x => x.RateType == RateType.FacilityByDriver)
                .Where(x => x.FacilityId == facilityId)
                .CountAsync();
        }
    }
}
