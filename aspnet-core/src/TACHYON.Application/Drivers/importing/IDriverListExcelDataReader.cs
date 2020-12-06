using System.Collections.Generic;
using Abp.Dependency;
using TACHYON.Authorization.Users.Importing.Dto;
using TACHYON.Drivers.importing.Dto;

namespace TACHYON.Drivers.importing
{
    public interface IDriverListExcelDataReader : ITransientDependency
    {
        List<ImportDriverDto> GetDriversFromExcel(byte[] fileBytes);
    }
}