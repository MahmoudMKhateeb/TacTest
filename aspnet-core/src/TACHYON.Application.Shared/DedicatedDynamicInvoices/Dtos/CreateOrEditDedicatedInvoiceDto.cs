using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using TACHYON.Invoices;

namespace TACHYON.DedicatedDynamicInvoices.Dtos
{
    public class CreateOrEditDedicatedInvoiceDto :EntityDto<long?>
    {
        public InvoiceAccountType InvoiceAccountType { get; set; }
        public int TenantId { get; set; }
        public long ShippingRequestId { get; set; }
        public decimal SubTotalAmount { get; set; }
        public decimal TaxVat { get; set; }
        public decimal VatAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public string Notes { get; set; }

        public List<CreateOrEditDedicatedInvoiceItemDto> DedicatedInvoiceItems { get; set; }
    }
}
