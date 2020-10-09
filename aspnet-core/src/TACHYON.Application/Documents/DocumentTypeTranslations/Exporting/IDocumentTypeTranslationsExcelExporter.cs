using System.Collections.Generic;
using TACHYON.Documents.DocumentTypeTranslations.Dtos;
using TACHYON.Dto;

namespace TACHYON.Documents.DocumentTypeTranslations.Exporting
{
    public interface IDocumentTypeTranslationsExcelExporter
    {
        FileDto ExportToFile(List<GetDocumentTypeTranslationForViewDto> documentTypeTranslations);
    }
}