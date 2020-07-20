using System.Collections.Generic;
using TACHYON.Routs.RoutTypes.Dtos;
using TACHYON.Dto;

namespace TACHYON.Routs.RoutTypes.Exporting
{
    public interface IRoutTypesExcelExporter
    {
        FileDto ExportToFile(List<GetRoutTypeForViewDto> routTypes);
    }
}