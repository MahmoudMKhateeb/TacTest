using System.Collections.Generic;
using System.Globalization;
using TACHYON.DataExporting.Excel.NPOI;
using TACHYON.Dto;
using TACHYON.Invoices.Balances.Dto;
using TACHYON.Invoices.Transactions.Dto;
using TACHYON.Storage;

namespace TACHYON.Invoices.Balances.Exporting
{
    public class TransactionExcelExporter : NpoiExcelExporterBase, ITransactionExcelExporter
    {
        public TransactionExcelExporter(

    ITempFileCacheManager tempFileCacheManager) :
base(tempFileCacheManager)
        {
        }

        public FileDto ExportToFile(List<TransactionListDto> Transactions)
        {
            return CreateExcelPackage(
                "Transactions",
                excelPackage =>
                {

                    var sheet = excelPackage.CreateSheet(L("Transactions"));

                    AddHeader(
                        sheet,
                        L("ClientName"),
                        L("Channel"),
                        L("Amount"),
                         L("CreationTime")
                        );

                    AddObjects(
                        sheet, 1, Transactions,
                        _ => _.ClientName,
                        _ => L(_.ChannelId.ToString()),
                        _ => _.Amount,
                        _ => _.CreationTime
                        );
                });
        }


    }
}
