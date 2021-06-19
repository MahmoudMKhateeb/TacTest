using Abp.Application.Services.Dto;
using Abp.Domain.Entities.Auditing;
using System;

namespace TACHYON.Invoices.InvoicesProformas.dto
{
    public class InvoicesProformaListDto : EntityDto<long>, IHasCreationTime
    {
        public DateTime CreationTime { get; set; }
        public string ClientName { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal VatAmount { get; set; }
    }
}
