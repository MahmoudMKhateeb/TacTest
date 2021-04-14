using System;
using System.Collections.Generic;
using TACHYON.DataExporting.Excel.NPOI;
using TACHYON.Dto;
using TACHYON.Storage;
using System.Linq;
using Abp.Dependency;

namespace TACHYON.Exporting
{
    public  class ExcelExporterManager<TSource>: NpoiExcelExporterBase, IExcelExporterManager<TSource>
    {
        public ExcelExporterManager(ITempFileCacheManager tempFileCacheManager) :base(tempFileCacheManager){}
        public FileDto ExportToFile(List<TSource> Sources,string SheetName, string[] headerTexts, Func<TSource, object>[] propertySelectors)
        {
            return CreateExcelPackage(
                $"{SheetName}",
                excelPackage =>
                {
                    var sheet = excelPackage.CreateSheet(L(SheetName));
                    AddHeader(sheet, headerTexts.Select(x=>L(x)).ToArray());
                    AddObjects(sheet, 1, Sources, propertySelectors);
                });
        }
    }
}
