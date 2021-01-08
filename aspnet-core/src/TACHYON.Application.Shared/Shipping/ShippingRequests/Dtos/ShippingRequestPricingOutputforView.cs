using System;
using System.Collections.Generic;
using System.Text;
using TACHYON.Vases.Dtos;

namespace TACHYON.Shipping.ShippingRequests.Dtos
{
    public class ShippingRequestPricingOutputforView
    {
        public decimal ShippingRequestPrice { get; set; }
        public List<ShippingRequestVasPriceDto> PricedVasesList { get; set; }

    }
}
