
using Abp.Application.Services.Dto;
using Abp.Collections.Extensions;
using Abp.Runtime.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TACHYON.Trucks.TruckStatusesTranslations.Dtos;

namespace TACHYON.Trucks.Dtos
{
    public class CreateOrEditTruckStatusDto : EntityDto<long?>
    {

        [Required]
        [StringLength(TruckStatusConsts.MaxDisplayNameLength, MinimumLength = TruckStatusConsts.MinDisplayNameLength)]
        public string DisplayName { get; set; }

    }
}