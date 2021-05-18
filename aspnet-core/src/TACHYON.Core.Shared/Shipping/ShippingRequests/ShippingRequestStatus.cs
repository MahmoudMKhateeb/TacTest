using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace TACHYON.Shipping.ShippingRequests
{
    public enum ShippingRequestStatus : byte
    {
        [Description("New")]
        PrePrice,
        [Description("Confiremed")]
        PostPrice,
        [Description("NeedsAction")]
        NeedsAction ,//carriers send price
        [Description("Expired")]
        Expired,
        [Description("Cancled")]
        Cancled,
        [Description("Completed")]
        Completed

    }
}
