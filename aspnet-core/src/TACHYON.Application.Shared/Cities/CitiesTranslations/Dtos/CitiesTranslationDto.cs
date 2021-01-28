using System;
using Abp.Application.Services.Dto;

namespace TACHYON.Cities.CitiesTranslations.Dtos
{
    public class CitiesTranslationDto : EntityDto
    {
        public string TranslatedDisplayName { get; set; }

        public string Language { get; set; }

        public int CoreId { get; set; }

    }
}