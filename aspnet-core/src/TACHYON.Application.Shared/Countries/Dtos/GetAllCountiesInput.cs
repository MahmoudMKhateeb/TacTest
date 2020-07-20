using Abp.Application.Services.Dto;
using System;

namespace TACHYON.Countries.Dtos
{
    public class GetAllCountiesInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }

		public string DisplayNameFilter { get; set; }

		public string CodeFilter { get; set; }



    }
}