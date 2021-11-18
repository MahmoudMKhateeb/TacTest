using System;
using System.Collections.Generic;
using System.Text;
using TACHYON.Routs.RoutPoints;
using TACHYON.Shipping.ShippingRequestTrips;

namespace TACHYON.Tracking
{
    public class PointTransactionArgs
    {
        public ShippingRequestTrip Trip { get; set; }
        public RoutPoint Point { get; set; }
        public string ConfirmationCode { get; set; }
        public string Action { get; set; }
    }
}