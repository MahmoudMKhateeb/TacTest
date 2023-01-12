using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Invoices.Dto
{
    public class SAASInvoiceItemDto
    {
        public string Sequence { get; set; }
        public string Type { get; set; }
        public string IsIntegratedWithBayan { get; set; }
        public decimal PricePerItem { get; set; }
        public int QTY { get; set; }
        public decimal ItemSubTotalAmount { get; set; }
        public decimal ItemTaxVat { get; set; }
        public decimal ItemVatAmount { get; set; }
        public decimal ItemTotalAmount { get; set; }
        public string Remarks { get; set; }
        // total amount
        public decimal TotalAmount { get; set; }
        public decimal VatAmount { get; set; }
        public decimal SubTotalAmount { get; set; }
    }
}
