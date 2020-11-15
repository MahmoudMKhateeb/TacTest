using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Shipping.ShippingRequestBids.Dtos
{
    public class ShippingRequestBidInput
    {
        public long ShippingRequestBidId { get; set; }
        public string CancledReason { get; set; }
    }
}
