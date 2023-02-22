using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;
using System.Text;
using TACHYON.Invoices;

namespace TACHYON.DedidcatedDynamicActorInvoices.Dtos
{
    public class DedicatedDynamicActorInvoiceDto : EntityDto<long>
    {
        public int? ShipperActorId { get; set; }
        public string ShipperActor { get; set; }
        public int? CarrierActorId { get; set; }
        public string CarrierActor { get; set; }
        public DateTime CreationTime { get; set; }
        public InvoiceAccountType InvoiceAccountType { get; set; }
        public string InvoiceAccountName { get; set; }
        public long ShippingRequestId { get; set; }
        public string ShippingRequestReference { get; set; }
        public decimal SubTotalAmount { get; set; }
        public decimal TaxVat { get; set; }
        public decimal VatAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public string InvoiceNumber { get; set; }
        public string SubmitInvoiceNumber { get; set; }
        public string Notes { get; set; }
    }
}
