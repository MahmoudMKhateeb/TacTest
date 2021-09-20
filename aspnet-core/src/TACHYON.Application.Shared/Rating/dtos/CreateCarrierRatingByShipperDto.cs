using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TACHYON.Rating.dtos
{
    public class CreateCarrierRatingByShipperDto
    {
        [Required]
        public int TripId { get; set; }
        //[Required]
        //public int CarrierId { get; set; }
        [Range(1,5)]
        public int Rate { get; set; }
        public string Note { get; set; }
    }
}
