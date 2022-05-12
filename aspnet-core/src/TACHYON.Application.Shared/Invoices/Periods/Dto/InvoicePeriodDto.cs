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
        public int FreqInterval { get; set; } = 1;
        public string FreqRecurrence { get; set; }
        public bool Enabled { get; set; }
        public bool ShipperOnlyUsed { get; set; }

        public FrequencyRelativeInterval FreqRelativeInterval { get; set; }


        [NotMapped] public string Cronexpression { get; set; }

        public void AddValidationErrors(CustomValidationContext context)
        {
            if (PeriodType == InvoicePeriodType.PayInAdvance || PeriodType == InvoicePeriodType.PayuponDelivery)
            {
                FreqRecurrence = null;
                FreqInterval = 0;
                FreqRelativeInterval = 0;
                FreqRecurrence = null;
            }
        }
    }
}