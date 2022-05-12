using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.Trucks.TruckCategories.TransportTypes.Dtos
{
    public class CreateOrEditTransportTypeDto : EntityDto<int?>
    {
        [Required]
        [StringLength(TransportTypeConsts.MaxDisplayNameLength,
            MinimumLength = TransportTypeConsts.MinDisplayNameLength)]
        public string DisplayName { get; set; }
    }
}