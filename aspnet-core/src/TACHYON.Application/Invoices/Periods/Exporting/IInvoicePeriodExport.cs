using System;
using System.Collections.Generic;
using System.Text;
using TACHYON.Dto;
using TACHYON.Invoices.Periods.Dto;

namespace TACHYON.Invoices.Periods.Exporting
{
  public  interface IInvoicePeriodExport
    {
        FileDto ExportToFile(List<InvoicePeriodDto> Periods);

    }
}
