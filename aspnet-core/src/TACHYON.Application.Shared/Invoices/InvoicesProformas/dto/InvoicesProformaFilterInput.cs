using Abp.Application.Services.Dto;
using System;

namespace TACHYON.Invoices.InvoicesProformas.dto
{
    public class InvoicesProformaFilterInput : PagedAndSortedResultRequestDto
    {
        public int? TenantId { get; set; }
        public decimal? MinAmount { get; set; }
        public decimal? MaxAmount { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}