using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TACHYON.Shipping.Trips.Dto;

namespace TACHYON.Shipping.Trips
{
    public interface IImportShipmentFromExcelAppService
    {
        Task<List<ImportTripDto>> ImportShipmentFromExcel(ImportShipmentFromExcelInput importShipmentFromExcelInput);
    }
}
