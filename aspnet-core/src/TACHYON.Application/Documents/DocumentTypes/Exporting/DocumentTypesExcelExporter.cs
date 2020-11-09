using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using System.Collections.Generic;
using TACHYON.DataExporting.Excel.NPOI;
using TACHYON.Documents.DocumentTypes.Dtos;
using TACHYON.Dto;
using TACHYON.Storage;

namespace TACHYON.Documents.DocumentTypes.Exporting
{
    public class DocumentTypesExcelExporter : NpoiExcelExporterBase, IDocumentTypesExcelExporter
    {

        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;

        public DocumentTypesExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IAbpSession abpSession,
            ITempFileCacheManager tempFileCacheManager) :
    base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _abpSession = abpSession;
        }

        public FileDto ExportToFile(List<GetDocumentTypeForViewDto> documentTypes)
        {
            return CreateExcelPackage(
                "DocumentTypes.xlsx",
                excelPackage =>
                {

                    var sheet = excelPackage.CreateSheet(L("DocumentTypes"));

                    AddHeader(
                        sheet,
                        L("DisplayName"),
                        L("IsRequired"),
                        L("RequiredFrom"),
                        L("SpecialConstant"),
                        L("HasExpirationDate"),
                        L("ExpirationAlertDays"),
                        L("InActiveToleranceDays"),
                        L("InActiveAccountExpired"),
                        L("HasNumber"),
                        L("IsNumberUnique"),
                        L("HasHijriExpirationDate"),
                        L("HasNotes")
                        );

                    AddObjects(
                        sheet, 2, documentTypes,
                        _ => _.DocumentType.DisplayName,
                        _ => _.DocumentType.IsRequired,
                        _ => _.DocumentType.RequiredFrom,
                        _ => _.DocumentType.SpecialConstant,
                        _ => _.DocumentType.HasExpirationDate,
                        _ => _.DocumentType.ExpirationAlertDays,
                        _ => _.DocumentType.InActiveToleranceDays,
                        _ => _.DocumentType.InActiveAccountExpired,
                        _ => _.DocumentType.HasNumber,
                        _ => _.DocumentType.IsNumberUnique,
                        _ => _.DocumentType.HasHijriExpirationDate,
                        _ => _.DocumentType.HasNotes
                        );


                    for (var i = 1; i <= documentTypes.Count; i++)
                    {
                        //SetCellDataFormat(sheet.GetRow(i).Cells[3], "yyyy-mm-dd");
                    }
                    sheet.AutoSizeColumn(3);
                });
        }
    }
}