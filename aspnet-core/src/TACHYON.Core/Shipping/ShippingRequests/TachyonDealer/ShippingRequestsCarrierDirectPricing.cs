using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System.ComponentModel.DataAnnotations.Schema;
using TACHYON.MultiTenancy;

namespace TACHYON.Shipping.ShippingRequests.TachyonDealer
{
    [Table("ShippingRequestsCarrierDirectPricing")]
    public class ShippingRequestsCarrierDirectPricing : FullAuditedEntity, IMustHaveTenant
    {
        public int TenantId { get; set; }
        [ForeignKey("TenantId")]
        public Tenant Tenant { get; set; }
        public int CarrirerTenantId { get; set; }
        [ForeignKey(nameof(CarrirerTenantId))]

        public Tenant Carrier { get; set; }

        public long RequestId { get; set; }

        [ForeignKey("RequestId")]
        public ShippingRequest Request { get; set; }
        /// <summary>
        ///  Carrier price
        /// </summary>
        public decimal? Price { get; set; }

        public ShippingRequestsCarrierDirectPricingStatus Status { get; set; }
        public string RejetcReason { get; set; }

        public ShippingRequestsCarrierDirectPricing() { }
        public ShippingRequestsCarrierDirectPricing(int TenantId,long RequestId,long CreatorUserId,int CarrirerTenantId) {
            this.TenantId = TenantId;
            this.RequestId = RequestId;
            this.CreatorUserId = CreatorUserId;
            this.CarrirerTenantId = CarrirerTenantId;
        }
    }
}
