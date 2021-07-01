using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace TACHYON.Shipping.ShippingRequests
{
    [Flags]
    public enum ShippingRequestStatus : byte
    {
        [Description("New")]
        PrePrice = 0,
        [Description("Confirmed")]
        PostPrice = 1,
        [Description("NeedsAction")]
        NeedsAction = 2,//carriers send price and waiting for response
        [Description("Expired")]
        Expired = 3,
        [Description("Cancled")]
        Cancled = 4,
        [Description("Completed")]
        Completed = 5,
        AcceptedAndWaitingCarrier=6

    }
}
