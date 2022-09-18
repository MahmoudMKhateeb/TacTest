using System.Collections.Generic;
using TACHYON.Integration.BayanIntegration.Dtos;
using TACHYON.Dto;

namespace TACHYON.Integration.BayanIntegration.Exporting
{
    public interface IBayanIntegrationResultsExcelExporter
    {
        FileDto ExportToFile(List<GetBayanIntegrationResultForViewDto> bayanIntegrationResults);
    }
}