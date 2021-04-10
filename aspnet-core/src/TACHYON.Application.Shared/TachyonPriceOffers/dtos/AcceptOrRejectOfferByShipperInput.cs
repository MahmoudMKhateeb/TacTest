using Abp.Application.Services.Dto;
using Abp.Runtime.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TACHYON.TachyonPriceOffers.dtos
{
    public class AcceptOrRejectOfferByShipperInput : EntityDto, ICustomValidate
    {
        public bool IsAccepted { get; set; }
        public string RejectedReason { get; set; }

        public void AddValidationErrors(CustomValidationContext context)
        {
           // RejectedReason = RejectedReason.Trim();
            if (!IsAccepted && string.IsNullOrEmpty(RejectedReason) )
            context.Results.Add(new ValidationResult("TheRejectedReasonIsRequired"));
        }
    }
}
