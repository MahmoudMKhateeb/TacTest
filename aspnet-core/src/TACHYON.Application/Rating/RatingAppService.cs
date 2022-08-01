using Abp.Application.Features;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Abp.Runtime.Validation;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TACHYON.AddressBook;
using TACHYON.Authorization.Users;
using TACHYON.Features;
using TACHYON.MultiTenancy;
using TACHYON.Rating.dtos;
using TACHYON.Routs.RoutPoints;
using TACHYON.Shipping.ShippingRequestTrips;
using TACHYON.Shipping.Trips;

namespace TACHYON.Rating
{
    public class RatingAppService : TACHYONAppServiceBase, IRatingAppService
    {
        private readonly IRepository<RatingLog, long> _ratingRepository;
        private readonly IRepository<ShippingRequestTrip> _shippingRequestTrip;
        private readonly IRepository<RoutPoint, long> _routePointRepository;
        private readonly UserManager _userManager;
        private readonly TenantManager _tenantManager;
        private readonly IRepository<Facility, long> _facilityRepository;
        private readonly RatingLogManager _ratingLogManager;

        public RatingAppService(IRepository<RatingLog, long> ratingRepository,
            IRepository<ShippingRequestTrip> shippingRequestTrip,
            IRepository<RoutPoint, long> routePointRepository,
            UserManager userManager,
            TenantManager tenantManager,
            IRepository<Facility, long> facilityRepository,
            RatingLogManager ratingLogManager)
        {
            _ratingRepository = ratingRepository;
            _shippingRequestTrip = shippingRequestTrip;
            _routePointRepository = routePointRepository;
            _userManager = userManager;
            _tenantManager = tenantManager;
            _facilityRepository = facilityRepository;
            _ratingLogManager = ratingLogManager;
        }

        #region CarrierRating

        [RequiresFeature(AppFeatures.Shipper)]
        public async Task CreateCarrierRatingByShipper(CreateCarrierRatingByShipperDto input)
        {
            var createdRatingLog = ObjectMapper.Map<RatingLog>(input);
            createdRatingLog.ShipperId = AbpSession.TenantId;
            createdRatingLog.RateType = RateType.CarrierByShipper;
            createdRatingLog.CarrierId = await GetTripAsync(createdRatingLog, x => x.ShippingRequestFk.CarrierTenantId); ;
            await _ratingLogManager.CreateRating(createdRatingLog);
        }

        public async Task CreateDriverAndDERatingByReceiver(CreateDriverAndDERatingByReceiverDto input)
        {

            var point = await _routePointRepository
               .GetAllIncluding(x => x.ShippingRequestTripFk).AsNoTracking()
               .Where(x => x.Code == input.CreateDriverRatingDtoByReceiverInput.Code)
               .Select(x => new
               {
                   x.ReceiverId,
                   x.Id,
                   DriverId = x.ShippingRequestTripFk.AssignedDriverUserId,
                   TripId = x.ShippingRequestTripId,
               }).FirstOrDefaultAsync();
            if (point == null) throw new AbpValidationException(L("WrongReceiverCode"));

            #region CreateDriverByReceiverRating

            var driverByReceiverRating = ObjectMapper.Map<RatingLog>(input.CreateDriverRatingDtoByReceiverInput);

            driverByReceiverRating.ReceiverId = point.ReceiverId;
            driverByReceiverRating.PointId = point.Id;
            driverByReceiverRating.DriverId = point.DriverId;
            //driverByReceiverRating.TripId = point.TripId;
            driverByReceiverRating.RateType = RateType.DriverByReceiver;

            await _ratingLogManager.CreateRating(driverByReceiverRating);

            #endregion


            #region CreateDeliveryExpRateByReceiver

            var deliveryRatingByReceiver = ObjectMapper.Map<RatingLog>(input.CreateDeliveryExpRateByReceiverInput);
            deliveryRatingByReceiver.ReceiverId = point.ReceiverId;
            deliveryRatingByReceiver.PointId = point.Id;
            deliveryRatingByReceiver.DriverId = point.DriverId;
            //deliveryRatingByReceiver.TripId = point.TripId;
            deliveryRatingByReceiver.RateType = RateType.DEByReceiver;
            await _ratingLogManager.CreateRating(deliveryRatingByReceiver);

            #endregion


        }



        #endregion

        #region ShipperRating

        [RequiresFeature(AppFeatures.Carrier)]
        public async Task CreateShipperRatingByCarrier(CreateShipperRateByCarrierDto input)
        {
            var shipperRating = ObjectMapper.Map<RatingLog>(input);
            shipperRating.RateType = RateType.ShipperByCarrier;
            shipperRating.CarrierId = AbpSession.TenantId;
            shipperRating.ShipperId = await GetTripAsync(shipperRating, x => x.ShippingRequestFk.TenantId);

            await _ratingLogManager.CreateRating(shipperRating);
        }

        #endregion

        #region Helpers

        /// <summary>
        /// With This Method Get what you want from ShippingRequestTrip<br/> Without load all of Trip object
        /// Note That this method not track ShippingRequestTrip Entity (For Read Data Only)
        /// </summary>
        /// <param name="rate"></param>
        /// <param name="selector"></param>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        /// <exception cref="UserFriendlyException"></exception>
        private async Task<TResult> GetTripAsync<TResult>(RatingLog rate,
            Expression<Func<ShippingRequestTrip, TResult>> selector)
        {
            DisableTenancyFilters();
            var trip = await _shippingRequestTrip
                .GetAllIncluding(x => x.ShippingRequestFk).AsNoTracking()
                .Where(x => x.Id == rate.TripId && x.Status == ShippingRequestTripStatus.Delivered)
                .WhereIf(rate.RateType == RateType.CarrierByShipper,
                    x => x.ShippingRequestFk.TenantId == AbpSession.TenantId)
                .WhereIf(rate.RateType == RateType.SEByDriver, x => x.AssignedDriverUserId == AbpSession.UserId)
                .WhereIf(rate.RateType == RateType.ShipperByCarrier,
                    x => x.ShippingRequestFk.CarrierTenantId == AbpSession.TenantId)
                .Select(selector).FirstOrDefaultAsync();

            if (trip != null) return trip;

            throw new AbpValidationException(L("TripNotFoundOrNotDelivered"));
        }

        #endregion
    }
}