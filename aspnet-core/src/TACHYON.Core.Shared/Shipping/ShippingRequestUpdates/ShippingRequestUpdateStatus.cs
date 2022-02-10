using System.ComponentModel;

namespace TACHYON.Shipping.ShippingRequestUpdates
{
    public enum ShippingRequestUpdateStatus
    {
        // The Description Here is a Localizable String Key
        
        [Description("PendingUpdate")]
        None = 1,
        [Description("RepricedShippingRequestUpdateAction")]
        Repriced = 2,
        [Description("KeepSamePriceShippingRequestUpdateAction")]
        KeepSamePrice = 3,
        [Description("DismissedShippingRequestUpdateAction")]
        Dismissed = 4
    }
}