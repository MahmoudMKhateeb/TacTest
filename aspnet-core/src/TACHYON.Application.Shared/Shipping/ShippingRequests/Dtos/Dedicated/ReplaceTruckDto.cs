using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Shipping.ShippingRequests.Dtos.Dedicated
{
    public class ReplaceTruckDto
    {
        public long OriginalDedicatedTruckId { get; set; }
        public long TruckId { get; set; }
        public string ReplacementReason { get; set; }
        public int ReplacementIntervalInDays { get; set; }
    }
}
