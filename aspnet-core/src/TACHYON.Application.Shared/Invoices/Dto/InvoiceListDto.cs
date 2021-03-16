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
        public string TenantName { get; set; }
        public string Period { get; set; }
        public DateTime DueDate { get; set; }
        public bool IsPaid { get; set; }
        public bool IsAccountReceivable { get; set; }
        public string Note { get; set; }
        public decimal AmountWithTaxVat { get; set; }
        public decimal VatAmount { get; set; }

        public decimal Amount { get; set; }
        public decimal TaxVat { get; set; }


    }


}
