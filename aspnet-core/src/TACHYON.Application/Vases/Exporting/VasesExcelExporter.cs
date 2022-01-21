using System.Collections.Generic;
using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using TACHYON.DataExporting.Excel.NPOI;
using TACHYON.Vases.Dtos;
using TACHYON.Dto;
using TACHYON.Storage;

namespace TACHYON.Vases.Exporting
{
    public class VasesExcelExporter : NpoiExcelExporterBase, IVasesExcelExporter
    {
        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;

        public VasesExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IAbpSession abpSession,
            ITempFileCacheManager tempFileCacheManager) :
            base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _abpSession = abpSession;
        }

        public FileDto ExportToFile(List<GetVasForViewDto> vases)
        {
            return CreateExcelPackage(
                "Vases.xlsx",
                excelPackage =>
                {
                    var sheet = excelPackage.CreateSheet(L("Vases"));

                    AddHeader(
                        sheet,
                        L("Name"),
                        L("HasAmount"),
                        L("HasCount")
                    );

                    AddObjects(
                        sheet, 2, vases,
                        _ => _.Vas.Name,
                        _ => _.Vas.HasAmount,
                        _ => _.Vas.HasCount
                    );
                });
        }
    }
}