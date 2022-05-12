using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Shipping.ShippingRequests.Dtos
{
    public class GetAllShippingRequestVasesOutput
    {
        public string VasName { get; set; }
        public int Count { get; set; }
        public int Amount { get; set; }
    }
}