using Abp.Application.Services.Dto;
using System;

namespace TACHYON.Trucks.TruckCategories.TransportTypes.Dtos
{
    public class GetAllTransportTypesInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }

		public string DisplayNameFilter { get; set; }




    }
}