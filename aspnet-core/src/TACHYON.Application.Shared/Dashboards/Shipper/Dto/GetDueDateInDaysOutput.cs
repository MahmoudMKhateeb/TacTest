using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Dashboards.Shipper.Dto
{
    public class GetDueDateInDaysOutput
    {
        public long Count { get; set; }
        public string TimeUnit { get; set; }
        public bool IsExpired { get; set; }
    }
}
