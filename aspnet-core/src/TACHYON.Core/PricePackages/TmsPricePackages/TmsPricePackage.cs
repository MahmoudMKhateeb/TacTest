﻿using System.ComponentModel.DataAnnotations.Schema;
using TACHYON.Cities;
using TACHYON.MultiTenancy;
using TACHYON.PricePackages.PricePackageAppendices;
using TACHYON.PricePackages.PricePackageProposals;
using TACHYON.Shipping.ShippingRequests;
using TACHYON.Shipping.ShippingTypes;

namespace TACHYON.PricePackages.TmsPricePackages
{
    [Table("TmsPricePackages")]
    public class TmsPricePackage : BasePricePackage
    {
        public int? OriginCityId { get; set; }

        [ForeignKey(nameof(OriginCityId))]
        public City OriginCity { get; set; }

        public int? DestinationCityId { get; set; }

        [ForeignKey(nameof(DestinationCityId))]
        public City DestinationCity { get; set; }

        /// <summary>
        /// ShipperId is a property tell us for who tenant is the p.p is created for 
        /// </summary>
        public int? DestinationTenantId { get; set; }

        [ForeignKey(nameof(DestinationTenantId))]
        public Tenant DestinationTenant { get; set; }

        public ShippingRequestRouteType RouteType { get; set; }
        
        public int? ShippingTypeId { get; set; }

        [ForeignKey(nameof(ShippingTypeId))]
        public ShippingType ShippingType { get; set; }
        
        // Total price = Commission value + price 
        // commission value = if (commission type == value) Commission;
        // else if (commission type == percentage) => (Commission * Price) / 100
        public decimal TotalPrice { get; set; }

        public PricePackageType Type { get; set; }

        public int? ProposalId { get; set; }

        [ForeignKey(nameof(ProposalId))]
        public PricePackageProposal Proposal { get; set; }

        public int? AppendixId { get; set; }

        [ForeignKey(nameof(AppendixId))]
        public PricePackageAppendix Appendix { get; set; }

    }
}