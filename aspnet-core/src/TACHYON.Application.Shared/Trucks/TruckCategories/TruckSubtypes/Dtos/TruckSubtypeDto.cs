
using System;
using Abp.Application.Services.Dto;

namespace TACHYON.Trucks.TruckCategories.TruckSubtypes.Dtos
{
    public class TruckSubtypeDto : EntityDto
    {
		public string DisplayName { get; set; }


		 public long? TrucksTypeId { get; set; }

		 
    }
}