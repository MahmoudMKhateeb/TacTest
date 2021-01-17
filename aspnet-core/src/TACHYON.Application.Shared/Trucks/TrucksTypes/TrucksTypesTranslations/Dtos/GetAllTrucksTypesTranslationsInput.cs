using Abp.Application.Services.Dto;
using System;

namespace TACHYON.Trucks.TrucksTypes.TrucksTypesTranslations.Dtos
{
    public class GetAllTrucksTypesTranslationsInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }

        public string TranslatedDisplayNameFilter { get; set; }

        public string LanguageFilter { get; set; }

        public string TrucksTypeDisplayNameFilter { get; set; }

    }
}