using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Dashboards.Host.TMS_HostDto
{
    public class GetUpcomingTripsOutput
    {
        public string OrigiCity { get; set; }
        public string DestinationCity { get; set; }
        public string WaybillNumber { get; set; }
        public DateTime StartTripDate {get; set; }
    }
}
