using System;
using Abp.Application.Services.Dto;

namespace TACHYON.Trucks.TruckCategories.TruckCapacities.TruckCapacitiesTranslations.Dtos
{
    public class TruckCapacitiesTranslationDto : EntityDto
    {
        public string TranslatedDisplayName { get; set; }

        public string Language { get; set; }

        public int CoreId { get; set; }
    }
}