using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Tracking.Dto
{
   public class TrackingByWaybillRoutPointDto
    {
        public string WaybillNumber { get; set; }
        public string MasterWaybillNumber { get; set; }
        public string CarrierName { get; set; }
        public string ShipperName { get; set; }
        public string TripStatus { get; set; }
        public string DropOffStatus { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public string PickupDate{ get; set; }
        public string DropOffDate { get; set; }
    }
}
