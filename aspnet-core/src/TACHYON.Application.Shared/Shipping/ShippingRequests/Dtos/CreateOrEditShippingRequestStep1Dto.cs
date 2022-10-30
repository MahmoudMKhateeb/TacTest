using Abp.Application.Services.Dto;
using Abp.Runtime.Validation;
using Abp.Timing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TACHYON.Shipping.ShippingRequests.Dtos
{
    public class CreateOrEditShippingRequestStep1Dto : CreateOrEditShippingRequestStep1BaseDto, ICustomValidate, IShouldNormalize
    {
        public string ShipperReference { get; set; }
        public string ShipperInvoiceNo { get; set; }
        [Required] public DateTime StartTripDate { get; set; }
        public DateTime? EndTripDate { get; set; }

        public void AddValidationErrors(CustomValidationContext context)
        {
            //if (this.StartTripDate.Date < Clock.Now.Date)
            //{
            //    context.Results.Add(new ValidationResult("Start trip date cannot be before today"));
            //}

            if (EndTripDate.HasValue && StartTripDate.Date > EndTripDate.Value.Date)
            {
                context.Results.Add(new ValidationResult("The start date must be or equal to end date."));
            }

            if (IsBid)
            {
                RequestType = ShippingRequestType.Marketplace;
            }
            else if (IsTachyonDeal)
            {
                RequestType = ShippingRequestType.TachyonManageService;
            }
            else
            {
                RequestType = ShippingRequestType.DirectRequest;
            }

            //if (IsDirectRequest && CarrierTenantIdForDirectRequest == null)
            //{
            //    context.Results.Add(new ValidationResult("You must choose one carrier to send direct request"));
            //}
        }

        public void Normalize()
        {
            // to get date only without time
            BidStartDate = BidStartDate?.Date;
        }
    }
}