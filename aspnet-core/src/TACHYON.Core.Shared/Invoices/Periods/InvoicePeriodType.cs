using System.ComponentModel;

namespace TACHYON.Invoices.Periods
{
    public enum InvoicePeriodType : byte
    {
        [Description("PayInAdvance")] PayInAdvance = 1,

        [Description("PayuponDelivery")] PayuponDelivery = 2,

        [Description("Period")] Period = 3,
        //Weekly = 4,
        //Monthly = 5
    }
}
