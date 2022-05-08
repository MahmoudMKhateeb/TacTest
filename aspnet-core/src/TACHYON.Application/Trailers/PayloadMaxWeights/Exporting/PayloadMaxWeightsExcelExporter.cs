using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using System.Collections.Generic;
using TACHYON.DataExporting.Excel.NPOI;
using TACHYON.Dto;
using TACHYON.Storage;
using TACHYON.Trailers.PayloadMaxWeights.Dtos;

namespace TACHYON.Trailers.PayloadMaxWeights.Exporting
{
    public class PayloadMaxWeightsExcelExporter : NpoiExcelExporterBase, IPayloadMaxWeightsExcelExporter
    {
        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;

        public PayloadMaxWeightsExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IAbpSession abpSession,
            ITempFileCacheManager tempFileCacheManager) :
            base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _abpSession = abpSession;
        }

        public FileDto ExportToFile(List<GetPayloadMaxWeightForViewDto> payloadMaxWeights)
        {
            return CreateExcelPackage(
                "PayloadMaxWeights.xlsx",
                excelPackage =>
                {
                    var sheet = excelPackage.CreateSheet(L("PayloadMaxWeights"));

                    AddHeader(
                        sheet,
                        L("DisplayName"),
                        L("MaxWeight")
                    );

                    AddObjects(
                        sheet, 2, payloadMaxWeights,
                        _ => _.PayloadMaxWeight.DisplayName,
                        _ => _.PayloadMaxWeight.MaxWeight
                    );
                });
        }
    }
}