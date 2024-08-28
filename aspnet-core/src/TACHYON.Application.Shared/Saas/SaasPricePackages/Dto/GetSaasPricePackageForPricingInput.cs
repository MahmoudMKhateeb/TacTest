using TACHYON.Shipping.ShippingRequests;

namespace TACHYON.Saas.SaasPricePackages.Dto
{
    public class GetSaasPricePackageForPricingInput
    {
        public long? ActorShipperId { get; set; }
        public int? OriginCityId { get; set; }
        public int? DestinationCityId { get; set; }
        public long? TruckId { get; set; }

        public long? TruckTypeId { get; set; }
        
        public ShippingTypeEnum? ShippingTypeId { get; set; }

        public int? GoodCategoryId { get; set; }

        public TripLoadingTypeEnum? tripLoadingType{ get; set; }

        public RoundTripType? RoundTripType { get; set; }
    }
}