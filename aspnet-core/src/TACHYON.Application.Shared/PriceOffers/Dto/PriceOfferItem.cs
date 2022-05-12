namespace TACHYON.PriceOffers.Dto
{
    public class PriceOfferItem
    {
        public long? SourceId { get; set; }
        public PriceOfferType PriceType { get; set; }
        public string ItemName { get; set; }
        public decimal? ItemPrice { get; set; }
        public decimal? ItemVatAmount { get; set; }
        public decimal? ItemTotalAmount { get; set; }


        public decimal? ItemSubTotalAmountWithCommission { get; set; }
        public decimal? ItemVatAmountWithCommission { get; set; }
        public decimal? ItemTotalAmountWithCommission { get; set; }

        public decimal ItemsTotalPricePreCommissionPreVat { get; set; }

        public decimal? TotalAmount { get; set; }
        public decimal? SubTotalAmount { get; set; }
        public decimal? VatAmount { get; set; }


        public decimal? TotalAmountWithCommission { get; set; }
        public decimal? SubTotalAmountWithCommission { get; set; }
        public decimal? VatAmountWithCommission { get; set; }

        public decimal? ItemCommissionAmount { get; set; }

        public decimal? CommissionAmount { get; set; }
        public PriceOfferCommissionType CommissionType { get; set; }
        public decimal? CommissionPercentageOrAddValue { get; set; }

        public int Quantity { get; set; } = 1;
    }
}