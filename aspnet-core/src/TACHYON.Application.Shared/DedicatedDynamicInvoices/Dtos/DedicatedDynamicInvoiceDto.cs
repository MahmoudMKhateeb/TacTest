using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using TACHYON.Invoices;
using static TACHYON.TACHYONDashboardCustomizationConsts.Widgets;

namespace TACHYON.DedicatedDynamicInvoices.Dtos
{
    public class DedicatedDynamicInvoiceDto : EntityDto<long>
    {
        public DateTime CreationTime { get; set; }
        public int TenantId { get; set; }
        public string TenantName { get; set; }
        public InvoiceAccountType InvoiceAccountType { get; set; }
        public string InvoiceAccountName { get; set; }
        public long ShippingRequestId { get; set; }
        public string ShippingRequestReference { get; set; }
        public decimal SubTotalAmount { get; set; }
        public decimal TaxVat { get; set; }
        public decimal VatAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public long InvoiceNumber { get; set; }
        public long SubmitInvoiceNumber { get; set; }
        public string Notes { get; set; }
    }
}
