using System.Collections.Generic;
using TACHYON.Countries.Dtos;
using TACHYON.Dto;

namespace TACHYON.Countries.Exporting
{
    public interface ICountiesExcelExporter
    {
        FileDto ExportToFile(List<GetCountyForViewDto> counties);
    }
}