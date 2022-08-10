using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using NUglify.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TACHYON.Authorization;
using TACHYON.Goods.Dtos;
using TACHYON.Goods.GoodsDetails;
using TACHYON.Routs.RoutPoints;
using TACHYON.Shipping.ShippingRequests;
using TACHYON.Shipping.ShippingRequestTrips;
using TACHYON.Shipping.Trips.Dto;
using TACHYON.Shipping.Trips.Importing.Dto;
using TACHYON.ShippingRequestTripVases;
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
        private readonly IImportTripVasesDataReader _importTripVasesDataReader;
        private readonly IRepository<ShippingRequestTripVas, long> _shippingRequestTripVas;
        private readonly IRepository<ShippingRequestTrip> _shippingRequestTripRepoitory;
        private readonly IRepository<ShippingRequest,long> _shippingRequestRepoitory;
        public ImportShipmentFromExcelAppService(IBinaryObjectManager binaryObjectManager, IShipmentListExcelDataReader shipmentListExcelDataReader, ShippingRequestTripManager shippingRequestTripManager, IRoutePointListDataReader routePointListExcelDataReader, IRepository<RoutPoint, long> routPointRepository, IGoodsDetailsListExcelDataReader goodsDetailsListExcelDataReader, IRepository<GoodsDetail, long> goodsDetailRepository, IImportTripVasesDataReader importTripVasesDataReader, IRepository<ShippingRequestTripVas, long> shippingRequestTripVas, IRepository<ShippingRequestTrip> shippingRequestTripRepoitory, IRepository<ShippingRequest, long> shippingRequestRepoitory)
        {
            _binaryObjectManager = binaryObjectManager;
            _shipmentListExcelDataReader = shipmentListExcelDataReader;
            _shippingRequestTripManager = shippingRequestTripManager;
            _routePointListExcelDataReader = routePointListExcelDataReader;
            _routPointRepository = routPointRepository;
            _goodsDetailsListExcelDataReader = goodsDetailsListExcelDataReader;
            _goodsDetailRepository = goodsDetailRepository;
            _importTripVasesDataReader = importTripVasesDataReader;
            _shippingRequestTripVas = shippingRequestTripVas;
            _shippingRequestTripRepoitory = shippingRequestTripRepoitory;
            _shippingRequestRepoitory = shippingRequestRepoitory;
        }

        #region ImportTrips
        public async Task<List<ImportTripDto>> ImportShipmentFromExcel(ImportShipmentFromExcelInput importShipmentFromExcelInput)
        {
            await DisableTenancyFiltersIfTachyonDealer();
            var request = _shippingRequestTripManager.GetShippingRequestByPermission(importShipmentFromExcelInput.ShippingRequestId);
            var trips = await GetShipmentListFromExcelOrNull(importShipmentFromExcelInput, IsSingleDropRequest(request));

            await _shippingRequestTripManager.ValidateNumberOfTrips(request, trips.Count);
            ValidateDuplicatedReferenceFromList(trips);
            return trips;
        }
        [AbpAuthorize(AppPermissions.Pages_ShippingRequestTrips_Create)]
        public async Task CreateShipmentsFromDto(List<ImportTripDto> importTripDtoList)
        {
            await DisableTenancyFiltersIfTachyonDealer();
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
            request.TotalsTripsAddByShippier += SuccessImportTripDtoList.Count;


        }
        #endregion

        #region ImportPoints
        public async Task<List<ImportRoutePointDto>> ImportRoutePointsFromExcel(ImportPointsFromExcelInput importPointFromExcelInput)
        {
            await DisableTenancyFiltersIfTachyonDealer();
            var request = _shippingRequestTripManager.GetShippingRequestByPermission(importPointFromExcelInput.ShippingRequestId);
            if (request == null)
                throw new UserFriendlyException(L("InvalidShippingRequest"));

            var points = await GetRoutePointListFromExcelOrNull(importPointFromExcelInput);

            BindTripIdFromReference(points, request);
            await ValidateRoutePoints(points,request);
            

            return points;
        }
        [AbpAuthorize(AppPermissions.Pages_ShippingRequestTrips_Create)]
        public async Task CreatePointsFromDto(List<ImportRoutePointDto> importRoutePointDtoList)
        {
            await DisableTenancyFiltersIfTachyonDealer();
            var request = _shippingRequestTripManager.GetShippingRequestByPermission(importRoutePointDtoList.First().ShippingRequestTripId);

            if (request == null)
                throw new UserFriendlyException(L("InvalidShippingRequest"));
            //clear and override exceptions
            importRoutePointDtoList.ForEach(x => x.Exception = null);

            await ValidateRoutePoints(importRoutePointDtoList,request);
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
            await DisableTenancyFiltersIfTachyonDealer();
            var request = _shippingRequestTripManager.GetShippingRequestByPermission(input.ShippingRequestId);
            var goodsDetails = await GetGoodsDetailsListFromExcelOrNull(input, request.GoodCategoryId.Value,IsSingleDropRequest(request));
            BindTripIdFromReference(goodsDetails, request);
            ValidateGoodsDetails(request, goodsDetails);

            return goodsDetails;
        }
        [AbpAuthorize(AppPermissions.Pages_ShippingRequestTrips_Create)]
        public async Task CreateGoodsDetailsFromDto(List<ImportGoodsDetailsDto> importGoodsDetailsDtoList)
        {
            await DisableTenancyFiltersIfTachyonDealer();
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

        #region Vases
        public async Task<List<ImportTripVasesDto>> ImportTripVasesFromExcel(ImportTripVasesFromExcelInput input)
        {
            await DisableTenancyFiltersIfTachyonDealer();
            var request = _shippingRequestTripManager.GetShippingRequestByPermission(input.ShippingRequestId);
            var tripVases = await GetTripVasListFromExcelOrNull(input, request.Id);
            await ValidateTripVases(request.Id, tripVases);

            return tripVases;
        }
        [AbpAuthorize(AppPermissions.Pages_ShippingRequestTrips_Create)]
        public async Task CreateTripVasesFromDto(List<ImportTripVasesDto> importTripVasesDtoList)
        {
            await DisableTenancyFiltersIfTachyonDealer();
            var request = _shippingRequestTripManager.GetShippingRequestByPermission(importTripVasesDtoList.First().ShippingRequestId);
            importTripVasesDtoList.ForEach(x => x.Exception = "");
            await ValidateTripVases(request.Id, importTripVasesDtoList);
            await CreateTripVasesAsync(importTripVasesDtoList);
        }

        
        #endregion

        #region Helper

        private async Task CreateShipments(List<ImportTripDto> Trips, bool isSingleDropRequest)
        {
            foreach (var trip in Trips)
            {
                var tripItem = ObjectMapper.Map<ShippingRequestTrip>(trip);

                
                if (isSingleDropRequest)
                {
                    //create points
                    var pickup = new RoutPoint
                    {
                        PickingType = PickingType.Pickup,
                        ReceiverId = trip.SenderId,
                        FacilityId = trip.OriginFacilityId.Value,
                        WorkFlowVersion = TACHYONConsts.PickUpRoutPointWorkflowVersion,
                        Code = (new Random().Next(100000, 999999)).ToString(),
                    };


                    var dropPoint = new RoutPoint
                    {
                        PickingType = PickingType.Dropoff,
                        ReceiverId = trip.ReceiverId,
                        FacilityId = trip.DestinationFacilityId.Value,
                        Code = (new Random().Next(100000, 999999)).ToString(),
                        WorkFlowVersion = trip.NeedsDeliveryNote
                            ? TACHYONConsts.DropOfWithDeliveryNoteRoutPointWorkflowVersion
                            : TACHYONConsts.DropOfRoutPointWorkflowVersion
                    };
                    tripItem.RoutPoints = new List<RoutPoint>
                    {
                        pickup,
                        dropPoint
                    };
                    tripItem.RoutPoints = tripItem.RoutPoints.OrderBy(x => x.PickingType).ToList();
                }
                var r = await  _shippingRequestTripRepoitory.InsertAndGetIdAsync(tripItem);

            }

        }

        private async Task<long> CreatePointAsync(ImportRoutePointDto input)
        {
            var point = ObjectMapper.Map<RoutPoint>(input);
            return await _shippingRequestTripManager.CreatePointAsync(point);
        }

        private async Task CreatePoints(List<ImportRoutePointDto> points)
        {
            List<RoutPoint> RoutPointList = new List<RoutPoint>();
            foreach (var point in points)
            {
                point.Id=await CreatePointAsync(point);
            }

            var groupedPointsByDeliverNote = points.GroupBy(x => x.TripNeedsDeliveryNote,
              (k, g) => new
              {
                  TripNeedsDeliveryNote = k,
                  points = g
              });
            //assign workflow version
            foreach (var point in groupedPointsByDeliverNote)
            {
                _shippingRequestTripManager.AssignWorkFlowVersionToRoutPoints(ObjectMapper.Map<List<RoutPoint>>(point.points.ToList()), point.TripNeedsDeliveryNote);
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

        private async Task ValidateRoutePoints(List<ImportRoutePointDto> points, ShippingRequest request)
        {
            if (IsSingleDropRequest(request))
            {
                throw new UserFriendlyException(L("RequestShouldnotBeSingleDrop"));
            }

            var GroupedPointssByTrip = points
                .GroupBy(x => x.TripReference,
                (k, g) => new
                {
                    tripRef = k,
                    importPointsDtoList = g.ToList()
                }).ToList();

            foreach (var tripGoodsDetails in GroupedPointssByTrip)
            {
                ValidatePointsDuplicatedReferenceFromList(tripGoodsDetails.importPointsDtoList);
    
                try
                {
                    _shippingRequestTripManager.ValidateNumberOfDrops(tripGoodsDetails.importPointsDtoList.Count(x => x.PickingType == PickingType.Dropoff), request);
                }
                catch (UserFriendlyException exception)
                {
                    points.Where(x=>x.TripReference== tripGoodsDetails.tripRef).ToList().ForEach(x => x.Exception += exception.Message);
                }
            }

            CheckExistPointsInDB(points);

            await ValidatePickupPointsFacility(points);

        }

        private async Task ValidatePickupPointsFacility(List<ImportRoutePointDto> points)
        {
            var pickupPoints = points.Where(x => x.PickingType == PickingType.Pickup).ToList();
            foreach (var pickupPoint in pickupPoints)
            {
                var trip = await _shippingRequestTripRepoitory.GetAsync(pickupPoint.ShippingRequestTripId);
                if (trip.OriginFacilityId != pickupPoint.FacilityId)
                {
                    pickupPoint.Exception += L("PickupPointFacilityMustBeSameToOriginalTripFacility") + ";";
                }
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

        private async Task<List<ImportTripVasesDto>> GetTripVasListFromExcelOrNull(ImportTripVasesFromExcelInput input,long ShippingRequestId)
        {
            using (CurrentUnitOfWork.SetTenantId(input.TenantId))
            {
                try
                {
                    var file = await _binaryObjectManager.GetOrNullAsync(input.BinaryObjectId);
                    return _importTripVasesDataReader.GetTripVasesFromExcel(file.Bytes, input.ShippingRequestId);
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


        private void CheckExistPointsInDB(List<ImportRoutePointDto> points)
        {
            var tripsRefs = points.Select(x => x.TripReference).Distinct().ToList();
            var AllPoints = _routPointRepository.GetAll().Where(x => points.Select(x => x.ShippingRequestTripId).Contains(x.ShippingRequestTripId)).ToList();

            foreach (var tripRef in tripsRefs)
            {
                var tripId=points.Where(x => x.TripReference == tripRef).Select(x => x.ShippingRequestTripId).FirstOrDefault();

                //check if there is points in trip
                var existPoints = AllPoints.Any(x => x.ShippingRequestTripId == tripId);
                    //_routPointRepository.FirstOrDefault(x => x.ShippingRequestTripId == tripId);
                if (existPoints)
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
                    foreach(var point in points.Where(x => x.TripReference == tripRef).ToList())
                    {
                        point.ShippingRequestTripId = trip.Id;
                        point.TripNeedsDeliveryNote = trip.NeedsDeliveryNote;
                    }
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
                GoodsDetailsExistsInDB(tripGoodsDetails, request.Id);
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

        private void GoodsDetailsExistsInDB(GroupedGoodsDetailsDto tripGoodsDetails, long shippingRequestId)
        {
            var goodsDetailsExistsForTrip = _goodsDetailRepository.GetAll()
                .Where(x => x.RoutPointFk.ShippingRequestTripFk.BulkUploadRef == tripGoodsDetails.tripRef &&
                x.RoutPointFk.ShippingRequestTripFk.ShippingRequestId == shippingRequestId).Any();
            if (goodsDetailsExistsForTrip)
            {
                tripGoodsDetails.importGoodsDetailsDtoList.ForEach(x =>
                x.Exception = L("GoodsDetailsAlreadyExistsForTrip") + " " + tripGoodsDetails.tripRef + ";");
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
        private async Task ValidateTripVases(long shippingRequestId, List<ImportTripVasesDto> tripVases)
        {
            await ValidateTripVasIfExistsInDB(tripVases);

            ValidateDuplicatedVas(tripVases);

            ValidateNumberOfVases(shippingRequestId, tripVases);
        }


        private void ValidateNumberOfVases(long shippingRequestId, List<ImportTripVasesDto> tripVases)
        {
            var groupedVases = tripVases
                .GroupBy(x => x.ShippingRequestVasId,
                (k, g) => new
                {
                    VasId = k,
                    Count = g.Count()
                }).ToList();

            foreach (var vas in groupedVases)
            {
                try
                {
                    if (!_shippingRequestTripManager.ValidateTripVasesNumber(shippingRequestId, vas.Count, vas.VasId))
                    {
                        tripVases.Where(x => x.ShippingRequestVasId == vas.VasId).ToList()
                            .ForEach(x => x.Exception = L("VasesCountMoreThanRequestVasNumberOfTrips") + ";");
                    }
                }
                catch
                {
                    tripVases.Where(x => x.ShippingRequestVasId == vas.VasId).ToList().ForEach(x =>
                        x.Exception = L("InvalidVas")+";");
                }
            }
        }

        private void ValidateDuplicatedVas(List<ImportTripVasesDto> tripVases)
        {

            var duplicatedVases = tripVases
                .GroupBy(x => new {x.TripReference, x.VasName})
                .Where(x=> x.Skip(1).Any())
                .Select(x=> x.Key).ToList();

            if (duplicatedVases.Any())
            {
                duplicatedVases.ForEach(dv =>
                {
                    tripVases.Where(x=> x.TripReference.Equals(dv.TripReference) && x.VasName.Equals(dv.VasName)).ForEach(x=> x.Exception = L("DuplicatedVas")+";");
                }); 
            }
        }

        private async Task ValidateTripVasIfExistsInDB(List<ImportTripVasesDto> tripVases)
        {
            var tripIds = tripVases.Select(x => x.ShippingRequestTripId).Distinct();

            if (await _shippingRequestTripVas.GetAll().AnyAsync(x => tripIds.Contains(x.ShippingRequestTripId)))
            {
                throw new UserFriendlyException(L("SomeTripVasesAlreadyExists"));
            }
        }

        private async Task CreateTripVasesAsync(List<ImportTripVasesDto> importTripVasesDtoList)
        {
            foreach (var item in importTripVasesDtoList)
            {
                var tripVas = ObjectMapper.Map<ShippingRequestTripVas>(item);
                await _shippingRequestTripVas.InsertAsync(tripVas);
            }
        }
        #endregion
    }
}

