using Abp.Application.Services.Dto;
using System;
using TACHYON.Invoices.PaymentMethod;

namespace TACHYON.Invoices.PaymentMethods.Dto
{
    public  class InvoicePaymentMethodListDto:EntityDto<int>
    {
        public string DisplayName { get; set; }
        public InvoicePaymentType PaymentType { get; set; }
        public int? InvoiceDueDateDays { get; set; }
        public DateTime CreationTime { get; set; }

    }
}
