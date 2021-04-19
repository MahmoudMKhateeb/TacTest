using Abp.Dependency;
using System;
using System.Collections.Generic;
using TACHYON.Dto;

namespace TACHYON.Exporting
{
    public interface IExcelExporterManager<TSource> 
    {
        FileDto ExportToFile(List<TSource> Sources, string SheetName, string[] headerTexts, Func<TSource, object>[] propertySelectors);
    }
}
