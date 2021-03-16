using System;
using System.Collections.Generic;
using System.Text;
using TACHYON.Dto;
using TACHYON.Invoices.Balances.Dto;
using TACHYON.Invoices.Transactions.Dto;

namespace TACHYON.Invoices.Balances.Exporting
{
   public interface ITransactionExcelExporter
    {
        FileDto ExportToFile(List<TransactionListDto> input);

    }
}
