using Abp.Application.Services.Dto;
using Abp.Runtime.Validation;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.Localization.Dto
{
    public class CreateOrEditAppLocalizationDto : EntityDto
    {
        [Required]
        [StringLength(300,MinimumLength =1)]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "ValidateEnglishOnlyWithoutSpace")]
        public string MasterKey { get; set; }
        [Required]
        [StringLength(4000, MinimumLength = 1)]
        public string MasterValue { get; set; }
        public ICollection<AppLocalizationTranslationDto> Translations { get; set; }

    }
}
