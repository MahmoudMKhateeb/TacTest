using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TACHYON.Trucks.TruckStatusesTranslations.Dtos;

namespace TACHYON.Trucks.Dtos
{
    public class TruckStatusDto : EntityDto<long>
    {
        [Required]
        [StringLength(TruckStatusConsts.MaxDisplayNameLength, MinimumLength = TruckStatusConsts.MinDisplayNameLength)]
        public string DisplayName { get; set; }
    }
}