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
                        L("ExpirationDate"),
                        L("HasExpirationDate"),
                        L("RequiredFrom")
                        );

                    AddObjects(
                        sheet, 2, documentTypes,
                        _ => _.DocumentType.DisplayName,
                        _ => _.DocumentType.IsRequired,
                        _ => _timeZoneConverter.Convert(_.DocumentType.ExpirationDate, _abpSession.TenantId, _abpSession.GetUserId()),
                        _ => _.DocumentType.HasExpirationDate,
                        _ => _.DocumentType.RequiredFrom
                        );


                    for (var i = 1; i <= documentTypes.Count; i++)
                    {
                        SetCellDataFormat(sheet.GetRow(i).Cells[3], "yyyy-mm-dd");
                    }
                    sheet.AutoSizeColumn(3);
                });
        }
    }
}