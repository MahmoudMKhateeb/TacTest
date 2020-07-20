using Abp.Application.Services.Dto;
using System;

namespace TACHYON.Cities.Dtos
{
    public class GetAllCitiesForExcelInput
    {
		public string Filter { get; set; }

		public string DisplayNameFilter { get; set; }

		public string CodeFilter { get; set; }

		public string LatitudeFilter { get; set; }

		public string LongitudeFilter { get; set; }


		 public string CountyDisplayNameFilter { get; set; }

		 
    }
}