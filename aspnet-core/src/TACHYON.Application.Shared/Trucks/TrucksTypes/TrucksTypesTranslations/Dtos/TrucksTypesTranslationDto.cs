using System;
using Abp.Application.Services.Dto;

namespace TACHYON.Trucks.TrucksTypes.TrucksTypesTranslations.Dtos
{
    public class TrucksTypesTranslationDto : EntityDto
    {
        public string TranslatedDisplayName { get; set; }

        public string Language { get; set; }

        //public long CoreId { get; set; }
        public string LanguageDisplayName { get; set; }
        public string icon { get; set; }

    }
}