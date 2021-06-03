using Abp.Runtime.Validation;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.Shipping.ShippingRequests.ShippingRequestVasesPricing.Dto
{
    public class ShippingRequestVasPricingDto: ICustomValidate
    {
        public int VasId { get; set; }
        public int Price { get; set; }
        [JsonIgnore]
        public decimal? CommissionPercentageOrAddValue { get; set; }
        [JsonIgnore]
        public ShippingRequestCommissionType CommissionType { get; set; }
        public void AddValidationErrors(CustomValidationContext context)
        {
            if (Price <= 0)
            {
                context.Results.Add(new ValidationResult("ThePriceMustBeGreaterThanZero"));
            }
        }
    }
}
