using System.Collections.Generic;
using TACHYON.Dto;
using TACHYON.Trailers.TrailerStatuses.Dtos;

namespace TACHYON.Trailers.TrailerStatuses.Exporting
{
    public interface ITrailerStatusesExcelExporter
    {
        FileDto ExportToFile(List<GetTrailerStatusForViewDto> trailerStatuses);
    }
}