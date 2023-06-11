using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Dashboards.Host.TMS_HostDto
{
    public class GetTopOWorstRatedTenantsOutput
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int NumberOfTrips { get; set; }
        public decimal Rating { get; set; }
    }
}
