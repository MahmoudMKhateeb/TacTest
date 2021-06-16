using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using TACHYON.Invoices.PaymentMethod;

namespace TACHYON.Invoices.PaymentMethods.Dto
{
  public  class InvoicePaymentMethodListDto:EntityDto<int>
    {
        public string DisplayName { get; set; }
        public InvoicePaymentMethodType PaymentMethodType { get; set; }
        public int InvoiceDueDateDays { get; set; }
        public DateTime CreationTime { get; set; }

    }
}
