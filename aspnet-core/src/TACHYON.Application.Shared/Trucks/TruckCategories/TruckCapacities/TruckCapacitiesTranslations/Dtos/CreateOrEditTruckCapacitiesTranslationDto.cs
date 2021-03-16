using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.Trucks.TruckCategories.TruckCapacities.TruckCapacitiesTranslations.Dtos
{
    public class CreateOrEditTruckCapacitiesTranslationDto : EntityDto<int?>
    {

        [Required]
        [StringLength(TruckCapacitiesTranslationConsts.MaxTranslatedDisplayNameLength, MinimumLength = TruckCapacitiesTranslationConsts.MinTranslatedDisplayNameLength)]
        public string TranslatedDisplayName { get; set; }

        [Required]
        [StringLength(TruckCapacitiesTranslationConsts.MaxLanguageLength, MinimumLength = TruckCapacitiesTranslationConsts.MinLanguageLength)]
        public string Language { get; set; }

        public int CoreId { get; set; }

    }
}