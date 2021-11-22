using System;
using System.Collections.Generic;
using System.Text;
using TACHYON.Routs.RoutPoints;
using TACHYON.Shipping.ShippingRequestTrips;

namespace TACHYON.Tracking
{
    public class PointTransactionArgs
    {
        public long PointId { get; set; }
        public string Code { get; set; }
    }
}