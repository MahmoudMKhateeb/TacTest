using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TACHYON.Invoices.Transactions
{
    [Table("TransactionsChannels")]
    public class TransactionChannel : Entity<byte>
    {
        public string Channel { get; set; }
    }
}
