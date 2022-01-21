using System.Collections.Generic;
using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using TACHYON.DataExporting.Excel.NPOI;
using TACHYON.Documents.DocumentTypeTranslations.Dtos;
using TACHYON.Dto;
using TACHYON.Storage;

namespace TACHYON.Documents.DocumentTypeTranslations.Exporting
{
    public class DocumentTypeTranslationsExcelExporter : NpoiExcelExporterBase, IDocumentTypeTranslationsExcelExporter
    {
        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;

        public DocumentTypeTranslationsExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IAbpSession abpSession,
            ITempFileCacheManager tempFileCacheManager) :
            base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _abpSession = abpSession;
        }

        public FileDto ExportToFile(List<GetDocumentTypeTranslationForViewDto> documentTypeTranslations)
        {
            return CreateExcelPackage(
                "DocumentTypeTranslations.xlsx",
                excelPackage =>
                {
                    var sheet = excelPackage.CreateSheet(L("DocumentTypeTranslations"));

                    AddHeader(
                        sheet,
                        L("Name"),
                        L("Language"),
                        (L("DocumentType")) + L("DisplayName")
                    );

                    AddObjects(
                        sheet, 2, documentTypeTranslations,
                        _ => _.DocumentTypeTranslation.Name,
                        _ => _.DocumentTypeTranslation.Language,
                        _ => _.DocumentTypeDisplayName
                    );
                });
        }
    }
}