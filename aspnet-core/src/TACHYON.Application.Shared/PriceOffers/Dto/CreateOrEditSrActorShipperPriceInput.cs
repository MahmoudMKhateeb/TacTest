using Abp.Runtime.Validation;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace TACHYON.PriceOffers.Dto
{
    public class CreateOrEditSrActorShipperPriceInput: ICustomValidate
    {
        public CreateOrEditActorShipperPriceDto ActorShipperPriceDto { get; set; }

        public List<CreateOrEditActorShipperPriceDto> VasActorShipperPriceDto { get; set; }
        
        public void AddValidationErrors(CustomValidationContext context)
        {
            if (VasActorShipperPriceDto.Any(x=> !x.ShippingRequestVasId.HasValue))
                context.Results.Add(new ValidationResult("You must add vas id for each vas price"));
            
            if (!ActorShipperPriceDto.ShippingRequestId.HasValue)
                context.Results.Add(new ValidationResult("You must add shipping Request id for each shipping Request price"));
        }
    }
}