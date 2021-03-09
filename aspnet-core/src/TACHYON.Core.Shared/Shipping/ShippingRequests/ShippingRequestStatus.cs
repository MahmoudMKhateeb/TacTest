using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Shipping.ShippingRequests
{
    public enum ShippingRequestStatus : byte
    {
        StandBy = 1,
        Started = 2,
        Finished= 3
    }
}
