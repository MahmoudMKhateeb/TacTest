using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Dashboards.Host.TMS_HostDto
{
    public class GetTopOWorstRatedTenantsInput
    {
        public byte RateType { get; set; }// 1 if top, 2 if worst
        public byte EditionType { get; set; } // 1 shipper, 2 carriers, 3 broker or carrier saas
    }
}
