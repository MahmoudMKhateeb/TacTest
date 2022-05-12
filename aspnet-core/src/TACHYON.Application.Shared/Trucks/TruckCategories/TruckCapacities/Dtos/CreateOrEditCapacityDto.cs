using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.Trucks.TruckCategories.TruckCapacities.Dtos
{
    public class CreateOrEditCapacityDto : EntityDto<int?>
    {
        [Required]
        [StringLength(CapacityConsts.MaxDisplayNameLength, MinimumLength = CapacityConsts.MinDisplayNameLength)]
        public string DisplayName { get; set; }


        public long TrucksTypeId { get; set; }
    }
}