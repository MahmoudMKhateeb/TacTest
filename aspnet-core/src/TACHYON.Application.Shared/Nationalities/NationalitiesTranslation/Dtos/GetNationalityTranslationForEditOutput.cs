using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.Nationalities.NationalitiesTranslation.Dtos
{
    public class GetNationalityTranslationForEditOutput
    {
        public CreateOrEditNationalityTranslationDto NationalityTranslation { get; set; }

        public string NationalityName { get; set; }

    }
}