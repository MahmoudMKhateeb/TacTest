using Abp;
using Abp.Application.Features;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Collections.Extensions;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.EntityHistory;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Abp.Runtime.Validation;
using Abp.Timing;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;
using TACHYON.Authorization;
using TACHYON.Authorization.Users;
using TACHYON.Documents;
using TACHYON.Documents.DocumentFiles;
using TACHYON.Documents.DocumentFiles.Dtos;
using TACHYON.Documents.DocumentTypes;
using TACHYON.Documents.DocumentTypes.Dtos;
using TACHYON.Dto;
using TACHYON.Features;
using TACHYON.Firebases;
using TACHYON.Goods.Dtos;
using TACHYON.Goods.GoodCategories;
using TACHYON.Goods.GoodsDetails;
using TACHYON.MultiTenancy;
using TACHYON.Notifications;
using TACHYON.Rating;
using TACHYON.Penalties;
using TACHYON.Routs.RoutPoints;
using TACHYON.Routs.RoutPoints.Dtos;
using TACHYON.Routs.RoutPoints.RoutPointSmartEnum;
using TACHYON.Shipping.ShippingRequests;
using TACHYON.Shipping.ShippingRequestTrips;
using TACHYON.Shipping.Trips.Dto;
using TACHYON.Shipping.Trips.RejectReasons.Dtos;
using TACHYON.ShippingRequestTripVases;
using TACHYON.Storage;
using TACHYON.Shipping.ShippingRequestAndTripNotes;
using TACHYON.Shipping.Notes;
using TACHYON.Shipping.Dedicated;

namespace TACHYON.Shipping.Trips
{
    [AbpAuthorize(AppPermissions.Pages_ShippingRequestTrips)]
    public class ShippingRequestsTripAppService : TACHYONAppServiceBase, IShippingRequestsTripAppService
    {
        private readonly IRepository<ShippingRequestTrip> _shippingRequestTripRepository;
        private readonly IRepository<ShippingRequest, long> _shippingRequestRepository;
        private readonly IRepository<RoutPoint, long> _routPointRepository;
        private readonly IRepository<ShippingRequestTripVas, long> _shippingRequestTripVasRepository;
        private readonly IRepository<GoodsDetail, long> _goodsDetailRepository;
        private readonly IRepository<GoodCategory> _goodCategoryRepository;
        private readonly UserManager _userManager;
        private readonly IAppNotifier _appNotifier;
        private readonly ShippingRequestManager _shippingRequestManager;
        private readonly DocumentFilesAppService _documentFilesAppService;
        private readonly IRepository<DocumentFile, Guid> _documentFileRepository;
        private readonly DocumentFilesManager _documentFilesManager;
        private readonly IRepository<DocumentType, long> _documentTypeRepository;
        private readonly IBinaryObjectManager _binaryObjectManager;
        private readonly ITempFileCacheManager _tempFileCacheManager;
        private readonly RatingLogManager _ratingLogManager;
        private readonly IEntityChangeSetReasonProvider _reasonProvider;
        private readonly PenaltyManager _penaltyManager;
        private readonly ShippingRequestTripManager _shippingRequestTripManager;
        private readonly TenantManager _tenantManager;
        private readonly IRepository<ShippingRequestAndTripNote> _ShippingRequestAndTripNoteRepository;
        private readonly IRepository<DedicatedShippingRequestDriver, long> _dedicatedShippingRequestDriverRepository;
        private readonly IRepository<DedicatedShippingRequestTruck, long> _dedicatedShippingRequestTrucksRepository;
        private readonly IRepository<User, long> _userRepository;

        public ShippingRequestsTripAppService(
            IRepository<ShippingRequestTrip> shippingRequestTripRepository,
            IRepository<ShippingRequest, long> shippingRequestRepository,
            IRepository<RoutPoint, long> routPointRepository,
            IRepository<ShippingRequestTripVas, long> shippingRequestTripVasRepository,
            IRepository<GoodsDetail, long> goodsDetailRepository,
            UserManager userManager,
            IAppNotifier appNotifier,
            ShippingRequestManager shippingRequestManager,
            DocumentFilesAppService documentFilesAppService,
            IRepository<GoodCategory> goodCategoryRepository,
            IRepository<DocumentFile, Guid> documentFileRepository,
            DocumentFilesManager documentFilesManager,
            IRepository<DocumentType, long> documentTypeRepository,
            IBinaryObjectManager binaryObjectManager,
            ITempFileCacheManager tempFileCacheManager,
            RatingLogManager ratingLogManager,
            IEntityChangeSetReasonProvider reasonProvider,
            PenaltyManager penaltyManager,
            ShippingRequestTripManager shippingRequestTripManager,
            TenantManager tenantManager,
            IRepository<ShippingRequestAndTripNote> ShippingRequestAndTripNoteRepository
,
            IRepository<DedicatedShippingRequestDriver, long> dedicatedShippingRequestDriverRepository,
            IRepository<DedicatedShippingRequestTruck, long> dedicatedShippingRequestTrucksRepository,
            IRepository<User, long> userRepository)
        {
            _shippingRequestTripRepository = shippingRequestTripRepository;
            _shippingRequestRepository = shippingRequestRepository;
            _routPointRepository = routPointRepository;
            _shippingRequestTripVasRepository = shippingRequestTripVasRepository;
            _goodsDetailRepository = goodsDetailRepository;
            _userManager = userManager;
            _appNotifier = appNotifier;
            _shippingRequestManager = shippingRequestManager;
            _documentFilesAppService = documentFilesAppService;
            _goodCategoryRepository = goodCategoryRepository;
            _documentFileRepository = documentFileRepository;
            _documentFilesManager = documentFilesManager;
            this._documentTypeRepository = documentTypeRepository;
            _binaryObjectManager = binaryObjectManager;
            _tempFileCacheManager = tempFileCacheManager;
            _ratingLogManager = ratingLogManager;
            _reasonProvider = reasonProvider;
            _penaltyManager = penaltyManager;
            _shippingRequestTripManager = shippingRequestTripManager;
            _tenantManager = tenantManager;
            _ShippingRequestAndTripNoteRepository = ShippingRequestAndTripNoteRepository;
            _dedicatedShippingRequestDriverRepository = dedicatedShippingRequestDriverRepository;
            _dedicatedShippingRequestTrucksRepository = dedicatedShippingRequestTrucksRepository;
            _userRepository = userRepository;
        }


