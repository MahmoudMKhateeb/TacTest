
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.UnitOfMeasures.Dtos
{
    public class CreateOrEditUnitOfMeasureDto : EntityDto<int?>
    {

		[Required]
		[StringLength(UnitOfMeasureConsts.MaxDisplayNameLength, MinimumLength = UnitOfMeasureConsts.MinDisplayNameLength)]
		public string DisplayName { get; set; }
		
		

    }
}