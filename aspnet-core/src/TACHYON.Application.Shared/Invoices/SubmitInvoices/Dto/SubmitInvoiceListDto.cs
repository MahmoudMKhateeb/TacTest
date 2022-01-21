using Abp.Application.Services.Dto;
using Abp.Domain.Entities.Auditing;
using System;

namespace TACHYON.Invoices.SubmitInvoices.Dto
{
    public class SubmitInvoiceListDto : EntityDto<long>, IHasCreationTime
    {
        public long? ReferencNumber { get; set; }
        public string TenantName { get; set; }
        public string Period { get; set; }
        public int PeriodId { get; set; }
        public SubmitInvoiceStatus Status { get; set; }
        public string StatusTitle { get { return Status.GetEnumDescription(); } }
        public Guid? DocumentId { get; set; }
        public string DocumentName { get; set; }
        public string DocumentContentType { get; set; }
        public DateTime CreationTime { get; set; }
        public decimal TotalAmount { get; set; }
    }
}