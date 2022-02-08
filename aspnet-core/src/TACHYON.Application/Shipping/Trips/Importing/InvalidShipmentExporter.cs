using Abp.Dependency;
using System;
using System.Collections.Generic;
using System.Text;
using TACHYON.DataExporting.Excel.NPOI;
using TACHYON.Dto;
using TACHYON.Shipping.Trips.Dto;
using TACHYON.Storage;

namespace TACHYON.Shipping.Trips.Importing
{
    public class InvalidShipmentExporter: NpoiExcelExporterBase, IInvalidShipmentExporter, ITransientDependency
    {
        public InvalidShipmentExporter(ITempFileCacheManager tempFileCacheManager) : base(tempFileCacheManager)
        {

        }
        public FileDto ExportToFile(List<ImportTripDto> truckListDtos)
        {
            return CreateExcelPackage(
                "InvalidShipmentImportList.xlsx",
                excelPackage =>
                {
                    var sheet = excelPackage.CreateSheet("InvalidShipmentImports");

                    AddHeader(
                        sheet,
                        L("Reference"),
                        L("Refuse Reason")
                    );

                    AddObjects(
                        sheet, 2, truckListDtos,
                        _ => _.BulkUploadReference,
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
