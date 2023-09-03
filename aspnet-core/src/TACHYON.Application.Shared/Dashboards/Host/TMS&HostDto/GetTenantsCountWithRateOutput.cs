using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Dashboards.Host.TMS_HostDto
{
    public class GetTenantsCountWithRateOutput
    {
        public string Name { get; set; }
        public decimal Rate { get; set; }
        public int NumberOfTrips { get; set; }
    }
}
