using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace TACHYON.Trucks.PlateTypes.Dtos
{
    public class CreateOrEditPlateTypeDto : EntityDto<int?>
    {

        //[Required]
        //[StringLength(PlateTypeConsts.MaxDisplayNameLength, MinimumLength = PlateTypeConsts.MinDisplayNameLength)]
        //public string DisplayName { get; set; }

        public ICollection<PlateTypeTranslationDto> Translations { get; set; }
    }
}