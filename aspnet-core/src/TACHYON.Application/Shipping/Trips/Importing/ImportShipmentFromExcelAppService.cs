using Abp.Application.Features;
using Abp.Authorization;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using NUglify.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TACHYON.Authorization;
using TACHYON.Features;
using TACHYON.Goods.Dtos;
using TACHYON.Goods.GoodCategories;
using TACHYON.Goods.GoodsDetails;
using TACHYON.Routs.RoutPoints;
using TACHYON.Shipping.ShippingRequests;
using TACHYON.Shipping.ShippingRequestTrips;
using TACHYON.Shipping.Trips.Dto;
using TACHYON.Shipping.Trips.Importing.Dto;
using TACHYON.ShippingRequestTripVases;
using TACHYON.Storage;
using TACHYON.Tracking;

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
        private readonly IRepository<GoodCategory> _goodCategoryRepoitory;
        private readonly IFeatureChecker _featureChecker;

        public ImportShipmentFromExcelAppService(IBinaryObjectManager binaryObjectManager, IShipmentListExcelDataReader shipmentListExcelDataReader, ShippingRequestTripManager shippingRequestTripManager, IRoutePointListDataReader routePointListExcelDataReader, IRepository<RoutPoint, long> routPointRepository, IGoodsDetailsListExcelDataReader goodsDetailsListExcelDataReader, IRepository<GoodsDetail, long> goodsDetailRepository, IImportTripVasesDataReader importTripVasesDataReader, IRepository<ShippingRequestTripVas, long> shippingRequestTripVas, IRepository<ShippingRequestTrip> shippingRequestTripRepoitory, IRepository<ShippingRequest, long> shippingRequestRepoitory, IRepository<GoodCategory> goodCategoryRepoitory, IFeatureChecker featureChecker)
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
            _goodCategoryRepoitory = goodCategoryRepoitory;
            _featureChecker = featureChecker;
        }

        #region ImportTrips
        public async Task<List<ImportTripDto>> ImportShipmentFromExcel(ImportShipmentFromExcelInput importShipmentFromExcelInput)
        {
            await DisableTenancyFiltersIfTachyonDealer();
            if(!await IsTachyonDealer() && !_featureChecker.IsEnabled(AbpSession.TenantId.Value, AppFeatures.ImportFunctionality))
            {
                throw new UserFriendlyException(L("YouDonnotHaveAccessToImportFunctionality"));
            }
            List<ImportTripDto> trips;
            if (importShipmentFromExcelInput.ShippingRequestId != null)
            {
                var request = _shippingRequestTripManager.GetShippingRequestByPermission(importShipmentFromExcelInput.ShippingRequestId.Value);

                trips = await GetShipmentListFromExcelOrNull(importShipmentFromExcelInput, IsSingleDropRequest(request), request.ShippingRequestFlag == ShippingRequestFlag.Dedicated);

                if (request.ShippingRequestFlag == ShippingRequestFlag.Normal)
                {
                    await _shippingRequestTripManager.ValidateNumberOfTrips(request, trips.Count);
                }
            }
            else
            {
                if(!await IsBroker()  && !await IsEnabledAsync(AppFeatures.ShipperClients)) { throw new UserFriendlyException(L("PermissionForDirectShipmentDenied")); }
                trips = await GetShipmentListFromExcelOrNull(importShipmentFromExcelInput, false,true);

            }
            ValidateDuplicatedReferenceFromList(trips);
            return trips;
        }
        [AbpAuthorize(AppPermissions.Pages_ShippingRequestTrips_Create)]
        public async Task CreateShipmentsFromDto(List<ImportTripDto> importTripDtoList)
        {
            await DisableTenancyFiltersIfTachyonDealer();
            var request = importTripDtoList.First().ShippingRequestId != null 
                ? _shippingRequestTripManager.GetShippingRequestByPermission(importTripDtoList.First().ShippingRequestId.Value)
                : null;

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
            await CreateShipments(SuccessImportTripDtoList,request);
            if(request!= null) request.TotalsTripsAddByShippier += SuccessImportTripDtoList.Count;


        }
        #endregion

        #region ImportPoints
        public async Task<List<ImportRoutePointDto>> ImportRoutePointsFromExcel(ImportPointsFromExcelInput importPointFromExcelInput)
        {
            await DisableTenancyFiltersIfTachyonDealer();
            var request = importPointFromExcelInput.ShippingRequestId != null  ?_shippingRequestTripManager.GetShippingRequestByPermission(importPointFromExcelInput.ShippingRequestId.Value) :null;
            if (request == null && importPointFromExcelInput.ShippingRequestId != null)
                throw new UserFriendlyException(L("InvalidShippingRequest"));

            var points = await GetRoutePointListFromExcelOrNull(importPointFromExcelInput);

            BindTripIdFromReference(points, request);
            ValidateRoutePoints(points,request);
            

            return points;
        }
        [AbpAuthorize(AppPermissions.Pages_ShippingRequestTrips_Create)]
        public async Task CreatePointsFromDto(List<ImportRoutePointDto> importRoutePointDtoList)
        {
            await DisableTenancyFiltersIfTachyonDealer();
            var request = _shippingRequestTripManager.GetShippingRequestByPermission(importRoutePointDtoList.First().ShippingRequestTripId);

            //if (request == null)
            //    throw new UserFriendlyException(L("InvalidShippingRequest"));
            //clear and override exceptions
            importRoutePointDtoList.ForEach(x => x.Exception = null);

            ValidateRoutePoints(importRoutePointDtoList,request);
            if (importRoutePointDtoList.All(x => string.IsNullOrEmpty(x.Exception)))
            {
                await CreatePoints(importRoutePointDtoList, request);
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
            var request = input.ShippingRequestId != null ? _shippingRequestTripManager.GetShippingRequestByPermission(input.ShippingRequestId.Value) : null ;
            var goodsDetails = input.ShippingRequestId != null ?await GetGoodsDetailsListFromExcelOrNull(input, request.GoodCategoryId.Value,IsSingleDropRequest(request) , request.ShippingRequestFlag == ShippingRequestFlag.Dedicated)
                : await GetGoodsDetailsListFromExcelOrNull(input, null, false, true);
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

        private async Task CreateShipments(List<ImportTripDto> Trips, ShippingRequest request)
        {
            foreach (var trip in Trips)
            {
                var tripItem = ObjectMapper.Map<ShippingRequestTrip>(trip);
                


                if (((request != null && request.RouteTypeId == ShippingRequestRouteType.SingleDrop)) || trip.RouteType == ShippingRequestRouteType.SingleDrop)
                {
                    //create points
                    var pickup = new RoutPoint
                    {
                        PickingType = PickingType.Pickup,
                        ReceiverId = trip.SenderId,
                        FacilityId = trip.OriginFacilityId.Value,
                        WorkFlowVersion = WorkflowVersionConst.PickupPointWorkflowVersion,
                        Code = (new Random().Next(100000, 999999)).ToString(),
                    };


                    var dropPoint = new RoutPoint
                    {
                        PickingType = PickingType.Dropoff,
                        ReceiverId = trip.ReceiverId,
                        FacilityId = trip.DestinationFacilityId.Value,
                        Code = (new Random().Next(100000, 999999)).ToString(),
                        WorkFlowVersion = trip.NeedsDeliveryNote
                            ? WorkflowVersionConst.DropOffWithDeliveryNotePointWorkflowVersion
                            : WorkflowVersionConst.DropOffWithoutDeliveryNotePointWorkflowVersion
                    };
                    tripItem.RoutPoints = new List<RoutPoint>
                    {
                        pickup,
                        dropPoint
                    };
                    tripItem.RoutPoints = tripItem.RoutPoints.OrderBy(x => x.PickingType).ToList();
                }
                if(request == null || request.ShippingRequestFlag == ShippingRequestFlag.Dedicated)
                {
                    tripItem.AssignedDriverUserId = trip.DriverUserId;
                    tripItem.AssignedTruckId = trip.TruckId;
                    // if trip is dedicated, insert all request vases automatic
                    if(request != null) AddRequestVasesAutomaticToDedicatedTrip(request, tripItem);

                    // Assign tenant to saas trip
                    if (request == null) tripItem.ShipperTenantId = AbpSession.TenantId;
                    tripItem.CarrierTenantId = AbpSession.TenantId;
                }

                tripItem.SaasInvoicingActivation = await _shippingRequestTripManager.GetSaasInvoicingActivation(trip.ShippingRequestId != null ? request.TenantId : tripItem.ShipperTenantId.Value);
                var r = await  _shippingRequestTripRepoitory.InsertAndGetIdAsync(tripItem);

            }

        }

        private static void AddRequestVasesAutomaticToDedicatedTrip(ShippingRequest request, ShippingRequestTrip tripItem)
        {
            var vasList = new List<ShippingRequestTripVas>();
            foreach (var requestVas in request.ShippingRequestVases)
            {
                var tripVas = new ShippingRequestTripVas
                {
                    ShippingRequestVasId = requestVas.Id
                };
                vasList.Add(tripVas);
            }
            tripItem.ShippingRequestTripVases = new List<ShippingRequestTripVas>();
            tripItem.ShippingRequestTripVases = vasList;
        }

        private async Task<RoutPoint> CreatePointAsync(ImportRoutePointDto input)
        {
            var point = ObjectMapper.Map<RoutPoint>(input);
            return await _shippingRequestTripManager.CreatePointAsync(point);
        }

        private async Task CreatePoints(List<ImportRoutePointDto> points, ShippingRequest request)
        {
            foreach (var point in points)
            {
                var pointDB = await CreatePointAsync(point);
                _shippingRequestTripManager.AssignWorkFlowVersionToRoutPoints(point.TripNeedsDeliveryNote, ShippingRequestTripFlag.Normal,request?.ShippingTypeId,request?.RoundTripType, pointDB);

            }

            //var groupedPointsByDeliverNote = points.GroupBy(x => x.TripNeedsDeliveryNote,
            //  (k, g) => new
            //  {
            //      TripNeedsDeliveryNote = k,
            //      points = g
            //  });
            ////assign workflow version
            //foreach (var point in groupedPointsByDeliverNote)
            //{
            //    _shippingRequestTripManager.AssignWorkFlowVersionToRoutPoints(ObjectMapper.Map<List<RoutPoint>>(point.points.ToList()), point.TripNeedsDeliveryNote,ShippingRequestTripFlag.Normal);
            //}
        }

        private async Task<List<ImportTripDto>> GetShipmentListFromExcelOrNull(ImportShipmentFromExcelInput importShipmentFromExcelInput, bool isSingleDropRequest, bool isDedicatedRequest)
        {
            using (CurrentUnitOfWork.SetTenantId(importShipmentFromExcelInput.TenantId))
            {
                try
                {
                    var file = await _binaryObjectManager.GetOrNullAsync(importShipmentFromExcelInput.BinaryObjectId);
                    return _shipmentListExcelDataReader.GetShipmentsFromExcel(file.Bytes, importShipmentFromExcelInput.ShippingRequestId, isSingleDropRequest, isDedicatedRequest);
                }
                catch
                {
                    return null;
                }
            }

        }

        private void ValidateRoutePoints(List<ImportRoutePointDto> points, ShippingRequest request)
        {
            if (request != null && request.ShippingRequestFlag == ShippingRequestFlag.Normal && IsSingleDropRequest(request))
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

            foreach (var tripDto in GroupedPointssByTrip)
            {
                ValidatePointsDuplicatedReferenceFromList(tripDto.importPointsDtoList);
                ShippingRequestTrip trip = default;
                try
                {
                     trip =  _shippingRequestTripRepoitory.GetAll()
                    .WhereIf(request!= null, x => x.BulkUploadRef == tripDto.tripRef && x.ShippingRequestId == request.Id)
                    .WhereIf(request == null, x => x.BulkUploadRef == tripDto.tripRef && x.ShipperTenantId == AbpSession.TenantId)
                            .FirstOrDefault();
                
                    if(request!= null && request.ShippingRequestFlag == ShippingRequestFlag.Normal)
                    {
                        _shippingRequestTripManager.ValidateNumberOfDrops(tripDto.importPointsDtoList.Count(x => x.PickingType == PickingType.Dropoff), request);
                    }
                    else
                    {
                        // get trip
                        DisableTenancyFilters();
                        if(trip.RouteType == ShippingRequestRouteType.SingleDrop)
                        {
                            throw new UserFriendlyException(L("TripShouldnotBeSingleDrop"));
                        }
                        _shippingRequestTripManager.ValidateDedicatedNumberOfDrops(tripDto.importPointsDtoList.Count(x => x.PickingType == PickingType.Dropoff), trip.NumberOfDrops);
                        if (request != null && (request.ShippingTypeId == ShippingTypeEnum.ImportPortMovements || request.ShippingTypeId == ShippingTypeEnum.ExportPortMovements))
                        {
                            _shippingRequestTripManager.ValidateDedicatedNumberOfPickups(tripDto.importPointsDtoList.Count(x => x.PickingType == PickingType.Pickup), trip.NumberOfDrops);
                        }
                    }
                }
                catch (UserFriendlyException exception)
                {
                    points.Where(x=>x.TripReference== tripDto.tripRef).ToList().ForEach(x => x.Exception += exception.Message);
                }

                //validate points facility
                 ValidatePickupPointsFacility(points, trip);

            }

            CheckExistPointsInDB(points);


        }

        private void ValidatePickupPointsFacility(List<ImportRoutePointDto> points, ShippingRequestTrip trip)
        {
            var pickupPoints = points.Where(x => x.PickingType == PickingType.Pickup).ToList();
            foreach (var pickupPoint in pickupPoints)
            {
                //var trip = await _shippingRequestTripRepoitory.GetAsync(pickupPoint.ShippingRequestTripId);
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

        private async Task<List<ImportGoodsDetailsDto>> GetGoodsDetailsListFromExcelOrNull(ImportGoodsDetailsFromExcelInput input, int? requestGoodsCategory, bool isSingleDropRequest, bool isDedicatedRequest)
        {
            using (CurrentUnitOfWork.SetTenantId(input.TenantId))
            {
                try
                {
                    var file = await _binaryObjectManager.GetOrNullAsync(input.BinaryObjectId);
                    return _goodsDetailsListExcelDataReader.GetGoodsDetailsFromExcel(file.Bytes, input.ShippingRequestId, requestGoodsCategory, isSingleDropRequest, isDedicatedRequest);
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
                var trip = trips.WhereIf(request != null ,x => x.BulkUploadRef == tripRef && x.ShippingRequestId == request.Id)
                    .Select(x=> new {x.Id, x.RouteType, x.BulkUploadRef, x.ShipperTenantId})
                    .WhereIf(request == null,x=> x.BulkUploadRef == tripRef && x.ShipperTenantId == AbpSession.TenantId).FirstOrDefault();

                if (trip == null)
                {
                    list.Where(x => x.TripReference == tripRef)
                    .ToList()
                    .ForEach(y => y.Exception += L("InvalidTripReference")+ "; ");
                }
                else
                {
                    list.Where(x => x.TripReference == tripRef)
                    .ToList()
                    .ForEach(y => y.ShippingRequestTripId = trip.Id);

                    if(trip.RouteType == ShippingRequestRouteType.MultipleDrops && list.Where(x=>x.TripReference == tripRef).Any(x=> string.IsNullOrEmpty(x.PointReference)))
                    {
                        list.Where(x => x.TripReference == tripRef && string.IsNullOrEmpty(x.PointReference)).ForEach(x => x.Exception += L("InvalidPointReference") +"; ");
                    }
                }
            }
        }

        private void BindTripIdFromReference(List<ImportRoutePointDto> points, ShippingRequest? request)
        {
            var tripsRefs = points.Select(x => x.TripReference).Distinct().ToList();
            var trips = _shippingRequestTripManager.GetShippingRequestTripsIdByBulkRefs(tripsRefs);
            foreach (var tripRef in tripsRefs)
            {
                //var trip = _shippingRequestTripManager.GetShippingRequestTripIdByBulkRef(tripRef, request);
                var trip = trips.WhereIf(request != null ,x => x.BulkUploadRef == tripRef && x.ShippingRequestId == request.Id)
                    .WhereIf(request == null, x => x.BulkUploadRef == tripRef && x.ShipperTenantId == AbpSession.TenantId).FirstOrDefault();
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
                ValidateAllDropGoodsExists(goodsDetails, tripGoodsDetails, request?.Id);
                GoodsDetailsExistsInDB(tripGoodsDetails, request?.Id);
                if(request != null) ValidateTotalWeight(request, tripGoodsDetails);
                // validate saas goods category
                if(request == null)
                {

                    var tripGoodsCategory = _shippingRequestTripRepoitory.GetAll().Select(x => new { x.GoodCategoryId, x.BulkUploadRef, x.ShipperTenantId, x.GoodCategoryFk.Key })
                        .FirstOrDefault(x => x.BulkUploadRef == tripGoodsDetails.tripRef && x.ShipperTenantId == AbpSession.TenantId);
                    var subgoods = tripGoodsDetails.importGoodsDetailsDtoList.Select(x => x.GoodCategoryId);
                    if(tripGoodsCategory == null)
                    {
                        tripGoodsDetails.importGoodsDetailsDtoList.ForEach(x =>
                        {
                            x.Exception = L("InvalidTripReference") + ";";
                        });
                        continue;
                    }
                    var AllsubIdsNotFromFather = _goodCategoryRepoitory.GetAll().Where(x => subgoods.Contains(x.Id) && x.FatherId != tripGoodsCategory.GoodCategoryId).Select(x => x.Id).ToList();
                    tripGoodsDetails.importGoodsDetailsDtoList.Where(x => AllsubIdsNotFromFather.Contains(x.GoodCategoryId.Value)).ForEach(x =>
                    {
                        x.Exception += L("CategoryMustBeSubFromTrip") + " - " + tripGoodsCategory.Key + ";";
                    });
                }
            }
        }

        private void ValidateAllDropGoodsExists(List<ImportGoodsDetailsDto> goodsDetails, GroupedGoodsDetailsDto tripGoodsDetails, long? ShippingRequestId)
        {
            var allDropPoints = _routPointRepository
                .GetAll()
                .Where(x => x.ShippingRequestTripFk.BulkUploadRef == tripGoodsDetails.tripRef && 
                x.PickingType == PickingType.Dropoff)
                .WhereIf(ShippingRequestId != null,x=> x.ShippingRequestTripFk.ShippingRequestId == ShippingRequestId)
                .WhereIf(ShippingRequestId  == null,x=> x.ShippingRequestTripFk.ShipperTenantId == AbpSession.TenantId)
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
                x.Exception += L("AllDropsMustHaveGoodsDetails") + ";");
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
                x.Exception +=
                    L("TheTotalWeightOfGoodsDetailsshouldNotBeGreaterThanShippingRequestWeight", request.TotalWeight) + ";");
            }
        }

        private void GoodsDetailsExistsInDB(GroupedGoodsDetailsDto tripGoodsDetails, long? shippingRequestId)
        {
            var goodsDetailsExistsForTrip = _goodsDetailRepository.GetAll()
                .Where(x => x.RoutPointFk.ShippingRequestTripFk.BulkUploadRef == tripGoodsDetails.tripRef)
                .WhereIf(shippingRequestId != null,x=>
                x.RoutPointFk.ShippingRequestTripFk.ShippingRequestId == shippingRequestId)
                .WhereIf(shippingRequestId == null ,x=> x.RoutPointFk.ShippingRequestTripFk.ShipperTenantId == AbpSession.TenantId)
                .Any();
            if (goodsDetailsExistsForTrip)
            {
                tripGoodsDetails.importGoodsDetailsDtoList.ForEach(x =>
                x.Exception += L("GoodsDetailsAlreadyExistsForTrip") + " " + tripGoodsDetails.tripRef + ";");
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

