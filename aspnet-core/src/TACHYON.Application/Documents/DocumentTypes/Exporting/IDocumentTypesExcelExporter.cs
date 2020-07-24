using System.Collections.Generic;
using TACHYON.Documents.DocumentTypes.Dtos;
using TACHYON.Dto;

namespace TACHYON.Documents.DocumentTypes.Exporting
{
    public interface IDocumentTypesExcelExporter
    {
        FileDto ExportToFile(List<GetDocumentTypeForViewDto> documentTypes);
    }
}