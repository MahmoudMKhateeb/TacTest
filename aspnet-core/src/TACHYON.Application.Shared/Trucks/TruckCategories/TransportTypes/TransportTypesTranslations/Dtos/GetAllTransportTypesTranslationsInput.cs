using Abp.Application.Services.Dto;
using System;

namespace TACHYON.Trucks.TruckCategories.TransportTypes.TransportTypesTranslations.Dtos
{
    public class GetAllTransportTypesTranslationsInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }

        public string TranslatedDisplayNameFilter { get; set; }

        public string LanguageFilter { get; set; }

        public string TransportTypeTranslatedDisplayNameFilter { get; set; }

    }
}