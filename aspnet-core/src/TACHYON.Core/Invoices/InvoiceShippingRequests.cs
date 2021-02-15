using Abp.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using TACHYON.Shipping.ShippingRequests;

namespace TACHYON.Invoices
{
    [Table("InvoiceShippingRequests")]
    public class InvoiceShippingRequests:Entity<long>
    {
        public long RequestId { get; set; }

        [ForeignKey("RequestId")]
        public ShippingRequest ShippingRequests { get; set; }
        public long InvoiceId { get; set; }

        [ForeignKey("InvoiceId")]
        public Invoice Invoice { get; set; }
    }
}
