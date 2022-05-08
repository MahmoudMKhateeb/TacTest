using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using System.Collections.Generic;
using TACHYON.DataExporting.Excel.NPOI;
using TACHYON.Dto;
using TACHYON.Offers.Dtos;
using TACHYON.Storage;

namespace TACHYON.Offers.Exporting
{
    public class OffersExcelExporter : NpoiExcelExporterBase, IOffersExcelExporter
    {
        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;

        public OffersExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IAbpSession abpSession,
            ITempFileCacheManager tempFileCacheManager) :
            base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _abpSession = abpSession;
        }

        public FileDto ExportToFile(List<GetOfferForViewDto> offers)
        {
            return CreateExcelPackage(
                "Offers.xlsx",
                excelPackage =>
                {
                    var sheet = excelPackage.CreateSheet(L("Offers"));

                    AddHeader(
                        sheet,
                        L("DisplayName"),
                        L("Description"),
                        L("Price"),
                        (L("TrucksType")) + L("DisplayName"),
                        (L("TrailerType")) + L("DisplayName"),
                        (L("GoodCategory")) + L("DisplayName"),
                        (L("Route")) + L("DisplayName")
                    );

                    AddObjects(
                        sheet, 2, offers,
                        _ => _.Offer.DisplayName,
                        _ => _.Offer.Description,
                        _ => _.Offer.Price,
                        _ => _.TrucksTypeDisplayName,
                        _ => _.TrailerTypeDisplayName,
                        _ => _.GoodCategoryDisplayName,
                        _ => _.RouteDisplayName
                    );
                });
        }
    }
}