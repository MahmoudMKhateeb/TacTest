using Abp.Application.Services.Dto;
using System;

namespace TACHYON.Shipping.ShippingRequestUpdate
{
    public class ShippingRequestUpdateListDto : EntityDto<Guid>
    {
        public long? ShippingRequestId { get; set; }

        public Guid EntityLogId { get; set; }
        
        public long PriceOfferId { get; set; }
        
        public string Status { get; set; }

        public DateTime CreationTime { get; set; }
    }
}