using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.Cities.CitiesTranslations.Dtos
{
    public class GetCitiesTranslationForEditOutput
    {
        public CreateOrEditCitiesTranslationDto CitiesTranslation { get; set; }

        public string CityDisplayName { get; set; }

    }
}