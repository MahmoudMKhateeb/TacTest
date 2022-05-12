using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TACHYON.Rating.dtos
{
    public class CreateDeliveryExpRateByReceiverDto : RatingDto
    {
        [Required]
        [StringLength(6, MinimumLength = 6)]
        public string code { get; set; }
    }
}