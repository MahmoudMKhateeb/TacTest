using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using TACHYON.Invoices.Periods;
using TACHYON.MultiTenancy;

namespace TACHYON.Invoices
{
    [Table("Invoices")]

    public class Invoice: FullAuditedEntity<long>, IMustHaveTenant
    {
        public int TenantId { get; set; }

        [ForeignKey("TenantId")]
        public Tenant Tenant { get; set; }

        public int PeriodId { get; set; }

        [ForeignKey("PeriodId")]
        public InvoicePeriod InvoicePeriod { get; set; }
        public DateTime DueDate { get; set; }
        public bool IsPaid { get; set; }

        public string Note { get; set; }
        public decimal? Amount { get; set; }
        public decimal? TotalSumExclVat { get; set; }
        public decimal? TotalVat { get; set; }

        public bool? IsAccountReceivable { get; set; }

        public List<InvoiceShippingRequests> ShippingRequests { get; set; }

    }
}
