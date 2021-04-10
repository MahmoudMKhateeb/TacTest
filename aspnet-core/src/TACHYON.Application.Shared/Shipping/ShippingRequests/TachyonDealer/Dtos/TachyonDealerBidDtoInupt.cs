using Abp.Application.Services.Dto;
using Abp.Runtime.Validation;
using Abp.Timing;
using System;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.Shipping.ShippingRequests.TachyonDealer.Dtos
{
    public class TachyonDealerBidDtoInupt : EntityDto<long>, ICustomValidate
    {

        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }

        public void AddValidationErrors(CustomValidationContext context)
        {
            if (StartDate.Date<Clock.Now.Date)
            {
                context.Results.Add(new ValidationResult("The start date must be higher or equal to current date"));

            }
            else if (StartDate.Date > EndDate.Date)
            {
                context.Results.Add(new ValidationResult("The start date must be lower or equal to end date."));
            }
        }

    }
}
