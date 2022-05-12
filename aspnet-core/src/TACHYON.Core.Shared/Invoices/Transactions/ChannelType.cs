using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace TACHYON.Invoices.Transactions
{
    public enum ChannelType : byte
    {
        [Description("Invoices")] Invoices = 1,
        [Description("Balance recharge")] BalanceRecharge = 2
    }
}