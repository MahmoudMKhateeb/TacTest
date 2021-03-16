using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.Shipping.TripStatuses.Dtos
{
    public class CreateOrEditTripStatusDto : EntityDto<int?>
    {

        [Required]
        public string DisplayName { get; set; }

    }
}