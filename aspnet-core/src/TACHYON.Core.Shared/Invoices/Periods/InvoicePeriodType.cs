using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Invoices.Periods
{


    public enum InvoicePeriodType : byte
    {
        PayInAdvance = 1,
        PayuponDelivery=2,
        Daily = 3,
        Weekly = 4,
        Monthly = 5

    }
}
