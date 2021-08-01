using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TACHYON.Shipping.Trips.Dto
{
    public class AssignDriverAndTruckToShippmentByCarrierInput : EntityDto
    {
        [Required]
        public long AssignedDriverUserId { get; set; }
        [Required]
        public long AssignedTruckId { get; set; }
        
    }
}
