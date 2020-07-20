using System.Collections.Generic;
using TACHYON.Cities.Dtos;
using TACHYON.Dto;

namespace TACHYON.Cities.Exporting
{
    public interface ICitiesExcelExporter
    {
        FileDto ExportToFile(List<GetCityForViewDto> cities);
    }
}