using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using System.Collections.Generic;
using TACHYON.DataExporting.Excel.NPOI;
using TACHYON.Dto;
using TACHYON.Shipping.ShippingRequests.Dtos;
using TACHYON.Storage;

namespace TACHYON.Shipping.ShippingRequests.Exporting
{
    public class ShippingRequestsExcelExporter : NpoiExcelExporterBase, IShippingRequestsExcelExporter
    {

        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;

        public ShippingRequestsExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IAbpSession abpSession,
            ITempFileCacheManager tempFileCacheManager) :
    base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _abpSession = abpSession;
        }

        public FileDto ExportToFile(List<GetShippingRequestForViewDto> shippingRequests)
        {
            return CreateExcelPackage(
                "ShippingRequests.xlsx",
                excelPackage =>
                {

                    var sheet = excelPackage.CreateSheet(L("ShippingRequests"));

                    AddHeader(
                        sheet,
                        L("Vas"),
                        (L("TrucksType")) + L("DisplayName"),
                        (L("TrailerType")) + L("DisplayName"),
                        (L("GoodsDetail")) + L("Name"),
                        (L("Route")) + L("DisplayName")
                        );

                    AddObjects(
                        sheet, 2, shippingRequests,
                        _ => _.ShippingRequest.Vas,
                        _ => _.TrucksTypeDisplayName,
                        _ => _.TrailerTypeDisplayName,
                        _ => _.GoodsDetailName,
                        _ => _.RouteDisplayName
                        );



                });
        }
    }
}