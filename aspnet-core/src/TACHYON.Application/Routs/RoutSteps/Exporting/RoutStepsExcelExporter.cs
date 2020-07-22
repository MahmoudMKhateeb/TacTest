using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using System.Collections.Generic;
using TACHYON.DataExporting.Excel.NPOI;
using TACHYON.Dto;
using TACHYON.Routs.RoutSteps.Dtos;
using TACHYON.Storage;

namespace TACHYON.Routs.RoutSteps.Exporting
{
    public class RoutStepsExcelExporter : NpoiExcelExporterBase, IRoutStepsExcelExporter
    {

        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;

        public RoutStepsExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IAbpSession abpSession,
            ITempFileCacheManager tempFileCacheManager) :
    base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _abpSession = abpSession;
        }

        public FileDto ExportToFile(List<GetRoutStepForViewDto> routSteps)
        {
            return CreateExcelPackage(
                "RoutSteps.xlsx",
                excelPackage =>
                {

                    var sheet = excelPackage.CreateSheet(L("RoutSteps"));

                    AddHeader(
                        sheet,
                        L("DisplayName"),
                        L("Latitude"),
                        L("Longitude"),
                        L("Order"),
                        (L("City")) + L("DisplayName"),
                        (L("City")) + L("DisplayName"),
                        (L("Route")) + L("DisplayName")
                        );

                    AddObjects(
                        sheet, 2, routSteps,
                        _ => _.RoutStep.DisplayName,
                        _ => _.RoutStep.Latitude,
                        _ => _.RoutStep.Longitude,
                        _ => _.RoutStep.Order,
                        _ => _.CityDisplayName,
                        _ => _.CityDisplayName2,
                        _ => _.RouteDisplayName
                        );



                });
        }
    }
}