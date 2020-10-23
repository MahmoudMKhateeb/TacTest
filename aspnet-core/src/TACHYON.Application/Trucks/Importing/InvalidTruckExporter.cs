using System.Collections.Generic;
using Abp.Dependency;
using TACHYON.DataExporting.Excel.NPOI;
using TACHYON.Dto;
using TACHYON.Storage;
using TACHYON.Trucks.Importing.Dto;

namespace TACHYON.Trucks.Importing
{
    public class InvalidTruckExporter : NpoiExcelExporterBase, IInvalidTruckExporter, ITransientDependency
    {
        public InvalidTruckExporter(ITempFileCacheManager tempFileCacheManager): base(tempFileCacheManager)
        {
        }

        public FileDto ExportToFile(List<ImportTruckDto> truckListDtos)
        {
            return CreateExcelPackage(
                "InvalidTruckImportList.xlsx",
                excelPackage =>
                {
                    var sheet = excelPackage.CreateSheet(L("InvalidTruckImports"));

                    AddHeader(
                        sheet,
                        L("ModelName"),
                        L("Refuse Reason")
                    );

                    AddObjects(
                        sheet, 2, truckListDtos,
                        _ => _.ModelName,
                        _ => _.Exception
                    );

                    for (var i = 0; i < 8; i++)
                    {
                        sheet.AutoSizeColumn(i);
                    }
                });
        }
    }
}