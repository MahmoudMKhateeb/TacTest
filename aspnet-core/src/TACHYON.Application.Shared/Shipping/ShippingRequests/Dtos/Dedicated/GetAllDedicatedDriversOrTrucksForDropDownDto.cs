using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Shipping.ShippingRequests.Dtos.Dedicated
{
    public class GetAllDedicatedDriversOrTrucksForDropDownDto
    {
        public string Id { get; set; }
        public string DisplayName { get; set; }
        public bool IsAvailable { get; set; }
    }
}
