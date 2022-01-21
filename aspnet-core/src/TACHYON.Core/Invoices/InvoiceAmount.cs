using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Invoices
{
    public class InvoiceAmount
    {
        public decimal Amount { get; set; }
        public decimal VatAmount { get; set; }
        public decimal TaxVat { get; set; }
        public decimal TotalAmount { get; set; }

        public InvoiceAmount(decimal TaxVat,
            decimal Amount,
            decimal VatAmount,
            decimal TotalAmount)
        {
            this.TaxVat = TaxVat;
            this.Amount = Amount;
            this.VatAmount = VatAmount;
            this.TotalAmount = TotalAmount;
        }

        public InvoiceAmount() { }
    }
}