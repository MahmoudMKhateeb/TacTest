using System.ComponentModel;

namespace TACHYON.PriceOffers
{
    public enum PriceOfferCommissionType : byte
    {
        [Description("CommissionPercentage")]
        CommissionPercentage = 1,
        [Description("CommissionValue")]
        CommissionValue = 2,
        [Description("CommissionMinimumValue")]
        CommissionMinimumValue = 3
    }
}
