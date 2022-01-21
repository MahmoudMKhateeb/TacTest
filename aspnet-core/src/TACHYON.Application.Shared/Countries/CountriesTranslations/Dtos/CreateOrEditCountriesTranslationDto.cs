using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.Countries.CountriesTranslations.Dtos
{
    public class CreateOrEditCountriesTranslationDto : EntityDto<int?>
    {
        [Required]
        [StringLength(CountriesTranslationConsts.MaxTranslatedDisplayNameLength,
            MinimumLength = CountriesTranslationConsts.MinTranslatedDisplayNameLength)]
        public string TranslatedDisplayName { get; set; }

        [Required]
        [StringLength(CountriesTranslationConsts.MaxLanguageLength,
            MinimumLength = CountriesTranslationConsts.MinLanguageLength)]
        public string Language { get; set; }

        public int CoreId { get; set; }
    }
}