using Abp.Application.Services.Dto;
using Abp.Runtime.Validation;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.PriceOffers.Dto
{
    public class CreateOrEditActorShipperPriceDto : EntityDto<int?>, ICustomValidate
    {
        public string VasDisplayName { get; set; }

        public long VasId { get; set; }
        
        public int? ShippingRequestId { get; set; }

        public long? ShippingRequestVasId { get; set; }
        
        public decimal? TotalAmountWithCommission { get; set; }
        public decimal? SubTotalAmountWithCommission { get; set; }
        public decimal? VatAmountWithCommission { get; set; }
        public decimal? TaxVat { get; set; }

        public void AddValidationErrors(CustomValidationContext context)
        {
            if (ShippingRequestId.HasValue && ShippingRequestVasId.HasValue)
                context.Results.Add(new ValidationResult("Actor shipper price can't be for shipping request and for vas at the same time"));
            if (!ShippingRequestId.HasValue && !ShippingRequestVasId.HasValue)
                context.Results.Add(new ValidationResult("you must set actor shipper price for shipping request or for vas"));
        }
    }
}