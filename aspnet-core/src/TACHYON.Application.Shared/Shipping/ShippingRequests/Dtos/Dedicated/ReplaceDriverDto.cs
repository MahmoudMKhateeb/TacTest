using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TACHYON.Shipping.ShippingRequests.Dtos.Dedicated
{
    public class ReplaceDriverDto
    {
        [Required]
        public long OriginalDedicatedDriverId { get; set; }
        [Required]
        public long DriverUserId { get; set; }
        public string ReplacementReason { get; set; }
        [Required]
        public int ReplacementIntervalInDays { get; set; }
    }
}
