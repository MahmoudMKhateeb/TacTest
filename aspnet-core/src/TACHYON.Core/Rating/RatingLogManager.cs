using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Linq.Extensions;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TACHYON.AddressBook;
using TACHYON.Authorization.Users;
using TACHYON.Configuration;
using TACHYON.Extension;
using TACHYON.MultiTenancy;
using TACHYON.Routs.RoutPoints;
using TACHYON.Shipping.ShippingRequestTrips;

namespace TACHYON.Rating
{
    public class RatingLogManager : TACHYONDomainServiceBase
    {
        private readonly IRepository<RatingLog, long> _ratingLogRepository;
        private readonly IRepository<RoutPoint, long> _routePointRepository;
        private readonly IRepository<ShippingRequestTrip> _tripRepository;
        private readonly UserManager _userManager;
        private readonly TenantManager _tenantManager;
        private readonly IRepository<Facility, long> _facilityRepository;

        public RatingLogManager(
            IRepository<RatingLog, long> ratingLogRepository,
            IRepository<RoutPoint, long> routePointRepository,
            UserManager userManager,
            TenantManager tenantManager,
            IRepository<Facility, long> facilityRepository,
            IRepository<ShippingRequestTrip> tripRepository)
        {
            _ratingLogRepository = ratingLogRepository;
            _routePointRepository = routePointRepository;
            _userManager = userManager;
            _tenantManager = tenantManager;
            _facilityRepository = facilityRepository;
            _tripRepository = tripRepository;
        }

        public async Task CreateRating(RatingLog ratingLog)
        {
            if (ratingLog.PointId != null) await CheckIfPointCompleted(ratingLog);

            if (await IsRateDoneBefore(ratingLog)) throw new UserFriendlyException(L("RateDoneBefore"));
            await _ratingLogRepository.InsertAndGetIdAsync(ratingLog);

            await ReCalculateTenantOrEntityRating(ratingLog);
        }

        public async Task DeleteAllTripAndPointsRatingAsync(RatingLog rate)
        {
            if (!rate.TripId.HasValue && !rate.PointId.HasValue) return;
            var ratePoints = rate.TripFk.RoutPoints.Select(y => y.Id).ToList();

            if (rate.TripId.HasValue)
            {
                await _ratingLogRepository.DeleteAsync(x =>
                    x.TripId == rate.TripId || ratePoints.Contains(x.PointId.Value));

                if (!rate.PointId.HasValue) return;
            }

            await _ratingLogRepository.DeleteAsync(x => x.PointId ==
                rate.PointId || x.TripId == rate.RoutePointFk.ShippingRequestTripId);
        }

        #region Recalculating

        private async Task ReCalculateTenantOrEntityRating(RatingLog rate)
        {
            //recalculate Entity rating (Driver/Facility)
            if (rate.RateType == RateType.DriverByReceiver && rate.DriverId.HasValue)
                await RecalculateRatingById(rate.DriverId.Value, typeof(User));

            if (rate.RateType == RateType.FacilityByDriver && rate.FacilityId.HasValue)
                await RecalculateRatingById(rate.FacilityId.Value, typeof(Facility));

            // Recalculate Tenant Rating (Shipper/Carrier)
            if (rate.RateType <= RateType.CarrierByShipper)
            {
                await ReCalculateTenantRating(rate, RateType.CarrierTripBySystem);
                return;
            }

            await ReCalculateTenantRating(rate, RateType.ShipperTripBySystem);
        }
        private async Task ReCalculateTenantRating(RatingLog log, RateType tenantType)
        {
            // Here RateType Must Be CarrierTripBySystem or ShipperTripBySystem and not any other values
            if (tenantType < RateType.ShipperTripBySystem) return;

            var tenantTripBySystem = await GetTripRating(log.RateType, log.TripId, log.PointId);

            var tenantId = await GetTenantIdForRate(log);

            // Create If Not Exists
            if (tenantTripBySystem == null)
            {
                var newRate = new RatingLog
                {
                    RateType = log.RateType,
                    TripId = log.TripId,
                    PointId = log.PointId,
                    CarrierId = tenantType == RateType.CarrierTripBySystem ? tenantId : null,
                    ShipperId = tenantType == RateType.ShipperTripBySystem ? tenantId : null,
                    Rate = log.Rate
                };

                await _ratingLogRepository.InsertAndGetIdAsync(newRate);

            }
            else 
                tenantTripBySystem.Rate = await UpdateAndRecalculateTenantTripRating(log);


            if (tenantId.HasValue)
                await RecalculateRatingById(tenantId.Value, typeof(Tenant));
        }

