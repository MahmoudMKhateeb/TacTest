using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.Trucks.TrucksTypes.TrucksTypesTranslations.Dtos
{
    public class CreateOrEditTrucksTypesTranslationDto : EntityDto<int?>
    {

        [Required]
        [StringLength(TrucksTypesTranslationConsts.MaxTranslatedDisplayNameLength,
            MinimumLength = TrucksTypesTranslationConsts.MinTranslatedDisplayNameLength)]
        public string TranslatedDisplayName { get; set; }

        [Required]
        [StringLength(TrucksTypesTranslationConsts.MaxLanguageLength,
            MinimumLength = TrucksTypesTranslationConsts.MinLanguageLength)]
        public string Language { get; set; }

        public long CoreId { get; set; }

    }
}