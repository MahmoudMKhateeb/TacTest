using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using TACHYON.Invoices.Periods;
using TACHYON.MultiTenancy;

namespace TACHYON.Invoices.Groups
{
    [Table("GroupPeriods")]

    public class GroupPeriod : FullAuditedEntity<long>, IMustHaveTenant
    {
        public int TenantId { get; set; }

        [ForeignKey("TenantId")]
        public Tenant Tenant { get; set; }
        public int PeriodId { get; set; }

        [ForeignKey("PeriodId")]
        public InvoicePeriod InvoicePeriod { get; set; }
        public bool IsDemand { get; set; }
        public string Note { get; set; }
        public decimal AmountWithTaxVat { get; set; }
        public decimal VatAmount { get; set; }

        public decimal Amount { get; set; }
        public decimal TaxVat { get; set; }
        public Guid? BinaryObjectId { get; set; }
        public string DemandFileName { get; set; }
        public string DemandFileContentType { get; set; }
      
        public bool IsClaim { get; set; }
        public List<GroupShippingRequests> ShippingRequests { get; set; }
    }
}
