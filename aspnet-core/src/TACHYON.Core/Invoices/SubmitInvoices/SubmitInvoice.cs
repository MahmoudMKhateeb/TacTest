using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using TACHYON.Invoices.Periods;
using TACHYON.MultiTenancy;
using TACHYON.Penalties;

namespace TACHYON.Invoices.SubmitInvoices
{
    [Table("SubmitInvoices")]
    public class SubmitInvoice : FullAuditedEntity<long>, IMustHaveTenant
    {
        public long? ReferencNumber { get; set; }
        public int TenantId { get; set; }
        [ForeignKey(nameof(TenantId))] public Tenant Tenant { get; set; }
        public int PeriodId { get; set; }
        [ForeignKey(nameof(PeriodId))] public InvoicePeriod InvoicePeriodsFK { get; set; }
        public InvoiceChannel Channel { get; set; }
        public decimal SubTotalAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal VatAmount { get; set; }
        public decimal TaxVat { get; set; }
        public Guid? DocumentId { get; set; }
        public string DocumentName { get; set; }
        public string DocumentContentType { get; set; }
        public ICollection<SubmitInvoiceTrip> Trips { get; set; }
        public SubmitInvoiceStatus Status { get; set; }
        public string RejectedReason { get; set; }
        public ICollection<Penalty> Penalties { get; set; }

        public SubmitInvoice()
        {
            Trips = new List<SubmitInvoiceTrip>();
        }
    }
}