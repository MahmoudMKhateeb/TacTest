using Abp.Application.Services.Dto;
using NetTopologySuite.Geometries;
using System;

namespace TACHYON.Invoices.SubmitInvoices.Dto
{
    public class SubmitInvoiceFilterInput : PagedAndSortedResultRequestDto
    {
        public int? TenantId { get; set; }
        public int? PeriodId { get; set; }

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }
        public SubmitInvoiceStatus? Status { get; set; }
    }

    public class GetAllSubmitInvoicesInput
    {
        public string LoadData { get; set; }
    }

    public class GetAllSubmitInvoicesSearchInput
    {
        public string AccountNumber { get; set; }
        public long? WaybillOrSubWaybillNumber { get; set; }
        public string ContainerNumber { get; set; }
    }
}