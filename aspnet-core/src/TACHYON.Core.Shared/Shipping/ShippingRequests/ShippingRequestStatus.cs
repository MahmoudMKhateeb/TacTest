using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Shipping.ShippingRequests
{
    public enum ShippingRequestStatus : byte
    {
        New,
        Confiremed,
        NeedsAction ,//carriers send price
        Expired,
        Cancled,
        Completed
    }
}
