using System.Collections.Generic;
using TACHYON.AddressBook.Ports.Dtos;
using TACHYON.Dto;

namespace TACHYON.AddressBook.Ports.Exporting
{
    public interface IPortsExcelExporter
    {
        FileDto ExportToFile(List<GetPortForViewDto> ports);
    }
}