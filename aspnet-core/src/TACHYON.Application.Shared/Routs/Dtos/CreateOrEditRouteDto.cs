
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.Routs.Dtos
{
    public class CreateOrEditRouteDto : EntityDto<int?>
    {

		[Required]
		[StringLength(RouteConsts.MaxDisplayNameLength, MinimumLength = RouteConsts.MinDisplayNameLength)]
		public string DisplayName { get; set; }
		
		
		[StringLength(RouteConsts.MaxDescriptionLength, MinimumLength = RouteConsts.MinDescriptionLength)]
		public string Description { get; set; }
		
		
		 public int? RoutTypeId { get; set; }
		 
		 
    }
}