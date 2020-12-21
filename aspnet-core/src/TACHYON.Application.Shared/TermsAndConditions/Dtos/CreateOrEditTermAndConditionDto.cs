
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.TermsAndConditions.Dtos
{
    public class CreateOrEditTermAndConditionDto : EntityDto<int?>
    {

		[Required]
		[StringLength(TermAndConditionConsts.MaxTitleLength, MinimumLength = TermAndConditionConsts.MinTitleLength)]
		public string Title { get; set; }
		
		
		[Required]
		public string Content { get; set; }
		
		
		public double Version { get; set; }
		
		
		public int? EditionId { get; set; }
		public bool IsActive { get; set; }



    }
}