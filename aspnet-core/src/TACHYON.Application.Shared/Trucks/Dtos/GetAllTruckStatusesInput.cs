using Abp.Application.Services.Dto;
using System;

namespace TACHYON.Trucks.Dtos
{
    public class GetAllTruckStatusesInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }

		public string DisplayNameFilter { get; set; }



    }
}