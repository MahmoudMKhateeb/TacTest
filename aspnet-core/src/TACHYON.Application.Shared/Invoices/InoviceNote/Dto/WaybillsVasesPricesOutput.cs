using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Invoices.InoviceNote.Dto
{
    public class WaybillsVasesPricesOutput
    {
        public decimal? price { get; set; }
        public decimal? TotalAmount { get; set; }
        public decimal? VatAmount { get; set; }
    }
}
