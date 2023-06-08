using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Invoices.Dto
{
    public class InvoiceInfoForExportDto
    {
       public long? InvoiceNumber { get; set; }
       public InvoiceChannel InvoiceChannel { get; set; }
    }
}
