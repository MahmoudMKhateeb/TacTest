using System;
using System.Collections.Generic;
using System.Linq;
using TACHYON.DataExporting.Excel.NPOI;
using TACHYON.Dto;
using TACHYON.Storage;

namespace TACHYON.Exporting
{
    public class ExcelExporterManager<TSource> : NpoiExcelExporterBase, IExcelExporterManager<TSource>
    {
        public ExcelExporterManager(ITempFileCacheManager tempFileCacheManager) : base(tempFileCacheManager) { }

        public FileDto ExportToFile(List<TSource> Sources,
            string SheetName,
            string[] headerTexts,
            Func<TSource, object>[] propertySelectors)
        {
            return CreateExcelPackage(
                $"{SheetName}",
                excelPackage =>
                {
                    var sheet = excelPackage.CreateSheet("Sheet1");
                    AddHeader(sheet, headerTexts.Select(x => L(x)).ToArray());
                    AddObjects(sheet, 1, Sources, propertySelectors);
                });
        }
    }
}