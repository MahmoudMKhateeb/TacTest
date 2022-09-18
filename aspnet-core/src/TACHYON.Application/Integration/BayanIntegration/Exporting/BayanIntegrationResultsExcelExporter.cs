using System.Collections.Generic;
using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using TACHYON.DataExporting.Excel.NPOI;
using TACHYON.Integration.BayanIntegration.Dtos;
using TACHYON.Dto;
using TACHYON.Storage;

namespace TACHYON.Integration.BayanIntegration.Exporting
{
    public class BayanIntegrationResultsExcelExporter : NpoiExcelExporterBase, IBayanIntegrationResultsExcelExporter
    {

        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;

        public BayanIntegrationResultsExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IAbpSession abpSession,
            ITempFileCacheManager tempFileCacheManager) :
    base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _abpSession = abpSession;
        }

        public FileDto ExportToFile(List<GetBayanIntegrationResultForViewDto> bayanIntegrationResults)
        {
            return CreateExcelPackage(
                "BayanIntegrationResults.xlsx",
                excelPackage =>
                {

                    var sheet = excelPackage.CreateSheet(L("BayanIntegrationResults"));

                    AddHeader(
                        sheet,
                        L("ActionName"),
                        L("InputJson"),
                        L("ResponseJson"),
                        L("Version"),
                        (L("ShippingRequestTrip")) + L("ContainerNumber")
                        );

                    AddObjects(
                        sheet, 2, bayanIntegrationResults,
                        _ => _.BayanIntegrationResult.ActionName,
                        _ => _.BayanIntegrationResult.InputJson,
                        _ => _.BayanIntegrationResult.ResponseJson,
                        _ => _.BayanIntegrationResult.Version,
                        _ => _.ShippingRequestTripContainerNumber
                        );

                });
        }
    }
}