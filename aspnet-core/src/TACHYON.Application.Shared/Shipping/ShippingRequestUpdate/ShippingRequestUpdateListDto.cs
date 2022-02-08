using Abp.Application.Services.Dto;
using System;
using TACHYON.Shipping.ShippingRequestUpdates;

namespace TACHYON.Shipping.ShippingRequestUpdate
{
    public class ShippingRequestUpdateListDto : EntityDto<Guid>
    {
        public long? ShippingRequestId { get; set; }

        public Guid EntityLogId { get; set; }
        
        public long PriceOfferId { get; set; }
        
        public ShippingRequestUpdateStatus Status { get; set; }
    }
}