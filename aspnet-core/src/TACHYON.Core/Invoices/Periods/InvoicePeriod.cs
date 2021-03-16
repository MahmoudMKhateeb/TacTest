using Abp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace TACHYON.Invoices.Periods
{
    [Table("InvoicePeriods")]
    public class InvoicePeriod : FullAuditedEntity
    {

        [Required]
        [StringLength(InvoicePeriodConst.MaxDisplayNameLength, MinimumLength = InvoicePeriodConst.MinDisplayNameLength)]
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public double CreditLimit { get; set; }
        public InvoicePeriodType PeriodType { get; set; }


        public int FreqInterval { get; set; }

        public FrequencyRelativeInterval FreqRelativeInterval { get; set; }


        public string FreqRecurrence { get; set; }

        public DateTime? NextRunDate { get; set; }

        public bool ShipperOnlyUsed { get; set; }

        public bool Enabled { get; set; }

        public DateTime? LastRunDate { get; set; }


        public string Cronexpression { get; set; }

    }
}
