using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TACHYON.Rating.dtos
{
    public class CreateDeliveryExpRateByReceiverDto: IHasRating
    {
        [Required]
        [StringLength(6, MinimumLength = 6)]
        public string code { get; set; }

        [Range(1, 5)]
        public int Rate { get; set; }
        public string Note { get; set; }
    }
}
