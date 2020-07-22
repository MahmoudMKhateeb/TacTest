using System.Collections.Generic;
using TACHYON.Dto;
using TACHYON.Routs.RoutSteps.Dtos;

namespace TACHYON.Routs.RoutSteps.Exporting
{
    public interface IRoutStepsExcelExporter
    {
        FileDto ExportToFile(List<GetRoutStepForViewDto> routSteps);
    }
}