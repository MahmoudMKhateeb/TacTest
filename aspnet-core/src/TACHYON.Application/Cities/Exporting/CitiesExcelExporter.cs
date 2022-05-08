using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using System.Collections.Generic;
using TACHYON.Cities.Dtos;
using TACHYON.DataExporting.Excel.NPOI;
using TACHYON.Dto;
using TACHYON.Storage;

namespace TACHYON.Cities.Exporting
{
    public class CitiesExcelExporter : NpoiExcelExporterBase, ICitiesExcelExporter
    {
        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;

        public CitiesExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IAbpSession abpSession,
            ITempFileCacheManager tempFileCacheManager) :
            base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _abpSession = abpSession;
        }

        public FileDto ExportToFile(List<GetCityForViewDto> cities)
        {
            return CreateExcelPackage(
                "Cities.xlsx",
                excelPackage =>
                {
                    var sheet = excelPackage.CreateSheet(L("Cities"));

                    AddHeader(
                        sheet,
                        L("DisplayName"),
                        L("Code"),
                        L("Latitude"),
                        L("Longitude"),
                        (L("County")) + L("DisplayName")
                    );

                    AddObjects(
                        sheet, 2, cities,
                        _ => _.City.DisplayName,
                        _ => _.City.Code,
                        _ => _.City.Latitude,
                        _ => _.City.Longitude,
                        _ => _.CountyDisplayName
                    );
                });
        }
    }
}