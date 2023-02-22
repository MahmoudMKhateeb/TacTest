using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TACHYON.Shipping.Drivers.Dto
{
    public class UpdateHomeDeliveryFacilityLocationInput
    {
        [Required]
        public double Longitude { get; set; }
        [Required]
        public double Latitude { get; set; }
        [Required]
        public long PointId { get; set; }
        [Required]
        public string Address { get; set; }
    }
}
