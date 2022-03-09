using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Tracking.Dto
{
   public class TrackingByWaybillNumberTripDto
    {
        public string  ReferanceNumber { get; set; }
        public string CarrierName { get; set; }
        public string ShipperName { get; set; }
        public string WaybillNumber { get; set; }
        public string TripStatus { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public string StartWorking { get; set; }
        public string EndWorking { get; set; }
    }
}
