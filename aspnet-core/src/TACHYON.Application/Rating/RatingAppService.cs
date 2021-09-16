using Abp.Application.Features;
using Abp.Domain.Repositories;
using Abp.UI;
using Abp.Collections.Extensions;
using Abp.Linq.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TACHYON.Features;
using TACHYON.Rating.dtos;
using TACHYON.Routs.RoutPoints;
using TACHYON.Shipping.ShippingRequestTrips;
using TACHYON.Shipping.Trips;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using TACHYON.Authorization.Users;
using TACHYON.MultiTenancy;
using TACHYON.AddressBook;

namespace TACHYON.Rating
{
    public class RatingAppService : TACHYONAppServiceBase, IRatingAppService
    {
        private readonly IRepository<RatingLog,long> _ratingRepository;
        private readonly IRepository<ShippingRequestTrip> _shippingRequestTrip;
        private readonly IRepository<RoutPoint, long> _routePointRepository;
        private readonly UserManager _userManager;
        private readonly TenantManager _tenantManager;
        private readonly IRepository<Facility,long> _facilityRepository;
        public RatingAppService(IRepository<RatingLog, long> ratingRepository, IRepository<ShippingRequestTrip> shippingRequestTrip, IRepository<RoutPoint, long> routePointRepository, UserManager userManager, TenantManager tenantManager, IRepository<Facility, long> facilityRepository)
        {
            _ratingRepository = ratingRepository;
            _shippingRequestTrip = shippingRequestTrip;
            _routePointRepository = routePointRepository;
            _userManager = userManager;
            _tenantManager = tenantManager;
            _facilityRepository = facilityRepository;
        }
        #region CarrierRating
        [RequiresFeature(AppFeatures.Shipper)]
        public async Task CreateCarrierRatingByShipper(CreateCarrierRatingByShipperDto input)
        {
            //var rate=ObjectMapper.Map<RatingLog>(input);
            //rate.RateType = RateType.CarrierByShipper;
            //rate.ShipperId = AbpSession.TenantId;

            //await CheckIfRateBefore(rate);
            //await _ratingRepository.InsertAndGetIdAsync(rate);
            await ValidateAndCreateRating(input, RateType.CarrierByShipper);
        }

        public async Task CreateDriverRatingByReceiver(CreateDriverRatingByReceiverDto input)
        {
            await ValidateAndCreateRating(input, RateType.DriverByReceiver);
        }

        public async Task CreateDeliveryExpRatingByReceiver(CreateDeliveryExpRateByReceiverDto input)
        {
            await ValidateAndCreateRating(input, RateType.DEByReceiver);
        }
        #endregion

        #region ShipperRating
        public async Task CreateFacilityRatingByDriver(CreateFacilityRateByDriverDto input)
        {
            await ValidateAndCreateRating(input, RateType.FacilityByDriver);
        }

        public async Task CreateShippingExpRatingByDriver(CreateShippingExpRateByDriverDto input)
        {
            await ValidateAndCreateRating(input, RateType.SEByDriver);
        }

        public async Task CreateShipperRatingByCarrier(CreateShipperRateByCarrierDto input)
        {
            await ValidateAndCreateRating(input, RateType.ShipperByCarrier);
        }

        #endregion

        #region helper
        
