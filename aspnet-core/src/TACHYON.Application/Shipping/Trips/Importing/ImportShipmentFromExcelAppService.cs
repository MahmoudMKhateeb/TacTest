using Abp.Threading;
using Abp.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TACHYON.Shipping.ShippingRequestTrips;
using TACHYON.Shipping.Trips.Dto;
using TACHYON.Storage;

namespace TACHYON.Shipping.Trips.Importing
{
    public class ImportShipmentFromExcelAppService : TACHYONAppServiceBase, IImportShipmentFromExcelAppService
    {
        private readonly IBinaryObjectManager _binaryObjectManager;
        private readonly IShipmentListExcelDataReader _shipmentListExcelDataReader;
        private readonly ShippingRequestTripManager _shippingRequestTripManager;
        // private readonly IInvalidShipmentExporter _invalidShipmentExporter;
        public ImportShipmentFromExcelAppService(IBinaryObjectManager binaryObjectManager, IShipmentListExcelDataReader shipmentListExcelDataReader, ShippingRequestTripManager shippingRequestTripManager)
        {
            _binaryObjectManager = binaryObjectManager;
            _shipmentListExcelDataReader = shipmentListExcelDataReader;
            _shippingRequestTripManager = shippingRequestTripManager;
        }

        public async Task<List<ImportTripDto>> ImportShipmentFromExcel(ImportShipmentFromExcelInput importShipmentFromExcelInput)
        {
            var trips =await GetShipmentListFromExcelOrNull(importShipmentFromExcelInput);
            return trips;
        }

        public async Task CreateShipmentsFromDto(List<ImportTripDto> importTripDtoList)
        {
            List<ImportTripDto> SuccessImportTripDtoList = new List<ImportTripDto>();
            //override exception message
            foreach(var trip in importTripDtoList)
            {
                try
                {
                    StringBuilder exceptionMessage = new StringBuilder();
                    _shippingRequestTripManager.ValidateTripDto(trip, exceptionMessage);
                    if (exceptionMessage.Length ==0)
                    {
                        SuccessImportTripDtoList.Add(trip);
                    }
                    else
                    {
                        trip.Exception = exceptionMessage.ToString();
                    }
                }
                catch (Exception ex)
                {
                    trip.Exception = ex.Message;
                }
            }
            await CreateShipments(SuccessImportTripDtoList);
        }

        private async Task CreateShipments(List<ImportTripDto> Trips)
        {
            var invalidTrips = new List<ImportTripDto>();

            foreach (var trip in Trips)
            {
                if (trip.CanBeImported())
                {
                    try
                    {
                        await CreateShipmentAsync(trip);
                    }
                    catch (UserFriendlyException exception)
                    {
                        trip.Exception = exception.Message;
                        invalidTrips.Add(trip);
                    }
                    catch (Exception exception)
                    {
                        trip.Exception = exception.ToString();
                        invalidTrips.Add(trip);
                    }
                }
                else
                {
                    invalidTrips.Add(trip);
                }

            }
            //await ProcessImportShipmentsResultAsync(args, invalidTrips));
        }

        private async Task CreateShipmentAsync(ImportTripDto input)
        {
            var trip = ObjectMapper.Map<ShippingRequestTrip>(input);
            await _shippingRequestTripManager.CreateAsync(trip);
        }


        private async Task<List<ImportTripDto>> GetShipmentListFromExcelOrNull(ImportShipmentFromExcelInput importShipmentFromExcelInput)
        {
            using (CurrentUnitOfWork.SetTenantId(importShipmentFromExcelInput.TenantId))
            {
                try
                {
                    var file = await _binaryObjectManager.GetOrNullAsync(importShipmentFromExcelInput.BinaryObjectId);
                    return _shipmentListExcelDataReader.GetShipmentsFromExcel(file.Bytes, importShipmentFromExcelInput.ShippingRequestId);
                }
                catch
                {
                    return null;
                }
            }

        }
    }
}

