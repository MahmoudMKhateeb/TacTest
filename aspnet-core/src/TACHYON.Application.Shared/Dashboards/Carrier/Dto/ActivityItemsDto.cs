using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Dashboards.Carrier.Dto
{
    public class ActivityItemsDto
    {
        public int ActiveItems { get; set; }
        public int NotActiveItems { get; set; }
        public int InTransitItems { get; set; }
        public int TotalItemsCount { get; set; }
    }
}