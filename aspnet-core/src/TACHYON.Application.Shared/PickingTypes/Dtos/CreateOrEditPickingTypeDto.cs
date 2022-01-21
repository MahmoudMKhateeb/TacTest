using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.PickingTypes.Dtos
{
    public class CreateOrEditPickingTypeDto : EntityDto<int?>
    {
        [Required]
        [StringLength(PickingTypeConsts.MaxDisplayNameLength, MinimumLength = PickingTypeConsts.MinDisplayNameLength)]
        public string DisplayName { get; set; }
    }
}