using Abp.Domain.Entities.Auditing;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using TACHYON.DynamicInvoices.DynamicInvoiceItems;
using TACHYON.Invoices;
using TACHYON.Invoices.SubmitInvoices;
using TACHYON.MultiTenancy;

namespace TACHYON.DynamicInvoices
{
    [Table("DynamicInvoices")]
    public class DynamicInvoice : FullAuditedEntity<long>
    {
        /// <summary>
        /// Credit Tenant it's a tenant that we will withdraw from its account balance
        /// <remarks>Usually `CreditTenantId` represent TenantId of a shipper</remarks>
        /// </summary>
        public int? CreditTenantId { get; set; }

        [ForeignKey(nameof(CreditTenantId))]
        public Tenant CreditTenant { get; set; }

        /// <summary>
        /// Debit Tenant it's a tenant that We will make a deposit into his account
        /// <remarks>Usually `DebitTenantId` represent TenantId of a carrier</remarks>
        /// </summary>
        public int? DebitTenantId { get; set; }

        [ForeignKey(nameof(DebitTenantId))]
        public Tenant DebitTenant { get; set; }

        public long? InvoiceId { get; set; }
        
        [ForeignKey(nameof(InvoiceId))]
        public Invoice Invoice { get; set; }
        
        public string Notes { get; set; }
        
        public long? SubmitInvoiceId {get; set; }
        
        [ForeignKey(nameof(SubmitInvoiceId))]
        public SubmitInvoice SubmitInvoice { get; set; }

        
        /// <summary>
        /// Total of Items Prices
        /// </summary>
        public decimal SubTotalAmount { get; set; }
        
        /// <summary>
        /// VatAmount = VatTax * SubTotalAmount
        /// </summary>
        public decimal VatAmount { get; set; }

        /// <summary>
        /// TotalAmount = VatAmount + SubTotalAmount
        /// </summary>
        public decimal TotalAmount { get; set; }
        
        public List<DynamicInvoiceItem> Items { get; set; }
    }
}