
using System;
using Abp.Application.Services.Dto;

namespace TACHYON.Trucks.TruckCategories.TruckCapacities.Dtos
{
    public class CapacityDto : EntityDto
    {
		public string DisplayName { get; set; }


		 public int TruckSubtypeId { get; set; }

		 
    }
}