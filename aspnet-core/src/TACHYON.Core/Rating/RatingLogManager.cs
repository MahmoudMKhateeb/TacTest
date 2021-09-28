using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Abp.Runtime.Session;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TACHYON.AddressBook;
using TACHYON.Authorization.Users;
using TACHYON.MultiTenancy;
using TACHYON.Routs.RoutPoints;
using TACHYON.Shipping.ShippingRequestTrips;
using TACHYON.Shipping.Trips;

namespace TACHYON.Rating
{
    public class RatingLogManager : TACHYONDomainServiceBase
    {
        public readonly IRepository<RatingLog, long> _ratingLogRepository;
        public readonly IRepository<RoutPoint, long> _routePointRepository;
        public readonly UserManager _userManager;
        public readonly TenantManager _tenantManager;
        public readonly IRepository<Facility, long> _facilityRepository;
        public readonly IRepository<ShippingRequestTrip> _shippingRequestTrip;
        public IAbpSession AbpSession { get; set; }

        public RatingLogManager(IRepository<RatingLog, long> ratingLogRepository, IRepository<RoutPoint, long> routePointRepository, UserManager userManager, TenantManager tenantManager, IRepository<Facility, long> facilityRepository, IRepository<ShippingRequestTrip> shippingRequestTrip)
        {
            _ratingLogRepository = ratingLogRepository;
            _routePointRepository = routePointRepository;
            _userManager = userManager;
            _tenantManager = tenantManager;
            _facilityRepository = facilityRepository;
            _shippingRequestTrip = shippingRequestTrip;
            AbpSession = NullAbpSession.Instance;
        }

        public async Task ValidateAndCreateRating<T>(T input, RateType rateType)
        {
            var rate = ObjectMapper.Map<RatingLog>(input);
            rate.RateType = rateType;
            //get delivered trip by permission
            ShippingRequestTrip trip = await GetTripAsync(rate);

            switch (rateType)
            {
                case RateType.CarrierByShipper:
                    rate.ShipperId = AbpSession.TenantId;
                    rate.CarrierId = trip
                        .ShippingRequestFk.CarrierTenantId;
                    break;
                case RateType.ShipperByCarrier:
                    rate.CarrierId = AbpSession.TenantId;
                    rate.ShipperId = trip.ShippingRequestFk.TenantId;
                    break;
                case RateType.FacilityByDriver:
                    var pointt = await _routePointRepository.FirstOrDefaultAsync(rate.PointId.Value);
                    rate.DriverId = AbpSession.UserId;
                    rate.FacilityId = pointt.FacilityId;
                    break;
                case RateType.SEByDriver:
                    rate.DriverId = AbpSession.UserId;
                    break;
                case RateType.DEByReceiver:
                case RateType.DriverByReceiver:
                    var point = await _routePointRepository.GetAll().Include(x => x.ShippingRequestTripFk)
                        .FirstOrDefaultAsync(x => x.Code == rate.Code);
                    if (point == null)
                    {
                        throw new UserFriendlyException(L("WrongReceiverCode"));
                    }
                    rate.ReceiverId = point.ReceiverId;
                    rate.PointId = point.Id;
                    rate.DriverId = point.ShippingRequestTripFk.AssignedDriverUserId;
                    break;
            }
            if (rate.PointId != null)
            {
                await CheckIfFinishOffLoadingPoint(rate.PointId.Value, rate.RateType);
            }
            //if (rate.TripId != null)
            //{
            //    await CheckIfDeliveredTrip(trip, rate.RateType);
            //}
            await CheckIfRateBefore(rate);
            var rateId = await _ratingLogRepository.InsertAndGetIdAsync(rate);

            await RecalculateRatingForTenantAndDriverAndFacility(rateId);
        }

        public async Task<ShippingRequestTrip> GetTripAsync(RatingLog rate)
        {
            var trip = new ShippingRequestTrip();
            if (rate.TripId != null)
            {
                DisableTenancyFilters();
                trip = await _shippingRequestTrip
                    .GetAllIncluding(x => x.ShippingRequestFk)
                    .WhereIf(rate.RateType == RateType.CarrierByShipper, x => x.ShippingRequestFk.TenantId == AbpSession.TenantId)
                    .WhereIf(rate.RateType == RateType.SEByDriver, x => x.AssignedDriverUserId == AbpSession.UserId)
                    .WhereIf(rate.RateType == RateType.ShipperByCarrier, x => x.ShippingRequestFk.CarrierTenantId == AbpSession.TenantId)
                    .FirstOrDefaultAsync(x => x.Id == rate.TripId
                    && x.Status == ShippingRequestTripStatus.Delivered
                    );

                if (trip == null)
                {
                    throw new UserFriendlyException(L("TripNotFoundOrNotDelivered"));
                }
            }

            return trip;
        }

