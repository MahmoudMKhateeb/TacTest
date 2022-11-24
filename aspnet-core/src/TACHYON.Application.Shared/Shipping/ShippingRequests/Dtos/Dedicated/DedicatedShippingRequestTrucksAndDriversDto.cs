using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Shipping.ShippingRequests.Dtos.Dedicated
{
    public class DedicatedShippingRequestTrucksAndDriversDto
    {
        public long TruckId { get; set; }
        public string TruckName { get; set; }
        public long DriverId { get; set; }
        public string DriverName { get; set; }
    }
}
