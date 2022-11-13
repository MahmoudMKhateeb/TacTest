using Abp.Application.Services.Dto;
using Abp.Runtime.Validation;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.PriceOffers.Dto
{
    public class CreateOrEditActorCarrierPrice : EntityDto<int?>, ICustomValidate
    {
        public string VasDisplayName { get; set; }

        public long VasId { get; set; }
        public long? ShippingRequestId { get; set; }

        public long? ShippingRequestVasId { get; set; }

        public bool IsActorCarrierHaveInvoice { get; set; }
        public decimal? SubTotalAmount { get; set; }
        public decimal? VatAmount { get; set; }
        public decimal? TaxVat { get; set; }
        
        public void AddValidationErrors(CustomValidationContext context)
        {
            if (ShippingRequestId.HasValue && ShippingRequestVasId.HasValue)
                context.Results.Add(new ValidationResult("Actor carrier price can't be for shipping request and for vas at the same time"));
            if (!ShippingRequestId.HasValue && !ShippingRequestVasId.HasValue)
                context.Results.Add(new ValidationResult("you must set actor carrier price for shipping request or for vas"));
        }
    }
}