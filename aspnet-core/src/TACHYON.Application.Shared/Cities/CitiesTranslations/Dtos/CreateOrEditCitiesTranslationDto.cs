﻿using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.Cities.CitiesTranslations.Dtos
{
    public class CreateOrEditCitiesTranslationDto : EntityDto<int?>
    {

        [Required]
        [StringLength(CitiesTranslationConsts.MaxTranslatedDisplayNameLength, MinimumLength = CitiesTranslationConsts.MinTranslatedDisplayNameLength)]
        public string TranslatedDisplayName { get; set; }

        [Required]
        [StringLength(CitiesTranslationConsts.MaxLanguageLength, MinimumLength = CitiesTranslationConsts.MinLanguageLength)]
        public string Language { get; set; }

        public int CoreId { get; set; }

    }
}