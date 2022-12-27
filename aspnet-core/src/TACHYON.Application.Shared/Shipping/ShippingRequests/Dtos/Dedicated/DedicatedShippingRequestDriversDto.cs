using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Shipping.ShippingRequests.Dtos.Dedicated
{
    public class DedicatedShippingRequestDriversDto : EntityDto<long>
    {
        public string DriverName { get; set; }
        public string PhoneNumber { get; set; }
        public string AccountNumber { get; set; }
        public string Status { get; set; }
        public string CarrierName { get; set; }
        public string Duration { get; set; }
        public string ShippingRequestReference { get; set; }

    }
}
