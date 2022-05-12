using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.Shipping.ShippingRequestStatuses.Dtos
{
    public class CreateOrEditShippingRequestStatusDto : EntityDto<int?>
    {
        [Required]
        [StringLength(ShippingRequestStatusConsts.MaxDisplayNameLength,
            MinimumLength = ShippingRequestStatusConsts.MinDisplayNameLength)]
        public string DisplayName { get; set; }
    }
}