using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System.ComponentModel.DataAnnotations.Schema;
using TACHYON.MultiTenancy;
using TACHYON.PricePackages;
using TACHYON.Shipping.ShippingRequests;

namespace TACHYON.Shipping.DirectRequests
{
    [Table("ShippingRequestDirectRequests")]

    public class ShippingRequestDirectRequest : FullAuditedEntity<long>, IMustHaveTenant
    {
        public int TenantId { get; set; }
        [ForeignKey(nameof(TenantId))]
        public Tenant Tenant { get; set; }
        public long? PricePackageOfferId { get; set; }
        [ForeignKey(nameof(PricePackageOfferId))]
        public PricePackageOffer PricePackageOfferFK { get; set; }
        public int CarrierTenantId { get; set; }

        [ForeignKey(nameof(CarrierTenantId))]

        public Tenant Carrier { get; set; }

        public long ShippingRequestId { get; set; }

        [ForeignKey(nameof(ShippingRequestId))]
        public ShippingRequest ShippingRequestFK { get; set; }

        public ShippingRequestDirectRequestStatus Status { get; set; }

        public string RejetcReason { get; set; }

    }
}