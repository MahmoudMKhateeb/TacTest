using System.Collections.Generic;
using TACHYON.Trailers.TrailerStatuses.Dtos;
using TACHYON.Dto;

namespace TACHYON.Trailers.TrailerStatuses.Exporting
{
    public interface ITrailerStatusesExcelExporter
    {
        FileDto ExportToFile(List<GetTrailerStatusForViewDto> trailerStatuses);
    }
}