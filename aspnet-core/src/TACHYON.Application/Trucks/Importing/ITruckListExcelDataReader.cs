using System.Collections.Generic;
using Abp.Dependency;
using TACHYON.Authorization.Users.Importing.Dto;
using TACHYON.Trucks.Importing.Dto;

namespace TACHYON.Trucks.Importing
{
    public interface ITruckListExcelDataReader : ITransientDependency
    {
        List<ImportTruckDto> GetTrucksFromExcel(byte[] fileBytes);
    }
}