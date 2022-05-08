using Abp.Application.Services.Dto;
using System;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.Cities.CitiesTranslations.Dtos
{
    public class GetAllCitiesTranslationsInput
    {
        public string LoadOptions { get; set; }
        public string CoreId { get; set; }
    }
}