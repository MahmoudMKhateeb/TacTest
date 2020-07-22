using System.Collections.Generic;
using TACHYON.Dto;
using TACHYON.Trailers.TrailerTypes.Dtos;

namespace TACHYON.Trailers.TrailerTypes.Exporting
{
    public interface ITrailerTypesExcelExporter
    {
        FileDto ExportToFile(List<GetTrailerTypeForViewDto> trailerTypes);
    }
}