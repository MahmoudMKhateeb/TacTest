
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.Countries.Dtos
{
    public class CreateOrEditCountyDto : EntityDto<int?>
    {

		[Required]
		[StringLength(CountyConsts.MaxDisplayNameLength, MinimumLength = CountyConsts.MinDisplayNameLength)]
		public string DisplayName { get; set; }
		
		
		[Required]
		[StringLength(CountyConsts.MaxCodeLength, MinimumLength = CountyConsts.MinCodeLength)]
		public string Code { get; set; }
		
		

    }
}