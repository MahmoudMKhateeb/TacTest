using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TACHYON.Rating.dtos
{
    public class CreateCarrierRatingByShipperDto : RatingDto
    {
        [Required]
        public int TripId { get; set; }
        //[Required]
        //public int CarrierId { get; set; }
    }
}
