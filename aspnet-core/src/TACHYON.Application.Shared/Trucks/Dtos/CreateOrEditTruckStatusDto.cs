
using Abp.Application.Services.Dto;
using System;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.Trucks.Dtos
{
    public class CreateOrEditTruckStatusDto : EntityDto<long?>
    {

        [Required]
        [StringLength(TruckStatusConsts.MaxDisplayNameLength, MinimumLength = TruckStatusConsts.MinDisplayNameLength)]
        public string DisplayName { get; set; }



    }
}