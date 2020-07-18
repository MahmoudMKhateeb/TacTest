using System.Collections.Generic;
using TACHYON.Trucks.Dtos;
using TACHYON.Dto;

namespace TACHYON.Trucks.Exporting
{
    public interface ITrucksExcelExporter
    {
        FileDto ExportToFile(List<GetTruckForViewDto> trucks);
    }
}