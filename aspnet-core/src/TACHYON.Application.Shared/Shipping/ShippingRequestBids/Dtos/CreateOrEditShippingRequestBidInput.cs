using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Shipping.ShippingRequestBids.Dtos
{
    public abstract class CreatOrEditShippingRequestBidDto
    {
        public long? Id { get; set; }
        public long ShippingRequestId { get; set; }
        public double price { get; set; }
    }
}
