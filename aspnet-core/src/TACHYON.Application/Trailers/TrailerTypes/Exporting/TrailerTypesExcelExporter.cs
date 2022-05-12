using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using System.Collections.Generic;
using TACHYON.DataExporting.Excel.NPOI;
using TACHYON.Dto;
using TACHYON.Storage;
using TACHYON.Trailers.TrailerTypes.Dtos;

namespace TACHYON.Trailers.TrailerTypes.Exporting
{
    public class TrailerTypesExcelExporter : NpoiExcelExporterBase, ITrailerTypesExcelExporter
    {
        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;

        public TrailerTypesExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IAbpSession abpSession,
            ITempFileCacheManager tempFileCacheManager) :
            base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _abpSession = abpSession;
        }

        public FileDto ExportToFile(List<GetTrailerTypeForViewDto> trailerTypes)
        {
            return CreateExcelPackage(
                "TrailerTypes.xlsx",
                excelPackage =>
                {
                    var sheet = excelPackage.CreateSheet(L("TrailerTypes"));

                    AddHeader(
                        sheet,
                        L("DisplayName")
                    );

                    AddObjects(
                        sheet, 2, trailerTypes,
                        _ => _.TrailerType.DisplayName
                    );
                });
        }
    }
}