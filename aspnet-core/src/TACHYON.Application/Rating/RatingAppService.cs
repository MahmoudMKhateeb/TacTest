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

namespace TACHYON.Rating
{
    public class RatingAppService : TACHYONAppServiceBase, IRatingAppService
    {
        private readonly IRepository<RatingLog,long> _ratingRepository;
        private readonly IRepository<ShippingRequestTrip> _shippingRequestTrip;
        private readonly IRepository<RoutPoint, long> _routePointRepository;
        private readonly UserManager _userManager;
        private readonly TenantManager _tenantManager;
        public RatingAppService(IRepository<RatingLog, long> ratingRepository, IRepository<ShippingRequestTrip> shippingRequestTrip, IRepository<RoutPoint, long> routePointRepository, UserManager userManager, TenantManager tenantManager)
        {
            _ratingRepository = ratingRepository;
            _shippingRequestTrip = shippingRequestTrip;
            _routePointRepository = routePointRepository;
            _userManager = userManager;
            _tenantManager = tenantManager;
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

            //todo recaculating rating and 10 min ratings to do it.
        }

        public async Task CreateDriverRatingByReceiver(CreateDriverRatingByReceiverDto input)
        {
            //var rate = ObjectMapper.Map<RatingLog>(input);
            //rate.RateType = RateType.DriverByReceiver;

            //await CheckIfRateBefore(rate);
            //await _ratingRepository.InsertAndGetIdAsync(rate);
            await ValidateAndCreateRating(input, RateType.DriverByReceiver);
        }

        public async Task CreateDeliveryExpRatingByReceiver(CreateDeliveryExpRateByReceiverDto input)
        {
            //var rate = ObjectMapper.Map<RatingLog>(input);
            //rate.RateType = RateType.DEByReceiver;

            //await CheckIfRateBefore(rate);
            //await _ratingRepository.InsertAndGetIdAsync(rate);
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
                    var point = await _routePointRepository.FirstOrDefaultAsync(x=>x.Code==rate.Code);
                    if (point == null)
                    {
                        throw new UserFriendlyException(L("WrongReceiverCode"));
                    }
                    rate.ReceiverId=point.ReceiverId;
                    rate.PointId = point.Id;
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

            await RecalculateRatingForTenantOrDriver(rateId);
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
               //.WhereIf(rateType == RateType.DEByReceiver, x => x.ReceiverId ==)
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

        private async Task RecalculateRatingForTenantOrDriver(long rateId)
        {
            var rate = await _ratingRepository.GetAll()
                .Include(x=>x.RoutePointFk)
                .Include(x=>x.TripFk)
                .ThenInclude(x=>x.ShippingRequestFk)
                .Include(x => x.TripFk)
                .ThenInclude(x=>x.RoutPoints)
                .FirstOrDefaultAsync(x=>x.Id== rateId);

            //recalculate driver rating
            if (rate.RateType == RateType.DriverByReceiver)
            {
                await RecaculateDriverRating(rate);
            }

            //Recalculate Carrier rating
            else if (rate.RateType == RateType.CarrierByShipper || rate.RateType == RateType.DEByReceiver || rate.RateType == RateType.DriverByReceiver)
            {
                await ReCalculateCarrierRating(rate);
            }

            //Recalculate Shipper rating
            else if(rate.RateType == RateType.FacilityByDriver || rate.RateType == RateType.SEByDriver || rate.RateType == RateType.ShipperByCarrier)
            {

            }

        }

        private async Task RecaculateDriverRating(RatingLog rate)
        {
            var allRatings = await _ratingRepository.GetAll().Where(x => x.DriverId == rate.DriverId).ToListAsync();
            if (allRatings.Count() > 10)
            {
                var avgRate = allRatings.Sum(x => x.Rate) / allRatings.Count();

                var user = await _userManager.GetUserByIdAsync(rate.DriverId.Value);
                user.Rate = avgRate;
            }
        }

        private async Task ReCalculateCarrierRating(RatingLog rate)
        {
            var CarrierTripBySystem = await _ratingRepository
                                .GetAll()
                                .WhereIf(rate.TripId != null, x => x.TripId == rate.TripId)
                                .WhereIf(rate.PointId != null, x => x.TripId == rate.RoutePointFk.ShippingRequestTripId)
                                .FirstOrDefaultAsync(x => x.RateType == RateType.CarrierTripBySystem);

            //recalculate For trip and trip points
            if (CarrierTripBySystem == null)
            {
                //insert
                var newRate = new RatingLog();
                newRate.RateType = RateType.CarrierTripBySystem;
                newRate.TripId = rate.TripId != null ? rate.TripId : rate.RoutePointFk.ShippingRequestTripId;
                newRate.CarrierId = rate.RoutePointFk.ShippingRequestTripFk.ShippingRequestFk.CarrierTenantId;
                newRate.Rate = rate.Rate;

                await _ratingRepository.InsertAndGetIdAsync(newRate);
            }
            else
            {
                //recalculate and update
                var tripId = rate.TripId != null ? rate.TripId : rate.RoutePointFk.ShippingRequestTripId;
                var tripPoints = new List<long>();
                if (tripId != null)
                {
                    tripPoints = rate.TripFk.RoutPoints.Select(x => x.Id).ToList();
                }
                else
                {
                    tripId = rate.RoutePointFk.ShippingRequestTripId;
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
            var carrierId = rate.CarrierId;
            if (carrierId == null)
            {
                carrierId = rate.RoutePointFk.ShippingRequestTripFk.ShippingRequestFk.CarrierTenantId;
            }

            //var allCarrierRatings =await _ratingRepository.GetAll().Where(x => x.TripId == tripId && x.CarrierId == carrierId).ToListAsync();
            var allCarrierRatings = await _ratingRepository.GetAll()
                .Where(x => x.CarrierId == carrierId && x.RateType == RateType.CarrierTripBySystem)
                .ToListAsync();
            if (allCarrierRatings.Count() > 10)
            {
                var finalCarrierTripsRate = allCarrierRatings.Sum(x => x.Rate) / allCarrierRatings.Count();

                var tenant = await _tenantManager.GetByIdAsync(carrierId.Value);
                tenant.Rate = finalCarrierTripsRate; //(tenant.Rate + finalCurrentCarrierTripRate) / 2;
            }
        }
        #endregion
    }
}
