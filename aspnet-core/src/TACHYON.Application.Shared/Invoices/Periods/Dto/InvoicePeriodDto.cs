using Abp.Application.Services.Dto;
using Abp.Runtime.Validation;
using Abp.Timing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TACHYON.Invoices.Periods.Dto
{
    public class InvoicePeriodDto : FullAuditedEntityDto<int?>, ICustomValidate
    {
        [Required]
        [StringLength(InvoicePeriodConst.MaxDisplayNameLength, MinimumLength = InvoicePeriodConst.MinDisplayNameLength)]
        public string DisplayName { get; set; }
        public InvoicePeriodType PeriodType { get; set; }
        public int FreqInterval { get; set; }= 1;
        public string FreqRecurrence { get; set; }
        public bool Enabled { get; set; }
        public bool ShipperOnlyUsed { get; set; }

        public FrequencyRelativeInterval FreqRelativeInterval { get; set; }

        public double CreditLimit { get; set; }

        [NotMapped]
        public string Cronexpression { get; set; }
        public void AddValidationErrors(CustomValidationContext context)
        {
            if (PeriodType == InvoicePeriodType.PayInAdvance || PeriodType==InvoicePeriodType.PayuponDelivery)
            {
                FreqRecurrence = null;
                CreditLimit = 0;
                FreqInterval = 0;
                FreqRelativeInterval = 0;
                FreqRecurrence = null;
            }
            else
            {
                if (CreditLimit<0)
                    context.Results.Add(new ValidationResult("	The field CreditLimit value must greater than or equal to 1."));
                switch (PeriodType)
                {

                    case InvoicePeriodType.Daily:
                        FreqRelativeInterval = 0;
                        FreqRecurrence = null;
                        if (FreqInterval < 1) context.Results.Add(new ValidationResult("	The field FreqInterval value must greater than or equal to 1."));
                        Cronexpression = $"0 59 23 {Clock.Now.Day}/{FreqInterval} * ?";
                        break;

                    case InvoicePeriodType.Weekly:
                        FreqRelativeInterval = 0;
                        if (FreqInterval < 1) context.Results.Add(new ValidationResult("	The field FreqInterval value must greater than or equal to 1."));
                        if (FreqRecurrence == null)
                            context.Results.Add(new ValidationResult("Please select at least one list for field FrequencyRecurrenceWeekDays	"));

                        Cronexpression = $"0 59 23 ? * {FreqRecurrence}#{FreqInterval}";
                        break;
                    case InvoicePeriodType.Monthly:
                        if (FreqRecurrence == null)
                            context.Results.Add(new ValidationResult("Please select at least one list for field FrequencyRecurrenceMonths	"));

                        if (FreqRelativeInterval == 0)
                        {
                            if (FreqInterval < 1 || FreqInterval > 31)

                                context.Results.Add(new ValidationResult("The field FreqIntervalMonthlyperday value must between 1 and 31.	"));

                            Cronexpression = $"0 59 23 {FreqInterval} {FreqRecurrence} ?";

                        }
                        else
                        {
                            Cronexpression = $"0 59 23 ? {FreqRecurrence} {FreqInterval}#{FreqRelativeInterval}";
                        }

                        break;
                }

            }
        }
    }
}