using System.Net;
using TACHYON.Shipping.ShippingRequests;

namespace TACHYON.Saas.SaasPricePackages.Dto
{
    public class SaasPricePackageForViewDto
    {
        public long Id { get; set; }
        public string DestinationCityDisplayName { get; set; }
        public string OriginCityDisplayName { get; set; }
        public string DisplayName { get; set; }
        public string ActorShipperFkCompanyName { get; set; }
        public string TrucksTypDisplayName { get; set; }
        public decimal ActorShipperPrice { get; set; }
        
        public string TransportType { get; set; }

        
        public ShippingTypeEnum? ShippingTypeId { get; set; }

         public int? GoodCategoryId { get; set; }

         public TripLoadingTypeEnum? tripLoadingType{ get; set; }

         public RoundTripType? RoundTripType { get; set; }
    }
}