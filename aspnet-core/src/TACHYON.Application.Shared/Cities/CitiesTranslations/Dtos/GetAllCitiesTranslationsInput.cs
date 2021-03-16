using Abp.Application.Services.Dto;
using System;

namespace TACHYON.Cities.CitiesTranslations.Dtos
{
    public class GetAllCitiesTranslationsInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }

        public string TranslatedDisplayNameFilter { get; set; }

        public string LanguageFilter { get; set; }

        public string CityDisplayNameFilter { get; set; }

    }
}