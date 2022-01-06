using Abp.Application.Features;
using Abp.Domain.Repositories;
using System.Threading.Tasks;
using TACHYON.AddressBook;
using TACHYON.Authorization.Users;
using TACHYON.Features;
using TACHYON.MultiTenancy;
using TACHYON.Rating.dtos;
using TACHYON.Routs.RoutPoints;
using TACHYON.Shipping.ShippingRequestTrips;

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
        public RatingAppService(IRepository<RatingLog, long> ratingRepository, IRepository<ShippingRequestTrip> shippingRequestTrip, IRepository<RoutPoint, long> routePointRepository, UserManager userManager, TenantManager tenantManager, IRepository<Facility, long> facilityRepository, RatingLogManager ratingLogManager)
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
            await _ratingLogManager.ValidateAndCreateRating(input, RateType.CarrierByShipper);
        }

        public async Task CreateDriverAndDERatingByReceiver(CreateDriverAndDERatingByReceiverDto input)
        {
            await _ratingLogManager.ValidateAndCreateRating(input.CreateDriverRatingDtoByReceiverInput, RateType.DriverByReceiver);
            await _ratingLogManager.ValidateAndCreateRating(input.CreateDeliveryExpRateByReceiverInput, RateType.DEByReceiver);
        }

        #endregion

        #region ShipperRating
        //this funct transfered in shippingRequestDriverAppService "SetRating"
        //public async Task CreateFacilityRatingByDriver(CreateFacilityRateByDriverDto input)
        //{
        //    await _ratingLogManager.ValidateAndCreateRating(input, RateType.FacilityByDriver);
        //}

        //this funct transfered in shippingRequestDriverAppService "SetShippingExpRating"
        //public async Task CreateShippingExpRatingByDriver(CreateShippingExpRateByDriverDto input)
        //{
        //    await _ratingLogManager.ValidateAndCreateRating(input, RateType.SEByDriver);
        //}

        [RequiresFeature(AppFeatures.Carrier)]
        public async Task CreateShipperRatingByCarrier(CreateShipperRateByCarrierDto input)
        {
            await _ratingLogManager.ValidateAndCreateRating(input, RateType.ShipperByCarrier);
        }

        #endregion
    }
}