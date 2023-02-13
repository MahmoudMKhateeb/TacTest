
using Abp.Runtime.Validation;
using Abp.Timing;
using System;
using System.ComponentModel.DataAnnotations;
using TACHYON.Actors;

namespace TACHYON.Dashboards.Broker
{
    public class GetMostActiveActorsInput : ICustomValidate
    {
        [Required]
        public ActorTypesEnum ActorType { get; set; }
        public DateRangeType RangeType { get; set; }

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }
        
        public void AddValidationErrors(CustomValidationContext context)
        {
            var currentDate = Clock.Now.Date;
            if (ActorType != ActorTypesEnum.Carrier && ActorType != ActorTypesEnum.Shipper)
                context.Results.Add(new ValidationResult("Not Supported Actor Type"));
            
            if (RangeType == default)
                context.Results.Add(new ValidationResult("You Must Select A Range"));
            else if (RangeType == DateRangeType.CustomRange)
            {
                if (!FromDate.HasValue || !ToDate.HasValue)
                {
                    context.Results.Add(new ValidationResult("Please Set From Date And To Date When Selecting Custom Range"));
                } else if (FromDate.Value.Date > currentDate || ToDate.Value.Date > currentDate)
                {
                    context.Results.Add(new ValidationResult("Please Set From Date And To Date When Selecting Custom Range"));
                }
            }
                
        }
    }
}