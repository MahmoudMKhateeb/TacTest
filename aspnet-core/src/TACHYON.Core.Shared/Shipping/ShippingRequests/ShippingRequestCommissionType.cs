using System.ComponentModel;

namespace TACHYON.Shipping.ShippingRequests
{
    public enum ShippingRequestCommissionType:byte
    {
        [Description("CommissionPercentage")]
        Percentage=1,
        [Description("CommissionValue")]
        Value =2
    }
}
