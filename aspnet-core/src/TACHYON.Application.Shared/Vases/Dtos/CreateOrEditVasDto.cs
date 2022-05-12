using Abp.Application.Services.Dto;
using Abp.Collections.Extensions;
using Abp.Runtime.Validation;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.Vases.Dtos
{
    public class CreateOrEditVasDto : EntityDto<int?>
    {
        [Required]
        [StringLength(VasConsts.MaxNameLength, MinimumLength = VasConsts.MinNameLength)]
        public string Key { get; set; }
        public bool HasAmount { get; set; }
        public bool HasCount { get; set; }

    }
}