using System;
using Abp.Application.Services.Dto;

namespace TACHYON.Nationalities.NationalitiesTranslation.Dtos
{
    public class NationalityTranslationDto : EntityDto
    {
        public string TranslatedName { get; set; }

        public string Language { get; set; }

        public int CoreId { get; set; }
    }
}