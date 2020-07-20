using System.Collections.Generic;
using TACHYON.Routs.Dtos;
using TACHYON.Dto;

namespace TACHYON.Routs.Exporting
{
    public interface IRoutesExcelExporter
    {
        FileDto ExportToFile(List<GetRouteForViewDto> routes);
    }
}