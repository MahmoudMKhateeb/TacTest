using Abp.Application.Services.Dto;
using Abp.Runtime.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TACHYON.Shipping.ShippingRequests.TachyonDealer.Dtos
{
    public class CarrirSetPriceForDirectRequestDto : EntityDto, ICustomValidate
    {
        [Required] public decimal Price { get; set; }

        public void AddValidationErrors(CustomValidationContext context)
        {
            if (Price < 0) context.Results.Add(new ValidationResult("ThePriceMustBeGreaterThanZero"));
        }
    }
}