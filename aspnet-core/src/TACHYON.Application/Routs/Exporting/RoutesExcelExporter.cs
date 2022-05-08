using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using System.Collections.Generic;
using TACHYON.DataExporting.Excel.NPOI;
using TACHYON.Dto;
using TACHYON.Routs.Dtos;
using TACHYON.Storage;

namespace TACHYON.Routs.Exporting
{
    public class RoutesExcelExporter : NpoiExcelExporterBase, IRoutesExcelExporter
    {
        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;

        public RoutesExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IAbpSession abpSession,
            ITempFileCacheManager tempFileCacheManager) :
            base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _abpSession = abpSession;
        }

        public FileDto ExportToFile(List<GetRouteForViewDto> routes)
        {
            return CreateExcelPackage(
                "Routes.xlsx",
                excelPackage =>
                {
                    var sheet = excelPackage.CreateSheet(L("Routes"));

                    AddHeader(
                        sheet,
                        L("DisplayName"),
                        L("Description"),
                        (L("RoutType")) + L("DisplayName")
                    );

                    AddObjects(
                        sheet, 2, routes,
                        _ => _.Route.DisplayName,
                        _ => _.Route.Description,
                        _ => _.RoutTypeDisplayName
                    );
                });
        }
    }
}