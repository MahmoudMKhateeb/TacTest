using Abp.Application.Services.Dto;
using System;

namespace TACHYON.Countries.CountriesTranslations.Dtos
{
    public class GetAllCountriesTranslationsInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }

        public string TranslatedDisplayNameFilter { get; set; }

        public string LanguageFilter { get; set; }

        public string CountyDisplayNameFilter { get; set; }

    }
}