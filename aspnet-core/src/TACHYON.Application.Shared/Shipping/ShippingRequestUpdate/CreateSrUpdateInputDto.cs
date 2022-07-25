using System;

namespace TACHYON.Shipping.ShippingRequestUpdate
{
    public class CreateSrUpdateInputDto 
    {
        public long ShippingRequestId { get; set; }
        
        public Guid EntityLogId { get; set; }
    }
}