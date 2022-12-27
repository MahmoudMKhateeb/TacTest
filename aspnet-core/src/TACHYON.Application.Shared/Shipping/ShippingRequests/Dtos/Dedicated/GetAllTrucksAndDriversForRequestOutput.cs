using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Shipping.ShippingRequests.Dtos.Dedicated
{
    public class GetAllTrucksAndDriversForRequestOutput
    {
        public List<DedicatedShippingRequestDriversDto> dedicatedShippingRequestDriversDtos { get; set; }
        public List<DedicatedShippingRequestTrucksDto> dedicatedShippingRequestTrucksDtos { get; set; }
    }
}
