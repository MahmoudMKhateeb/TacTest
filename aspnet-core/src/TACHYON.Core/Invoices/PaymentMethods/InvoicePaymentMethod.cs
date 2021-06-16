using Abp.Domain.Entities.Auditing;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using TACHYON.Invoices.PaymentMethod;

namespace TACHYON.Invoices.PaymentMethods
{
    [Table("InvoicePaymentMethods")]
    public class InvoicePaymentMethod : FullAuditedEntity<int>
    {
        [StringLength(250)]
        public string DisplayName { get; set; }
        public InvoicePaymentType PaymentType { get; set; }

        public int InvoiceDueDateDays { get; set; }

    }
}
