using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Shipping
{
    public class AssignDedicatedTrucksAndDriversInput
    {
        public long ShippingRequestId { get; set; }
        public List<long> TrucksList { get; set; }
        public List<long> DriversList { get; set; }
    }
}
