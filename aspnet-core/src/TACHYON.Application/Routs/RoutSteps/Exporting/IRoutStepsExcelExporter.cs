using System.Collections.Generic;
using TACHYON.Routs.RoutSteps.Dtos;
using TACHYON.Dto;

namespace TACHYON.Routs.RoutSteps.Exporting
{
    public interface IRoutStepsExcelExporter
    {
        FileDto ExportToFile(List<GetRoutStepForViewDto> routSteps);
    }
}