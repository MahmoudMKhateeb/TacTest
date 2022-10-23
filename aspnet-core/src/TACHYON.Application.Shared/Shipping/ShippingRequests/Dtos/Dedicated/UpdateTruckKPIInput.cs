using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Shipping.ShippingRequests.Dtos.Dedicated
{
    public class UpdateTruckKPIInput
    {
        public long DedicatedTruckId { get; set; }
        public double KPI { get; set; }
    }
}
