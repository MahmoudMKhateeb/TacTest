using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.Trucks.TrucksTypes.TrucksTypesTranslations.Dtos
{
    public class GetTrucksTypesTranslationForEditOutput
    {
        public CreateOrEditTrucksTypesTranslationDto TrucksTypesTranslation { get; set; }

        public string TrucksTypeDisplayName { get; set; }

    }
}