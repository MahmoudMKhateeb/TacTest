using Abp.Application.Services.Dto;
using System;

namespace TACHYON.Trucks.TruckStatusesTranslations.Dtos
{
    public class GetAllTruckStatusesTranslationsInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }

        public string TranslatedDisplayNameFilter { get; set; }

        public string LanguageFilter { get; set; }

        public string TruckStatusDisplayNameFilter { get; set; }

    }
}