using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Shipping.ShippingRequests.Dtos.Dedicated
{
    public class UpdateRequestKPIInput
    {
        public long ShippingRequestId { get; set; }
        public double KPI { get; set; }
    }
}
