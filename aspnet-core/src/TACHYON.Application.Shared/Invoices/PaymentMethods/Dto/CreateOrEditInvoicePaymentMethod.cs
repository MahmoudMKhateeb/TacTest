using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using TACHYON.Invoices.PaymentMethod;

namespace TACHYON.Invoices.PaymentMethods.Dto
{
  public  class CreateOrEditInvoicePaymentMethod:EntityDto<int>,IValidatableObject
    {
        [StringLength(250)]
        [Required]
        public string DisplayName { get; set; }
        [Required]
        public InvoicePaymentMethodType PaymentMethodType { get; set; }
        public int InvoiceDueDateDays { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            
            throw new NotImplementedException();
        }
    }
}
