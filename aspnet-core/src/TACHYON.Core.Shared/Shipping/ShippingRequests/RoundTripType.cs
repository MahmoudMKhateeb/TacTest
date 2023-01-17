using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Shipping.ShippingRequests
{
    public enum RoundTripType : byte
    {
        WithReturnTrip =1,
        WithoutReturnTrip = 2,
        TwoWayRoutsWithPortShuttling = 3,
        TwoWayRoutsWithoutPortShuttling = 4,
        OneWayRoutWithPortShuttling = 5,
    }
}
