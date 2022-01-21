using System.Collections.Generic;
using TACHYON.DataExporting.Excel.NPOI;
using TACHYON.Dto;
using TACHYON.Invoices.Balances.Dto;
using TACHYON.Storage;

namespace TACHYON.Invoices.Balances.Exporting
{
    public class BalanceRechargeExcelExporter : NpoiExcelExporterBase, IBalanceRechargeExcelExporter
    {
        public BalanceRechargeExcelExporter(
            ITempFileCacheManager tempFileCacheManager) :
            base(tempFileCacheManager)
        {
        }

        public FileDto ExportToFile(List<BalanceRechargeListDto> balanceRecharges)
        {
            return CreateExcelPackage(
                "BalanceManagement",
                excelPackage =>
                {
                    var sheet = excelPackage.CreateSheet("Sheet1");

                    AddHeader(
                        sheet,
                        L("ShipperName"),
                        L("Amount"),
                        L("CreationTime")
                    );

                    AddObjects(
                        sheet, 1, balanceRecharges,
                        _ => _.TenantName,
                        _ => _.Amount,
                        _ => _.CreationTime
                    );
                });
        }
    }
}