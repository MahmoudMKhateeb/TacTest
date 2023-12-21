using System.Collections.Generic;
using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using TACHYON.DataExporting.Excel.NPOI;
using TACHYON.Redemption.Dtos;
using TACHYON.Dto;
using TACHYON.Storage;

namespace TACHYON.Redemption.Exporting
{
    public class RedeemCodesExcelExporter : NpoiExcelExporterBase, IRedeemCodesExcelExporter
    {

        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;

        public RedeemCodesExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IAbpSession abpSession,
            ITempFileCacheManager tempFileCacheManager) :
    base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _abpSession = abpSession;
        }

        public FileDto ExportToFile(List<GetRedeemCodeForViewDto> redeemCodes)
        {
            return CreateExcelPackage(
                "RedeemCodes.xlsx",
                excelPackage =>
                {

                    var sheet = excelPackage.CreateSheet(L("RedeemCodes"));

                    AddHeader(
                        sheet,
                        L("Code"),
                        L("ExpiryDate"),
                        L("IsActive"),
                        L("Value"),
                        L("Note"),
                        L("percentage")
                        );

                    AddObjects(
                        sheet, 2, redeemCodes,
                        _ => _.RedeemCode.Code,
                        _ => _timeZoneConverter.Convert(_.RedeemCode.ExpiryDate, _abpSession.TenantId, _abpSession.GetUserId()),
                        _ => _.RedeemCode.IsActive,
                        _ => _.RedeemCode.Value,
                        _ => _.RedeemCode.Note,
                        _ => _.RedeemCode.percentage
                        );

                    for (var i = 1; i <= redeemCodes.Count; i++)
                    {
                        SetCellDataFormat(sheet.GetRow(i).Cells[2], "yyyy-mm-dd");
                    }
                    sheet.AutoSizeColumn(2);
                });
        }
    }
}