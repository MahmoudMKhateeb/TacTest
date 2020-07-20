
using System;
using Abp.Application.Services.Dto;

namespace TACHYON.Routs.Dtos
{
    public class RouteDto : EntityDto
    {
		public string DisplayName { get; set; }

		public string Description { get; set; }


		 public int? RoutTypeId { get; set; }

		 
    }
}