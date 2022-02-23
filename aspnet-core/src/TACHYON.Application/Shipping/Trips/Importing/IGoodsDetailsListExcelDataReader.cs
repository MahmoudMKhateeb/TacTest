using Abp.Dependency;
using System;
using System.Collections.Generic;
using System.Text;
using TACHYON.Shipping.Trips.Importing.Dto;

namespace TACHYON.Shipping.Trips.Importing
{
    public interface IGoodsDetailsListExcelDataReader : ITransientDependency
    {
        List<ImportGoodsDetailsDto> GetGoodsDetailsFromExcel(byte[] fileBytes, long shippingRequestId,int requestGoodsCategoryId , bool isSingleDropRequest);
    }
}
