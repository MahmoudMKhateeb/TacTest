using Abp.Application.Services.Dto;
using Abp.Runtime.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TACHYON.Shipping.ShippingRequests.TachyonDealer.Dtos
{
 public   class TachyonDealerDirectOfferToShipperInputDto: EntityDto<long>, ICustomValidate
    {
        [Required]
        public decimal Price { get; set; }

        public void AddValidationErrors(CustomValidationContext context)
        {
            if (Price<1)
            {
                context.Results.Add(new ValidationResult("The price  must be higher zero"));

            }
        }
    }
}
