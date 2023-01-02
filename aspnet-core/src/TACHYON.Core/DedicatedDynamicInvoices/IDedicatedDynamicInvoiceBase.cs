using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using TACHYON.DedicatedDynamicActorInvoices.DedicatedDynamicActorInvoiceItems;
using TACHYON.Invoices.SubmitInvoices;
using TACHYON.Invoices;
using TACHYON.Shipping.ShippingRequests;

namespace TACHYON.DedicatedDynamicInvoices
{
    public interface IDedicatedDynamicInvoiceBase
    {
        InvoiceAccountType InvoiceAccountType { get; set; }
        long ShippingRequestId { get; set; }
        [ForeignKey("ShippingRequestId")]
        ShippingRequest ShippingRequest { get; set; }
        decimal SubTotalAmount { get; set; }
        decimal TaxVat { get; set; }
        decimal VatAmount { get; set; }
        decimal TotalAmount { get; set; }

        string Notes { get; set; }
        //public InvoiceStatus InvoiceStatus { get; set; }
    }
}