        //public async Task CheckIfDeliveredTrip(long tripId,RateType rateType)
        //{
        //    DisableTenancyFilters();
        //    if (await _shippingRequestTrip
        //        .GetAll()
        //       .WhereIf(rateType==RateType.CarrierByShipper, x=>x.ShippingRequestFk.TenantId==AbpSession.TenantId)
        //       .WhereIf(rateType == RateType.SEByDriver, x=>x.AssignedDriverUserId==AbpSession.UserId)
        //       .WhereIf(rateType == RateType.ShipperByCarrier, x=>x.ShippingRequestFk.CarrierTenantId==AbpSession.TenantId)
        //       .CountAsync(x => x.Id == tripId && x.Status == ShippingRequestTripStatus.Delivered) == 0)
        //    {
        //        throw new UserFriendlyException(L("TripNotFound"));
        //    }
        //}

        public async Task CheckIfFinishOffLoadingPoint(long pointId, RateType rateType)
        {
            DisableTenancyFilters();
            if (await _routePointRepository
                .GetAll()
               .WhereIf(rateType == RateType.FacilityByDriver, x => x.ShippingRequestTripFk.AssignedDriverUserId == AbpSession.UserId)
                .CountAsync(x => x.Id == pointId && x.Status >= RoutePointStatus.FinishOffLoadShipment) == 0)
            {
                throw new UserFriendlyException(L("PointNotFoundOrNotFinished"));
            }
        }

        public async Task CheckIfRateBefore(RatingLog rate)
        {
            if (await _ratingLogRepository.CountAsync(x => x.CarrierId == rate.CarrierId && x.DriverId == rate.DriverId &&
             x.PointId == rate.PointId && x.ReceiverId == rate.ReceiverId && x.ShipperId == rate.ShipperId &&
             x.TripId == rate.TripId && x.FacilityId == rate.FacilityId && x.RateType == rate.RateType) > 0)
            {
                throw new UserFriendlyException(L("RateDoneBefore"));
            }
        }

        public async Task RecalculateRatingForTenantAndDriverAndFacility(long rateId)
        {
            var rate = await _ratingLogRepository.GetAll()
                .Include(x => x.RoutePointFk)
                .ThenInclude(x => x.ShippingRequestTripFk)
                .ThenInclude(x => x.ShippingRequestFk)
                .Include(x => x.TripFk)
                .ThenInclude(x => x.RoutPoints)
                .Include(x => x.TripFk)
                .ThenInclude(x => x.ShippingRequestFk)
                .FirstOrDefaultAsync(x => x.Id == rateId);

            //Recalculate Carrier rating
            if (rate.RateType == RateType.CarrierByShipper || rate.RateType == RateType.DEByReceiver || rate.RateType == RateType.DriverByReceiver)
            {
                await ReCalculateCarrierOrDriverRating(rate);
            }

            //Recalculate Shipper rating
            else if (rate.RateType == RateType.FacilityByDriver || rate.RateType == RateType.SEByDriver || rate.RateType == RateType.ShipperByCarrier)
            {
                await ReCalculateShipperAndFacilityRating(rate);
            }

        }

        public async Task ReCalculateCarrierOrDriverRating(RatingLog rate)
        {
            //recalculate driver rating
            if (rate.RateType == RateType.DriverByReceiver)
            {
                await RecaculateDriverRating(rate);
            }

            var CarrierTripBySystem = await _ratingLogRepository
                                .GetAll()
                                .WhereIf(rate.TripId != null, x => x.TripId == rate.TripId)
                                .WhereIf(rate.PointId != null, x => x.TripId == rate.RoutePointFk.ShippingRequestTripId)
                                .FirstOrDefaultAsync(x => x.RateType == RateType.CarrierTripBySystem);

            //recalculate For trip and trip points
            var carrierId = rate.CarrierId != null ? rate.CarrierId : rate.PointId != null ? rate.RoutePointFk.ShippingRequestTripFk.ShippingRequestFk.CarrierTenantId : rate.TripFk.ShippingRequestFk.CarrierTenantId;
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
                var tripId = rate.TripId;
                var tripPoints = new List<long>();
                if (tripId != null)
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
                    counter = counter + 1;
                }

                var DriverByReceiver = allTripPointsRatings.Where(x => x.RateType == RateType.DriverByReceiver);

                decimal DriverByReceiverAvg = default(decimal);

                if (DriverByReceiver.Count() > 0)
                {
                    DriverByReceiverAvg = DriverByReceiver.Sum(x => x.Rate) / DriverByReceiver.Count();
                    counter = counter + 1;
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
                        counter = counter + 1;
                    }
                }
                else
                {
                    counter = counter + 1;
                }

                CarrierTripBySystem.Rate = Convert.ToDecimal(((DEByReceiverAvg + DriverByReceiverAvg + CarrierRatingByShipperInCurrentTrip) / counter).ToString("0.00"));

