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
    public class CreateOrEditShippingRequestStep1Dto : CreateOrEditShippingRequestStep1BaseDto, ICustomValidate
    {
        public string ShipperReference { get; set; }
        public string ShipperInvoiceNo { get; set; }
        [Required] public DateTime StartTripDate { get; set; }
        public DateTime? EndTripDate { get; set; }

        public void AddValidationErrors(CustomValidationContext context)
        {

            if (EndTripDate.HasValue && StartTripDate.Date > EndTripDate.Value.Date)
            {
                context.Results.Add(new ValidationResult("The start date must be or equal to end date."));
            }
        }

    }
}