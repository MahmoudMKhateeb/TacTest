using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Shipping.ShippingRequests.Dtos.Dedicated
{
    public class RequestReplaceDriverInput
    {
        public long DedicatedDriverId { get; set; }
        public string ReplacementReason { get; set; }
        public int ReplacementIntervalInDays { get; set; }
    }
}
