
using System;
using Abp.Application.Services.Dto;

namespace TACHYON.Routs.RoutSteps.Dtos
{
    public class RoutStepDto : EntityDto<long>
    {
		public string DisplayName { get; set; }

		public string Latitude { get; set; }

		public string Longitude { get; set; }

		public int Order { get; set; }


		 public int? OriginCityId { get; set; }

		 		 public int? DestinationCityId { get; set; }

		 		 public int RouteId { get; set; }

		 
    }
}