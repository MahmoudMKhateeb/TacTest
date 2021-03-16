using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TACHYON.Invoices.Groups
{
    [Table("GroupPeriodsInvoices")]

    public class GroupPeriodInvoice:Entity<long>
    {
        public long InvoiceId { get; set; }
        [ForeignKey("InvoiceId")]
        public Invoice Invoice { get; set; }

        public long GroupId { get; set; }

        [ForeignKey("GroupId")]
        public GroupPeriod GroupPeriod { get; set; }

    }
}
