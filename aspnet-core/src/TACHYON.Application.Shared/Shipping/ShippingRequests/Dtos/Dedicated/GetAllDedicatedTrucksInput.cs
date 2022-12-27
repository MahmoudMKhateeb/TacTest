using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Shipping.ShippingRequests.Dtos.Dedicated
{
    public class GetAllDedicatedTrucksInput
    {
        public string Filter { get; set; }
        public long? ShippingRequestId { get; set; }
    }
}
