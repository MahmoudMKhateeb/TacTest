using System.Collections.Generic;
using TACHYON.Dto;
using TACHYON.Routs.RoutTypes.Dtos;

namespace TACHYON.Routs.RoutTypes.Exporting
{
    public interface IRoutTypesExcelExporter
    {
        FileDto ExportToFile(List<GetRoutTypeForViewDto> routTypes);
    }
}