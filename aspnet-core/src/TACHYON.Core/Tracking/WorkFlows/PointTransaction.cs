using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TACHYON.Routs.RoutPoints;
using TACHYON.Shipping.ShippingRequestTrips;

namespace TACHYON.Tracking.WorkFlows
{
    public class PointTransaction
    {
        public string Action { get; set; }
        public string Name { get; set; }
        public RoutePointStatus FromStatus { get; set; }
        public RoutePointStatus ToStatus { get; set; }
        public bool IsOptional { get; set; }
        public bool IsResolved { get; set; }
        public PickingType PickingType { get; set; }
        public Action<RoutPoint, ShippingRequestTrip> Func { get; set; }

    }
}