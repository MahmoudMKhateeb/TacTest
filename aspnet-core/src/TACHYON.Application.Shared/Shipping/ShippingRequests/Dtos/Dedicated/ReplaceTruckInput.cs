using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Shipping.ShippingRequests.Dtos.Dedicated
{
    public class ReplaceTruckInput
    {
        public long ShippingRequestId { get; set; }
        public List<ReplaceTruckDto> ReplaceTruckDtos { get; set; }

    }
}
