using System.Collections.Generic;
using TACHYON.Shipping.ShippingRequests.Dtos;
using TACHYON.Dto;

namespace TACHYON.Shipping.ShippingRequests.Exporting
{
    public interface IShippingRequestsExcelExporter
    {
        FileDto ExportToFile(List<GetShippingRequestForViewDto> shippingRequests);
    }
}