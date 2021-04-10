using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Invoices.Periods.Dto
{
  public  class InvoicePeriodList1: FullAuditedEntityDto
    {
        public string DisplayName { get; set; }
        public InvoicePeriodType PeriodType { get; set; }

        public int FreqInterval { get; set; } = 1;
        public FrequencyRelativeInterval FreqRelativeInterval { get; set; }
        public string FreqRecurrence { get; set; }
        public bool Enabled { get; set; }
        public bool ShipperOnlyUsed { get; set; }

    }
}
