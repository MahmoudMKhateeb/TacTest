using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Dashboards.Host.TMS_HostDto
{
    public class GetRequestsHeapMapOutput
    {
        public string CityName { get; set; }
        public string CityType { get; set; }
        public int CityId { get; set; }
        public int NumberOfRequests { get; set; }
    }
}