        private async Task<decimal> UpdateAndRecalculateTenantTripRating(RatingLog rate)
        {
            RateType entityRatingType, experienceRatingType, tenantRatingType;
            if (rate.RateType <= RateType.CarrierByShipper)
            {
                entityRatingType = RateType.DriverByReceiver;
                experienceRatingType = RateType.DEByReceiver;
                tenantRatingType = RateType.CarrierByShipper;
            }
            else
            {
                entityRatingType = RateType.FacilityByDriver;
                experienceRatingType = RateType.SEByDriver;
                tenantRatingType = RateType.ShipperByCarrier;
            }

            List<long> tripPoints = await GetPointsIds(rate.TripId, rate.PointId);

            //Get points ratings
            List<Tuple<decimal, RateType>> pointsRatings = await _ratingLogRepository.GetAll()
                .Where(x => x.PointId != null && tripPoints.Contains(x.PointId.Value) &&
                            (x.RateType == entityRatingType || x.RateType == experienceRatingType))
                .Select(x => new Tuple<decimal, RateType>(x.Rate, x.RateType))
                .ToListAsync();

            decimal experienceRatingAverage = pointsRatings
                .GetEntityRatingAverage(x => x.Item2 == experienceRatingType);

            int counter = experienceRatingAverage <= 0 ? 0 : 1;

            decimal entityRatingAverage = pointsRatings
                .GetEntityRatingAverage(x => x.Item2 == entityRatingType);

            counter += entityRatingAverage <= 0 ? 0 : 1;

            decimal tenantTripRating = rate.Rate;

            // if rating log type is not carrier by shipper that's mean
            // we need to get carrier rating from the correct rating log
            if (rate.RateType != tenantRatingType)
            {
                using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MayHaveTenant,AbpDataFilters.MustHaveTenant))
                {
                    var tripId = rate.TripId ?? await _routePointRepository.GetAll().Where(x => x.Id == rate.PointId).Select(x => x.ShippingRequestTripId).FirstOrDefaultAsync();
                    tenantTripRating = await _ratingLogRepository.GetAll().Where(x =>
                            x.RateType == tenantRatingType && x.TripId == tripId)
                        .Select(x => x.Rate)
                        .FirstOrDefaultAsync();
                }
            }

            counter += tenantTripRating <= 0 ? 0 : 1;

