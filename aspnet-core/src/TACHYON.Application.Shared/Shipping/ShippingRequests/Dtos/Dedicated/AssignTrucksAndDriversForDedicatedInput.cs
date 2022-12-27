using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Shipping.ShippingRequests.Dtos.Dedicated
{
    public class AssignTrucksAndDriversForDedicatedInput
    {
        public long ShippingRequestId { get; set; }
        public List<DedicatedShippingRequestTrucksAndDriversDto>  DedicatedShippingRequestTrucksAndDriversDtos { get; set; }
    }
}
