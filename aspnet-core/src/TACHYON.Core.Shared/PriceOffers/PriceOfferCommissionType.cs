using System.ComponentModel;

namespace TACHYON.PriceOffers
{
    public enum PriceOfferCommissionType : byte
    {
        [Description("NoCommission")]
        None = 0,
        [Description("CommissionPercentage")]
        Percentage = 1,
        [Description("CommissionValue")]
        Value = 2,
        [Description("CommissionMinimumValue")]
        MinValue = 3
    }
}
