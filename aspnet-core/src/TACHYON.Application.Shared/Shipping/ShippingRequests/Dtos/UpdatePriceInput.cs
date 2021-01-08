using System.Collections.Generic;
using TACHYON.Vases.Dtos;

namespace TACHYON.Shipping.ShippingRequests.Dtos
{
    public class UpdatePriceInput
    {
        public decimal Price { get; set; }
        public long Id { get; set; }
        public List<ShippingRequestVasPriceDto> PricedVasesList { get; set; }
    }
}