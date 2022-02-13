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
            var trips = await GetShipmentListFromExcelOrNull(importShipmentFromExcelInput);
            ValidateDuplicatedReferenceFromList(trips);
            return trips;
        }

        public async Task CreateShipmentsFromDto(List<ImportTripDto> importTripDtoList)
        {
            List<ImportTripDto> SuccessImportTripDtoList = new List<ImportTripDto>();
            List<ImportTripDto> InvalidShipments = new List<ImportTripDto>();

            //check if no duplication in reference
            ValidateDuplicatedReferenceFromList(importTripDtoList);

            //override trip validation
            foreach (var trip in importTripDtoList)
            {
                StringBuilder exceptionMessage = new StringBuilder();
                _shippingRequestTripManager.ValidateTripDto(trip, exceptionMessage);
                if (exceptionMessage.Length == 0)
                {
                    SuccessImportTripDtoList.Add(trip);
                }
                else
                {
                    trip.Exception = exceptionMessage.ToString();
                    InvalidShipments.Add(trip);
                }
            }


            //check if all list is valid
            if (InvalidShipments.Count>0)
                throw new UserFriendlyException(L("AllShipmentsMustBeValid"));

            //save
            await CreateShipments(SuccessImportTripDtoList);

        }
        
        private async Task CreateShipments(List<ImportTripDto> Trips)
        {
            foreach (var trip in Trips)
            {
               await CreateShipmentAsync(trip);
            }
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

        private void ValidateDuplicatedReferenceFromList(List<ImportTripDto> trips)
        {
            var duplicatedReferenceTrips = _shippingRequestTripManager.DuplicatedReferenceFromList(trips);
            //check if no duplication in reference
            if (duplicatedReferenceTrips.Count() > 0)
            {
                foreach (var trip in duplicatedReferenceTrips)
                {
                    trip.Exception = trip.Exception + L("Reference is Duplicated;");

                }
            }
        }
    }
}

