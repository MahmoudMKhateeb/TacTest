using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using TACHYON.MultiTenancy;
using TACHYON.Shipping.ShippingRequestBids;
using TACHYON.Shipping.ShippingRequests;

namespace TACHYON.TachyonPriceOffers
{
    [Table("TachyonPriceOffers")]
    public class TachyonPriceOffer : FullAuditedEntity,IMustHaveTenant
    {
        public int TenantId { get; set; }

        public int? CarrirerTenantId { get; set; }
        [ForeignKey("CarrirerTenantId")]
        public Tenant CarrirerTenant { get; set; }
        public virtual long ShippingRequestId { get; set; } 
        [ForeignKey("ShippingRequestId")]
        public ShippingRequest ShippingRequestFk { get; set; }
        public long? ShippingRequestBidId { get; set; }
        [ForeignKey("ShippingRequestBidId")]
        public ShippingRequestBid ShippingRequestBidFk { get; set; }
        public OfferStatus OfferStatus { get; set; }
        /// <summary>
        /// If shipper reject offer, will place reason of rejected
        /// </summary>
        public string RejectedReason { get; set; }
        /// <summary>
        /// Type of price that tachyon dealer send to shipper, if there is existing accepted price; ex:bidding the type will be bidding
        /// </summary>
        public PriceType PriceType { get; set; }

        public decimal? CarrierPrice { get; set; }
        /// <summary>
        /// Price without vat amount
        /// </summary>
        public decimal? SubTotalAmount { get; set; }
        /// <summary>
        /// Total price include vat amount
        /// </summary>
        public decimal TotalAmount { get; set; }
        public decimal? VatAmount { get; set; }
        public decimal? TotalCommission { get; set; }

        public decimal? VatSetting { get; set; }

        public decimal? CommissionValueSetting { get; set; }
        public decimal? PercentCommissionSetting { get; set; }
        public decimal? MinCommissionValueSetting { get; set; }

        public decimal? ActualCommissionValue { get; set; }
        public decimal? ActualPercentCommission { get; set; }

        public decimal? ActualMinCommissionValue { get; set; }

    }
}
