using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Dashboards.Host.TMS_HostDto
{
    public class GetTopTenantsCreatedTripsOutput
    {
        public List<GetTenantsCountWithRateOutput> Shippers { get; set; }
        public List<GetTenantsCountWithRateOutput> Carriers { get; set; }
    }
}
