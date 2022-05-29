using Abp.Dependency;
using System;
using System.Collections.Generic;
using System.Text;
using TACHYON.Shipping.Trips.Importing.Dto;

namespace TACHYON.Shipping.Trips.Importing
{
    public interface IRoutePointListDataReader :ITransientDependency
    {
        List<ImportRoutePointDto> GetPointsFromExcel(byte[] fileBytes, long ShippingRequestId);
    }
}
