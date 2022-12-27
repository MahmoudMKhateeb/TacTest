using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Shipping.ShippingRequests.Dtos.Dedicated
{
    public class GetAllTrucksWithDriversListDto
    {
        public long TruckId { get; set; }
        public long? DriverUserId { get; set; }
        public string TruckName { get; set; }
        public string DriverName { get; set; }
    }
}
