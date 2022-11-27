using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Shipping.ShippingRequests.Dtos.Dedicated
{
    public class RequestReplaceTruckInput
    {
        public long DedicatedTruckId { get; set; }
        public string ReplacementReason { get; set; }
        public int ReplacementIntervalInDays { get; set; }
    }
}
