using System.Collections.Generic;
using TACHYON.Dto;
using TACHYON.Trailers.Dtos;

namespace TACHYON.Trailers.Exporting
{
    public interface ITrailersExcelExporter
    {
        FileDto ExportToFile(List<GetTrailerForViewDto> trailers);
    }
}