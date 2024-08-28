using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using FirebaseAdmin.Auth.Multitenancy;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TACHYON.Actors;
using TACHYON.Cities;
using TACHYON.Goods.GoodCategories;
using TACHYON.PricePackages;
using TACHYON.Shipping.ShippingRequests;
using TACHYON.Trucks.TruckCategories.TransportTypes;
using TACHYON.Trucks.TrucksTypes;

namespace TACHYON.Saas.SaasPricePackages
{
     [Table("SaasPricePackages")]
    public class SaasPricePackage: FullAuditedEntity<long>, IMustHaveTenant
    {
        public string PricePackageReference { get; set; }

        [Required]
        public string DisplayName { get; set; }

        public int TenantId { get; set; }
        
        public int TransportTypeId { get; set; }
        [ForeignKey(nameof(TransportTypeId))] public TransportType TransportTypeFk { get; set; }
        public long TruckTypeId { get; set; }
        [ForeignKey(nameof(TruckTypeId))] public TrucksType TrucksTypeFk { get; set; }
        public int? OriginCityId { get; set; }

        [ForeignKey(nameof(OriginCityId))] public City OriginCity { get; set; }

        public int? DestinationCityId { get; set; }

        [ForeignKey(nameof(DestinationCityId))] public City DestinationCity { get; set; }

        public int? ActorShipperId { get; set; }

        [ForeignKey(nameof(ActorShipperId))] public Actor ActorShipperFk { get; set; }

        public decimal ActorShipperPrice { get; set; }

         public ShippingTypeEnum? ShippingTypeId { get; set; }

         public int? GoodCategoryId { get; set; }

        [ForeignKey(nameof(GoodCategoryId))] 
         public GoodCategory goodCategory{ get; set; }

         public TripLoadingTypeEnum? tripLoadingType{ get; set; }

         public RoundTripType? RoundTripType { get; set; }
    }
}
