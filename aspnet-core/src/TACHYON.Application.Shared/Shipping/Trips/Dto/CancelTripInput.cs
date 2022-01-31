using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TACHYON.Shipping.Trips.Dto
{
    public class CancelTripInput
    {
        [Required]
        public long id { get; set; }
        [Required]
        [StringLength(ShippingRequestTripConsts.MaxCanceledReasonLength, MinimumLength = ShippingRequestTripConsts.MinCanceledReasonLength)]
        public string CanceledReason { get; set; }
        public bool IsApproved { get; set; } = true;
        public string RejectedCancelingReason { get; set; }
    }
}