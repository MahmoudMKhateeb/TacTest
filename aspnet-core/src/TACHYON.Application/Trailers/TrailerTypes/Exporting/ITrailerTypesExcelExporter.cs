using System.Collections.Generic;
using TACHYON.Trailers.TrailerTypes.Dtos;
using TACHYON.Dto;

namespace TACHYON.Trailers.TrailerTypes.Exporting
{
    public interface ITrailerTypesExcelExporter
    {
        FileDto ExportToFile(List<GetTrailerTypeForViewDto> trailerTypes);
    }
}