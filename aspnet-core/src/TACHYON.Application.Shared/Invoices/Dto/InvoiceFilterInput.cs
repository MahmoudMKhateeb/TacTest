using Abp.Application.Services.Dto;
using System;

namespace TACHYON.Invoices.Dto
{
    public class InvoiceFilterInput : PagedAndSortedResultRequestDto
    {
        public int? TenantId { get; set; }
        public int? PeriodId { get; set; }

        public bool? IsPaid { get; set; }

        public InvoiceAccountType? AccountType { get; set; }

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public DateTime? DueFromDate { get; set; }

        public DateTime? DueToDate { get; set; }
    }
}