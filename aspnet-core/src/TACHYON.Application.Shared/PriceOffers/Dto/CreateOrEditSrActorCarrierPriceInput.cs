using Abp.Application.Services.Dto;
using Abp.Runtime.Validation;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace TACHYON.PriceOffers.Dto
{
    public class CreateOrEditSrActorCarrierPriceInput : ICustomValidate
    {
        public CreateOrEditActorCarrierPrice ActorCarrierPrice { get; set; }
        
        public List<CreateOrEditActorCarrierPrice> VasActorCarrierPrices { get; set; }
        public void AddValidationErrors(CustomValidationContext context)
        {
            if (VasActorCarrierPrices.Any(x=> !x.ShippingRequestVasId.HasValue))
                context.Results.Add(new ValidationResult("You must add vas id for each vas price"));
            
            if (!ActorCarrierPrice.ShippingRequestId.HasValue)
                context.Results.Add(new ValidationResult("You must add shipping Request id for each shipping Request price"));
        }
    }
}