using System;
using System.Collections.Generic;
using System.Text;
using TACHYON.Dto;
using TACHYON.Invoices.Balances.Dto;

namespace TACHYON.Invoices.Balances.Exporting
{
   public interface IBalanceRechargeExcelExporter
    {
        FileDto ExportToFile(List<BalanceRechargeListDto> balanceRecharges);

    }
}
