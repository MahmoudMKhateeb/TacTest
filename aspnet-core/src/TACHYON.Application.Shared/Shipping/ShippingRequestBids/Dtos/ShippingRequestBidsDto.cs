using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Shipping.ShippingRequestBids.Dtos
{
    public class ShippingRequestBidDto :EntityDto<long>
    {
       public decimal price { get; set; }
        public long ShippingRequestId { get; set; }
        public bool IsAccepted { get; set; }
        public bool IsRejected { get; set; }
        public string CancledReason { get; set; }
        public DateTime? AcceptedDate { get; set; }
        public DateTime? CanceledDate { get; set; }
        public DateTime CreationTime { get; set; }
        public string CarrierName { get; set; }
    }
}
