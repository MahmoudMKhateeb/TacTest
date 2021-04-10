using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Shipping.ShippingRequests
{
    public enum ShippingRequestStatus : byte
    {
        PrePrice,
        PostPrice,
        Started ,//The driver start working on trips
        Rejected,
        Cancled,
        Finished
    }
}
