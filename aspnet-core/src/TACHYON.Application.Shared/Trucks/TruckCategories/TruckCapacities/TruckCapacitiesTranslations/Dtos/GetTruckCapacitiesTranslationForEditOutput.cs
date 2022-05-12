using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.Trucks.TruckCategories.TruckCapacities.TruckCapacitiesTranslations.Dtos
{
    public class GetTruckCapacitiesTranslationForEditOutput
    {
        public CreateOrEditTruckCapacitiesTranslationDto TruckCapacitiesTranslation { get; set; }

        public string CapacityDisplayName { get; set; }
    }
}