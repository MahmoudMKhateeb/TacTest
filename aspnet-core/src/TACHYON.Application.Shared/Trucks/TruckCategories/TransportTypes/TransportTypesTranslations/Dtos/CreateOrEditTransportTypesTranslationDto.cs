using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.Trucks.TruckCategories.TransportTypes.TransportTypesTranslations.Dtos
{
    public class CreateOrEditTransportTypesTranslationDto : EntityDto<int?>
    {
        [Required]
        [StringLength(TransportTypesTranslationConsts.MaxDisplayNameLength,
            MinimumLength = TransportTypesTranslationConsts.MinDisplayNameLength)]
        public string TranslatedDisplayName { get; set; }

        [Required]
        [StringLength(TransportTypesTranslationConsts.MaxLanguageLength,
            MinimumLength = TransportTypesTranslationConsts.MinLanguageLength)]
        public string Language { get; set; }

        public int CoreId { get; set; }
    }
}