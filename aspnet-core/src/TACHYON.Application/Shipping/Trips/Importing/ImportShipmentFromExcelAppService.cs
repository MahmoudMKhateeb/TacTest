using Abp.Domain.Repositories;
using Abp.Threading;
using Abp.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TACHYON.Routs.RoutPoints;
using TACHYON.Shipping.ShippingRequests;
using TACHYON.Shipping.ShippingRequestTrips;
using TACHYON.Shipping.Trips.Dto;
using TACHYON.Shipping.Trips.Importing.Dto;
using TACHYON.Storage;

namespace TACHYON.Shipping.Trips.Importing
{
    public class ImportShipmentFromExcelAppService : TACHYONAppServiceBase, IImportShipmentFromExcelAppService
    {
        private readonly IBinaryObjectManager _binaryObjectManager;
        private readonly IShipmentListExcelDataReader _shipmentListExcelDataReader;
        private readonly ShippingRequestTripManager _shippingRequestTripManager;
        private readonly IRoutePointListDataReader _routePointListExcelDataReader;
        private readonly IRepository<RoutPoint, long> _routPointRepository;
        public ImportShipmentFromExcelAppService(IBinaryObjectManager binaryObjectManager, IShipmentListExcelDataReader shipmentListExcelDataReader, ShippingRequestTripManager shippingRequestTripManager, IRoutePointListDataReader routePointListExcelDataReader, IRepository<RoutPoint, long> routPointRepository)
        {
            _binaryObjectManager = binaryObjectManager;
            _shipmentListExcelDataReader = shipmentListExcelDataReader;
            _shippingRequestTripManager = shippingRequestTripManager;
            _routePointListExcelDataReader = routePointListExcelDataReader;
            _routPointRepository = routPointRepository;
        }

