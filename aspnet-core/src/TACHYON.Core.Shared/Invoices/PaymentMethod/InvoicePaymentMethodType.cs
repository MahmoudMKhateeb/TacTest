using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Invoices.PaymentMethod
{
    public enum InvoicePaymentMethodType :byte
    {
        PayInAdvance = 1,
        PayuponDelivery=2,
        Days = 3,
    }
}
