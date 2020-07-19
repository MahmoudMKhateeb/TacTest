using Abp.Application.Services.Dto;
using System;

namespace TACHYON.Trailers.TrailerTypes.Dtos
{
    public class GetAllTrailerTypesForExcelInput
    {
		public string Filter { get; set; }

		public string DisplayNameFilter { get; set; }



    }
}