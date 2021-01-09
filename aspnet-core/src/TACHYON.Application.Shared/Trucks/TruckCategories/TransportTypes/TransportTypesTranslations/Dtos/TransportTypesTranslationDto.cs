using System;
using Abp.Application.Services.Dto;

namespace TACHYON.Trucks.TruckCategories.TransportTypes.TransportTypesTranslations.Dtos
{
    public class TransportTypesTranslationDto : EntityDto
    {
        public string TranslatedDisplayName { get; set; }

        public string Language { get; set; }

        public int CoreId { get; set; }

    }
}