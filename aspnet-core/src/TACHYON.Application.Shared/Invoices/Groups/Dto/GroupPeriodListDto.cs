using Abp.Application.Services.Dto;
using Abp.Domain.Entities.Auditing;
using System;

namespace TACHYON.Invoices.Groups.Dto
{
    public class GroupPeriodListDto: EntityDto<long>, IHasCreationTime
    {
        public string TenantName { get; set; }
        public string Period { get; set; }
        public bool IsDemand { get; set; }
        public Guid? BinaryObjectId { get; set; }
        public bool IsClaim { get; set; }
        public string Note { get; set; }
        public decimal AmountWithTaxVat { get; set; }
        public decimal VatAmount { get; set; }

        public decimal Amount { get; set; }
        public decimal TaxVat { get; set; }
        public DateTime CreationTime { get; set; }
    }
}
