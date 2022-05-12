using System.Collections.Generic;
using TACHYON.DataExporting.Excel.NPOI;
using TACHYON.Dto;
using TACHYON.Invoices.Periods.Dto;
using TACHYON.Storage;

namespace TACHYON.Invoices.Periods.Exporting
{
    public class InvoicePeriodExport : NpoiExcelExporterBase, IInvoicePeriodExport
    {
        public InvoicePeriodExport(
            ITempFileCacheManager tempFileCacheManager) :
            base(tempFileCacheManager)
        {
        }

        public FileDto ExportToFile(List<InvoicePeriodDto> Periods)
        {
            return CreateExcelPackage(
                "Periods",
                excelPackage =>
                {
                    var sheet = excelPackage.CreateSheet("Sheet1");

                    AddHeader(
                        sheet,
                        L("DisplayName"),
                        L("PeriodType")
                    );

                    AddObjects(
                        sheet, 2, Periods,
                        _ => _.DisplayName,
                        _ => _.PeriodType
                    );
                });
        }
    }
}