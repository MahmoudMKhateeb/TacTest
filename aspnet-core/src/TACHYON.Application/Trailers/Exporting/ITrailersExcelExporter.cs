using System.Collections.Generic;
using TACHYON.Trailers.Dtos;
using TACHYON.Dto;

namespace TACHYON.Trailers.Exporting
{
    public interface ITrailersExcelExporter
    {
        FileDto ExportToFile(List<GetTrailerForViewDto> trailers);
    }
}