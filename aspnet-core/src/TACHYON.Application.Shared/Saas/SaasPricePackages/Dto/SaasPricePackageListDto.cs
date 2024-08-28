using Abp.Application.Services.Dto;
using TACHYON.Shipping.ShippingRequests;

namespace TACHYON.Saas.SaasPricePackages.Dto
{
    public class SaasPricePackageListDto : EntityDto<long>
    {
        public string PricePackageReference { get; set; }
        
        public string DisplayName { get; set; }

        public int TenantId { get; set; }

        public int TransportTypeId { get; set; }

        public long TruckTypeId { get; set; }

        public int? OriginCityId { get; set; }
        
        public int? DestinationCityId { get; set; }
        
        public int? ActorShipperId { get; set; }
        
        public decimal ActorShipperPrice { get; set; }

        public ShippingTypeEnum? ShippingTypeId { get; set; }

         public int? GoodCategoryId { get; set; }

         public TripLoadingTypeEnum? tripLoadingType{ get; set; }

         public RoundTripType? RoundTripType { get; set; }
    }
}