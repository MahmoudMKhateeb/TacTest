using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TACHYON.Rating.dtos
{
    public class CreateDriverRatingByReceiverDto: IHasRating
    {
        [Required]
        public int DriverId { get; set; }
        [Required]
        [StringLength(6, MinimumLength = 6)]
        public string Code { get; set; }
        [Range(1, 5)]
        public int Rate { get; set; }
        public string Note { get; set; }
    }
}
