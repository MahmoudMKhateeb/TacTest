using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace TACHYON.Shipping.ShippingRequests
{
    public enum ShippingRequestBidStatus : byte
    {
        [Description("New")]
        StandBy,
        [Description("OnGoing")]
        OnGoing,
        [Description("Confirmed")]
        Closed,
        Cancled
    }
}