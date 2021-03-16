using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.Shipping.ShippingTypes.Dtos
{
    public class CreateOrEditShippingTypeDto : EntityDto<int?>
    {

        [Required]
        [StringLength(ShippingTypeConsts.MaxDisplayNameLength, MinimumLength = ShippingTypeConsts.MinDisplayNameLength)]
        public string DisplayName { get; set; }

        [StringLength(ShippingTypeConsts.MaxDescriptionLength, MinimumLength = ShippingTypeConsts.MinDescriptionLength)]
        public string Description { get; set; }

    }
}