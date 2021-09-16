using Abp.Domain.Repositories;
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

        public async Task<List<RatingLog>> GetAllCarriersRatingAsync()
        {
            return await _ratingLogRepository
                .GetAll().Where(x => x.RateType == RateType.CarrierTripBySystem).ToListAsync();
        }
    }
}
