using Abp.Application.Services.Dto;
using Abp.Runtime.Validation;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TACHYON.Shipping.ShippingRequests.ShippingRequestVasesPricing.Dto;

namespace TACHYON.MarketPlaces.Dto
{
    public class CreateOrEditMarketPlaceBidInput : ICustomValidate
    {
        public long ShippingRequestId { get; set; }
        public bool IsNew { get; set; }
        public decimal TripPrice { get; set; }
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
