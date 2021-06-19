using System;
using System.ComponentModel;

namespace TACHYON.Invoices
{
    public enum InvoiceAccountType : Byte
    {
        [Description("Shipper")]
        AccountReceivable=1,
        [Description("Carrier")]
        AccountPayable =2
    }
}