            decimal rateSum = experienceRatingAverage + entityRatingAverage + tenantTripRating;
            return rateSum == 0 || counter == 0 ? 0 : Convert.ToDecimal((rateSum / counter).ToString("0.00"));
        }

        public async Task RecalculateRatingById<TRatingEntityId>(TRatingEntityId id, Type type)
            where TRatingEntityId : IComparable
        {
            if (!typeof(IHasRating).IsAssignableFrom(type)) return;
            IHasRating ratingEntity;

            DisableTenancyFilters();

            if (type == typeof(Facility))
                ratingEntity = await _facilityRepository.GetAll().AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id.Equals(id));
            else if (type == typeof(User) && id is long uId)
                ratingEntity = await _userManager.GetUserByIdAsync(uId);
            else if (type == typeof(Tenant) && id is int tId)
                ratingEntity = await _tenantManager.GetByIdAsync(tId);
            else return;

            await RecalculateRating(ratingEntity);
        }

        private async Task RecalculateRating<TRatingEntity>(TRatingEntity ratingEntity)
            where TRatingEntity : IHasRating
        {
            List<decimal> allRatings = ratingEntity switch
            {
                User user => await GetAllRatingAsync(driverId: user.Id),
                Facility facility => await GetAllRatingAsync(RateType.FacilityByDriver, facility.Id),
                Tenant tenant => await GetAllRatingByTenantEdition(tenant.Id, tenant.EditionId),
                _ => null
            };

            if (allRatings == null || allRatings.Count < 10) return;
            ratingEntity.Rate = Convert.ToDecimal((allRatings.Sum() / allRatings.Count).ToString("0.0"));
            ratingEntity.RateNumber = allRatings.Count;
        }

        #endregion

        #region helper

        private async Task<List<decimal>> GetAllRatingAsync(RateType? rateType = null,
            long? facilityId = null,
            int? shipperId = null,
            int? carrierId = null,
            long? driverId = null)
        {
            return await _ratingLogRepository.GetAll().AsNoTracking()
                .WhereIf(rateType.HasValue, x => x.RateType == rateType)
                .WhereIf(facilityId.HasValue, x => x.FacilityId == facilityId)
                .WhereIf(shipperId.HasValue, x => x.ShipperId == shipperId)
                .WhereIf(carrierId.HasValue, x => x.CarrierId == carrierId)
                .WhereIf(driverId.HasValue, x => x.DriverId == driverId)
                .Select(x => x.Rate)
                .ToListAsync();
        }

        public async Task<bool> IsRateDoneBefore(RatingLog rate)
            => await _ratingLogRepository.GetAll().AsNoTracking()
                .AnyAsync(RatingLogEquals(rate));

        private async Task CheckIfPointCompleted(RatingLog log)
        {
            DisableTenancyFilters();
            var isRoutPointCompleted = await _routePointRepository.GetAll().AsNoTracking()
                .WhereIf(log.RateType == RateType.FacilityByDriver,
                    x => x.ShippingRequestTripFk.AssignedDriverUserId == log.DriverId)
                .AnyAsync(x => x.Id == log.PointId && x.IsComplete);

            if (isRoutPointCompleted) return;
            throw new UserFriendlyException(L("PointNotFoundOrNotCompleted"));
        }

        private async Task<int?> GetTenantIdForRate(RatingLog log)
        {
            var type = log.RateType <= RateType.CarrierByShipper
                ? RateType.CarrierTripBySystem
                : RateType.ShipperTripBySystem;

            if (type == log.RateType)
                return type switch
                {
                    // I like to use switch expression here (underscore mean's default) 
                    RateType.ShipperTripBySystem when log.ShipperId.HasValue => log.ShipperId,
                    RateType.CarrierTripBySystem when log.CarrierId.HasValue => log.CarrierId,
                    _ => null
                };


            if (!log.PointId.HasValue && !log.TripId.HasValue) return null;
            return await _tripRepository.GetAll().AsNoTracking()
                .WhereIf(log.TripId.HasValue, x => x.Id == log.TripId)
                .WhereIf(log.PointId.HasValue, x => x.RoutPoints.Any(i => i.Id == log.PointId))
                .Select(x => type == RateType.ShipperTripBySystem
                    ? x.ShippingRequestFk.TenantId
                    : x.ShippingRequestFk.CarrierTenantId)
                .FirstOrDefaultAsync();
        }

        

        private async Task<RatingLog> GetTripRating(RateType type, int? tripId, long? pointId)
        {
            if (tripId == null && pointId == null) return null;
            return await _ratingLogRepository
                .GetAll()
                .WhereIf(tripId != null, x => x.TripId == tripId)
                .WhereIf(pointId != null, x => x.TripId == tripId)
                .FirstOrDefaultAsync(x => x.RateType == type);
        }

        private async Task<List<long>> GetPointsIds(int? tripId, long? pointId = null)
        {
            if (!tripId.HasValue && !pointId.HasValue) return null;
            tripId ??= await _routePointRepository.GetAll().AsNoTracking()
                .Where(x => x.Id == pointId)
                .Select(x => x.ShippingRequestTripId)
                .FirstOrDefaultAsync();

            return await _routePointRepository.GetAll().AsNoTracking()
                .Where(x => x.ShippingRequestTripId == tripId)
                .Select(x => x.Id).ToListAsync();
        }

        private async Task<List<decimal>> GetAllRatingByTenantEdition(int tenantId, int? editionId)
        {
            var shipper = await SettingManager.GetSettingValueAsync(AppSettings.Editions.ShipperEditionId);
            var carrier = await SettingManager.GetSettingValueAsync(AppSettings.Editions.CarrierEditionId);
            if (editionId == int.Parse(shipper))
                return await GetAllRatingAsync(RateType.ShipperTripBySystem, shipperId: tenantId);
            if (editionId == int.Parse(carrier))
                return await GetAllRatingAsync(RateType.CarrierTripBySystem, carrierId: tenantId);

            return null;
        }

        private static Expression<Func<RatingLog, bool>> RatingLogEquals(RatingLog log) =>
            ratingLog => log != null && ratingLog.CarrierId == log.CarrierId &&
                         ratingLog.DriverId == log.DriverId &&
                         ratingLog.ReceiverId == log.ReceiverId && ratingLog.PointId == log.PointId &&
                         ratingLog.ShipperId == log.ShipperId && ratingLog.TripId == log.TripId &&
                         ratingLog.FacilityId == log.FacilityId && ratingLog.RateType == log.RateType;

        #endregion
    }
}