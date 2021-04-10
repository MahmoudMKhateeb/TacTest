using Abp.Application.Services.Dto;
using Abp.Domain.Entities.Auditing;
using System;
using TACHYON.Invoices.SubmitInvoices;

namespace TACHYON.Invoices.Groups.Dto
{
    public class GroupPeriodListDto: EntityDto<long>, IHasCreationTime
    {
        public string TenantName { get; set; }
        public string Period { get; set; }
        public SubmitInvoiceStatus Status { get; set; }
        public string StatusTitle { get; set; }
        //  public bool IsDemand { get; set; }
        public Guid? DocumentId { get; set; }
        public string DocumentName { get; set; }
        public string DocumentContentType { get; set; }
        //    public bool IsClaim { get; set; }
        public string Note { get; set; }
        public decimal AmountWithTaxVat { get; set; }
        public decimal VatAmount { get; set; }

        public decimal Amount { get; set; }
        public decimal TaxVat { get; set; }
        public DateTime CreationTime { get; set; }
    }
}
