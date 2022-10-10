using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Shipping.ShippingRequests.Dtos.Dedicated
{
    public class DedicatedShippingRequestTrucksDto: EntityDto<long>
    {
        public string TruckName { get; set; }
        public string Status { get; set; }
    }
}
