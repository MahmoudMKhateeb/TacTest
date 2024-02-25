using System.Collections.Generic;
using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using TACHYON.DataExporting.Excel.NPOI;
using TACHYON.Redemption.Dtos;
using TACHYON.Dto;
using TACHYON.Storage;

namespace TACHYON.Redemption.Exporting
{
    public class RedemptionCodesExcelExporter : NpoiExcelExporterBase, IRedemptionCodesExcelExporter
    {

        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;

        public RedemptionCodesExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IAbpSession abpSession,
            ITempFileCacheManager tempFileCacheManager) :
    base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _abpSession = abpSession;
        }

        public FileDto ExportToFile(List<GetRedemptionCodeForViewDto> redemptionCodes)
        {
            return CreateExcelPackage(
                "RedemptionCodes.xlsx",
                excelPackage =>
                {

                    var sheet = excelPackage.CreateSheet(L("RedemptionCodes"));

                    AddHeader(
                        sheet,
                        L("RedemptionDate"),
                        L("RedemptionTenantId"),
                        (L("RedeemCode")) + L("Code")
                        );

                    AddObjects(
                        sheet, 2, redemptionCodes,
                        _ => _timeZoneConverter.Convert(_.RedemptionCode.RedemptionDate, _abpSession.TenantId, _abpSession.GetUserId()),
                        _ => _.RedemptionCode.RedemptionTenantId,
                        _ => _.RedeemCodeCode
                        );

                    for (var i = 1; i <= redemptionCodes.Count; i++)
                    {
                        SetCellDataFormat(sheet.GetRow(i).Cells[1], "yyyy-mm-dd");
                    }
                    sheet.AutoSizeColumn(1);
                });
        }
    }
}