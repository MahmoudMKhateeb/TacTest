using Abp.Application.Services.Dto;
using Abp.Runtime.Validation;
using Castle.Core.Internal;
using System;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.DynamicInvoices
{
    public class CreateOrEditDynamicInvoiceCustomItemDto : EntityDto<long?>, ICustomValidate
    {
        [Required]
        public string ItemName { get; set; }
        [Required]
        public string Description { get; set; }
        public decimal VatAmount { get; set; }
        public decimal VatTax { get; set; }
        public decimal TotalAmount { get; set; }
        public int? Quantity { get; set; }
        public decimal Price { get; set; }
        public void AddValidationErrors(CustomValidationContext context)
        {
           
        }

        
    }
}