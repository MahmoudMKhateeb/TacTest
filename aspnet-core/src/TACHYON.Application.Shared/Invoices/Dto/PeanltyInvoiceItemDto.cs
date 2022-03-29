using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Invoices.Dto
{
    public class PeanltyInvoiceItemDto
    {
        public string Sequence { get; set; }
        public string WayBillNumber { get; set; }
        public decimal ItmePrice { get; set; }
        public decimal VatAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public string Date { get; set; }
        public string Remarks { get; set; }
        public string ContainerNumber { get; set; }
    }
}
