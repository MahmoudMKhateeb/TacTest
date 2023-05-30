using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Dashboards.Host.TMS_HostDto
{
    public class GetRegisteredCompaniesNumberOutput
    {
        public long TotalNumber { get; set; }
        public long ShippersNumber { get; set; }
        public long CarriersCount { get; set; }
    }
}
