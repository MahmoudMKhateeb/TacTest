using System;
using System.Collections.Generic;
using System.Text;
using TACHYON.Dto;
using TACHYON.Shipping.Trips.Dto;

namespace TACHYON.Shipping.Trips.Importing
{
    public interface IInvalidShipmentExporter
    {
        FileDto ExportToFile(List<ImportTripDto> truckListDtos);
    }
}
