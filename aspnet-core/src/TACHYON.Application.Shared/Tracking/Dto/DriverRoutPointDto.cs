using System;
using System.Collections.Generic;
using System.Text;
using TACHYON.Shipping.Trips;

namespace TACHYON.Tracking.Dto
{
    public class DriverRoutPointDto
    {
        public ShippingRequestTripStatus TripStatus { get; set; }
        public int TripId { get; set; }
        public long? WaybillNumber { get; set; }
        public bool HasAccident { get; set; }
        public long ShippingRequestId { get; set; }
        public List<RoutPointsMobileDto> RoutPoint { get; set; }
    }
}