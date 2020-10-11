using Abp.Application.Services.Dto;
using System;

namespace TACHYON.Trucks.TruckCategories.TruckSubtypes.Dtos
{
    public class GetAllTruckSubtypesInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }

		public string DisplayNameFilter { get; set; }


		 public string TrucksTypeDisplayNameFilter { get; set; }

		 

    }
}