        private async Task ValidateAndCreateRating<T>(T input,RateType rateType)
        {
            var rate = ObjectMapper.Map<RatingLog>(input);
            rate.RateType = rateType;
            switch (rateType)
            {
                case RateType.CarrierByShipper:
                    rate.ShipperId = AbpSession.TenantId;
                    break;
                case RateType.ShipperByCarrier:
                    rate.CarrierId = AbpSession.TenantId;
                    break;
                case RateType.FacilityByDriver:
                case RateType.SEByDriver:
                    rate.DriverId = AbpSession.UserId;
                    break;
                case RateType.DEByReceiver:
                case RateType.DriverByReceiver:
                    var point = await _routePointRepository.GetAll().Include(x=>x.ShippingRequestTripFk)
                        .FirstOrDefaultAsync(x=>x.Code==rate.Code);
                    if (point == null)
                    {
                        throw new UserFriendlyException(L("WrongReceiverCode"));
                    }
                    rate.ReceiverId=point.ReceiverId;
                    rate.PointId = point.Id;
                    rate.DriverId = point.ShippingRequestTripFk.AssignedDriverUserId;
                    break;
            }
            if (rate.PointId != null)
            {
                await CheckIfFinishOffLoadingPoint(rate.PointId.Value, rate.RateType);
            }
            if (rate.TripId != null)
            {
                await CheckIfDeliveredTrip(rate.TripId.Value, rate.RateType);
            }
            await CheckIfRateBefore(rate);
            var rateId=await _ratingRepository.InsertAndGetIdAsync(rate);

            await RecalculateRatingForTenantAndDriverAndFacility(rateId);
        }

        private async Task CheckIfDeliveredTrip(int tripId,RateType rateType)
        {
            DisableTenancyFilters();
            if (await _shippingRequestTrip
                .GetAll()
               .WhereIf(rateType==RateType.CarrierByShipper, x=>x.ShippingRequestFk.TenantId==AbpSession.TenantId)
               .WhereIf(rateType == RateType.SEByDriver, x=>x.AssignedDriverUserId==AbpSession.UserId)
               .WhereIf(rateType == RateType.ShipperByCarrier, x=>x.ShippingRequestFk.CarrierTenantId==AbpSession.TenantId)
               .CountAsync(x => x.Id == tripId && x.Status == ShippingRequestTripStatus.Delivered) == 0)
            {
                throw new UserFriendlyException(L("TripNotFound"));
            }
        }

        private async Task CheckIfFinishOffLoadingPoint(long pointId,RateType rateType)
        {
            DisableTenancyFilters();
            if (await _routePointRepository
                .GetAll()
               .WhereIf(rateType == RateType.FacilityByDriver, x => x.ShippingRequestTripFk.AssignedDriverUserId == AbpSession.UserId)
                .CountAsync(x => x.Id == pointId && x.Status < RoutePointStatus.FinishOffLoadShipment) == 0)
            {
                throw new UserFriendlyException(L("PointNotFound"));
            }
        }

        private async Task CheckIfRateBefore(RatingLog rate)
        {
            if (await _ratingRepository.CountAsync(x => x.CarrierId == rate.CarrierId && x.DriverId == rate.DriverId &&
             x.PointId == rate.PointId && x.ReceiverId == rate.ReceiverId && x.ShipperId == rate.ShipperId &&
             x.TripId == rate.TripId && x.FacilityId==rate.FacilityId) > 0)
            {
                throw new UserFriendlyException(L("RateDoneBefore"));
            }
        }

        private async Task RecalculateRatingForTenantAndDriverAndFacility(long rateId)
        {
            var rate = await _ratingRepository.GetAll()
                .Include(x=>x.RoutePointFk)
                .Include(x=>x.TripFk)
                .ThenInclude(x=>x.ShippingRequestFk)
                .Include(x => x.TripFk)
                .ThenInclude(x=>x.RoutPoints)
                .FirstOrDefaultAsync(x=>x.Id== rateId);

            //Recalculate Carrier rating
            if (rate.RateType == RateType.CarrierByShipper || rate.RateType == RateType.DEByReceiver || rate.RateType == RateType.DriverByReceiver)
            {
                await ReCalculateCarrierOrDriverRating(rate);
            }

            //Recalculate Shipper rating
            else if(rate.RateType == RateType.FacilityByDriver || rate.RateType == RateType.SEByDriver || rate.RateType == RateType.ShipperByCarrier)
            {
                await ReCalculateShipperAndFacilityRating(rate);
            }

        }

