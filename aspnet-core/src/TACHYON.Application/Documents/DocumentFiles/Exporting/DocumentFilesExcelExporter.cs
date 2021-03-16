using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using System.Collections.Generic;
using TACHYON.DataExporting.Excel.NPOI;
using TACHYON.Documents.DocumentFiles.Dtos;
using TACHYON.Dto;
using TACHYON.Storage;

namespace TACHYON.Documents.DocumentFiles.Exporting
{
    public class DocumentFilesExcelExporter : NpoiExcelExporterBase, IDocumentFilesExcelExporter
    {

        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;

        public DocumentFilesExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IAbpSession abpSession,
            ITempFileCacheManager tempFileCacheManager) :
    base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _abpSession = abpSession;
        }

        public FileDto ExportToFile(List<GetDocumentFileForViewDto> documentFiles)
        {
            return CreateExcelPackage(
                "DocumentFiles.xlsx",
                excelPackage =>
                {

                    var sheet = excelPackage.CreateSheet(L("DocumentFiles"));

                    AddHeader(
                        sheet,
                        L("Name"),
                        L("Extn"),
                        L("BinaryObjectId"),
                        L("ExpirationDate"),
                        L("IsAccepted"),
                       // (L("DocumentType")) + L("DisplayName"),
                        (L("Truck")) + L("PlateNumber"),
                        (L("Trailer")) + L("TrailerCode"),
                        (L("User")) + L("Name"),
                        (L("RoutStep")) + L("DisplayName")
                        );

                    AddObjects(
                        sheet, 2, documentFiles,
                        _ => _.DocumentFile.Name,
                        _ => _.DocumentFile.Extn,
                        _ => _.DocumentFile.BinaryObjectId,
                        _ => _timeZoneConverter.Convert(_.DocumentFile.ExpirationDate, _abpSession.TenantId, _abpSession.GetUserId()),
                        _ => _.DocumentFile.IsAccepted,
                       // _ => _.DocumentTypeDisplayName,
                        _ => _.TruckId,
                        _ => _.TrailerTrailerCode,
                        _ => _.UserName,
                        _ => _.RoutStepDisplayName
                        );


                    for (var i = 1; i <= documentFiles.Count; i++)
                    {
                        SetCellDataFormat(sheet.GetRow(i).Cells[4], "yyyy-mm-dd");
                    }
                    sheet.AutoSizeColumn(4);
                });
        }
    }
}