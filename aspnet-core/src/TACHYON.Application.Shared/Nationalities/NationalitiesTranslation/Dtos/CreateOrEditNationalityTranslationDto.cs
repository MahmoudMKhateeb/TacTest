using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.Nationalities.NationalitiesTranslation.Dtos
{
    public class CreateOrEditNationalityTranslationDto : EntityDto<int?>
    {

        [Required]
        public string TranslatedName { get; set; }

        [Required]
        public string Language { get; set; }

        public int CoreId { get; set; }

    }
}