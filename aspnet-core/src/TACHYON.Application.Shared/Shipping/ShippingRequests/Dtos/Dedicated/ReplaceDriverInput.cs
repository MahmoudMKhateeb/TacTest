using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TACHYON.Shipping.ShippingRequests.Dtos.Dedicated
{
    public class ReplaceDriverInput
    {
        [Required]
        public long ShippingRequestId { get; set; }
        public List<ReplaceDriverDto> ReplaceDriverDtos { get; set; }
    }
}
