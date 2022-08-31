using Abp.Domain.Entities.Auditing;
using System.ComponentModel.DataAnnotations.Schema;

namespace TACHYON.DynamicInvoices.DynamicInvoiceItems
{
    [Table("DynamicInvoiceItems")]
    public class DynamicInvoiceItem : FullAuditedEntity<long>
    {
        public string Description { get; set; }

        public decimal Price { get; set; }

        /// <summary>
        /// All Dynamic invoice Items is grouped by DynamicInvoiceId
        /// <remarks>DynamicInvoice `Root` Id</remarks>
        /// </summary>
        public long DynamicInvoiceId { get; set; }

        [ForeignKey(nameof(DynamicInvoiceId))]
        public DynamicInvoice DynamicInvoice { get; set; }
        
    }
}