﻿using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Tracking.Dto
{
   public class TrackingByWaybillDto
    {
        public string CarrierName { get; set; }
        public string ShipperName { get; set; }
        public string WaybillNumber { get; set; }
        public string TripStatus { get; set; }
        public string DropOffStatus { get; set; }
    }
}