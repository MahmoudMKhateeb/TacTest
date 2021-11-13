using Abp.Domain.Entities.Auditing;

namespace TACHYON.PriceOffers.Base
{
    public class PriceOfferBase : FullAuditedEntity<long>, IPriceOfferBase
    {
        /// <summary>
        /// Get id from source entity
        /// </summary>
        public long? SourceId { get; set; }

        public PriceOfferType PriceType { get; set; }
        #region Invoice


        #region Single  pricing for carrier
        public decimal ItemPrice { get; set; }
        /// <summary>
        /// ItemPrice * TaxVat
        /// </summary>
        public decimal ItemVatAmount { get; set; }
        /// <summary>
        ///  ItemPrice + ItemVatAmount;
        /// </summary>
        public decimal ItemTotalAmount { get; set; }



        #endregion


        #region Single item  pricing with commission for shipper or tachyon dealer
        /// <summary>
        /// ItemPrice + ItemCommissionAmount
        /// </summary>
        public decimal ItemSubTotalAmountWithCommission { get; set; }

        /// <summary>
        /// ItemSubTotalAmountWithCommission * TaxVat
        /// </summary>
        public decimal ItemVatAmountWithCommission { get; set; }

        /// <summary>
        ///  ItemSubTotalAmountWithCommission + ItemVatAmountWithCommission;
        /// (ItemPrice + ItemCommissionAmount) + ((ItemPrice + ItemCommissionAmount) * TaxVat)
        /// </summary>
        public decimal ItemTotalAmountWithCommission { get; set; }
        #endregion


        #region Pricing Totals of Items and Details
        /// <summary>
        /// Price Per trip * Number of Trips.
        /// </summary>
        public decimal ItemsTotalPricePreCommissionPreVat { get; set; }


        /// <summary>
        /// ( ItemTotalAmount * Quantity ) + PriceOfferDetails.Sum(x => x.TotalAmount)
        /// </summary>
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// (ItemPrice *  Quantity) + PriceOfferDetails.Sum(x => x.SubTotalAmount)
        /// </summary>
        public decimal SubTotalAmount { get; set; }


        /// <summary>
        /// (ItemVatAmount  Quantity) + PriceOfferDetails.Sum(x => x.VatAmount)
        /// </summary>
        public decimal VatAmount { get; set; }

        /// <summary>
        /// (ItemTotalAmountWithCommission *  Quantity) + PriceOfferDetails.Sum(x => x.TotalAmountWithCommission)
        /// </summary>
        public decimal TotalAmountWithCommission { get; set; }

        /// <summary>
        /// (ItemSubTotalAmountWithCommission *  Quantity) + PriceOfferDetails.Sum(x => x.SubTotalAmountWithCommission)
        /// </summary>
        public decimal SubTotalAmountWithCommission { get; set; }

        /// <summary>
        /// (ItemVatAmountWithCommission *  Quantity) + PriceOfferDetails.Sum(x => x.VatAmountWithCommission)
        /// </summary>
        public decimal VatAmountWithCommission { get; set; }
        #endregion

        #endregion

        #region Commission
        /// <summary>
        /// set of predefined settings for the Commission that is applied to Shippers according to their subscriptions type.
        /// </summary>
        public PriceOfferCommissionType CommissionType { get; set; }
        public decimal ItemCommissionAmount { get; set; }
        public decimal CommissionPercentageOrAddValue { get; set; }

        /// <summary>
        /// ( ItemCommissionAmount *  Quantity) + PriceOfferDetails.Sum(x => x.CommissionAmount)
        /// </summary>
        public decimal CommissionAmount { get; set; }
        #endregion

        /// <summary>
        /// shippingRequest.NumberOfTrips
        /// </summary>
        public int Quantity { get; set; } = 1;

    }
}
