using System.Collections.Generic;
using TACHYON.AddressBook.Dtos;
using TACHYON.Dto;

namespace TACHYON.AddressBook.Exporting
{
    public interface IFacilitiesExcelExporter
    {
        FileDto ExportToFile(List<GetFacilityForViewOutput> facilities);
    }
}