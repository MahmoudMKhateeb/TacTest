using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.Countries.CountriesTranslations.Dtos
{
    public class GetCountriesTranslationForEditOutput
    {
        public CreateOrEditCountriesTranslationDto CountriesTranslation { get; set; }

        public string CountyDisplayName { get; set; }
    }
}