        public async Task<PagedResultDto<ShippingRequestsTripListDto>> GetAll(ShippingRequestTripFilterInput input)
        {
            DisableTenancyFilters();
            var isBroker = await FeatureChecker.IsEnabledAsync(true, AppFeatures.CarrierClients, AppFeatures.ShipperClients);

            var request = await _shippingRequestRepository.GetAll()
                .WhereIf(AbpSession.TenantId.HasValue && await IsEnabledAsync(AppFeatures.Carrier),
                    x => x.CarrierTenantId == AbpSession.TenantId)
                .WhereIf(AbpSession.TenantId.HasValue && await IsEnabledAsync(AppFeatures.Shipper) && !isBroker,
                    x => x.TenantId == AbpSession.TenantId)
                .WhereIf(isBroker,x=> x.TenantId == AbpSession.TenantId || x.CarrierTenantId == AbpSession.TenantId)
                .FirstOrDefaultAsync(x => x.Id == input.RequestId);
            if (request == null)
                throw new UserFriendlyException(L("ShippingRequestIsNotFound"));
            
            var query = _shippingRequestTripRepository
                .GetAll()
                .AsNoTracking()
                .Include(x => x.OriginFacilityFk)
                .ThenInclude(x => x.CityFk).ThenInclude(x=> x.Translations)
                .Include(x => x.DestinationFacilityFk)
                .ThenInclude(x => x.CityFk).ThenInclude(x=> x.Translations)
                .Include(x => x.AssignedTruckFk)
                .Include(x => x.AssignedDriverUserFk)
                .Include(x => x.ShippingRequestTripRejectReason)
                .ThenInclude(t => t.Translations)
        .Where(x => x.ShippingRequestId == request.Id)
        .WhereIf(input.Status.HasValue, e => e.Status == input.Status)
        .OrderBy(!input.Sorting.IsNullOrEmpty() && !input.Sorting.Contains("Facility") ? input.Sorting : "Status asc");


            var resultPage = await query.PageBy(input).ToListAsync();
            resultPage.ForEach(r =>
            {
                if (r.ShippingRequestTripRejectReason != null)
                {
                    var reasone =
                        ObjectMapper.Map<ShippingRequestTripRejectReasonListDto>(r.ShippingRequestTripRejectReason);
                    if (reasone != null)
                    {
                        if (!string.IsNullOrEmpty(r.RejectedReason))
                        {
                            r.RejectedReason = $"{reasone.Name}-{r.RejectedReason}";
                        }
                        else
                        {
                            r.RejectedReason = reasone.Name;
                        }
                    }
                }
            });

            var pageResult = ObjectMapper.Map<List<ShippingRequestsTripListDto>>(resultPage);

            var canAssignTrucksAndDrivers = request.CarrierTenantId == AbpSession.TenantId || await IsTachyonDealer();
            foreach (var r in pageResult)
            {
                r.NotesCount = await GetTripNotesCount(r.Id);
                r.CanAssignTrucksAndDrivers = canAssignTrucksAndDrivers;
            }
            
            if (!input.Sorting.IsNullOrEmpty() && input.Sorting.Contains("Facility"))
                pageResult = SortByFacility(input.Sorting, pageResult);

            var totalCount = await query.CountAsync();

            var allRatingLogList = await _ratingLogManager.GetAllRatingByUserAsync(await IsShipper() ? RateType.CarrierByShipper : RateType.ShipperByCarrier,
                request.TenantId, request.CarrierTenantId, null);
            
            var canTripsCreateTemplate = (from trip in pageResult
                    select (trip.Id,_routPointRepository.GetAllIncluding(x => x.GoodsDetails)
                        .Where(x => x.ShippingRequestTripId == trip.Id && x.PickingType == PickingType.Dropoff)
                        .All(x => x.GoodsDetails != null && x.GoodsDetails.Any()))).ToList();

            foreach (var x in pageResult)
            {
                //x.IsTripRateBefore = await _ratingLogManager.IsRateDoneBefore(new RatingLog
                //{
                //    ShipperId = request.TenantId,
                //    CarrierId = request.CarrierTenantId,
                //    RateType = await IsShipper() ? RateType.CarrierByShipper : RateType.ShipperByCarrier,
                //    TripId = x.Id
                //});
                x.IsTripRateBefore = allRatingLogList.Any(x => x.TripId == x.Id);
                x.CanCreateTemplate = canTripsCreateTemplate.FirstOrDefault(i => i.Id == x.Id).Item2 ;
            }

            return new PagedResultDto<ShippingRequestsTripListDto>(
                totalCount,
                pageResult
            );
        }

        private static List<ShippingRequestsTripListDto> SortByFacility(string sorting, List<ShippingRequestsTripListDto> pageResult)
        {

            if (sorting.Contains("DESC"))
                pageResult = pageResult
                    .OrderByDescending(x => sorting.Contains("origin") ? x.OriginCity : x.DestinationCity)
                    .ToList();

            else
                pageResult = pageResult
                    .OrderBy(x => sorting.Contains("origin") ? x.OriginCity : x.DestinationCity)
                    .ToList();
            return pageResult;
        }

        public async Task<ShippingRequestsTripForViewDto> GetShippingRequestTripForView(int id)
        {
            DisableTenancyFilters();
            var trip = await _shippingRequestTripRepository.GetAllIncluding(x => x.ShippingRequestFk).Where(x=>x.Id==id).FirstOrDefaultAsync();
            var shippingRequestTrip = await GetShippingRequestTripForMapper<ShippingRequestsTripForViewDto>(id);
            if (shippingRequestTrip.HasAttachment)
            {
                var documentFile =
                    await _documentFileRepository.FirstOrDefaultAsync(x => x.ShippingRequestTripId == id);
                if (documentFile != null)
                {
                    shippingRequestTrip.DocumentFile = ObjectMapper.Map<DocumentFileDto>(documentFile);
                }
            }
            shippingRequestTrip.RoutPoints = shippingRequestTrip.RoutPoints.OrderBy(x => x.PickingType).ToList();
            shippingRequestTrip.NotesCount = await GetTripNotesCount(id);
            shippingRequestTrip.CanAssignDriversAndTrucks = await IsTachyonDealer() || AbpSession.TenantId == trip.ShippingRequestFk.CarrierTenantId;
            return shippingRequestTrip;
        }


        public async Task<CreateOrEditShippingRequestTripDto> GetShippingRequestTripForEdit(EntityDto input)
        {
            var shippingRequestTrip =
                await GetShippingRequestTripForMapper<CreateOrEditShippingRequestTripDto>(input.Id);

            // fill Attachment file
            if (shippingRequestTrip.HasAttachment)
            {
                var documentFile =
                    await _documentFileRepository.FirstOrDefaultAsync(x => x.ShippingRequestTripId == input.Id);
                if (documentFile != null)
                {
                    shippingRequestTrip.CreateOrEditDocumentFileDto =
                        ObjectMapper.Map<CreateOrEditDocumentFileDto>(documentFile);
                }
            }
            else
            {
                var documentType = await _documentTypeRepository.SingleAsync(x =>
                    x.SpecialConstant.Contains(TACHYONConsts.TripAttachmentDocumentTypeSpecialConstant));
                shippingRequestTrip.CreateOrEditDocumentFileDto = new CreateOrEditDocumentFileDto
                {
                    ShippingRequestTripId = shippingRequestTrip.Id
                };
                shippingRequestTrip.CreateOrEditDocumentFileDto.DocumentTypeDto =
                    ObjectMapper.Map<DocumentTypeDto>(documentType);
            }

            return shippingRequestTrip;
        }

