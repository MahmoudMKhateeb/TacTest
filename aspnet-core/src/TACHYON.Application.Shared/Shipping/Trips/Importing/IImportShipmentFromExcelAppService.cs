using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TACHYON.Shipping.Trips.Dto;
using TACHYON.Shipping.Trips.Importing.Dto;

namespace TACHYON.Shipping.Trips
{
    public interface IImportShipmentFromExcelAppService
    {
        Task<List<ImportTripDto>> ImportShipmentFromExcel(ImportShipmentFromExcelInput importShipmentFromExcelInput);
        Task CreateShipmentsFromDto(List<ImportTripDto> importTripDtoList);
        Task<List<ImportRoutePointDto>> ImportRoutePointsFromExcel(ImportPointsFromExcelInput importShipmentFromExcelInput);
        Task CreatePointsFromDto(List<ImportRoutePointDto> importRoutePointDtoList);
        Task<List<ImportGoodsDetailsDto>> ImportGoodsDetailsFromExcel(ImportGoodsDetailsFromExcelInput input);
        Task CreateGoodsDetailsFromDto(List<ImportGoodsDetailsDto> importGoodsDetailsDtoList);

    }
}
