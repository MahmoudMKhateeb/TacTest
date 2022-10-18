using System.ComponentModel.DataAnnotations.Schema;
using TACHYON.Cities;
using TACHYON.MultiTenancy;
using TACHYON.PricePackages.PricePackageProposals;
using TACHYON.Shipping.ShippingRequests;

namespace TACHYON.PricePackages
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
        public int? ShipperId { get; set; }

        [ForeignKey(nameof(ShipperId))]
        public Tenant Shipper { get; set; }

        public ShippingRequestRouteType RouteType { get; set; }

        public decimal DirectRequestPrice { get; set; }

        public decimal TachyonManagePrice { get; set; }
        
        public decimal DirectRequestCommission { get; set; }
        
        public decimal TachyonManageCommission { get; set; }

        /// <summary>
        /// The total price is the `DirectRequestPrice` + `DirectRequestCommission`
        /// if 
        /// note that the user can set it manually
        /// </summary>
        public decimal DirectRequestTotalPrice { get; set; }
        
        /// <summary>
        /// The total price is the `TachyonManagePrice` + `TachyonManageCommission`
        /// note that the user can set it manually
        /// </summary>
        public decimal TachyonManageTotalPrice { get; set; }

        public PricePackageCommissionType CommissionType { get; set; }
        
        public PricePackageType Type { get; set; }

        public int? ProposalId { get; set; }

        [ForeignKey(nameof(ProposalId))]
        public PricePackageProposal Proposal { get; set; }
    }
}