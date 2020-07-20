using Abp.Application.Services.Dto;
using System;

namespace TACHYON.Routs.RoutSteps.Dtos
{
    public class GetAllRoutStepsForExcelInput
    {
		public string Filter { get; set; }

		public string DisplayNameFilter { get; set; }

		public string LatitudeFilter { get; set; }

		public string LongitudeFilter { get; set; }

		public int? MaxOrderFilter { get; set; }
		public int? MinOrderFilter { get; set; }


		 public string CityDisplayNameFilter { get; set; }

		 		 public string CityDisplayName2Filter { get; set; }

		 		 public string RouteDisplayNameFilter { get; set; }

		 
    }
}