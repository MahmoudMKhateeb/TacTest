using Abp.Application.Services.Dto;
using System;

namespace TACHYON.Trucks.TruckCategories.TruckCapacities.TruckCapacitiesTranslations.Dtos
{
    public class GetAllTruckCapacitiesTranslationsInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }

        public string TranslatedDisplayNameFilter { get; set; }

        public string LanguageFilter { get; set; }

        public string CapacityDisplayNameFilter { get; set; }
    }
}