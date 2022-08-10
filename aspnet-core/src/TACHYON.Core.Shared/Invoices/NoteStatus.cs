using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Invoices
{
    public enum NoteStatus : byte
    {
        Draft,
        Confirm,
        Canceled,
        WaitingtobePaid,
        Paid
    }
}
