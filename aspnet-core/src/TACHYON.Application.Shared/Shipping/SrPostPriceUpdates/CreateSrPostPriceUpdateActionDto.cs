using Abp.Application.Services.Dto;
using Abp.Extensions;
using Abp.Runtime.Validation;

using System.ComponentModel.DataAnnotations;
using TACHYON.PriceOffers.Dto;

namespace TACHYON.Shipping.SrPostPriceUpdates
{
    public class CreateSrPostPriceUpdateActionDto : EntityDto<long>, ICustomValidate
    {
        [Required]
        public SrPostPriceUpdateAction Action { get; set; }

        public string RejectionReason { get; set; }

        public CreateOrEditPriceOfferInput Offer { get; set; }


        public void AddValidationErrors(CustomValidationContext context)
        {
            switch (Action)
            {
                case SrPostPriceUpdateAction.Pending:
                    context.Results.Add(new ValidationResult("YouMustTakeAnAction"));
                    break;
                case SrPostPriceUpdateAction.Accept:
                    break;
                case SrPostPriceUpdateAction.ChangePrice:
                    if (Offer == null)
                        context.Results.Add(new ValidationResult("YouMustAddANewOffer"));
                    break;
                case SrPostPriceUpdateAction.Reject:
                    if (RejectionReason.IsNullOrEmpty())
                        context.Results.Add(new ValidationResult("YouMustProvideRejectionReason"));
                    break;
                default: context.Results.Add(new ValidationResult("YouMustTakeAnAction"));
                    break;
                
            }
        }
    }
}