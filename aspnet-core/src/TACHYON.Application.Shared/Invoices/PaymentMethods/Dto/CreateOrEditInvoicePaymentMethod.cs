using Abp.Application.Services.Dto;
using Abp.Runtime.Validation;
using System.ComponentModel.DataAnnotations;
using TACHYON.Invoices.PaymentMethod;

namespace TACHYON.Invoices.PaymentMethods.Dto
{
    public class CreateOrEditInvoicePaymentMethod : EntityDto<int>, ICustomValidate
    {
        [StringLength(250)] [Required] public string DisplayName { get; set; }
        [Required] public InvoicePaymentType PaymentType { get; set; }
        public int? InvoiceDueDateDays { get; set; }

        public void AddValidationErrors(CustomValidationContext context)
        {
            if (InvoiceDueDateDays < 0 && PaymentType == InvoicePaymentType.Days)
            {
                context.Results.Add(new ValidationResult("TheNumberOfDaysMustBeGreatThanZero"));
            }
            else if (PaymentType != InvoicePaymentType.Days)
                InvoiceDueDateDays = default;
        }
    }
}