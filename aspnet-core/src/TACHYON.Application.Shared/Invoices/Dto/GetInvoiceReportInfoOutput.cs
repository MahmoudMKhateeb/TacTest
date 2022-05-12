using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Invoices.Dto
{
    public class GetInvoiceReportInfoOutput
    {
        public long InvoiceNo { get; set; }
        public DateTime InvoiceDate { get; set; }
        public DateTime DueDate { get; set; }
        public string Attn { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
        public string ContractNo { get; set; }
        public string BillTo { get; set; }
        public string CR { get; set; }
        public string Address { get; set; }
        public string ProjectName { get; set; }

        // price
        public decimal InvoiceSubTotal { get; set; }
        public decimal VatAmount { get; set; }
        public decimal DueAmount { get; set; }
    }
}