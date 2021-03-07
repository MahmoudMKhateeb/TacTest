using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Shipping.Drivers.Dto
{
   public class ShippingRequestTripDriverRoutePointDto
    {
        public string DisplayName { get; set; }
        public long? ParentId { get; set; }

        public string Address { get; set; }

        public Point Location { get; set; }


    }
}
