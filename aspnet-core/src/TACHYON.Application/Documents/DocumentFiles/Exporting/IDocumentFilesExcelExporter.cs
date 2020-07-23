using System.Collections.Generic;
using TACHYON.Documents.DocumentFiles.Dtos;
using TACHYON.Dto;

namespace TACHYON.Documents.DocumentFiles.Exporting
{
    public interface IDocumentFilesExcelExporter
    {
        FileDto ExportToFile(List<GetDocumentFileForViewDto> documentFiles);
    }
}