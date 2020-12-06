using System.Collections.Generic;
using Abp.Collections.Extensions;
using Abp.Dependency;
using TACHYON.DataExporting.Excel.NPOI;
using TACHYON.Drivers.importing.Dto;
using TACHYON.Dto;
using TACHYON.Storage;

namespace TACHYON.Drivers.importing
{
    public class InvalidDriverExporter : NpoiExcelExporterBase, IInvalidDriverExporter, ITransientDependency
    {
        public InvalidDriverExporter(ITempFileCacheManager tempFileCacheManager)
            : base(tempFileCacheManager)
        {
        }

        public FileDto ExportToFile(List<ImportDriverDto> userListDtos)
        {
            return CreateExcelPackage(
                "InvalidUserImportList.xlsx",
                excelPackage =>
                {
                    var sheet = excelPackage.CreateSheet(L("InvalidUserImports"));

                    AddHeader(
                        sheet,
                        L("UserName"),
                        L("Name"),
                        L("Surname"),
                        L("PhoneNumber"),
                        L("Refuse Reason")
                    );

                    AddObjects(
                        sheet, 2, userListDtos,
                        _ => _.UserName,
                        _ => _.Name,
                        _ => _.Surname,
                        _ => _.PhoneNumber,
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