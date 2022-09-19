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
        public DateTime? SubmittedDateFrom { get; set; }
        public DateTime? SubmittedDateTo { get; set; }
        public DateTime? AcceptanceDateFrom { get; set; }
        public DateTime? AcceptanceDateTo { get; set; }
        public DateTime? PaymentDateFrom { get; set; }
        public DateTime? PaymentDateTo { get; set; }
        public long? WaybillOrSubWaybillNumber { get; set; }
        public string ContainerNumber { get; set; }
    }
}