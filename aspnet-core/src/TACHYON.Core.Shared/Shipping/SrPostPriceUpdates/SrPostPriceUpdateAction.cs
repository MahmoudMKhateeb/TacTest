using System.ComponentModel;

namespace TACHYON.Shipping.SrPostPriceUpdates
{
    public enum SrPostPriceUpdateAction
    {
        [Description("NoActionYet")]
        Pending = 1,
        [Description("AcceptedPostPriceUpdateByCarrier")]
        Accept = 2,
        [Description("ChangePriceRequestedForPostPriceUpdate")]
        ChangePrice = 3,
        [Description("RejectedPostPriceUpdateByCarrier")]
        Reject = 4,
    }
}