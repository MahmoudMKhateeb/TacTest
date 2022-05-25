using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace TACHYON.Invoices
{
    public enum NoteType : byte
    {
        [Description("Credit")]
        Credit = 1,
        [Description("Debit")]
        Debit = 2,
    }
}
