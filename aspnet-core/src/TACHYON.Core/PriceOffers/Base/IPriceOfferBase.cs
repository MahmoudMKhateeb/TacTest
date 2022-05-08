// unset

namespace TACHYON.PriceOffers.Base
{
    public interface IPriceOfferBase
    {
        /// <summary>
        /// Get id from source entity
        /// </summary>
        long? SourceId { get; set; }

        PriceOfferType PriceType { get; set; }
        decimal ItemPrice { get; set; }

        /// <summary>
        /// ItemPrice * TaxVat
        /// </summary>
        decimal ItemVatAmount { get; set; }

        /// <summary>
        ///  ItemPrice + ItemVatAmount;
        /// </summary>
        decimal ItemTotalAmount { get; set; }

        /// <summary>
        /// ItemPrice + ItemCommissionAmount
        /// </summary>
        decimal ItemSubTotalAmountWithCommission { get; set; }

        /// <summary>
        /// ItemSubTotalAmountWithCommission * TaxVat
        /// </summary>
        decimal ItemVatAmountWithCommission { get; set; }

        /// <summary>
        ///  ItemSubTotalAmountWithCommission + ItemVatAmountWithCommission;
        /// (ItemPrice + ItemCommissionAmount) + ((ItemPrice + ItemCommissionAmount) * TaxVat)
        /// </summary>
        decimal ItemTotalAmountWithCommission { get; set; }

        /// <summary>
        /// Price Per trip * Number of Trips.
        /// </summary>
        decimal ItemsTotalPricePreCommissionPreVat { get; set; }

        /// <summary>
        /// ( ItemTotalAmount * Quantity ) + PriceOfferDetails.Sum(x => x.TotalAmount)
        /// </summary>
        decimal TotalAmount { get; set; }

        /// <summary>
        /// (ItemPrice *  Quantity) + PriceOfferDetails.Sum(x => x.SubTotalAmount)
        /// </summary>
        decimal SubTotalAmount { get; set; }

        /// <summary>
        /// (ItemVatAmount  Quantity) + PriceOfferDetails.Sum(x => x.VatAmount)
        /// </summary>
        decimal VatAmount { get; set; }

        /// <summary>
        /// (ItemTotalAmountWithCommission *  Quantity) + PriceOfferDetails.Sum(x => x.TotalAmountWithCommission)
        /// </summary>
        decimal TotalAmountWithCommission { get; set; }

        /// <summary>
        /// (ItemSubTotalAmountWithCommission *  Quantity) + PriceOfferDetails.Sum(x => x.SubTotalAmountWithCommission)
        /// </summary>
        decimal SubTotalAmountWithCommission { get; set; }

        /// <summary>
        /// (ItemVatAmountWithCommission *  Quantity) + PriceOfferDetails.Sum(x => x.VatAmountWithCommission)
        /// </summary>
        decimal VatAmountWithCommission { get; set; }

        /// <summary>
        /// set of predefined settings for the Commission that is applied to Shippers according to their subscriptions type.
        /// </summary>
        PriceOfferCommissionType CommissionType { get; set; }

        decimal ItemCommissionAmount { get; set; }
        decimal CommissionPercentageOrAddValue { get; set; }

        /// <summary>
        /// ( ItemCommissionAmount *  Quantity) + PriceOfferDetails.Sum(x => x.CommissionAmount)
        /// </summary>
        decimal CommissionAmount { get; set; }

        /// <summary>
        /// shippingRequest.NumberOfTrips
        /// </summary>
        int Quantity { get; set; }
    }
}