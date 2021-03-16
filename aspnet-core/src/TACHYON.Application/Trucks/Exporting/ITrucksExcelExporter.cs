using System.Collections.Generic;
using TACHYON.Dto;
using TACHYON.Trucks.Dtos;

namespace TACHYON.Trucks.Exporting
{
    public interface ITrucksExcelExporter
    {
        FileDto ExportToFile(List<GetTruckForViewOutput> trucks);
    }
}