using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Invoices.Dto
{
    public class InvoiceOutSideDto : EntityDto<long>
    {
        public long? InvoiceNumber { get; set; }
        public string CompanyName { get; set; }
        public DateTime DueDate { get; set; }
        public string VATNumber { get; set; }
        public decimal SubTotalAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal VatAmount { get; set; }
        public int TenantId { get; set; }
    }
}