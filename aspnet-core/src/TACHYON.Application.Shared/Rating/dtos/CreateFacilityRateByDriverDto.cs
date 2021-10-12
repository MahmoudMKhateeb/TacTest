using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TACHYON.Rating.dtos
{
    public class CreateFacilityRateByDriverDto : IHasRating
    {
        [Required]
        public long PointId { get; set; }
        [Range(1, 5)]
        public int Rate { get; set; }
        public string Note { get; set; }
    }
}