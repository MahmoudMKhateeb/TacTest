using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Shipping.ShippingRequestBids.Dtos
{
    public class CancelShippingRequestBidInput
    {
        public long ShippingRequestBidId { get; set; }
        public string CancledReason { get; set; }
    }
}
