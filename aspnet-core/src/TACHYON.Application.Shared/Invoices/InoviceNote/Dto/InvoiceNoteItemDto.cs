using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Invoices.InoviceNote.Dto
{
    public class InvoiceNoteItemDto
    {
        public string WayBillNumber { get; set; }
        public decimal Price { get; set; }
        public decimal VatAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public string Sequence { get; set; }
        public string Date { get; set; }
    }
}
