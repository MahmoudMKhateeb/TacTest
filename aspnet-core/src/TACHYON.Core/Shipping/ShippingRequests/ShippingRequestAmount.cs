namespace TACHYON.Shipping.ShippingRequests
{
    public class ShippingRequestAmount
    {
        /// <summary>
        ///  Carrier price
        /// </summary>
        public decimal CarrierPrice { get; set; }

        /// <summary>
        /// Price without vat amount
        /// </summary>
        public decimal SubTotalAmount { get; set; }

        /// <summary>
        /// Total price include vat amount
        /// </summary>
        public decimal TotalAmount { get; set; }

        public decimal VatAmount { get; set; }
        public decimal TotalCommission { get; set; }

        public decimal VatSetting { get; set; }

        public decimal CommissionValueSetting { get; set; }
        public decimal PercentCommissionSetting { get; set; }
        public decimal MinCommissionValueSetting { get; set; }

        public decimal ActualCommissionValue { get; set; }
        public decimal ActualPercentCommission { get; set; }

        public decimal ActualMinCommissionValue { get; set; }
    }
}