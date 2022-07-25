using Abp.Dependency;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Text;
using TACHYON.Shipping.Trips.Importing.Dto;

namespace TACHYON.Shipping.Trips.Importing
{
    public interface IImportTripVasesDataReader : ITransientDependency
    {
        List<ImportTripVasesDto> GetTripVasesFromExcel(byte[] fileBytes, long ShippingRequestId);
    }
}
