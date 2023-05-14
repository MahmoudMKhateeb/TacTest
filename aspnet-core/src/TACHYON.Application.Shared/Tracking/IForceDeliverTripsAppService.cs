using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TACHYON.Shipping.Trips.Importing.Dto;
using TACHYON.Tracking.Dto;

namespace TACHYON.Tracking
{
    public interface IForceDeliverTripsAppService
    {
        Task<List<ImportTripTransactionFromExcelDto>> ReadForceDeliverTripsFromExcel(ForceDeliverTripInput args);
        Task ForceDeliverTripFromDto(List<ImportTripTransactionFromExcelDto> importedTripDeliveryDetails);
    }
}
