using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System.ComponentModel.DataAnnotations.Schema;
using TACHYON.MultiTenancy;

namespace TACHYON.Shipping.ShippingRequests
{

    [Table("ShippingRequestPricings")]
    public class ShippingRequestPricing : FullAuditedEntity<long>, IMustHaveTenant
    {
        public long? ParentId { get; set; }
        [ForeignKey(nameof(ParentId))]
        public ShippingRequestPricing ShippingRequestPricingFK { get; set; }
        public long ShippingRequestId { get; set; }

        [ForeignKey(nameof(ShippingRequest))]
        public ShippingRequest ShippingRequestFK { get; set; }
        /// <summary>
        /// Get id from source entity
        /// </summary>
        public long SourceId { get; set; }

        public int TenantId { get; set; }
        [ForeignKey(nameof(TenantId))]
        public Tenant Tenant { get; set; }

        public ShippingRequestPricingChannel Channel { get; set; }
        public ShippingRequestPricingStatus Status { get; set; }
        #region Invoice
        #region Single trip pricing
        public decimal TripPrice { get; set; }
        public decimal TripVatAmount { get; set; }
        public decimal TripTotalAmount { get; set; }
        public decimal TripCommissionValue { get; set; }
        public decimal TripCommissionPercentage { get; set; }
        public decimal TripCommissionAmount { get; set; }
        #endregion
        #region Pricing Totals of trips and vass
        public decimal TotalAmount { get; set; }
        public decimal SubTotalAmount { get; set; }
        public decimal VatAmount { get; set; }
        #endregion
        public decimal TaxVat { get; set; }
        #endregion
        #region Commission
        public ShippingRequestCommissionType CommissionType { get; set; }
        public decimal CommissionValue { get; set; }
        public decimal CommissionPercentage { get; set; }
        public decimal CommissionAmount { get; set; }
        #endregion


        /// <summary>
        /// If shipper reject offer, will place reason of rejected
        /// </summary>
        public string RejectedReason { get; set; }
        /// <summary>
        /// If the shipper or TAD view this pricing,Help us when the carrier edit the price to check if the is view sent notification to stachholder the the carrier update price else sent there new price add.
        /// </summary>
        public bool IsView { get; set; }

    }
}