        #region ImportTrips
        public async Task<List<ImportTripDto>> ImportShipmentFromExcel(ImportShipmentFromExcelInput importShipmentFromExcelInput)
        {
            var trips = await GetShipmentListFromExcelOrNull(importShipmentFromExcelInput);
            var request = _shippingRequestTripManager.GetShippingRequestByPermission(importShipmentFromExcelInput.ShippingRequestId);
            await _shippingRequestTripManager.ValidateNumberOfTrips(request, trips.Count);
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
        #endregion

        #region ImportPoints
        public async Task<List<ImportRoutePointDto>> ImportRoutePointsFromExcel(ImportPointsFromExcelInput importPointFromExcelInput)
        {
            var points = await GetRoutePointListFromExcelOrNull(importPointFromExcelInput);
            var request = _shippingRequestTripManager.GetShippingRequestByPermission(importPointFromExcelInput.ShippingRequestId);
            if (request == null)
                throw new UserFriendlyException(L("invalidShippingRequest"));

            ValidateRoutePoints(points,request);

            return points;
        }

        public async Task CreatePointsFromDto(List<ImportRoutePointDto> importRoutePointDtoList)
        {
            var request = _shippingRequestTripManager.GetShippingRequestByPermission(importRoutePointDtoList.First().ShippingRequestTripId);

            if (request == null)
                throw new UserFriendlyException(L("invalidShippingRequest"));
            //clear and override exceptions
            importRoutePointDtoList.ForEach(x => x.Exception = null);

            ValidateRoutePoints(importRoutePointDtoList,request);

            if (importRoutePointDtoList.All(x => string.IsNullOrEmpty(x.Exception)))
            {
                await CreatePoints(importRoutePointDtoList);
            }
            else
            {
                throw new UserFriendlyException(L("AllPointsMustBeValid"));
            }
        }
       

        #endregion

        #region Helper

        private async Task CreateShipments(List<ImportTripDto> Trips)
        {
            foreach (var trip in Trips)
            {
               await CreateShipmentAsync(trip);
            }
        }

        private async Task CreatePointAsync(ImportRoutePointDto input)
        {
            var point = ObjectMapper.Map<RoutPoint>(input);
            await _shippingRequestTripManager.CreatePointAsync(point);
        }

        private async Task CreatePoints(List<ImportRoutePointDto> points)
        {
            foreach (var point in points)
            {
                await CreatePointAsync(point);
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

        private void ValidateRoutePoints(List<ImportRoutePointDto> points, ShippingRequest request)
        {
            ValidatePointsDuplicatedReferenceFromList(points);
            CheckExistPointsAndBindTripIdFromReference(points);

            CheckRequestRouteType(request);
            try
            {
                _shippingRequestTripManager.ValidateNumberOfDrops(points.Count(x => x.PickingType == PickingType.Dropoff), request);
            }
            catch (UserFriendlyException exception)
            {
                points.ForEach(x => x.Exception += exception.Message);
            }
        }

        private async Task<List<ImportRoutePointDto>> GetRoutePointListFromExcelOrNull(ImportPointsFromExcelInput importPointsFromExcelInput)
        {
            using (CurrentUnitOfWork.SetTenantId(importPointsFromExcelInput.TenantId))
            {
                try
                {
                    var file = await _binaryObjectManager.GetOrNullAsync(importPointsFromExcelInput.BinaryObjectId);
                    return _routePointListExcelDataReader.GetPointsFromExcel(file.Bytes, importPointsFromExcelInput.ShippingRequestId);
                }
                catch
                {
                    return null;
                }
            }
        }

        private void ValidateDuplicatedReferenceFromList(List<ImportTripDto> trips)
        {
            var duplicatedReferenceTrips = _shippingRequestTripManager.DuplicatedReferenceFromList(trips.Select(x=>x.BulkUploadRef).ToList());
            //check if no duplication in reference
            if (duplicatedReferenceTrips.Count() > 0)
            {
                foreach (var duplicateRef in duplicatedReferenceTrips)
                {
                    //trip.Exception = trip.Exception + L("Reference is Duplicated;");
                    trips.Where(x => x.BulkUploadRef == duplicateRef).ToList().ForEach(x =>
                        x.Exception += L("DuplicatedReference") + ";");
                }
            }
        }

        private void ValidatePointsDuplicatedReferenceFromList(List<ImportRoutePointDto> points)
        {
            var duplicatedReferencePoints = _shippingRequestTripManager.DuplicatedReferenceFromList(points.Select(x=>x.BulkUploadReference).ToList());
            //check if no duplication in reference
            if (duplicatedReferencePoints.Count() > 0)
            {
                foreach (var duplicateRef in duplicatedReferencePoints)
                {
                    //trip.Exception = trip.Exception + L("Reference is Duplicated;");
                    points.Where(x => x.BulkUploadReference == duplicateRef).ToList().ForEach(x =>
                        x.Exception += L("DuplicatedReference") + ";");
                }
            }
        }

        private void CheckRequestRouteType(ShippingRequest request)
        {
            if (request.RouteTypeId == ShippingRequests.ShippingRequestRouteType.SingleDrop)
            {
                throw new UserFriendlyException(L("RequestShouldnotBeSingleDrop"));
            }
        }


        private void CheckExistPointsAndBindTripIdFromReference(List<ImportRoutePointDto> points)
        {
            var tripsRefs = points.Select(x => x.TripReference).Distinct().ToList();
            foreach (var tripRef in tripsRefs)
            {
                var trip = _shippingRequestTripManager.GetShippingRequestTripIdByBulkRef(tripRef);
                if (trip == null)
                {
                    points.Where(x => x.TripReference == tripRef)
                        .ToList()
                        .ForEach(y => y.Exception += L("InvalidTrip"));
                }
                else
                {
                    points.Where(x => x.TripReference == tripRef)
                        .ToList()
                        .ForEach(y => y.ShippingRequestTripId = trip.Id);

                    //check if there is points in trip
                    var existPoints = _routPointRepository.FirstOrDefault(x => x.ShippingRequestTripId == trip.Id);
                    if (existPoints!= null)
                    {
                        points.Where(x => x.TripReference == tripRef)
                        .ToList()
                        .ForEach(y => y.Exception += L("PointsAlreadyAddedToTrip")+";");
                    }
                }
            }
        }
        #endregion
    }
}

