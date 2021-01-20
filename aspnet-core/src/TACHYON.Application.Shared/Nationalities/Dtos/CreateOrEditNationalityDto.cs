using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.Nationalities.Dtos
{
    public class CreateOrEditNationalityDto : EntityDto<int?>
    {

        [Required]
        [StringLength(NationalityConsts.MaxNameLength, MinimumLength = NationalityConsts.MinNameLength)]
        public string Name { get; set; }

    }
}