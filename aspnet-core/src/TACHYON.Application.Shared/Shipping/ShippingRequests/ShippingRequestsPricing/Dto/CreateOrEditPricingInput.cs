using Abp.Runtime.Validation;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using TACHYON.Shipping.ShippingRequests.ShippingRequestVasesPricing.Dto;

namespace TACHYON.Shipping.ShippingRequests.ShippingRequestsPricing.Dto
{
   public class CreateOrEditPricingInput : ICustomValidate
    {
        public long ShippingRequestId { get; set; }
        public bool IsNew { get; set; }
        public decimal TripPrice { get; set; }
        [JsonIgnore]
        public ShippingRequestPricingChannel Channel { get; set; }
        public List<ShippingRequestVasPricingDto> ShippingRequestVasPricing;
        public void AddValidationErrors(CustomValidationContext context)
        {
            if (TripPrice <= 0)
            {
                context.Results.Add(new ValidationResult("ThePriceMustBeGreaterThanZero"));
            }
        }
    }

}

