using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Dashboards.Host.TMS_HostDto
{
    public class GetInvoicesPaidBeforeDueDateOutput
    {
        public int totalInvoices { get; set; }
        public decimal PaidInvoicesBeforeDueDatePercantage { get; set; }
        public decimal UnPaidInvoicesBeforeDueDatePercantage { get; set; }
    }
}
