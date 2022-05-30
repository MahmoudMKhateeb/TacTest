using Abp.Application.Services.Dto;
using Abp.Extensions;
using Abp.Runtime.Validation;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.Shipping.SrPostPriceUpdates
{
    public class CreateSrPostPriceUpdateOfferActionDto : EntityDto<long>, ICustomValidate
    {

        [Required]
        public SrPostPriceUpdateOfferAction OfferAction { get; set; }

        public string OfferRejectionReason { get; set; }
        
        public void AddValidationErrors(CustomValidationContext context)
        {
            if (OfferAction == SrPostPriceUpdateOfferAction.Reject && OfferRejectionReason.IsNullOrEmpty())
                context.Results.Add(new ValidationResult("YouMustProvideOfferRejectionReasonWhenRejectOffer"));
        }
    }
}