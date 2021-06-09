using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using TACHYON.MultiTenancy;
using TACHYON.Shipping.ShippingRequests;

namespace TACHYON.PriceOffers
{
    [Table("PriceOffers")]
    public class PriceOffer : FullAuditedEntity<long>, IMustHaveTenant
    {
        public long? ReferenceNumber { get; set; }
        public long? ParentId { get; set; }
        [ForeignKey(nameof(ParentId))]
        public PriceOffer PriceOfferFK { get; set; }
        public long ShippingRequestId { get; set; }

        [ForeignKey(nameof(ShippingRequestId))]
        public ShippingRequest ShippingRequestFK { get; set; }
        /// <summary>
        /// Get id from source entity
        /// </summary>
        public long? SourceId { get; set; }

        public int TenantId { get; set; }
        [ForeignKey(nameof(TenantId))]
        public Tenant Tenant { get; set; }

        public PriceOfferChannel Channel { get; set; }
        public PriceOfferStatus Status { get; set; }
        public PriceOfferType PriceType { get; set; }
        #region Invoice
        #region Single  pricing for carrier
        public decimal ItemPrice { get; set; }
        public decimal ItemVatAmount { get; set; }
        public decimal ItemTotalAmount { get; set; }

        #endregion
        #region Single item  pricing with commission for shipper or tachyon dealer
        public decimal ItemSubTotalAmountWithCommission { get; set; }
        public decimal ItemVatAmountWithCommission { get; set; }
        public decimal ItemTotalAmountWithCommission { get; set; }
        #endregion
        #region Pricing Totals of Items and Details
        public decimal TotalAmount { get; set; }
        public decimal SubTotalAmount { get; set; }
        public decimal VatAmount { get; set; }


        public decimal TotalAmountWithCommission { get; set; }
        public decimal SubTotalAmountWithCommission { get; set; }
        public decimal VatAmountWithCommission { get; set; }
        #endregion
        public decimal TaxVat { get; set; }
        #endregion
        #region Commission
        public PriceOfferCommissionType CommissionType { get; set; }
        public decimal ItemCommissionAmount { get; set; }
        public decimal CommissionPercentageOrAddValue { get; set; }
        public decimal CommissionAmount { get; set; }
        #endregion

        public int Quantity { get; set; } = 1;
        public ICollection<PriceOfferDetail> PriceOfferDetails { get; set; } = new List<PriceOfferDetail>();
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
