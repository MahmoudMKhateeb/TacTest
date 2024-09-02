using Abp.Application.Services.Dto;
using System;

namespace TACHYON.DynamicInvoices.Dto
{
    public class DynamicInvoiceCustomItemDto : EntityDto<long>
    {

        public string ItemName { get; set; }
        public string Description { get; set; }
        public decimal VatAmount { get; set; }
        public decimal VatTax { get; set; }
        public decimal TotalAmount { get; set; }
        public int? Quantity { get; set; }
        public decimal Price { get; set; }

    }
}