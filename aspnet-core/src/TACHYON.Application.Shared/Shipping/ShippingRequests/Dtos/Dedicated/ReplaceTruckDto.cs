using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TACHYON.Shipping.ShippingRequests.Dtos.Dedicated
{
    public class ReplaceTruckDto
    {
        [Required]
        public long OriginalDedicatedTruckId { get; set; }
        [Required]
        public long TruckId { get; set; }
        public string ReplacementReason { get; set; }
        [Required]
        public int ReplacementIntervalInDays { get; set; }
    }
}
