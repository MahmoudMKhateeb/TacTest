using System.Collections.Generic;
using TACHYON.Dto;
using TACHYON.Shipping.ShippingRequests.Dtos;

namespace TACHYON.Shipping.ShippingRequests.Exporting
{
    public interface IShippingRequestsExcelExporter
    {
        FileDto ExportToFile(List<GetShippingRequestForViewDto> shippingRequests);
    }
}