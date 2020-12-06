using System.Collections.Generic;
using TACHYON.Authorization.Users.Importing.Dto;
using TACHYON.Drivers.importing.Dto;
using TACHYON.Dto;

namespace TACHYON.Drivers.importing
{
    public interface IInvalidDriverExporter
    {
        FileDto ExportToFile(List<ImportDriverDto> userListDtos);
    }
}