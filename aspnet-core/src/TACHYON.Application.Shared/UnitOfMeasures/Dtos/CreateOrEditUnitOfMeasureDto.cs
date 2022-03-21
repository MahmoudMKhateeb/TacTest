
using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.UnitOfMeasures.Dtos
{
    public class CreateOrEditUnitOfMeasureDto : EntityDto<int?>
    {

        [Required]
        [StringLength(UnitOfMeasureConsts.MaxDisplayNameLength, MinimumLength = UnitOfMeasureConsts.MinDisplayNameLength)]
        public string Key { get; set; }


    }
}