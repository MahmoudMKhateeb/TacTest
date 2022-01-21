using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Shipping.ShippingRequestBids.Dtos
{
    public class CreatOrEditShippingRequestBidDto
    {
        public long? Id { get; set; }
        public long ShippingRequestId { get; set; }
        public decimal BasePrice { get; set; }
    }
}