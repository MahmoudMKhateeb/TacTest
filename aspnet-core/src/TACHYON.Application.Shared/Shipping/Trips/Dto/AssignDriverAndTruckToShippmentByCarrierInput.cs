using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Shipping.Trips.Dto
{
    public class AssignDriverAndTruckToShippmentByCarrierInput : EntityDto
    {
        public long? AssignedDriverUserId { get; set; }
        public long? AssignedTruckId { get; set; }
        
    }
}
