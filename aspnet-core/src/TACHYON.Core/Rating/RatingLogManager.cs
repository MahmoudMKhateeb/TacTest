using Abp.Configuration;
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
using TACHYON.Shipping.ShippingRequests;
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
        private readonly SettingManager _settingManager;

        public RatingLogManager(
            IRepository<RatingLog, long> ratingLogRepository,
            IRepository<RoutPoint, long> routePointRepository,
            UserManager userManager,
            TenantManager tenantManager,
            IRepository<Facility, long> facilityRepository,
            IRepository<ShippingRequestTrip> tripRepository,
            SettingManager settingManager)
        {
            _ratingLogRepository = ratingLogRepository;
            _routePointRepository = routePointRepository;
            _userManager = userManager;
            _tenantManager = tenantManager;
            _facilityRepository = facilityRepository;
            _tripRepository = tripRepository;
            _settingManager = settingManager;
        }

        public async Task CreateRating(RatingLog ratingLog)
        {
            if (await IsSaasRating(ratingLog))
                throw new UserFriendlyException(L("CanNotAddRatingForSaasShipment"));

            if (ratingLog.PointId != null) await CheckIfPointCompleted(ratingLog);

            if (await IsRateDoneBefore(ratingLog)) throw new UserFriendlyException(L("RateDoneBefore"));
            await _ratingLogRepository.InsertAndGetIdAsync(ratingLog);

            await ReCalculateTenantOrEntityRating(ratingLog);
        }

        public async Task DeleteAllTripAndPointsRatingAsync(RatingLog rate)
        {
            if (!rate.TripId.HasValue && !rate.PointId.HasValue) return;
            var ratePoints = await _routePointRepository.GetAll()
                .Where(x => x.Id == rate.PointId || x.ShippingRequestTripId == rate.TripId)
                .Select(y => y.Id).ToListAsync();

            if (rate.TripId.HasValue)
            {
                await _ratingLogRepository.DeleteAsync(x =>
                    x.TripId == rate.TripId || ratePoints.Contains(x.PointId.Value));

                if (!rate.PointId.HasValue) return;
            }

            await _ratingLogRepository.DeleteAsync(x => x.PointId ==
                rate.PointId || x.TripId == rate.RoutePointFk.ShippingRequestTripId);
        }


        public async Task<List<RatingLog>> GetAllRatingByUserAsync(RateType? rateType = null,
            int? shipperId = null, int? carrierId = null,
            long? driverId = null)
        {
            return await _ratingLogRepository.GetAll().AsNoTracking()
                .WhereIf(rateType.HasValue, x => x.RateType == rateType)
                .WhereIf(shipperId.HasValue, x => x.ShipperId == shipperId)
                .WhereIf(carrierId.HasValue, x => x.CarrierId == carrierId)
                .WhereIf(driverId.HasValue, x => x.DriverId == driverId)
                .ToListAsync();
        }
        #region Recalculating

        public async Task RecalculateCarrierRating(RatingLog rate)
        {

            var tripId = rate.TripId;
            if (rate.TripId == null)
            {
                tripId = await _routePointRepository.GetAll()
                .Where(x => x.Id == rate.PointId)
                .Select(x => x.ShippingRequestTripId)
                .FirstOrDefaultAsync();
            }

            var CarrierTripBySystem = await _ratingLogRepository
                                .GetAll()
                                .Where(x=>x.TripId==tripId)
                                .FirstOrDefaultAsync(x => x.RateType == RateType.CarrierTripBySystem);

            //recalculate For trip and trip points
            
            var carrierId = rate.CarrierId != null ? rate.CarrierId : rate.PointId != null 
                ? (await GetRequestFromPointAsync(rate.PointId.Value)).CarrierTenantId 
                : (await GetRequestFromTripAsync(rate.TripId.Value)).CarrierTenantId;

            if (CarrierTripBySystem == null)
            {
                //insert
                var newRate = new RatingLog();
                newRate.RateType = RateType.CarrierTripBySystem;
                newRate.TripId = rate.TripId != null ? rate.TripId : rate.RoutePointFk.ShippingRequestTripId;
                newRate.CarrierId = carrierId;
                newRate.Rate = rate.Rate;

                await _ratingLogRepository.InsertAndGetIdAsync(newRate);
            }
            else
            {
                //recalculate and update
                var tripPoints = new List<long>();
                if (rate.TripId != null)
                {
                    //trip points that rated in the system to calculate the trip rate with, in this case the rate is directly to trip
                    tripPoints = rate.TripFk.RoutPoints.Select(x => x.Id).ToList();
                }
                else
                {
                    //in this case the rate is to "point" (pointId not null).. so we will get the other points related to this point trip, to make recalculation
                    tripPoints = _routePointRepository.GetAll().Where(x => x.ShippingRequestTripId == rate.RoutePointFk.ShippingRequestTripId).Select(x => x.Id).ToList();
                }

                //Get points ratings
                var allTripPointsRatings = await _ratingLogRepository.GetAll()
                    .Where(x => x.PointId != null && tripPoints.Contains(x.PointId.Value) &&
                    (x.RateType == RateType.DEByReceiver || x.RateType == RateType.DriverByReceiver))
                    .ToListAsync();

                var DEByReceiver = allTripPointsRatings.Where(x => x.RateType == RateType.DEByReceiver);

                decimal DEByReceiverAvg = default(decimal);
                int counter = 0;
                if (DEByReceiver.Count() > 0)
                {
                    DEByReceiverAvg = DEByReceiver.Sum(x => x.Rate) / DEByReceiver.Count();
                    counter++;
                }

                var DriverByReceiver = allTripPointsRatings.Where(x => x.RateType == RateType.DriverByReceiver);

                decimal DriverByReceiverAvg = default(decimal);

                if (DriverByReceiver.Count() > 0)
                {
                    DriverByReceiverAvg = DriverByReceiver.Sum(x => x.Rate) / DriverByReceiver.Count();
                    counter++;
                }

                //Get Shipper rating for trip
                var CarrierRatingByShipperInCurrentTrip = rate.RateType == RateType.CarrierByShipper ? rate.Rate : 0;
                if (CarrierRatingByShipperInCurrentTrip == 0)
                {
                    var CarrierRatingByShipperInCurrentTripDB = await _ratingLogRepository
                   .FirstOrDefaultAsync(x => x.RateType == RateType.CarrierByShipper &&
                   x.TripId == rate.RoutePointFk.ShippingRequestTripId);

                    if (CarrierRatingByShipperInCurrentTripDB != null)
                    {
                        CarrierRatingByShipperInCurrentTrip = CarrierRatingByShipperInCurrentTripDB.Rate;
                        counter++;
                    }
                }
                else
                {
                    counter = counter + 1;
                }

                CarrierTripBySystem.Rate = Convert.ToDecimal(((DEByReceiverAvg + DriverByReceiverAvg + CarrierRatingByShipperInCurrentTrip) / counter).ToString("0.00"));

            }

            await RecalculateCarrierRatingByCarrierTenantId(carrierId.Value);
        }

        public async Task RecalculateShipperRating(RatingLog rate)
        {
            //recalculate shipper rating
            var ShipperTripBySystem = await _ratingLogRepository
                                .GetAll()
                                .WhereIf(rate.TripId != null, x => x.TripId == rate.TripId)
                                .WhereIf(rate.PointId != null, x => x.TripId == rate.RoutePointFk.ShippingRequestTripId)
                                .FirstOrDefaultAsync(x => x.RateType == RateType.ShipperTripBySystem);

            DisableTenancyFilters();
            rate =await _ratingLogRepository.GetAll()
                .Include(x => x.TripFk)
                .ThenInclude(x => x.RoutPoints)
                .Include(x => x.TripFk)
                .ThenInclude(x => x.ShippingRequestFk)
                .Include(x=>x.RoutePointFk)
                .ThenInclude(x=>x.ShippingRequestTripFk)
                .ThenInclude(x=>x.ShippingRequestFk)
                .FirstOrDefaultAsync(x => x.Id == rate.Id);

            //recalculate For trip and trip points
            var shipperId = rate.ShipperId != null 
                ? rate.ShipperId 
                : rate.PointId != null
                    ? rate.RoutePointFk.ShippingRequestTripFk.ShippingRequestFk.TenantId 
                    : rate.TripFk.ShippingRequestFk.TenantId;
            if (ShipperTripBySystem == null)
            {
                //insert
                var newRate = new RatingLog();
                newRate.RateType = RateType.ShipperTripBySystem;
                newRate.TripId = rate.TripId != null ? rate.TripId : rate.RoutePointFk.ShippingRequestTripId;
                newRate.ShipperId = shipperId;
                newRate.Rate = rate.Rate;

                await _ratingLogRepository.InsertAndGetIdAsync(newRate);
            }
            else
            {
                //recalculate and update
                var tripId = rate.TripId;
                var tripPoints = new List<long>();
                if (tripId != null)
                {
                    //trip points that rated in the system to calculate the trip rate with, in this case the rate is directly to trip
                    tripPoints = _routePointRepository.GetAll().Where(x => x.ShippingRequestTripId == rate.TripId).Select(y=>y.Id).ToList();
                    //tripPoints = rate.TripFk.RoutPoints.Select(x => x.Id).ToList();
                }
                else
                {
                    //in this case the rate is to "point" (pointId not null).. so we will get the other points related to this point trip, to make recalculation
                    tripPoints = _routePointRepository.GetAll().Where(x => x.ShippingRequestTripId == rate.RoutePointFk.ShippingRequestTripId).Select(x => x.Id).ToList();
                }

                //Get points ratings to calculate avg rating of points for each rateType seperatly
                var allFacilityByDriverRatings = await _ratingLogRepository.GetAll()
                    .Where(x => x.PointId != null && tripPoints.Contains(x.PointId.Value) &&
                    (x.RateType == RateType.FacilityByDriver))
                    .ToListAsync();


                decimal allFacilityByDriverRatingsAvg = 0;
                int counter = 0;
                if (allFacilityByDriverRatings.Count() > 0)
                {
                    allFacilityByDriverRatingsAvg = allFacilityByDriverRatings.Sum(x => x.Rate) / allFacilityByDriverRatings.Count();
                    counter++;
                }

                //Get Shipper rating for trip
                //carrier by shipper
                var CarrierRatingByShipperOrDriverInCurrentTripDB = await _ratingLogRepository.GetAll()
                    .WhereIf(rate.TripId != null, x => x.TripId == rate.TripId)
                    .WhereIf(rate.PointId != null, x => x.TripId == rate.RoutePointFk.ShippingRequestTripId)
                   .Where(x => x.RateType == RateType.ShipperByCarrier || x.RateType == RateType.SEByDriver)
                   .ToListAsync();

                var ShipperRatingByCarrierInCurrentTrip = rate.RateType == RateType.ShipperByCarrier ? rate.Rate : 0;
                if (ShipperRatingByCarrierInCurrentTrip == 0)
                {
                    var ShipperRatingByCarrierInCurrentTripDB = CarrierRatingByShipperOrDriverInCurrentTripDB.FirstOrDefault(x => x.RateType == RateType.ShipperByCarrier);

                    if (ShipperRatingByCarrierInCurrentTripDB != null)
                    {
                        ShipperRatingByCarrierInCurrentTrip = ShipperRatingByCarrierInCurrentTripDB.Rate;
                        counter++;
                    }
                }
                else
                {
                    counter++;
                }


                //Shipping Experience by driver
                var SERatingByDriverInCurrentTrip = rate.RateType == RateType.SEByDriver ? rate.Rate : 0;
                if (SERatingByDriverInCurrentTrip == 0)
                {
                    var SERatingByDriverInCurrentTripDB = CarrierRatingByShipperOrDriverInCurrentTripDB
                   .FirstOrDefault(x => x.RateType == RateType.SEByDriver);

                    if (SERatingByDriverInCurrentTripDB != null)
                    {
                        SERatingByDriverInCurrentTrip = SERatingByDriverInCurrentTripDB.Rate;
                        counter++;
                    }
                }
                else
                {
                    counter++;
                }

                ShipperTripBySystem.Rate = (allFacilityByDriverRatingsAvg + ShipperRatingByCarrierInCurrentTrip + SERatingByDriverInCurrentTrip) / counter;

            }

            //recalculate Shipper
            await RecalculateShipperRatingByShipperTenantId(shipperId.Value);

        }


        public async Task RecalculateShipperRatingByShipperTenantId(int shipperId)
        {
            var allShipperRatings = await GetAllShippersRatingAsync(shipperId);
            var ratingMinCount = _settingManager.GetSettingValue<int>(AppSettings.Rating.TenantRatingMinNumber);
            if (allShipperRatings.Count() > ratingMinCount)
            {
                //recalculate for shipper
                var finalShipperTripsRate = allShipperRatings.Sum(x => x.Rate) / allShipperRatings.Count();

                var tenant = await _tenantManager.GetByIdAsync(shipperId);
                tenant.Rate = Convert.ToDecimal(finalShipperTripsRate.ToString("0.0"));
                tenant.RateNumber = allShipperRatings.Count();
            }
        }

        public async Task RecalculateCarrierRatingByCarrierTenantId(int carrierId)
        {
            var allCarrierRatings = await GetAllCarriersRatingAsync(carrierId);
            var ratingMinCount = _settingManager.GetSettingValue<int>(AppSettings.Rating.TenantRatingMinNumber);

            if (allCarrierRatings.Count() > ratingMinCount)
            {
                var finalCarrierTripsRate = allCarrierRatings.Sum(x => x.Rate) / allCarrierRatings.Count();

                var tenant = await _tenantManager.GetByIdAsync(carrierId);
                tenant.Rate = Convert.ToDecimal(finalCarrierTripsRate.ToString("0.0")); //(tenant.Rate + finalCurrentCarrierTripRate) / 2;
                tenant.RateNumber = allCarrierRatings.Count();
            }
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
        public async Task<bool> IsRateDoneBefore(RatingLog rate)
         => await _ratingLogRepository.GetAll().AsNoTracking()
        .AnyAsync(RatingLogEquals(rate));


        #endregion

        #region helper
        private async Task ReCalculateTenantOrEntityRating(RatingLog rate)
        {
            //recalculate Entity rating (Driver/Facility)
            if (rate.RateType == RateType.DriverByReceiver)
                await RecalculateRatingById(rate.DriverId.Value, typeof(User));

            if (rate.RateType == RateType.FacilityByDriver)
                await RecalculateRatingById(rate.FacilityId.Value, typeof(Facility));

            // Recalculate Tenant Rating (Shipper/Carrier)
            if (rate.RateType.In(new[] { RateType.CarrierByShipper, RateType.DriverByReceiver, RateType.DEByReceiver }))// <= RateType.CarrierByShipper)
            {
                //await ReCalculateTenantRating(rate, RateType.CarrierTripBySystem);
                await UpdateAndRecalculateTenantTripRating(rate);
                return;
            }

            await UpdateAndRecalculateTenantTripRating(rate);
        }
        private async Task<List<RatingLog>> GetAllCarriersRatingAsync(int? CarrierId)
        {
            return await _ratingLogRepository
                .GetAll().Where(x => x.RateType == RateType.CarrierTripBySystem)
                .WhereIf(CarrierId != null, x => x.CarrierId == CarrierId)
                .ToListAsync();
        }


        private async Task UpdateAndRecalculateTenantTripRating(RatingLog rate)
        {
            rate = await _ratingLogRepository.GetAll()
                .Include(x => x.TripFk)
                .ThenInclude(x => x.ShippingRequestFk)
                .Include(x => x.TripFk)
                .ThenInclude(x => x.RoutPoints)
                .Include(x => x.RoutePointFk)
                .ThenInclude(x => x.ShippingRequestTripFk)
                .ThenInclude(x => x.ShippingRequestFk)
                .FirstOrDefaultAsync(x => x.Id == rate.Id);
            // carrier rating
            if (rate.RateType.In(new[] { RateType.CarrierByShipper, RateType.DriverByReceiver, RateType.DEByReceiver }))
            {
                await RecalculateCarrierRating(rate);
            }
            else //shipper rating
            {
                await RecalculateShipperRating(rate);
            }
        }

        private async Task<List<RatingLog>> GetAllShippersRatingAsync(int? ShipperId)
        {
            return await _ratingLogRepository
                .GetAll().Where(x => x.RateType == RateType.ShipperTripBySystem)
                .WhereIf(ShipperId != null, x => x.ShipperId == ShipperId)
                .ToListAsync();
        }

        private async Task RecalculateRating<TRatingEntity>(TRatingEntity ratingEntity)
    where TRatingEntity : IHasRating
        {
            List<decimal> allRatings = ratingEntity switch
            {
                User user => await GetAllRatingAsync(RateType.DriverByReceiver, driverId: user.Id),
                Facility facility => await GetAllRatingAsync(RateType.FacilityByDriver, facility.Id),
                Tenant tenant => await GetAllRatingByTenantEdition(tenant.Id, tenant.EditionId),
                _ => null
            };

            if (allRatings == null || allRatings.Count < 10) return;
            ratingEntity.Rate = Convert.ToDecimal((allRatings.Sum() / allRatings.Count).ToString("0.0"));
            ratingEntity.RateNumber = allRatings.Count;
        }

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


        private async Task CheckIfPointCompleted(RatingLog log)
        {
            DisableTenancyFilters();
            var isRoutPointCompleted = await _routePointRepository.GetAll().AsNoTracking()
                .WhereIf(log.RateType == RateType.FacilityByDriver,
                    x => x.ShippingRequestTripFk.AssignedDriverUserId == log.DriverId)
                .AnyAsync(x => x.Id == log.PointId &&
                (x.Status == RoutePointStatus.FinishOffLoadShipment || x.Status == RoutePointStatus.FinishLoading || x.Status == RoutePointStatus.ReceiverConfirmed ||
                 x.Status == RoutePointStatus.DeliveryNoteUploded || x.Status == RoutePointStatus.DeliveryConfirmation));

            if (isRoutPointCompleted) return;
            throw new UserFriendlyException(L("PointNotFoundOrNotCompleted"));
        }

        private async Task<ShippingRequest> GetRequestFromTripAsync(int tripId)
        {
            return await _tripRepository.GetAll().Where(x => x.Id == tripId).Select(x=>x.ShippingRequestFk).FirstOrDefaultAsync();
        }

        private async Task<ShippingRequest> GetRequestFromPointAsync(long pointId)
        {
            return await _routePointRepository.GetAll().Where(x => x.Id == pointId).Select(x=>x.ShippingRequestTripFk.ShippingRequestFk).FirstOrDefaultAsync();
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

        private async Task<bool> IsSaasRating(RatingLog log)
        {
            if (log.CarrierId.HasValue && log.ShipperId.HasValue)
                return log.CarrierId == log.ShipperId;

            return await _routePointRepository.GetAll().AsNoTracking()
                .Include(x => x.ShippingRequestTripFk)
                .ThenInclude(x => x.ShippingRequestFk)
                .Where(x => x.Id == log.PointId || x.ShippingRequestTripId == log.TripId)
                .AnyAsync(x => x.ShippingRequestTripFk.ShippingRequestFk.CarrierTenantId
                               == x.ShippingRequestTripFk.ShippingRequestFk.TenantId);
        }
        #endregion
    }
}