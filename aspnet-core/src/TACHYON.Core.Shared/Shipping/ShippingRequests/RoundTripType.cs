using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace TACHYON.Shipping.ShippingRequests
{
    public enum RoundTripType : byte
    {
        [Description("With return trip")]
        WithReturnTrip =1,
        [Description("Without return trip")]
        WithoutReturnTrip = 2,
        [Description("Two way routs with port shuttling")]
        TwoWayRoutsWithPortShuttling = 3,
        [Description("Two way routs without port shuttling")]
        TwoWayRoutsWithoutPortShuttling = 4,
        [Description("One way rout with port shuttling")]
        OneWayRoutWithoutPortShuttling = 5,
    }
}
