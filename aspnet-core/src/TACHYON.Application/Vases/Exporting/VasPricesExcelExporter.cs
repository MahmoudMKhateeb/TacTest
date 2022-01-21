using System.Collections.Generic;
using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using TACHYON.DataExporting.Excel.NPOI;
using TACHYON.Vases.Dtos;
using TACHYON.Dto;
using TACHYON.Storage;

namespace TACHYON.Vases.Exporting
{
    public class VasPricesExcelExporter : NpoiExcelExporterBase, IVasPricesExcelExporter
    {
        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;

        public VasPricesExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IAbpSession abpSession,
            ITempFileCacheManager tempFileCacheManager) :
            base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _abpSession = abpSession;
        }

        public FileDto ExportToFile(List<GetVasPriceForViewDto> vasPrices)
        {
            return CreateExcelPackage(
                "VasPrices.xlsx",
                excelPackage =>
                {
                    var sheet = excelPackage.CreateSheet(L("VasPrices"));

                    AddHeader(
                        sheet,
                        L("Price"),
                        L("MaxAmount"),
                        L("MaxCount"),
                        (L("Vas")) + L("Name")
                    );

                    AddObjects(
                        sheet, 2, vasPrices,
                        _ => _.VasPrice.Price,
                        _ => _.VasPrice.MaxAmount,
                        _ => _.VasPrice.MaxCount,
                        _ => _.VasName
                    );
                });
        }
    }
}