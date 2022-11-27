using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Shipping.ShippingRequests.Dtos.Dedicated
{
    public class ReplaceDriverInput
    {
        public long ShippingRequestId { get; set; }
        public List<ReplaceDriverDto> ReplaceDriverDtos { get; set; }
    }
}
