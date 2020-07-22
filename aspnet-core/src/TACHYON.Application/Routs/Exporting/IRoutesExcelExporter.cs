using System.Collections.Generic;
using TACHYON.Dto;
using TACHYON.Routs.Dtos;

namespace TACHYON.Routs.Exporting
{
    public interface IRoutesExcelExporter
    {
        FileDto ExportToFile(List<GetRouteForViewDto> routes);
    }
}