        public async Task<CreateOrEditShippingRequestTripDto> GetShippingRequestTripForCreate()
        {
            var shippingRequestTrip =
                new CreateOrEditShippingRequestTripDto
                {
                    CreateOrEditDocumentFileDto = new CreateOrEditDocumentFileDto()
                };
            //Fill documentType  
            var documentType = await _documentTypeRepository.SingleAsync(x =>
                x.SpecialConstant.Contains(TACHYONConsts.TripAttachmentDocumentTypeSpecialConstant));
            shippingRequestTrip.CreateOrEditDocumentFileDto.DocumentTypeDto =
                ObjectMapper.Map<DocumentTypeDto>(documentType);


            return shippingRequestTrip;
        }

        public async Task CreateOrEdit(CreateOrEditShippingRequestTripDto input)
        {
            await DisableTenancyFiltersIfTachyonDealer();
            var request = await GetShippingRequestByPermission(input.ShippingRequestId);

            //if (await IsEnabledAsync(AppFeatures.TachyonDealer) && !await FeatureChecker.IsEnabledAsync(request.TenantId, AppFeatures.AddTripsByTachyonDeal))
            //    throw new AbpValidationException(L("AddTripsByTachyonDealIsNotEnabledFromShipper"));
            await ValidateGoodsCategory(input.RoutPoints, request.GoodCategoryId);
            if (request.ShippingRequestFlag == ShippingRequestFlag.Normal)
            {
                _shippingRequestTripManager.ValidateTripDates(input, request);
                _shippingRequestTripManager.ValidateNumberOfDrops(input.RoutPoints.Count(x => x.PickingType == PickingType.Dropoff), request);
                _shippingRequestTripManager.ValidateTotalweight(input.RoutPoints.Where(x => x.PickingType == PickingType.Dropoff).SelectMany(x => x.GoodsDetailListDto).ToList<ICreateOrEditGoodsDetailDtoBase>(), request);
            }
            else if(request.ShippingRequestFlag == ShippingRequestFlag.Dedicated)
            {
                if (input.RouteType == ShippingRequestRouteType.SingleDrop) input.NumberOfDrops = 1;
                _shippingRequestTripManager.ValidateDedicatedRequestTripDates(input, request);
                _shippingRequestTripManager.ValidateDedicatedNumberOfDrops(input.RoutPoints.Count(x => x.PickingType == PickingType.Dropoff), input.NumberOfDrops);
                await ValidateTruckAndDriver(input);
            }
            //ValidateNumberOfDrops(input, request);
            //ValidateTotalweight(input, request);
            if (!input.Id.HasValue)
            {
                //int requestNumberOfTripsAdd = await _shippingRequestTripRepository.GetAll()
                //    .Where(x => x.ShippingRequestId == input.ShippingRequestId).CountAsync() + 1;
                //if (requestNumberOfTripsAdd > request.NumberOfTrips)
                //    throw new UserFriendlyException(L("The number of trips " + request.NumberOfTrips));
                if (request.ShippingRequestFlag == ShippingRequestFlag.Normal)
                {
                    await _shippingRequestTripManager.ValidateNumberOfTrips(request, 1);
                }
                await Create(input, request);
                request.TotalsTripsAddByShippier += 1;
            }
            else
            {
                await Update(input, request);
            }
        }

        //[RequiresFeature(AppFeatures.Shipper)]
        //public async Task ChangeAddTripsByTmsFeature()
        //{
        //    if (!AbpSession.TenantId.HasValue) return;
        //    await TenantManager.SetFeatureValueAsync(AbpSession.TenantId.Value, AppFeatures.AddTripsByTachyonDeal,
        //        await IsEnabledAsync(AppFeatures.AddTripsByTachyonDeal) ? "false" : "true");
        //}
        public async Task<FileDto> GetTripAttachmentFileDto(int id)
        {
            DisableTenancyFilters();
            var documentFile = new DocumentFile();
            using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MustHaveTenant, AbpDataFilters.MayHaveTenant))
            {
                var user = await _userManager.FindByIdAsync(AbpSession.UserId.ToString());
                documentFile = await _documentFileRepository.GetAll()
                    .WhereIf(IsEnabled(AppFeatures.Shipper),
                        x => x.ShippingRequestTripFk.ShippingRequestFk.TenantId == AbpSession.TenantId)
                    .WhereIf(IsEnabled(AppFeatures.Carrier),
                        x => x.ShippingRequestTripFk.ShippingRequestFk.CarrierTenantId == AbpSession.TenantId)
                    .WhereIf(user.IsDriver, x => x.ShippingRequestTripFk.AssignedDriverUserId == AbpSession.UserId)
                    .FirstOrDefaultAsync(x => x.ShippingRequestTripId == id && x.ShippingRequestTripFk.HasAttachment);
            }

            if (documentFile == null)
            {
                throw new UserFriendlyException(L("TheFileIsNotFound"));
            }

            var binaryObject = await _binaryObjectManager.GetOrNullAsync(documentFile.BinaryObjectId.Value);
            var file = new FileDto(documentFile.Name, documentFile.Extn);

            _tempFileCacheManager.SetFile(file.FileToken, binaryObject.Bytes);

