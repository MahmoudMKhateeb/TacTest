using Abp.Domain.Repositories;
using Abp.Threading;
using Abp.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TACHYON.Goods.Dtos;
using TACHYON.Goods.GoodsDetails;
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
        private readonly IRepository<GoodsDetail, long> _goodsDetailRepository;
        private readonly IGoodsDetailsListExcelDataReader _goodsDetailsListExcelDataReader;
        public ImportShipmentFromExcelAppService(IBinaryObjectManager binaryObjectManager, IShipmentListExcelDataReader shipmentListExcelDataReader, ShippingRequestTripManager shippingRequestTripManager, IRoutePointListDataReader routePointListExcelDataReader, IRepository<RoutPoint, long> routPointRepository, IGoodsDetailsListExcelDataReader goodsDetailsListExcelDataReader, IRepository<GoodsDetail, long> goodsDetailRepository)
        {
            _binaryObjectManager = binaryObjectManager;
            _shipmentListExcelDataReader = shipmentListExcelDataReader;
            _shippingRequestTripManager = shippingRequestTripManager;
            _routePointListExcelDataReader = routePointListExcelDataReader;
            _routPointRepository = routPointRepository;
            _goodsDetailsListExcelDataReader = goodsDetailsListExcelDataReader;
            _goodsDetailRepository = goodsDetailRepository;
        }

        #region ImportTrips
        public async Task<List<ImportTripDto>> ImportShipmentFromExcel(ImportShipmentFromExcelInput importShipmentFromExcelInput)
        {
            var request = _shippingRequestTripManager.GetShippingRequestByPermission(importShipmentFromExcelInput.ShippingRequestId);
            var trips = await GetShipmentListFromExcelOrNull(importShipmentFromExcelInput, IsSingleDropRequest(request));

            await _shippingRequestTripManager.ValidateNumberOfTrips(request, trips.Count);
            ValidateDuplicatedReferenceFromList(trips);
            return trips;
        }

        public async Task CreateShipmentsFromDto(List<ImportTripDto> importTripDtoList)
        {
            var request = _shippingRequestTripManager.GetShippingRequestByPermission(importTripDtoList.First().ShippingRequestId);

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
                throw new UserFriendlyException(L("DataMustBeValid"));

            //save
            await CreateShipments(SuccessImportTripDtoList, IsSingleDropRequest(request));
            request.TotalsTripsAddByShippier += 1;


        }
        #endregion

        #region ImportPoints
        public async Task<List<ImportRoutePointDto>> ImportRoutePointsFromExcel(ImportPointsFromExcelInput importPointFromExcelInput)
        {
            var request = _shippingRequestTripManager.GetShippingRequestByPermission(importPointFromExcelInput.ShippingRequestId);
            if (request == null)
                throw new UserFriendlyException(L("InvalidShippingRequest"));

            var points = await GetRoutePointListFromExcelOrNull(importPointFromExcelInput);

            BindTripIdFromReference(points, request);
            ValidateRoutePoints(points,request);
            

            return points;
        }

        public async Task CreatePointsFromDto(List<ImportRoutePointDto> importRoutePointDtoList)
        {
            var request = _shippingRequestTripManager.GetShippingRequestByPermission(importRoutePointDtoList.First().ShippingRequestTripId);

            if (request == null)
                throw new UserFriendlyException(L("InvalidShippingRequest"));
            //clear and override exceptions
            importRoutePointDtoList.ForEach(x => x.Exception = null);

            ValidateRoutePoints(importRoutePointDtoList,request);

            if (importRoutePointDtoList.All(x => string.IsNullOrEmpty(x.Exception)))
            {
                await CreatePoints(importRoutePointDtoList);
            }
            else
            {
                throw new UserFriendlyException(L("DataMustBeValid"));
            }
        }


        #endregion

        #region GoodsDetails
        public async Task<List<ImportGoodsDetailsDto>> ImportGoodsDetailsFromExcel(ImportGoodsDetailsFromExcelInput input)
        {
            var request = _shippingRequestTripManager.GetShippingRequestByPermission(input.ShippingRequestId);
            var goodsDetails = await GetGoodsDetailsListFromExcelOrNull(input, request.GoodCategoryId.Value,IsSingleDropRequest(request));
            BindTripIdFromReference(goodsDetails, request);
            ValidateGoodsDetails(request, goodsDetails);

            return goodsDetails;
        }

        public async Task CreateGoodsDetailsFromDto(List<ImportGoodsDetailsDto> importGoodsDetailsDtoList)
        {
            var request = _shippingRequestTripManager.GetShippingRequestByPermission(importGoodsDetailsDtoList.First().ShippingRequestTripId);
            try
            {
                ValidateGoodsDetails(request, importGoodsDetailsDtoList);
            }
            catch
            {
                throw new UserFriendlyException(L("DataMustBeValid"));
            }

            await CreateGoodsDetailsAsync(importGoodsDetailsDtoList);
        }


        #endregion

        #region Helper

        private async Task CreateShipments(List<ImportTripDto> Trips, bool isSingleDropRequest)
        {
            foreach (var trip in Trips)
            {
                //await CreateShipmentAsync(trip);
                var tripItem = ObjectMapper.Map<ShippingRequestTrip>(trip);
                trip.Id = await _shippingRequestTripManager.CreateAndGetIdAsync(tripItem);

                if (isSingleDropRequest)
                {
                    //create points
                    await CreatePointsFromSingleDropTrip(trip);
                }
            }
        }

        private async Task CreatePointsFromSingleDropTrip(ImportTripDto trip)
        {
            var pickup = new RoutPoint
            {
                PickingType = PickingType.Pickup,
                ReceiverId = trip.SenderId,
                FacilityId = trip.OriginalFacilityId.Value,
                ShippingRequestTripId = trip.Id
            };
            await _routPointRepository.InsertAsync(pickup);

            var dropPoint = new RoutPoint
            {
                PickingType = PickingType.Dropoff,
                ReceiverId = trip.ReceiverId,
                FacilityId = trip.DestinationFacilityId.Value,
                ShippingRequestTripId = trip.Id
            };
            await _routPointRepository.InsertAsync(dropPoint);
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

        private async Task<List<ImportTripDto>> GetShipmentListFromExcelOrNull(ImportShipmentFromExcelInput importShipmentFromExcelInput, bool isSingleDropRequest)
        {
            using (CurrentUnitOfWork.SetTenantId(importShipmentFromExcelInput.TenantId))
            {
                try
                {
                    var file = await _binaryObjectManager.GetOrNullAsync(importShipmentFromExcelInput.BinaryObjectId);
                    return _shipmentListExcelDataReader.GetShipmentsFromExcel(file.Bytes, importShipmentFromExcelInput.ShippingRequestId, isSingleDropRequest);
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
            CheckExistPointsInDB(points, request);

            if (IsSingleDropRequest(request))
            {
                throw new UserFriendlyException(L("RequestShouldnotBeSingleDrop"));
            }
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

        private async Task<List<ImportGoodsDetailsDto>> GetGoodsDetailsListFromExcelOrNull(ImportGoodsDetailsFromExcelInput input, int requestGoodsCategory, bool isSingleDropRequest)
        {
            using (CurrentUnitOfWork.SetTenantId(input.TenantId))
            {
                try
                {
                    var file = await _binaryObjectManager.GetOrNullAsync(input.BinaryObjectId);
                    return _goodsDetailsListExcelDataReader.GetGoodsDetailsFromExcel(file.Bytes, input.ShippingRequestId, requestGoodsCategory, isSingleDropRequest);
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
                    points.Where(x => x.BulkUploadReference == duplicateRef).ToList().ForEach(x =>
                        x.Exception += L("DuplicatedPointReference") + ";");
                }
            }
        }

        private bool IsSingleDropRequest(ShippingRequest request)
        {
            return request.RouteTypeId == ShippingRequestRouteType.SingleDrop;
        }


        private void CheckExistPointsInDB(List<ImportRoutePointDto> points, ShippingRequest request)
        {
            var tripsRefs = points.Select(x => x.TripReference).Distinct().ToList();
            foreach (var tripRef in tripsRefs)
            {
                var tripId=points.Where(x => x.TripReference == tripRef).Select(x => x.ShippingRequestTripId).FirstOrDefault();

                //check if there is points in trip
                var existPoints = _routPointRepository.FirstOrDefault(x => x.ShippingRequestTripId == tripId);
                if (existPoints!= null)
                {
                    points.Where(x => x.TripReference == tripRef)
                    .ToList()
                    .ForEach(y => y.Exception += L("SomePointsAlreadyAddedToTrip")+";");
                }
                
            }
        }

        private void BindTripIdFromReference(List<ImportGoodsDetailsDto> list, ShippingRequest request)
        {
            var tripsRefs = list.Select(x => x.TripReference).Distinct().ToList();
            var trips = _shippingRequestTripManager.GetShippingRequestTripsIdByBulkRefs(tripsRefs);

            foreach (var tripRef in tripsRefs)
            {
                //var trip = _shippingRequestTripManager.GetShippingRequestTripIdByBulkRef(tripRef, request);
                var trip = trips.Where(x => x.BulkUploadRef == tripRef && x.ShippingRequestId == request.Id).FirstOrDefault();

                if (trip == null)
                {
                    list.Where(x => x.TripReference == tripRef)
                    .ToList()
                    .ForEach(y => y.Exception += L("InvalidTrip"));
                }
                else
                {
                    list.Where(x => x.TripReference == tripRef)
                    .ToList()
                    .ForEach(y => y.ShippingRequestTripId = trip.Id);
                }
            }
        }

        private void BindTripIdFromReference(List<ImportRoutePointDto> points, ShippingRequest request)
        {
            var tripsRefs = points.Select(x => x.TripReference).Distinct().ToList();
            var trips = _shippingRequestTripManager.GetShippingRequestTripsIdByBulkRefs(tripsRefs);
            foreach (var tripRef in tripsRefs)
            {
                //var trip = _shippingRequestTripManager.GetShippingRequestTripIdByBulkRef(tripRef, request);
                var trip = trips.Where(x => x.BulkUploadRef == tripRef && x.ShippingRequestId == request.Id).FirstOrDefault();
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

                }
            }
        }

        private void ValidateGoodsDetails(ShippingRequest request, List<ImportGoodsDetailsDto> goodsDetails)
        {
            //group goods details by trip reference and start validation for each trip
            var GroupedGoodsDetailsByTrip = goodsDetails
                .GroupBy(x => x.TripReference,
                (k, g) => new GroupedGoodsDetailsDto
                {
                    tripRef = k,
                    importGoodsDetailsDtoList = g.ToList<ICreateOrEditGoodsDetailDtoBase>()
                }).ToList();

            foreach (var tripGoodsDetails in GroupedGoodsDetailsByTrip)
            {
                ValidateAllDropGoodsExists(goodsDetails, tripGoodsDetails);
                GoodsDetailsExistsInDB(tripGoodsDetails);
                ValidateTotalWeight(request, tripGoodsDetails);
            }
        }

        private void ValidateAllDropGoodsExists(List<ImportGoodsDetailsDto> goodsDetails, GroupedGoodsDetailsDto tripGoodsDetails)
        {
            var allDropPoints = _routPointRepository
                .GetAll()
                .Where(x => x.ShippingRequestTripFk.BulkUploadRef == tripGoodsDetails.tripRef &&
                x.PickingType == PickingType.Dropoff)
                .Select(x => x.BulkUploadReference)
                .ToList();

            var allDropsGoodsDetailsExists = goodsDetails
                .Where(x => x.TripReference == tripGoodsDetails.tripRef)
                .Select(x => x.PointReference)
                .Distinct()
                .Intersect(allDropPoints)
                .Any();

            if (!allDropsGoodsDetailsExists)
            {
                tripGoodsDetails.importGoodsDetailsDtoList.ForEach(x =>
                x.Exception = L("AllDropsMustHaveGoodsDetails") + ";");
            }
        }

        private void ValidateTotalWeight(ShippingRequest request, GroupedGoodsDetailsDto tripGoodsDetails)
        {
            try
            {
                _shippingRequestTripManager.ValidateTotalweight(tripGoodsDetails.importGoodsDetailsDtoList, request);
            }
            catch
            {
                tripGoodsDetails.importGoodsDetailsDtoList.ForEach(x =>
                x.Exception =
                    L("TheTotalWeightOfGoodsDetailsshouldNotBeGreaterThanShippingRequestWeight", request.TotalWeight) + ";");
            }
        }

        private void GoodsDetailsExistsInDB(GroupedGoodsDetailsDto tripGoodsDetails)
        {
            var goodsDetailsExistsForTrip = _goodsDetailRepository.GetAll()
                .Where(x => x.RoutPointFk.ShippingRequestTripFk.BulkUploadRef == tripGoodsDetails.tripRef).Any();
            if (goodsDetailsExistsForTrip)
            {
                tripGoodsDetails.importGoodsDetailsDtoList.ForEach(x =>
                x.Exception = L("DropGoodsDetailsMustBeEmptyForTrip") + " " + tripGoodsDetails.tripRef + ";");
            }
        }

        private async Task CreateGoodsDetailsAsync(List<ImportGoodsDetailsDto> importGoodsDetailsDtoList)
        {
            foreach (var goodsDetail in importGoodsDetailsDtoList)
            {
                var goodsDetailItem = ObjectMapper.Map<GoodsDetail>(goodsDetail);
                await _goodsDetailRepository.InsertAsync(goodsDetailItem);
            }
        }
        #endregion
    }
}

