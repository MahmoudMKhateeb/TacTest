using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Shipping.ShippingRequestBids.Dtos
{
    public class ShippingRequestBidsDto :EntityDto<long>
    {
       public double price { get; set; }
        public long ShippingRequestId { get; set; }
        public bool IsCancled { get; set; }
        public bool IsAccepted { get; set; }

    }
}