        private async Task ReCalculateCarrierOrDriverRating(RatingLog rate)
        {
            //recalculate driver rating
            if (rate.RateType == RateType.DriverByReceiver)
            {
                await RecaculateDriverRating(rate);
            }

            var CarrierTripBySystem = await _ratingRepository
                                .GetAll()
                                .WhereIf(rate.TripId != null, x => x.TripId == rate.TripId)
                                .WhereIf(rate.PointId != null, x => x.TripId == rate.RoutePointFk.ShippingRequestTripId)
                                .FirstOrDefaultAsync(x => x.RateType == RateType.CarrierTripBySystem);

            //recalculate For trip and trip points
            var carrierId= rate.CarrierId != null ? rate.CarrierId : rate.PointId != null ? rate.RoutePointFk.ShippingRequestTripFk.ShippingRequestFk.CarrierTenantId : rate.TripFk.ShippingRequestFk.CarrierTenantId;
            if (CarrierTripBySystem == null)
            {
                //insert
                var newRate = new RatingLog();
                newRate.RateType = RateType.CarrierTripBySystem;
                newRate.TripId = rate.TripId != null ? rate.TripId : rate.RoutePointFk.ShippingRequestTripId;
                newRate.CarrierId = carrierId;
                newRate.Rate = rate.Rate;

                await _ratingRepository.InsertAndGetIdAsync(newRate);
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
                var allTripPointsRatings = await _ratingRepository.GetAll()
                    .Where(x => x.PointId != null && tripPoints.Contains(x.PointId.Value) &&
                    (x.RateType == RateType.DEByReceiver || x.RateType == RateType.DriverByReceiver))
                    .ToListAsync();

                var DEByReceiver = allTripPointsRatings.Where(x => x.RateType == RateType.DEByReceiver);

                var DEByReceiverAvg = 0;
                int counter = 0;
                if (DEByReceiver != null)
                {
                    DEByReceiverAvg = DEByReceiver.Sum(x => x.Rate) / DEByReceiver.Count();
                    counter = counter + 1;
                }

                var DriverByReceiver = allTripPointsRatings.Where(x => x.RateType == RateType.DriverByReceiver);

                var DriverByReceiverAvg = 0;

                if (DriverByReceiver != null)
                {
                    DriverByReceiverAvg = DriverByReceiver.Sum(x => x.Rate) / DriverByReceiver.Count();
                    counter = counter + 1;
                }

                //Get Shipper rating for trip
                var CarrierRatingByShipperInCurrentTrip = rate.RateType == RateType.CarrierByShipper ? rate.Rate : 0;
                if (CarrierRatingByShipperInCurrentTrip == 0)
                {
                   var CarrierRatingByShipperInCurrentTripDB = await _ratingRepository
                  .FirstOrDefaultAsync(x => x.RateType == RateType.CarrierByShipper &&
                  x.TripId == rate.TripId);

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
                
                CarrierTripBySystem.Rate = (DEByReceiverAvg + DriverByReceiverAvg + CarrierRatingByShipperInCurrentTrip) / counter;

                CurrentUnitOfWork.SaveChanges();

            }

            //recalculate Carrier
            //check minimum 10 ratings
            var allCarrierRatings = await _ratingRepository.GetAll()
                .Where(x => x.CarrierId == carrierId && x.RateType == RateType.CarrierTripBySystem)
                .ToListAsync();
            if (allCarrierRatings.Count() > 10)
            {
                var finalCarrierTripsRate = allCarrierRatings.Sum(x => x.Rate) / allCarrierRatings.Count();

                var tenant = await _tenantManager.GetByIdAsync(carrierId.Value);
                tenant.Rate = Convert.ToDecimal(finalCarrierTripsRate.ToString("0.0")); //(tenant.Rate + finalCurrentCarrierTripRate) / 2;
            }
        }

        private async Task RecaculateDriverRating(RatingLog rate)
        {
            var allRatings = await _ratingRepository.GetAll().Where(x => x.DriverId == rate.DriverId).ToListAsync();
            if (allRatings.Count() > 10)
            {
                var avgRate = allRatings.Sum(x => x.Rate) / allRatings.Count();

                var user = await _userManager.GetUserByIdAsync(rate.DriverId.Value);
                user.Rate = Convert.ToDecimal(avgRate.ToString("0.0"));
            }
        }

        private async Task ReCalculateShipperAndFacilityRating(RatingLog rate)
        {
            //recalculate facility rating
            await RecalculateFacilityRating(rate);

            //recalculate shipper rating
            var ShipperTripBySystem = await _ratingRepository
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

                await _ratingRepository.InsertAndGetIdAsync(newRate);
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
                var allFacilityByDriverRatings = await _ratingRepository.GetAll()
                    .Where(x => x.PointId != null && tripPoints.Contains(x.PointId.Value) &&
                    (x.RateType == RateType.FacilityByDriver))
                    .ToListAsync();


                var allFacilityByDriverRatingsAvg = 0;
                int counter = 0;
                if (allFacilityByDriverRatings != null)
                {
                    allFacilityByDriverRatingsAvg = allFacilityByDriverRatings.Sum(x => x.Rate) / allFacilityByDriverRatings.Count();
                    counter = counter + 1;
                }

                //Get Shipper rating for trip
                //carrier by shipper
                var CarrierRatingByShipperOrDriverInCurrentTripDB = await _ratingRepository.GetAll()
                   .Where(x => (x.RateType == RateType.CarrierByShipper || x.RateType == RateType.SEByDriver) &&
                   x.TripId == rate.TripId).ToListAsync();

                var ShipperRatingByShipperInCurrentTrip = rate.RateType == RateType.CarrierByShipper ? rate.Rate : 0;
                if (ShipperRatingByShipperInCurrentTrip == 0)
                {
                    var CarrierRatingByShipperInCurrentTripDB = CarrierRatingByShipperOrDriverInCurrentTripDB.FirstOrDefault(x => x.RateType == RateType.CarrierByShipper);

                    if (CarrierRatingByShipperInCurrentTripDB != null)
                    {
                        ShipperRatingByShipperInCurrentTrip = CarrierRatingByShipperInCurrentTripDB.Rate;
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
                    var CarrierRatingByShipperInCurrentTripDB = CarrierRatingByShipperOrDriverInCurrentTripDB
                   .FirstOrDefault(x => x.RateType == RateType.SEByDriver);

                    if (CarrierRatingByShipperInCurrentTripDB != null)
                    {
                        SERatingByDriverInCurrentTrip = CarrierRatingByShipperInCurrentTripDB.Rate;
                        counter = counter + 1;
                    }
                }
                else
                {
                    counter = counter + 1;
                }

                ShipperTripBySystem.Rate = (allFacilityByDriverRatingsAvg + ShipperRatingByShipperInCurrentTrip + SERatingByDriverInCurrentTrip) / counter;

                CurrentUnitOfWork.SaveChanges();
            }

            //recalculate Shipper
            //check minimum 10 ratings

            var allShipperRatings = await _ratingRepository.GetAll()
                .Where(x => x.ShipperId == shipperId && x.RateType == RateType.ShipperTripBySystem)
                .ToListAsync();
            if (allShipperRatings.Count() > 10)
            {
                var finalCarrierTripsRate = allShipperRatings.Sum(x => x.Rate) / allShipperRatings.Count();

                var tenant = await _tenantManager.GetByIdAsync(shipperId.Value);
                tenant.Rate = Convert.ToDecimal(finalCarrierTripsRate.ToString("0.0")); //(tenant.Rate + finalCurrentCarrierTripRate) / 2;
            }
        }

        private async Task RecalculateFacilityRating(RatingLog rate)
        {
            if (rate.RateType == RateType.FacilityByDriver)
            {
                var facilityRatings = await _ratingRepository.GetAll()
                    .Where(x => x.FacilityId == rate.FacilityId && x.RateType == RateType.FacilityByDriver)
                    .ToListAsync();

                var facility = await _facilityRepository.FirstOrDefaultAsync(rate.FacilityId.Value);
                facility.Rate = Convert.ToDecimal((facilityRatings.Sum(x => x.Rate) / facilityRatings.Count()).ToString("0.0"));
            }
        }
        #endregion
    }
}
