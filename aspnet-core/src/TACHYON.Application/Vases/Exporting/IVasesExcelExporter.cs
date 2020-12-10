using System.Collections.Generic;
using TACHYON.Vases.Dtos;
using TACHYON.Dto;

namespace TACHYON.Vases.Exporting
{
    public interface IVasesExcelExporter
    {
        FileDto ExportToFile(List<GetVasForViewDto> vases);
    }
}