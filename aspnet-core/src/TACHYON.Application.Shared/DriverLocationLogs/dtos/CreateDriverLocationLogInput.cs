using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.DriverLocationLogs.dtos
{
    public class CreateDriverLocationLogInput
    {
        public long? TripId { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
    }
}