using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using TACHYON.Invoices.Periods.Dto;

namespace TACHYON.Invoices.Dto
{
   public class InvoiceListDto: FullAuditedEntityDto<long>
    {
        public int PeriodId { get; set; }
        public InvoiceTenantDto Tenant { get; set; }
        public InvoicePeriodDto InvoicePeriodDto { get; set; }
        public DateTime DueDate { get; set; }
        public bool? IsPaid { get; set; }

        public string Note { get; set; }
        public decimal? Amount { get; set; }
        public decimal? TotalSumExclVat { get; set; }
        public decimal? TotalVat { get; set; }


    }


}
