using Abp.Application.Services.Dto;
using System;

namespace TACHYON.Trailers.TrailerTypes.Dtos
{
    public class GetAllTrailerTypesInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }

		public string DisplayNameFilter { get; set; }



    }
}