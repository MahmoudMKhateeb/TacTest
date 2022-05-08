using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.Shipping.ShippingTypes.Dtos
{
    public class GetShippingTypeForEditOutput
    {
        public CreateOrEditShippingTypeDto ShippingType { get; set; }
    }
}