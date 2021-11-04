using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.DriverLocationLogs.dtos
{
    public class GetAllDriverLocationLogsInput
    {
        public long DriverId { get; set; }
        public string Filter { get; set; }
        public int? TripId { get; set; }
    }
}