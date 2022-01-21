using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.Trucks.TruckStatusesTranslations.Dtos
{
    public class CreateOrEditTruckStatusesTranslationDto : EntityDto<int?>
    {
        [Required]
        [StringLength(TruckStatusesTranslationConsts.MaxTranslatedDisplayNameLength,
            MinimumLength = TruckStatusesTranslationConsts.MinTranslatedDisplayNameLength)]
        public string TranslatedDisplayName { get; set; }

        [Required]
        [StringLength(TruckStatusesTranslationConsts.MaxLanguageLength,
            MinimumLength = TruckStatusesTranslationConsts.MinLanguageLength)]
        public string Language { get; set; }

        public long CoreId { get; set; }
    }
}