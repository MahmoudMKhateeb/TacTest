using Abp.Application.Services.Dto;
using Abp.Runtime.Validation;
using JetBrains.Annotations;
using System;
using System.ComponentModel.DataAnnotations;
using TACHYON.PriceOffers.Dto;
using TACHYON.Shipping.ShippingRequestUpdates;

namespace TACHYON.Shipping.ShippingRequestUpdate
{
    public class CreateSrUpdateActionInputDto : EntityDto<Guid>, ICustomValidate
    {
        public ShippingRequestUpdateStatus Status { get; set; }

        [CanBeNull] 
        public CreateOrEditPriceOfferInput PriceOfferInput { get; set; }

        public void AddValidationErrors(CustomValidationContext context)
        {
            if (Status == ShippingRequestUpdateStatus.Repriced && PriceOfferInput == null)
                context.Results.Add(new ValidationResult("YouMustAddPriceWhenTakeRepriceAction"));
        }
    }
}