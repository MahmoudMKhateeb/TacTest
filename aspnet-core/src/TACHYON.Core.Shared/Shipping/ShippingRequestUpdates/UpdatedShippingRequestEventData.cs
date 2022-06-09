using Abp.Events.Bus;
using System;

namespace TACHYON.Shipping.ShippingRequestUpdates
{
    public class UpdatedShippingRequestEventData : EventData
    {
        public long ShippingRequestId { get; set; }

        public Guid EntityLogId { get; set; }
    }
}