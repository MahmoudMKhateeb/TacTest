using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TACHYON.Rating
{
    public abstract class RatingDto
    {
        [Range(1, 5)] public int Rate { get; set; }
        public string Note { get; set; }
    }
}