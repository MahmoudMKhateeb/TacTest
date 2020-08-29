using Abp.Application.Services.Dto;
using System;

namespace TACHYON.UnitOfMeasures.Dtos
{
    public class GetAllUnitOfMeasuresInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }

		public string DisplayNameFilter { get; set; }



    }
}