                CurrentUnitOfWork.SaveChanges();

            }

            //recalculate Carrier
            //check minimum 10 ratings
            var allCarrierRatings = await GetAllCarriersRatingAsync(carrierId);
            if (allCarrierRatings.Count() > 10)
            {
                var finalCarrierTripsRate = allCarrierRatings.Sum(x => x.Rate) / allCarrierRatings.Count();

                var tenant = await _tenantManager.GetByIdAsync(carrierId.Value);
                tenant.Rate = Convert.ToDecimal(finalCarrierTripsRate.ToString("0.0")); //(tenant.Rate + finalCurrentCarrierTripRate) / 2;
                tenant.RateNumber = allCarrierRatings.Count();
            }
        }

        public async Task RecaculateDriverRating(RatingLog rate)
        {
            var allRatings = await _ratingLogRepository.GetAll().Where(x => x.DriverId == rate.DriverId).ToListAsync();
            if (allRatings.Count() > 10)
            {
                var avgRate = allRatings.Sum(x => x.Rate) / allRatings.Count();

                var user = await _userManager.GetUserByIdAsync(rate.DriverId.Value);
                user.Rate = Convert.ToDecimal(avgRate.ToString("0.0"));
                user.RateNumber = allRatings.Count();
            }
        }

        public async Task ReCalculateShipperAndFacilityRating(RatingLog rate)
        {
            //recalculate facility rating
            await RecalculateFacilityRating(rate);

            //recalculate shipper rating
            var ShipperTripBySystem = await _ratingLogRepository
                                .GetAll()
                                .WhereIf(rate.TripId != null, x => x.TripId == rate.TripId)
                                .WhereIf(rate.PointId != null, x => x.TripId == rate.RoutePointFk.ShippingRequestTripId)
                                .FirstOrDefaultAsync(x => x.RateType == RateType.ShipperTripBySystem);

            //recalculate For trip and trip points
            var shipperId = rate.ShipperId != null ? rate.ShipperId : rate.PointId != null ? rate.RoutePointFk.ShippingRequestTripFk.ShippingRequestFk.TenantId : rate.TripFk.ShippingRequestFk.TenantId;
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
                    tripPoints = rate.TripFk.RoutPoints.Select(x => x.Id).ToList();
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
                    counter = counter + 1;
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
                        counter = counter + 1;
                    }
                }
                else
                {
                    counter = counter + 1;
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
                        counter = counter + 1;
                    }
                }
                else
                {
                    counter = counter + 1;
                }

                ShipperTripBySystem.Rate = (allFacilityByDriverRatingsAvg + ShipperRatingByCarrierInCurrentTrip + SERatingByDriverInCurrentTrip) / counter;

                CurrentUnitOfWork.SaveChanges();
            }

            //recalculate Shipper
            //check minimum 10 ratings

            var allShipperRatings = await GetAllShippersRatingAsync(shipperId);
            if (allShipperRatings.Count() > 10)
            {
                //recalculate for shipper
                var finalShipperTripsRate = allShipperRatings.Sum(x => x.Rate) / allShipperRatings.Count();

                var tenant = await _tenantManager.GetByIdAsync(shipperId.Value);
                tenant.Rate = Convert.ToDecimal(finalShipperTripsRate.ToString("0.0"));
                tenant.RateNumber = allShipperRatings.Count();
            }
        }

        public async Task RecalculateFacilityRating(RatingLog rate)
        {
            if (rate.RateType == RateType.FacilityByDriver)
            {
                var facilityRatings = await GetAllFacilityRatingAsync(rate.FacilityId);

                if (facilityRatings.Count() > 10)
                {
                    var facility = await _facilityRepository.FirstOrDefaultAsync(rate.FacilityId.Value);
                    facility.Rate = Convert.ToDecimal((facilityRatings.Sum(x => x.Rate) / facilityRatings.Count()).ToString("0.0"));
                    facility.RateNumber = facilityRatings.Count();
                }
            }
        }


        #region helper
        private async Task<List<RatingLog>> GetAllCarriersRatingAsync(int? CarrierId)
        {
            return await _ratingLogRepository
                .GetAll().Where(x => x.RateType == RateType.CarrierTripBySystem)
                .WhereIf(CarrierId != null, x => x.CarrierId == CarrierId)
                .ToListAsync();
        }

        private async Task<List<RatingLog>> GetAllShippersRatingAsync(int? ShipperId)
        {
            return await _ratingLogRepository
                .GetAll().Where(x => x.RateType == RateType.ShipperTripBySystem)
                .WhereIf(ShipperId != null, x => x.ShipperId == ShipperId)
                .ToListAsync();
        }

        private async Task<List<RatingLog>> GetAllFacilityRatingAsync(long? facilityId)
        {
            return await _ratingLogRepository
                .GetAll().Where(x => x.RateType == RateType.FacilityByDriver)
                .WhereIf(facilityId != null, x => x.FacilityId == facilityId)
                .ToListAsync();
        }

        #endregion
    }
}