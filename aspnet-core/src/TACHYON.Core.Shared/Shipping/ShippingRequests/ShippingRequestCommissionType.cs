using System.ComponentModel;

namespace TACHYON.Shipping.ShippingRequests
{
    public enum ShippingRequestCommissionType:byte
    {
        [Description("NoCommission")]
        None =0,
        [Description("CommissionPercentage")]
        Percentage=1,
        [Description("CommissionValue")]
        Value =2,
        [Description("CommissionMinimumValue")]
        MinValue = 3
    }
}
