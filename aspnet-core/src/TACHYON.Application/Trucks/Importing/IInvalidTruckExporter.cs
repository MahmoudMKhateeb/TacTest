using System.Collections.Generic;
using TACHYON.Dto;
using TACHYON.Trucks.Importing.Dto;

namespace TACHYON.Trucks.Importing
{
    public interface IInvalidTruckExporter
    {
        FileDto ExportToFile(List<ImportTruckDto> truckListDtos);
    }
}