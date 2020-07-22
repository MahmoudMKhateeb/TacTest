using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using System.Collections.Generic;
using TACHYON.DataExporting.Excel.NPOI;
using TACHYON.Dto;
using TACHYON.Storage;
using TACHYON.Trailers.Dtos;

namespace TACHYON.Trailers.Exporting
{
    public class TrailersExcelExporter : NpoiExcelExporterBase, ITrailersExcelExporter
    {

        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;

        public TrailersExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IAbpSession abpSession,
            ITempFileCacheManager tempFileCacheManager) :
    base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _abpSession = abpSession;
        }

        public FileDto ExportToFile(List<GetTrailerForViewDto> trailers)
        {
            return CreateExcelPackage(
                "Trailers.xlsx",
                excelPackage =>
                {

                    var sheet = excelPackage.CreateSheet(L("Trailers"));

                    AddHeader(
                        sheet,
                        L("TrailerCode"),
                        L("PlateNumber"),
                        L("Model"),
                        L("Year"),
                        L("Width"),
                        L("Height"),
                        L("Length"),
                        L("IsLiftgate"),
                        L("IsReefer"),
                        L("IsVented"),
                        L("IsRollDoor"),
                        (L("TrailerStatus")) + L("DisplayName"),
                        (L("TrailerType")) + L("DisplayName"),
                        (L("PayloadMaxWeight")) + L("DisplayName"),
                        (L("Truck")) + L("PlateNumber")
                        );

                    AddObjects(
                        sheet, 2, trailers,
                        _ => _.Trailer.TrailerCode,
                        _ => _.Trailer.PlateNumber,
                        _ => _.Trailer.Model,
                        _ => _.Trailer.Year,
                        _ => _.Trailer.Width,
                        _ => _.Trailer.Height,
                        _ => _.Trailer.Length,
                        _ => _.Trailer.IsLiftgate,
                        _ => _.Trailer.IsReefer,
                        _ => _.Trailer.IsVented,
                        _ => _.Trailer.IsRollDoor,
                        _ => _.TrailerStatusDisplayName,
                        _ => _.TrailerTypeDisplayName,
                        _ => _.PayloadMaxWeightDisplayName,
                        _ => _.TruckPlateNumber
                        );



                });
        }
    }
}