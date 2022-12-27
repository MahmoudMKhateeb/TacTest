using System;
using System.Collections.Generic;
using System.Text;
using TACHYON.Shipping.ShippingRequests.Dtos.Dedicated;

namespace TACHYON.Shipping
{
    public class AssignDedicatedTrucksAndDriversInput
    {
        public long ShippingRequestId { get; set; }
        public List<DedicatedTruckDto> TrucksList { get; set; }
        public List<DedicatedDriversDto> DriversList { get; set; }
    }
}
