using Abp.Application.Services.Dto;
using System;

namespace TACHYON.Routs.Dtos
{
    public class GetAllRoutesInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }

		public string DisplayNameFilter { get; set; }


		 public string RoutTypeDisplayNameFilter { get; set; }

		 
    }
}