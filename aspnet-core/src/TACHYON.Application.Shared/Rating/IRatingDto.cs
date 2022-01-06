using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TACHYON.Rating
{
    public interface IRatingDto
    {
        [Range(1, 5)]
        int Rate { get; set; }
        string Note { get; set; }
    }
}
