using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using TACHYON.MultiTenancy;
using TACHYON.PriceOffers.Base;
using TACHYON.Shipping.ShippingRequests;

namespace TACHYON.PriceOffers
{
    [Table("PriceOffers")]
    public class PriceOffer : PriceOfferBase, IMustHaveTenant
    {
        public long? ReferenceNumber { get; set; }
        public long? ParentId { get; set; }
        [ForeignKey(nameof(ParentId))]
        public PriceOffer PriceOfferFk { get; set; }
        public long ShippingRequestId { get; set; }

        [ForeignKey(nameof(ShippingRequestId))]
        public ShippingRequest ShippingRequestFk { get; set; }

        public int TenantId { get; set; }
        [ForeignKey(nameof(TenantId))]
        public Tenant Tenant { get; set; }

        public PriceOfferChannel Channel { get; set; }
        public PriceOfferStatus Status { get; set; }


        #region Invoice

        #region Pricing Totals of Items and Details
        /// <summary>
        /// ItemsTotalPricePreCommissionPreVat * TaxVat
        /// </summary>
        public decimal ItemsTotalVatAmountPreCommission { get; set; }

        /// <summary>
        /// ItemCommissionAmount *  Quantity
        /// </summary>
        public decimal ItemsTotalCommission { get; set; }

        /// <summary>
        /// ItemsTotalCommission + ItemsTotalPricePreCommissionPreVat
        /// </summary>
        public decimal ItemsTotalPricePostCommissionPreVat { get; set; }

        /// <summary>
        /// ItemsTotalPricePostCommissionPreVat + ItemsTotalCommission
        /// </summary>
        public decimal ItemsTotalVatPostCommission { get; set; }

        /// <summary>
        /// Summation of a For All VAS (Price Per VAS * Number of VAS Items).
        /// </summary>
        public decimal DetailsTotalPricePreCommissionPreVat { get; set; }

        /// <summary>
        /// DetailsTotalPricePreCommissionPreVat * TaxVat
        /// </summary>
        public decimal DetailsTotalVatAmountPreCommission { get; set; }

        /// <summary>
        /// PriceOfferDetails.Sum(x => x.CommissionAmount)
        /// </summary>
        public decimal DetailsTotalCommission { get; set; }

        /// <summary>
        /// DetailsTotalCommission + DetailsTotalPricePreCommissionPreVat
        /// </summary>
        public decimal DetailsTotalPricePostCommissionPreVat { get; set; }

        /// <summary>
        /// DetailsTotalPricePostCommissionPreVat + DetailsTotalCommission
        /// </summary>
        public decimal DetailsTotalVatPostCommission { get; set; }


        #endregion

        /// <summary>
        /// AppSettings.HostManagement.TaxVat
        /// </summary>
        public decimal TaxVat { get; set; }
        #endregion


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