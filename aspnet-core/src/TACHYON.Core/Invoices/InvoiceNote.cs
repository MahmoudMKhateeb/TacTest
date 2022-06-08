using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Stripe;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using TACHYON.Invoices.SubmitInvoices;
using TACHYON.MultiTenancy;

namespace TACHYON.Invoices
{
    [Table("InvoiceNotes")]
    public class InvoiceNote : FullAuditedEntity<long>, IMustHaveTenant
    {
        public int TenantId { get; set; }
        [ForeignKey("TenantId")]
        public Tenant Tenant { get; set; }
        public NoteType NoteType { get; set; }
        public NoteStatus Status { get; set; }
        public string Remarks { get; set; }
        public string WaybillNumber { get; set; }
        public decimal VatAmount { get; set; }
        public decimal Price { get; set; }
        public decimal TotalValue { get; set; }
        public string ReferanceNumber { get; set; }
        public long? InvoiceNumber { get; set; }
        public VoidType VoidType { get; set; }
        public bool IsManual { get; set; }
        #region Note
        public bool CanBePrinted { get; set; }
        public string Note { get; set; }
        #endregion
        public List<InvoiceNoteItem> InvoiceItems { get; set; }
    }
}
