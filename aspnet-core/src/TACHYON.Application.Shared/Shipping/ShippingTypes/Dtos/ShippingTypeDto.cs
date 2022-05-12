using System;
using Abp.Application.Services.Dto;

namespace TACHYON.Shipping.ShippingTypes.Dtos
{
    public class ShippingTypeDto : EntityDto
    {
        public string DisplayName { get; set; }

        public string Description { get; set; }
    }
}