using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.TermsAndConditions.Dtos
{
    public class CreateOrEditTermAndConditionTranslationDto : EntityDto<int?>
    {
        [Required] public string Content { get; set; }

        [Required]
        [StringLength(TermAndConditionTranslationConsts.MaxLanguageLength,
            MinimumLength = TermAndConditionTranslationConsts.MinLanguageLength)]
        public string Language { get; set; }

        public int? CoreId { get; set; }
    }
}