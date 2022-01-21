using System;
using Abp.Application.Services.Dto;

namespace TACHYON.Countries.CountriesTranslations.Dtos
{
    public class CountriesTranslationDto : EntityDto
    {
        public string TranslatedDisplayName { get; set; }

        public string Language { get; set; }

        public int CoreId { get; set; }
    }
}