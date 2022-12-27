using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using TACHYON.DedicatedDynamicInvoices.DedicatedDynamicInvoiceItems;
using TACHYON.Invoices;
using TACHYON.Invoices.SubmitInvoices;
using TACHYON.MultiTenancy;
using TACHYON.Shipping.ShippingRequests;

namespace TACHYON.DedicatedInvoices
{
    [Table("DedicatedDynamicInvoices")]
    public class DedicatedDynamicInvoice :FullAuditedEntity<long>
    {
        //todo will add info for submit invoice
        public int TenantId { get; set; }
        [ForeignKey("TenantId")]
        public Tenant Tenant { get; set; }
        public InvoiceAccountType InvoiceAccountType { get; set; }
        public long ShippingRequestId { get; set; }
        [ForeignKey("ShippingRequestId")]
        public ShippingRequest ShippingRequest { get; set; }
        public decimal SubTotalAmount { get; set; }
        public decimal TaxVat { get; set; }
        public decimal VatAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public long? InvoiceId { get; set; }
        [ForeignKey(nameof(InvoiceId))]
        public Invoice Invoice { get; set; }
        public long? SubmitInvoiceId { get; set; }
        [ForeignKey(nameof(SubmitInvoiceId))]
        public SubmitInvoice SubmitInvoice { get; set; }

        public string Notes { get; set; }
        //public InvoiceStatus InvoiceStatus { get; set; }
        public ICollection<DedicatedDynamicInvoiceItem> DedicatedDynamicInvoiceItems { get; set; }
    }
}
