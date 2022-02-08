using Abp.Dependency;
using System;
using System.Collections.Generic;
using System.Text;
using TACHYON.Shipping.Trips.Dto;

namespace TACHYON.Shipping.Trips.Importing
{
    public interface IShipmentListExcelDataReader : ITransientDependency
    {
        List<ImportTripDto> GetShipmentsFromExcel(byte[] fileBytes, long shippingRequestId);
    }
}
