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
        public long? InvoiceNumber { get; set; }
        public int TenantId { get; set; }
        [ForeignKey(nameof(TenantId))]
        public Tenant Tenant { get; set; }
        public int PeriodId { get; set; }
        [ForeignKey(nameof(PeriodId))]
        public InvoicePeriod InvoicePeriod { get; set; }
        public DateTime DueDate { get; set; }
        public bool IsPaid { get; set; }
        public string Note { get; set; }
        public decimal SubTotalAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal VatAmount { get; set; }
        public decimal TaxVat { get; set; }
        public InvoiceAccountType AccountType { get; set; }
        public ICollection<InvoiceTrip> Trips { get; set; }
        public Invoice()
        {
            Trips = new List<InvoiceTrip>();
        }

    }
}
