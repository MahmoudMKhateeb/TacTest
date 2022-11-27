using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Shipping.ShippingRequests.Dtos.Dedicated
{
    public class ReplaceDriverDto
    {
        public long OriginalDedicatedDriverId { get; set; }
        public long DriverUserId { get; set; }
        public string ReplacementReason { get; set; }
        public int ReplacementIntervalInDays { get; set; }
    }
}
