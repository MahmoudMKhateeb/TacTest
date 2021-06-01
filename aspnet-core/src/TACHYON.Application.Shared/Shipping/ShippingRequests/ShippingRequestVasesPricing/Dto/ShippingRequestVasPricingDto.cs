using Abp.Runtime.Validation;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.Shipping.ShippingRequests.ShippingRequestVasesPricing.Dto
{
    public class ShippingRequestVasPricingDto: ICustomValidate
    {
        public int VasId { get; set; }
        public int Price { get; set; }

        public void AddValidationErrors(CustomValidationContext context)
        {
            if (Price <= 0)
            {
                context.Results.Add(new ValidationResult("ThePriceMustBeGreaterThanZero"));
            }
        }
    }
}
