using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace TACHYON.Invoices
{
    public enum VoidType
    {
        [Description("None")]
        None = 0,
        [Description("FullVoid")]
        FullVoid = 1,
        [Description("PartialVoid")]
        PartialVoid = 2,
    }
}
