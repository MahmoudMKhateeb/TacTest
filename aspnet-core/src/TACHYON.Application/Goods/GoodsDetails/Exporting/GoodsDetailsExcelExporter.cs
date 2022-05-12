using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using System.Collections.Generic;
using TACHYON.DataExporting.Excel.NPOI;
using TACHYON.Dto;
using TACHYON.Goods.GoodsDetails.Dtos;
using TACHYON.Storage;

namespace TACHYON.Goods.GoodsDetails.Exporting
{
    public class GoodsDetailsExcelExporter : NpoiExcelExporterBase, IGoodsDetailsExcelExporter
    {
        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;

        public GoodsDetailsExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IAbpSession abpSession,
            ITempFileCacheManager tempFileCacheManager) :
            base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _abpSession = abpSession;
        }

        public FileDto ExportToFile(List<GetGoodsDetailForViewDto> goodsDetails)
        {
            return CreateExcelPackage(
                "GoodsDetails.xlsx",
                excelPackage =>
                {
                    var sheet = excelPackage.CreateSheet(L("GoodsDetails"));

                    AddHeader(
                        sheet,
                        //L("Name"),
                        L("Description"),
                        L("Quantity"),
                        L("Weight"),
                        L("Dimentions"),
                        L("IsDangerousGood"),
                        L("DangerousGoodsCode"),
                        (L("GoodCategory")) + L("DisplayName")
                    );

                    AddObjects(
                        sheet, 2, goodsDetails,
                        //_ => _.GoodsDetail.Name,
                        _ => _.GoodsDetail.Description,
                        _ => _.GoodsDetail.Amount,
                        _ => _.GoodsDetail.Weight,
                        _ => _.GoodsDetail.Dimentions,
                        _ => _.GoodsDetail.IsDangerousGood,
                        _ => _.GoodsDetail.DangerousGoodsCode,
                        _ => _.GoodCategoryDisplayName
                    );
                });
        }
    }
}