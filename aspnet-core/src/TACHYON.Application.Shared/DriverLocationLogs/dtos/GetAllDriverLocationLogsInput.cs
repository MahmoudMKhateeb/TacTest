using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.DriverLocationLogs.dtos
{
    public class GetAllDriverLocationLogsInput
    {
        public DateTime? DateFilter { get; set; }
        public long DriverId { get; set; }
        public int? TripId { get; set; }
    }
}