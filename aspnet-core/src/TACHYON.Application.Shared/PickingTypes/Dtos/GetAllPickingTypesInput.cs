using Abp.Application.Services.Dto;
using System;

namespace TACHYON.PickingTypes.Dtos
{
    public class GetAllPickingTypesInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }

		public string DisplayNameFilter { get; set; }



    }
}