using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Dashboards.Host.TMS_HostDto
{
    public class GetNeedsActionTripsAndRequestsOutput
    {
        public string ActionName { get; set; }
        public string OriginCity {get; set; }
        public List<string> DestinationCity { get; set; }
        public string WaybillOrRequestReference { get; set; }
    }
}
