using Abp.Application.Services.Dto;
using System;

namespace TACHYON.Trucks.TruckCategories.TransportSubtypes.Dtos
{
    public class GetAllTransportSubtypesInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }

		public string DisplayNameFilter { get; set; }


		 public string TransportTypeDisplayNameFilter { get; set; }

		 

    }
}