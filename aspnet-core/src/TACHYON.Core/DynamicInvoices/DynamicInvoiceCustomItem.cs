
using Abp.Domain.Entities.Auditing;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TACHYON.DynamicInvoices.DynamicInvoiceItems;
using TACHYON.Invoices;
using TACHYON.Invoices.SubmitInvoices;
using TACHYON.MultiTenancy;

namespace TACHYON.DynamicInvoices
{
    [Table("DynamicInvoiceCustomItems")]
    public class DynamicInvoiceCustomItem : FullAuditedEntity<long>
    {
        public long DynamicInvoiceId { get; set; }
        [ForeignKey(nameof(DynamicInvoiceId))]
        public DynamicInvoice DynamicInvoice { get; set; }

        [Required]
        public string ItemName { get; set; }

        [Required]
        public string Description { get; set; }
        public decimal VatAmount { get; set; }
        public decimal VatTax { get; set; }
        public decimal TotalAmount { get; set; }
        public int? Quantity { get; set; }
        public decimal Price { get; set; }
    }
}

