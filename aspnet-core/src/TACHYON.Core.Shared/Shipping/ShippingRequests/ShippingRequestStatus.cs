using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Shipping.ShippingRequests
{
    public enum ShippingRequestStatus : byte
    {
        PrePrice=1,
        PostPrice=2,
        Started=3 ,//The driver start working on trips
        Rejected=4,
        Cancled=5,
        Finished=6
    }
}
