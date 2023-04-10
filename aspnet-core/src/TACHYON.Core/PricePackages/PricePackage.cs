using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TACHYON.Cities;
using TACHYON.Common;
using TACHYON.MultiTenancy;
using TACHYON.PricePackages.PricePackageAppendices;
using TACHYON.PricePackages.PricePackageProposals;
using TACHYON.ServiceAreas;
using TACHYON.Shipping.ShippingRequests;
using TACHYON.Shipping.ShippingTypes;
using TACHYON.Trucks.TruckCategories.TransportTypes;
using TACHYON.Trucks.TrucksTypes;

namespace TACHYON.PricePackages
{
    [Table("PricePackages")]
    public class PricePackage : FullAuditedEntity<long>, IMustHaveTenant
    {
        public string PricePackageReference { get; set; }

        [Required]
        [StringLength(PricePackagesConst.MaxDisplayNameLength, MinimumLength = PricePackagesConst.MinDisplayNameLength)]
        public string DisplayName { get; set; }

        public int TenantId { get; set; }

        [ForeignKey(nameof(TenantId))] public Tenant Tenant { get; set; }
        public int TransportTypeId { get; set; }
        [ForeignKey(nameof(TransportTypeId))] public TransportType TransportTypeFk { get; set; }
        public long TruckTypeId { get; set; }
        [ForeignKey(nameof(TruckTypeId))] public TrucksType TrucksTypeFk { get; set; }
        public int? OriginCityId { get; set; }

        [ForeignKey(nameof(OriginCityId))] public City OriginCity { get; set; }

        public int? DestinationCityId { get; set; }

        [ForeignKey(nameof(DestinationCityId))]
        public City DestinationCity { get; set; }

        /// <summary>
        /// ShipperId is a property tell us for who tenant is the p.p is created for 
        /// </summary>
        public int? DestinationTenantId { get; set; }

        [ForeignKey(nameof(DestinationTenantId))]
        public Tenant DestinationTenant { get; set; }

        public ShippingRequestRouteType? RouteType { get; set; }

        public ShippingTypeEnum ShippingTypeId { get; set; }

        // when the price package creator is carrier the total price is the Tachyon manage service price
        public decimal TotalPrice { get; set; }

        public PricePackageType Type { get; set; }

        public int? ProposalId { get; set; }

        [ForeignKey(nameof(ProposalId))] public PricePackageProposal Proposal { get; set; }

        public int? AppendixId { get; set; }

        [ForeignKey(nameof(AppendixId))] public PricePackageAppendix Appendix { get; set; }

        public PricePackageUsageType UsageType { get; set; }

        public decimal? PricePerAdditionalDrop { get; set; }

        // this will have a value when the P.P created by carrier 
        public decimal? DirectRequestPrice { get; set; }

        public int? OriginCountryId { get; set; }

        public ICollection<ServiceArea> ServiceAreas { get; set; }

        public string ProjectName { get; set; }

        public string ScopeOfWork { get; set; }

        public RoundTripType? RoundTrip { get; set; }

        public long? OriginFacilityPortId { get; set; }
        
        public long? DestinationFacilityPortId { get; set; }

    }
}