using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.Vases.Dtos
{
    public class CreateOrEditVasDto : EntityDto<int?>
    {

        [StringLength(VasConsts.MaxNameLength, MinimumLength = VasConsts.MinNameLength)]
        public string Name { get; set; }

        [StringLength(VasConsts.MaxDisplayNameLength, MinimumLength = VasConsts.MinDisplayNameLength)]
        public string DisplayName { get; set; }

        public bool HasAmount { get; set; }

        public bool HasCount { get; set; }

    }
}