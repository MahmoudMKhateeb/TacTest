using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Vases.Dtos
{
   public class ShippingRequestVasPriceDto
    {
        public ShippingRequestVasListOutput ShippingRequestVas { get; set; }
        public double? ActualPrice { get; set; }
        public double? DefaultPrice { get; set; }
        public long ShippingRequestVasId { get; set; }

    }
}
