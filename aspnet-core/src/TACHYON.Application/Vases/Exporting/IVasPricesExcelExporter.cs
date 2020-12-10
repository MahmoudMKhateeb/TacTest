using System.Collections.Generic;
using TACHYON.Vases.Dtos;
using TACHYON.Dto;

namespace TACHYON.Vases.Exporting
{
    public interface IVasPricesExcelExporter
    {
        FileDto ExportToFile(List<GetVasPriceForViewDto> vasPrices);
    }
}