            return file;
        }

        //private void ValidateTotalweight(CreateOrEditShippingRequestTripDto input, ShippingRequest request)
        //{
        //    if (request.TotalWeight > 0)
        //    {
        //        var totalWeight = input.RoutPoints.Where(x => x.GoodsDetailListDto != null)
        //            .Sum(x => x.GoodsDetailListDto.Sum(g => g.Weight * g.Amount));
        //        if (totalWeight > request.TotalWeight)
        //        {
        //            throw new UserFriendlyException(L(
        //                "TheTotalWeightOfGoodsDetailsshouldNotBeGreaterThanShippingRequestWeight",
        //                request.TotalWeight));
        //        }
        //    }
        //}

        //private void ValidateNumberOfDrops(CreateOrEditShippingRequestTripDto input, ShippingRequest request)
        //{
        //    if (input.RoutPoints.Count(x => x.PickingType == PickingType.Dropoff) != request.NumberOfDrops)
        //    {
        //        throw new UserFriendlyException(L("The number of drop points must be" + request.NumberOfDrops));
        //    }
        //}

        //private void ValidateTripDates(ICreateOrEditTripDtoBase input, ShippingRequest request)
        //{
        //    if (
        //        input.StartTripDate?.Date > request.EndTripDate?.Date ||
        //        input.StartTripDate?.Date < request.StartTripDate?.Date ||
        //        (input.EndTripDate != null && input.EndTripDate.Value.Date > request.EndTripDate?.Date) ||
        //        (input.EndTripDate != null && input.EndTripDate.Value.Date < request.StartTripDate?.Date)
        //    )
        //    {
        //        throw new UserFriendlyException(L("The trip date range must between shipping request range date"));
        //    }
        //}


        public async Task UpdateExpectedDeliveryTimeForTrip(UpdateExpectedDeliveryTimeInput input)
        {
            DisableTenancyFilters();
            var trip = await _shippingRequestTripRepository.GetAllIncluding(x=> x.ShippingRequestFk)
                .SingleAsync(x=> x.Id == input.Id);

            await ValidateExpectedDeliveryTime(input.ExpectedDeliveryTime, trip);
            trip.ExpectedDeliveryTime = input.ExpectedDeliveryTime;
        }

        public async Task AssignDriverAndTruckToShippmentByCarrier(AssignDriverAndTruckToShippmentByCarrierInput input)
        {
            ShippingRequestTrip trip;
            bool isDriverChanged = false;

            using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MayHaveTenant, AbpDataFilters.MustHaveTenant))
            {
                trip = await _shippingRequestTripRepository.GetAll().Include(e => e.ShippingRequestFk)
                    .Include(d => d.AssignedDriverUserFk)
                    .Where(e => e.Id == input.Id)
                    //.Where(e => e.ShippingRequestFk.CarrierTenantId == AbpSession.TenantId)
                    //.Where(e => e.Status != ShippingRequestTripStatus.Delivered)
                    .FirstOrDefaultAsync();
            }

            if (trip == null) throw new UserFriendlyException(L("NoTripToAssignDriver"));

            if (trip.Status == ShippingRequestTripStatus.InTransit && await _shippingRequestManager.CheckIfDriverWorkingOnAnotherTrip(input.AssignedDriverUserId))
                throw new UserFriendlyException(L("TheDriverAreadyWorkingOnAnotherTrip"));

            if (await _shippingRequestManager.CheckIfDriverIsRented(input.AssignedDriverUserId))
                throw new UserFriendlyException(L("TheDriverAreadyRented"));
            if (await _shippingRequestManager.CheckIfTruckIsRented(input.AssignedTruckId))
                throw new UserFriendlyException(L("TheTruckAreadyRented"));

            long? oldAssignedDriverUserId = trip.AssignedDriverUserId;
            long? oldAssignedTruckId = trip.AssignedTruckId;
            trip.AssignedDriverUserId = input.AssignedDriverUserId;
            trip.AssignedTruckId = input.AssignedTruckId;
            bool isTruckChanged = oldAssignedTruckId != input.AssignedTruckId;

            //reset driver status when change 
            if (trip.DriverStatus != ShippingRequestTripDriverStatus.None)
            {
                trip.DriverStatus = ShippingRequestTripDriverStatus.None;
                trip.RejectedReason = string.Empty;
                trip.RejectReasonId = default(int?);
            }

            if (oldAssignedDriverUserId != trip.AssignedDriverUserId)
            {
                trip.AssignedDriverTime = Clock.Now;
                // Send Notification To New Driver
                if (oldAssignedDriverUserId.HasValue)
                {
                    await _appNotifier.NotifyDriverWhenUnassignedTrip(trip.Id, trip.WaybillNumber.ToString(),
                        new UserIdentifier(AbpSession.TenantId, oldAssignedDriverUserId.Value));

                    await UserManager.UpdateUserDriverStatus(oldAssignedDriverUserId.Value, UserDriverStatus.Available);
                    isDriverChanged = true;
                }
            }

            #region SetUpdateReason

            string reason;

            switch (isDriverChanged)
            {
                case true when isTruckChanged:
                    reason = nameof(RoutPointAction4);
                    break;
                case true:
                    reason = nameof(RoutPointAction1);
                    break;
                case false when isTruckChanged:
                    reason = nameof(RoutPointAction2);
                    break;
                default: return;
            }

            _reasonProvider.Use(reason);
            #endregion

            await UserManager.UpdateUserDriverStatus(input.AssignedDriverUserId, UserDriverStatus.NotAvailable);

            if (oldAssignedTruckId != trip.AssignedTruckId && trip.ShippingRequestFk.CarrierTenantId != null)
            {
                var driver = await _userManager.GetUserByIdAsync(trip.AssignedDriverUserId.Value);
                var notifyTripInput = new NotifyTripUpdatedInput()
                {
                    CarrierTenantId = trip.ShippingRequestFk.CarrierTenantId.Value,
                    TripId = trip.Id,
                    WaybillNumber = trip.WaybillNumber.ToString(),
                    DriverIdentifier = new UserIdentifier(driver.TenantId, trip.AssignedDriverUserId.Value)
                };

                await _appNotifier.NotifyCarrierWhenTripUpdated(notifyTripInput);
            }
            if (!oldAssignedTruckId.HasValue && !oldAssignedDriverUserId.HasValue)
                await _penaltyManager.ApplyNotAssigningTruckAndDriverPenalty(trip.ShippingRequestFk.CarrierTenantId.Value, trip.ShippingRequestFk.TenantId, trip.StartTripDate, trip.Id);

            // Send Notification To New Driver
            await _appNotifier.NotifyDriverWhenAssignTrip(trip.Id,
                new UserIdentifier(trip.ShippingRequestFk.CarrierTenantId, trip.AssignedDriverUserId.Value));



            await CurrentUnitOfWork.SaveChangesAsync();
        }

        [AbpAuthorize(AppPermissions.Pages_ShippingRequestTrips_Create)]
        private async Task Create(CreateOrEditShippingRequestTripDto input, ShippingRequest request)
        {
            var routePoint = input.RoutPoints.OrderBy(x => x.PickingType);

            ShippingRequestTrip trip = ObjectMapper.Map<ShippingRequestTrip>(input);
            if(request.ShippingRequestFlag == ShippingRequestFlag.Dedicated)
            {
                trip.AssignedDriverUserId = input.DriverUserId;
                trip.AssignedTruckId = input.TruckId;
            }
            //AssignWorkFlowVersionToRoutPoints(trip);
            _shippingRequestTripManager.AssignWorkFlowVersionToRoutPoints(trip.RoutPoints.ToList(), trip.NeedsDeliveryNote);
            //insert trip 
            var shippingRequestTripId = await _shippingRequestTripRepository.InsertAndGetIdAsync(trip);

            // add document file
            var docFileDto = input.CreateOrEditDocumentFileDto;
            if (trip.HasAttachment)
            {
                docFileDto.Name = input.CreateOrEditDocumentFileDto.DocumentTypeDto.DisplayName + "_" +
                                  shippingRequestTripId;
                docFileDto.ShippingRequestTripId = shippingRequestTripId;
                await _documentFilesAppService.CreateOrEdit(docFileDto);
            }

            //Notify Carrier with trip details
            if(request.ShippingRequestFlag==ShippingRequestFlag.Normal)
            await _shippingRequestTripManager.NotifyCarrierWithTripDetails(trip, request.CarrierTenantId, true, true, true);
        }
        public async Task AddRemarks(RemarksInputDto input)
        {
            DisableTenancyFilters();
            var tenantId = AbpSession.TenantId;
            var shippginRequstTrip = await _shippingRequestTripRepository.GetAll()
                .WhereIf(tenantId.HasValue && await IsEnabledAsync(AppFeatures.Carrier), x => x.ShippingRequestFk.CarrierTenantId == tenantId)
                .WhereIf(tenantId.HasValue && await IsEnabledAsync(AppFeatures.Shipper), x => x.ShippingRequestFk.TenantId == tenantId)
                .WhereIf(tenantId.HasValue && await IsEnabledAsync(AppFeatures.TachyonDealer), x => x.ShippingRequestFk.IsTachyonDeal)
                .Include(x => x.ShippingRequestFk) // include shipping request because we need it in trip update event when send Notfication
                .FirstOrDefaultAsync(x => x.Id == input.Id);

            if (shippginRequstTrip == null) throw new UserFriendlyException(L("YouCannotAddRemarksForThisTrip"));

            shippginRequstTrip.RoundTrip = input.RoundTrip;
            shippginRequstTrip.CanBePrinted = input.CanBePrinted;
            shippginRequstTrip.ContainerNumber = input.ContainerNumber;
        }
        public async Task<RemarksInputDto> GetRemarks(int tripId)
        {
            return await _shippingRequestTripRepository.GetAll()
                  .Select(y => new RemarksInputDto()
                  {
                      Id = y.Id,
                      RoundTrip = y.RoundTrip,
                      ContainerNumber = y.ContainerNumber,
                      CanBePrinted = y.CanBePrinted
                  }).FirstOrDefaultAsync(x => x.Id == tripId);
        }
        private void AssignWorkFlowVersionToRoutPoints(ShippingRequestTrip trip)
        {
            if (trip.RoutPoints != null && trip.RoutPoints.Any())
            {
                foreach (var point in trip.RoutPoints)
                {
                    point.WorkFlowVersion = point.PickingType == PickingType.Pickup
                        ? TACHYONConsts.PickUpRoutPointWorkflowVersion
                        : trip.NeedsDeliveryNote
                            ? TACHYONConsts.DropOfWithDeliveryNoteRoutPointWorkflowVersion
                            : TACHYONConsts.DropOfRoutPointWorkflowVersion;
                }
            }
        }

        private async Task<int> GetTripNotesCount(long TripId)
        {
            DisableTenancyFilters();
            return await _ShippingRequestAndTripNoteRepository
                .GetAll()
                .Include(r=>r.TripFK)
                .ThenInclude(r => r.ShippingRequestFk)
                 //.WhereIf(AbpSession.TenantId.HasValue && await IsEnabledAsync(AppFeatures.Shipper), x => x.TenantId == AbpSession.TenantId)
                 //.WhereIf(AbpSession.TenantId.HasValue && await IsEnabledAsync(AppFeatures.Carrier), x => x.TenantId == AbpSession.TenantId)
                 //.WhereIf(AbpSession.TenantId.HasValue && await IsEnabledAsync(AppFeatures.TachyonDealer), x => x.TenantId == AbpSession.TenantId)
                 .WhereIf(AbpSession.TenantId.HasValue && await IsEnabledAsync(AppFeatures.Shipper),
                    x => x.TenantId == AbpSession.TenantId ||
                    (x.TenantId != AbpSession.TenantId && (x.Visibility == VisibilityNotes.ShipperOnly || x.Visibility == VisibilityNotes.Internal)))
                .WhereIf(AbpSession.TenantId.HasValue && IsEnabled(AppFeatures.Carrier),
                    x => ((x.TripFK.ShippingRequestFk.CarrierTenantId == AbpSession.TenantId || x.TripFK.ShippingRequestFk.CarrierTenantIdForDirectRequest == AbpSession.TenantId) &&
                    (x.Visibility == VisibilityNotes.Internal ||
                    x.Visibility == VisibilityNotes.CarrierOnly ||
                    x.Visibility == VisibilityNotes.TMSAndCarrier)) ||
                    (x.TenantId == AbpSession.TenantId)
                    )
              .WhereIf(AbpSession.TenantId.HasValue && IsEnabled(AppFeatures.TachyonDealer),
                    x => (x.TripFK.ShippingRequestFk.IsTachyonDeal &&
                   (x.Visibility == VisibilityNotes.TMSOnly
                   || x.Visibility == VisibilityNotes.TMSAndCarrier)) ||
                   (x.TenantId == AbpSession.TenantId)
                   )
                .CountAsync(x => x.TripId == TripId);
        }

        [AbpAuthorize(AppPermissions.Pages_ShippingRequestTrips_Edit)]
        private async Task Update(CreateOrEditShippingRequestTripDto input, ShippingRequest request)
        {
            var trip = await GetTrip((int)input.Id, input.ShippingRequestId);
            TripCanEditOrDelete(trip);
            if (trip.ShippingRequestFk.ShippingRequestFlag == ShippingRequestFlag.Dedicated)
            {
                trip.AssignedDriverUserId = input.DriverUserId;
                trip.AssignedTruckId = input.TruckId;
            }

            //await ValidateGoodsCategory(input.RoutPoints, request.GoodCategoryId);
            await RemoveDeletedTripPoints(input, trip);
            await RemoveDeletedTripVases(input, trip);

            await ValidateExpectedDeliveryTime(input.ExpectedDeliveryTime, trip);

            if (input.HasAttachment)
            {
                if (input.CreateOrEditDocumentFileDto.UpdateDocumentFileInput != null)
                {
                    await _documentFilesAppService.CreateOrEdit(input.CreateOrEditDocumentFileDto);
                }
            }
            else
            {
                //remove file if exists
                var documentFile =
                    await _documentFileRepository.FirstOrDefaultAsync(x => x.ShippingRequestTripId == trip.Id);
                if (documentFile != null)
                {
                    await _documentFilesManager.DeleteDocumentFile(documentFile);
                }
            }

            ObjectMapper.Map(input, trip);
        }

        private async Task ValidateExpectedDeliveryTime(DateTime? expectedDeliveryTime, ShippingRequestTrip trip)
        {
            if (!expectedDeliveryTime.Equals(trip.ExpectedDeliveryTime))
            {
                var isPickupPointCompleted = await _routPointRepository.GetAll()
                    .AnyAsync(x => x.PickingType == PickingType.Pickup &&
                                   x.ShippingRequestTripId == trip.Id && x.Status == RoutePointStatus.FinishLoading);
                if (isPickupPointCompleted)
                    throw new UserFriendlyException(L("YouCanNotEditExpectedDeliveryTime"));
            }
        }


        [AbpAuthorize(AppPermissions.Pages_ShippingRequestTrips_Delete)]
        public async Task Delete(EntityDto input)
        {
            var trip = await _shippingRequestTripRepository.FirstOrDefaultAsync(
                x => x.Id == input.Id &&
                     x.Status == ShippingRequestTripStatus.New);


            if (trip != null)
            {
                var request = await GetShippingRequestByPermission(trip.ShippingRequestId);
                TripCanEditOrDelete(trip);
                request.TotalsTripsAddByShippier -= 1;
                await _shippingRequestTripRepository.DeleteAsync(trip);
            }
        }

        //[AbpAuthorize(AppPermissions.Pages_ShippingRequestTrips_Acident_Cancel)]
        //public async Task CancelByAccident(long id, bool isForce)
        //{
        //    DisableTenancyFilters();
        //    var trip = await _shippingRequestTripRepository.GetAll().Include(x => x.ShippingRequestFk)
        //             .WhereIf(IsEnabled(AppFeatures.Carrier), x => x.ShippingRequestFk.CarrierTenantId == AbpSession.TenantId && !x.IsApproveCancledByCarrier)
        //             .WhereIf(IsEnabled(AppFeatures.Shipper), x => x.ShippingRequestFk.TenantId == AbpSession.TenantId && !x.IsApproveCancledByShipper)
        //             .WhereIf(IsEnabled(AppFeatures.TachyonDealer), x => !x.IsApproveCancledByShipper || !x.IsApproveCancledByCarrier)
        //        .FirstOrDefaultAsync(x => x.Id == id && x.HasAccident);
        //    if (trip != null)
        //    {
        //        List<UserIdentifier> userIdentifiers = new List<UserIdentifier>();
        //        if (IsEnabled(AppFeatures.Shipper))
        //        {
        //            trip.IsApproveCancledByShipper = true;
        //            userIdentifiers.Add(await GetAdminTenant((int)trip.ShippingRequestFk.CarrierTenantId));
        //        }
        //        else if (IsEnabled(AppFeatures.Carrier))
        //        {
        //            trip.IsApproveCancledByCarrier = true;
        //            userIdentifiers.Add(new UserIdentifier(trip.ShippingRequestFk.TenantId, (long)trip.ShippingRequestFk.CreatorUserId));

        //        }
        //        else if (IsEnabled(AppFeatures.TachyonDealer))
        //        {
        //            if (isForce)
        //            {
        //                trip.IsApproveCancledByTachyonDealer = true;
        //                trip.IsForcedCanceledByTachyonDealer = true;
        //            }
        //            else
        //            {
        //                trip.IsApproveCancledByTachyonDealer = true;
        //            }
        //            userIdentifiers.Add(await GetAdminTenant((int)trip.ShippingRequestFk.CarrierTenantId));
        //            userIdentifiers.Add(new UserIdentifier(trip.ShippingRequestFk.TenantId, (long)trip.ShippingRequestFk.CreatorUserId));

        //        }

        //        //send notification to tachyon dealer in every request canceled
        //        userIdentifiers.Add(await _userManager.GetTachyonDealerUserIdentifierAsync());

        //        if ((!trip.ShippingRequestFk.IsTachyonDeal && trip.IsApproveCancledByShipper && trip.IsApproveCancledByCarrier) ||
        //            (trip.IsForcedCanceledByTachyonDealer) ||
        //        (!trip.IsForcedCanceledByTachyonDealer && trip.ShippingRequestFk.IsTachyonDeal && trip.IsApproveCancledByShipper && trip.IsApproveCancledByCarrier && trip.IsApproveCancledByTachyonDealer))
        //        {
        //            if (!_shippingRequestTripRepository.GetAll().Any(x => x.Id != trip.Id && x.HasAccident))
        //            {
        //                var request = trip.ShippingRequestFk;
        //                request.HasAccident = false;
        //            }
        //            trip.Status = ShippingRequestTripStatus.Canceled;
        //        }
        //        await _appNotifier.ShippingRequestTripCancelByAccident(userIdentifiers, trip, GetCurrentUser());
        //    }
        //    //await _shippingRequestRepository.DeleteAsync(input.Id);
        //}


        [AbpAuthorize(AppPermissions.Pages_ShippingRequestTrips_Cancel)]
        public async Task CancelTrip(CancelTripInput input)
        {
            DisableTenancyFilters();
            var trip = await _shippingRequestTripRepository.GetAll().Include(x => x.ShippingRequestFk)
                     //add preprice  & !payInadvance condition now, then in future post price cancelation will be added
                     .Where(x => x.ShippingRequestFk.Status == ShippingRequestStatus.PrePrice)
                     .Where(x => x.Status == ShippingRequestTripStatus.New ||
                     x.Status == ShippingRequestTripStatus.InTransit)
                     .WhereIf(IsEnabled(AppFeatures.Carrier), x => x.ShippingRequestFk.CarrierTenantId == AbpSession.TenantId && x.CancelStatus == ShippingRequestTripCancelStatus.None)
                     .WhereIf(IsEnabled(AppFeatures.Shipper), x => x.ShippingRequestFk.TenantId == AbpSession.TenantId && x.CancelStatus == ShippingRequestTripCancelStatus.None)
                .FirstOrDefaultAsync(x => x.Id == input.id);
            if (trip != null)
            {
                var carrierIdent = default(UserIdentifier);
                //if the request is in post price
                if (trip.ShippingRequestFk.CarrierTenantId != null)
                {
                    carrierIdent = await GetAdminTenant((int)trip.ShippingRequestFk.CarrierTenantId);
                }
                //check Invoice type, skip if pay in advance
                await CancelTripAsync(input, trip, carrierIdent);
            }
            else
            {
                throw new UserFriendlyException(L("TripNotFound"));
            }
        }

        #region Heleper

        /// <summary>
        /// Check can delete or edit trip if trip on stand by status
        /// </summary>
        /// <param name="trip"></param>
        private void TripCanEditOrDelete(ShippingRequestTrip trip)
        {
            // When Edit Or Delete
            if (trip.ShippingRequestFk.ShippingRequestFlag==ShippingRequestFlag.Normal && trip.Status != ShippingRequestTripStatus.New)
            {
                throw new UserFriendlyException(L("CanNotEditOrDeleteTrip"));
            }
        }

        private async Task<ShippingRequestTrip> GetTrip(int tripid, long? requestId = null)
        {
            using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MustHaveTenant, AbpDataFilters.MayHaveTenant))
            {
                var trip = await _shippingRequestTripRepository
                    .GetAll()
                    .Include(x => x.ShippingRequestFk)
                    .Include(x => x.OriginFacilityFk)
                    .Include(x => x.DestinationFacilityFk)
                    .Include(x => x.AssignedTruckFk)
                    .Include(x => x.AssignedDriverUserFk)
                    .Include(x => x.ShippingRequestTripRejectReason)
                    .Include(x => x.RoutPoints)
                    .ThenInclude(r => r.FacilityFk)
                    .Include(x => x.RoutPoints)
                    .ThenInclude(r => r.GoodsDetails)
                    .ThenInclude(c => c.GoodCategoryFk)
                    .ThenInclude(r => r.Translations)
                    .Include(x => x.RoutPoints)
                    .ThenInclude(r => r.GoodsDetails)
                    .ThenInclude(c => c.UnitOfMeasureFk)
                    .ThenInclude(x => x.Translations)
                .Include(x => x.RoutPoints)
                    .ThenInclude(c => c.ReceiverFk)
                    .Include(x => x.ShippingRequestTripVases)
                    .ThenInclude(v => v.ShippingRequestVasFk)
                    .ThenInclude(v => v.VasFk)
                    .WhereIf(requestId.HasValue, x => x.ShippingRequestId == requestId)
                    .FirstOrDefaultAsync(x => x.Id == tripid);
                trip.RoutPoints = trip.RoutPoints.OrderBy(x => x.PickingType).ToList();
                if (trip == null) throw new UserFriendlyException(L("ShippingRequestTripIsNotFound"));
                return trip;
            }
        }

        /// <summary>
        /// Return Request when the user loging as shipper or host or carrier 
        /// </summary>
        /// <param name="shippingRequestId"></param>
        /// <returns></returns>
        private async Task<ShippingRequest> GetShippingRequestByPermission(long shippingRequestId)
        {
            var request = await _shippingRequestRepository.GetAll()
                .WhereIf(AbpSession.TenantId != null && await IsEnabledAsync(AppFeatures.Carrier), x => x.CarrierTenantId == AbpSession.TenantId)
                .WhereIf(AbpSession.TenantId != null && await IsEnabledAsync(AppFeatures.Shipper), x => x.TenantId == AbpSession.TenantId)
                .WhereIf(AbpSession.TenantId != null && await IsEnabledAsync(AppFeatures.TachyonDealer), x => x.IsTachyonDeal)
                .WhereIf(AbpSession.TenantId==null, x=> true)
                .FirstOrDefaultAsync(x => x.Id == shippingRequestId);
            if (request == null)
            {
                throw new UserFriendlyException(L("ShippingRequestIsNotFound"));
            }

            return request;
        }


        /// <summary>
        /// Generic  auto mapper fro trip when view or edit 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        private async Task<T> GetShippingRequestTripForMapper<T>(int id)
        {
            DisableTenancyFilters();
            var trip = await GetTrip(id);
            var hasCarrierClients = await IsEnabledAsync(AppFeatures.CarrierClients); // that's mean he is broker
            var userHasAccess = await _shippingRequestRepository.GetAll()
                .Where(x => x.Id == trip.ShippingRequestId)
                .WhereIf(AbpSession.TenantId.HasValue && await IsEnabledAsync(AppFeatures.Shipper) && !hasCarrierClients,
                    x => x.TenantId == AbpSession.TenantId)
                .WhereIf(AbpSession.TenantId.HasValue && await IsEnabledAsync(AppFeatures.Carrier) && !hasCarrierClients,
                    x => x.CarrierTenantId == AbpSession.TenantId)
                .WhereIf(AbpSession.TenantId.HasValue && hasCarrierClients,
                    x => x.CarrierTenantId == AbpSession.TenantId || x.TenantId == AbpSession.TenantId)
                .AnyAsync();

            if (!userHasAccess) throw new UserFriendlyException(L("YouDoNotHaveAccess"));

            return ObjectMapper.Map<T>(trip);
        }

        private async Task<UserIdentifier> GetAdminTenant(int tenantId)
        {
            return new UserIdentifier(tenantId, (await _userManager.GetAdminByTenantIdAsync(tenantId)).Id);
        }

        private async Task ValidateGoodsCategory(IEnumerable<CreateOrEditRoutPointDto> routPoints,
            int? shippingRequestGoodCategoryId)
        {
            var goodsCategories = await _goodCategoryRepository.GetAllListAsync();

            // todo Add Localized String Here
            foreach (var goodsDetail in routPoints.Where(x => x.GoodsDetailListDto != null)
                         .SelectMany(routPoint => routPoint.GoodsDetailListDto))
            {
                if (goodsDetail.GoodCategoryId != null)
                {
                    if (shippingRequestGoodCategoryId == null)
                        throw new UserFriendlyException(L("ErrorWhenCreateTrip")); // Need Review

                    var goodCategory = goodsCategories
                        .FirstOrDefault(x => x.Id == goodsDetail.GoodCategoryId.Value);

                    if (goodCategory.FatherId == null)
                        throw new UserFriendlyException(L("GoodsCategoryMustBeSubCategoryNotMainCategory"));

                    if (goodCategory.FatherId != shippingRequestGoodCategoryId)
                        throw new UserFriendlyException(L("GoodsCategoryMustBeSubOfShippingRequestGoodCategory"));
                }
                else throw new UserFriendlyException(L("GoodsCategoryIsRequired"));
            }
        }


        private async Task RemoveDeletedTripVases(CreateOrEditShippingRequestTripDto input, ShippingRequestTrip trip)
        {
            foreach (var vas in trip.ShippingRequestTripVases)
            {
                if (!input.ShippingRequestTripVases.Any(x => x.Id == vas.Id))
                {
                    await _shippingRequestTripVasRepository.DeleteAsync(vas);
                }
            }
        }

        private async Task RemoveDeletedTripPoints(CreateOrEditShippingRequestTripDto input, ShippingRequestTrip trip)
        {
            foreach (var point in trip.RoutPoints)
            {
                if (!input.RoutPoints.Any(x => x.Id == point.Id))
                {
                    await _routPointRepository.DeleteAsync(point);
                }

                foreach (var g in point.GoodsDetails.Where(x => x.Id != 0))
                {
                    if (!input.RoutPoints.Any(x => x.GoodsDetailListDto.Any(d => d.Id == g.Id)))
                    {
                        await _goodsDetailRepository.DeleteAsync(g);
                    }
                }
            }
        }

        //private async Task NotifyCarrierWithTripDetails(ShippingRequestTrip trip,
        //    int? carrierTenantId,
        //    bool hasAttachmentNotification,
        //    bool needseliverNoteNotification,
        //    bool hasAttachment)
        //{
        //    //Notify carrier when trip has attachment or needs delivery note
        //    if (trip.ShippingRequestFk.CarrierTenantId != null && trip.HasAttachment && hasAttachmentNotification)
        //    {
        //        await _appNotifier.NotifyCarrierWhenTripHasAttachment(trip.Id, carrierTenantId, hasAttachment);
        //    }

        //    if (trip.ShippingRequestFk.CarrierTenantId != null && trip.NeedsDeliveryNote && needseliverNoteNotification)
        //    {
        //        await _appNotifier.NotifyCarrierWhenTripNeedsDeliverNote(trip.Id, carrierTenantId);
        //    }
        //}

        

        

        private async Task CancelTripAsync(CancelTripInput input, ShippingRequestTrip trip, UserIdentifier carrierIdent)
        {
            List<UserIdentifier> userIdentifiers = new List<UserIdentifier>();

            ValidateCanceledReason(input, trip);

            if (IsEnabled(AppFeatures.Shipper))
            {
                //add carrier when request in post price
                if (trip.ShippingRequestFk.CarrierTenantId != null)
                {
                    userIdentifiers.Add(carrierIdent);
                }
                userIdentifiers.Add(await _userManager.GetTachyonDealerUserIdentifierAsync());
                var shipperTenant = await _tenantManager.GetByIdAsync(trip.ShippingRequestFk.TenantId);

                if (!trip.ShippingRequestFk.IsTachyonDeal)
                {
                    trip.Status = ShippingRequestTripStatus.Canceled;
                    trip.CancelStatus = ShippingRequestTripCancelStatus.Canceled;
                    await _appNotifier.ShippingRequestTripCanceled(userIdentifiers, trip, shipperTenant.TenancyName);
                }
                else
                {
                    trip.CancelStatus = ShippingRequestTripCancelStatus.WaitingForTMSApproval;
                    await _appNotifier.NotifyTmsWhenCancellationRequestedByShipper(
                        trip.ShippingRequestFk.ReferenceNumber, trip.WaybillNumber.ToString(),
                        shipperTenant.TenancyName,trip.ShippingRequestId);
                }
            }
            else if (IsEnabled(AppFeatures.Carrier))
            {
                trip.IsApproveCancledByCarrier = true;
                userIdentifiers.Add(new UserIdentifier(trip.ShippingRequestFk.TenantId, (long)trip.ShippingRequestFk.CreatorUserId));
                userIdentifiers.Add(await _userManager.GetTachyonDealerUserIdentifierAsync());

                if (!trip.ShippingRequestFk.IsTachyonDeal)
                {
                    trip.Status = ShippingRequestTripStatus.Canceled;
                    trip.CancelStatus = ShippingRequestTripCancelStatus.Canceled;
                    await _appNotifier.ShippingRequestTripCanceled(userIdentifiers, trip, (await _tenantManager.GetByIdAsync(carrierIdent.TenantId.Value)).TenancyName);
                }
                else
                {
                    trip.CancelStatus = ShippingRequestTripCancelStatus.WaitingForTMSApproval;
                }
            }
            else if (IsEnabled(AppFeatures.TachyonDealer))
            {
                var TMSIdent = await _userManager.GetTachyonDealerUserIdentifierAsync();
                trip.IsApproveCancledByTachyonDealer = input.IsApproved;
                if (trip.IsApproveCancledByTachyonDealer)
                {
                    trip.Status = ShippingRequestTripStatus.Canceled;
                    trip.CancelStatus = ShippingRequestTripCancelStatus.Canceled;
                    if (trip.ShippingRequestFk.CarrierTenantId != null)
                    {
                        userIdentifiers.Add(await GetAdminTenant((int)trip.ShippingRequestFk.CarrierTenantId));
                    }
                    
                    var shipperAdmin = await UserManager.GetAdminByTenantIdAsync(trip.ShippingRequestFk.TenantId);
                    userIdentifiers.Add(shipperAdmin.ToUserIdentifier());
                    await _appNotifier.ShippingRequestTripCanceled(userIdentifiers, trip, (await _tenantManager.GetByIdAsync(TMSIdent.TenantId.Value)).TenancyName);
                }
                else
                {
                    ValidateRejectedReason(input, trip);
                    trip.RejectedCancelingReason = input.RejectedCancelingReason;
                    trip.CancelStatus = ShippingRequestTripCancelStatus.Rejected;
                    await _appNotifier.ShippingRequestTripRejectCancelByTachyonDealer(userIdentifiers, trip.ShippingRequestFk);
                }
            }

        }

        private void ValidateRejectedReason(CancelTripInput input, ShippingRequestTrip trip)
        {
            if (string.IsNullOrWhiteSpace(input.RejectedCancelingReason) &&
                                    trip.CancelStatus == ShippingRequestTripCancelStatus.WaitingForTMSApproval)
            {
                throw new UserFriendlyException(L("YouMustEnterRejectedReason"));
            }
        }

        private void ValidateCanceledReason(CancelTripInput input, ShippingRequestTrip trip)
        {
            if (trip.CancelStatus == ShippingRequestTripCancelStatus.None)
            {
                if (string.IsNullOrWhiteSpace(input.CanceledReason))
                {
                    throw new UserFriendlyException(L("CanceledReasonIsRequired"));
                }
                trip.CanceledReason = input.CanceledReason;
            }
        }

        private async Task ValidateTruckAndDriver(CreateOrEditShippingRequestTripDto input)
        {
            if (input.DriverUserId != null)
            {
                //Check if driver user is from assigned
                if (!await _dedicatedShippingRequestDriverRepository.GetAll().AnyAsync(x => x.ShippingRequestId == input.ShippingRequestId && x.DriverUserId == input.DriverUserId))
                {
                    throw new UserFriendlyException(L("DriverUserMustBeFromAssigned"));
                }
            }
            if (input.TruckId != null)
            {
                if (!await _dedicatedShippingRequestTrucksRepository.GetAll().AnyAsync(x => x.ShippingRequestId == input.ShippingRequestId && x.TruckId == input.TruckId))
                {
                    throw new UserFriendlyException(L("TruckMustBeFromAssigned"));
                }
            }
        }
        #endregion
    }
}