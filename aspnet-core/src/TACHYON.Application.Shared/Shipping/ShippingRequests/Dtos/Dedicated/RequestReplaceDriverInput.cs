﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TACHYON.Shipping.ShippingRequests.Dtos.Dedicated
{
    public class RequestReplaceDriverInput
    {

        [Required]
        public long DedicatedDriverId { get; set; }
        public string ReplacementReason { get; set; }
        [Required]
        public int ReplacementIntervalInDays { get; set; }
    }
}