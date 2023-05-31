using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Trucks.Dtos
{
    public class GetTenantExceedsNumberOfTrucksDto
    {
        public bool IsTenantExceedsNumberOfTrucks { get; set; }
        public bool CanAddTruck { get; set; } = true;
    }
}
