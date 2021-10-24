using Abp;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Abp.Timing;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
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
using TACHYON.Goods.GoodCategories;
using TACHYON.Goods.GoodsDetails;
using TACHYON.Notifications;
using TACHYON.Routs.RoutPoints;
using TACHYON.Routs.RoutPoints.Dtos;
using TACHYON.Shipping.ShippingRequests;
using TACHYON.Shipping.ShippingRequestTrips;
using TACHYON.Shipping.Trips.Dto;
using TACHYON.Shipping.Trips.RejectReasons.Dtos;
using TACHYON.ShippingRequestTripVases;
using TACHYON.Storage;

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
        private readonly IFirebaseNotifier _firebase;
        private readonly ShippingRequestManager _shippingRequestManager;
        private readonly DocumentFilesAppService _documentFilesAppService;
        private readonly IRepository<DocumentFile, Guid> _documentFileRepository;
        private readonly DocumentFilesManager _documentFilesManager;
        private readonly IRepository<DocumentType, long> _documentTypeRepository;
        private readonly IBinaryObjectManager _binaryObjectManager;
        private readonly ITempFileCacheManager _tempFileCacheManager;




        public ShippingRequestsTripAppService(
            IRepository<ShippingRequestTrip> shippingRequestTripRepository,
            IRepository<ShippingRequest, long> shippingRequestRepository,
            IRepository<RoutPoint, long> routPointRepository,
            IRepository<ShippingRequestTripVas, long> shippingRequestTripVasRepository,
            IRepository<GoodsDetail, long> goodsDetailRepository,
            UserManager userManager,
            IAppNotifier appNotifier,
            IFirebaseNotifier firebase,
            ShippingRequestManager shippingRequestManager, DocumentFilesAppService documentFilesAppService, IRepository<GoodCategory> goodCategoryRepository, IRepository<DocumentFile, Guid> documentFileRepository, DocumentFilesManager documentFilesManager, IRepository<DocumentType, long> documentTypeRepository, IBinaryObjectManager binaryObjectManager, ITempFileCacheManager tempFileCacheManager)
        {
            _shippingRequestTripRepository = shippingRequestTripRepository;
            _shippingRequestRepository = shippingRequestRepository;
            _routPointRepository = routPointRepository;
            _shippingRequestTripVasRepository = shippingRequestTripVasRepository;
            _goodsDetailRepository = goodsDetailRepository;
            _userManager = userManager;
            _appNotifier = appNotifier;
            _firebase = firebase;
            _shippingRequestManager = shippingRequestManager;
            _documentFilesAppService = documentFilesAppService;
            _goodCategoryRepository = goodCategoryRepository;
            _documentFileRepository = documentFileRepository;
            _documentFilesManager = documentFilesManager;
            this._documentTypeRepository = documentTypeRepository;
            _binaryObjectManager = binaryObjectManager;
            _tempFileCacheManager = tempFileCacheManager;
        }



        public async Task<PagedResultDto<ShippingRequestsTripListDto>> GetAll(ShippingRequestTripFilterInput input)
        {
            DisableTenancyFilters();
            var request = await GetShippingRequestByPermission(input.RequestId);
            var query = _shippingRequestTripRepository
        .GetAll()
        .AsNoTracking()
            .Include(x => x.OriginFacilityFk)
            .ThenInclude(x => x.CityFk)
            .Include(x => x.DestinationFacilityFk)
            .ThenInclude(x => x.CityFk)
            .Include(x => x.AssignedTruckFk)
            .Include(x => x.AssignedDriverUserFk)
            .Include(x => x.ShippingRequestTripRejectReason)
                .ThenInclude(t => t.Translations)
        .Where(x => x.ShippingRequestId == request.Id)
        .WhereIf(input.Status.HasValue, e => e.Status == input.Status)
        .OrderBy(input.Sorting ?? "Status asc");


            var resultPage = await query.PageBy(input).ToListAsync();
            resultPage.ForEach(r =>
            {
                if (r.ShippingRequestTripRejectReason != null)
                {
                    var reasone = ObjectMapper.Map<ShippingRequestTripRejectReasonListDto>(r.ShippingRequestTripRejectReason);
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

            var totalCount = await query.CountAsync();
            return new PagedResultDto<ShippingRequestsTripListDto>(
                totalCount,
                ObjectMapper.Map<List<ShippingRequestsTripListDto>>(resultPage)

            );

        }

        public async Task<ShippingRequestsTripForViewDto> GetShippingRequestTripForView(int id)
        {

            var shippingRequestTrip = await GetShippingRequestTripForMapper<ShippingRequestsTripForViewDto>(id);
            if (shippingRequestTrip.HasAttachment)
            {
                var documentFile = await _documentFileRepository.FirstOrDefaultAsync(x => x.ShippingRequestTripId == id);
                if (documentFile != null)
                {
                    shippingRequestTrip.DocumentFile = ObjectMapper.Map<DocumentFileDto>(documentFile);
                }
            }

            return shippingRequestTrip;
        }


        public async Task<CreateOrEditShippingRequestTripDto> GetShippingRequestTripForEdit(EntityDto input)
        {

            var shippingRequestTrip = await GetShippingRequestTripForMapper<CreateOrEditShippingRequestTripDto>(input.Id);

            // fill Attachment file
            if (shippingRequestTrip.HasAttachment)
            {
                var documentFile = await _documentFileRepository.FirstOrDefaultAsync(x => x.ShippingRequestTripId == input.Id);
                if (documentFile != null)
                {
                    shippingRequestTrip.CreateOrEditDocumentFileDto = ObjectMapper.Map<CreateOrEditDocumentFileDto>(documentFile);
                }
            }
            else
            {
                var documentType = await _documentTypeRepository.SingleAsync(x => x.SpecialConstant.Contains(TACHYONConsts.TripAttachmentDocumentTypeSpecialConstant));
                shippingRequestTrip.CreateOrEditDocumentFileDto = new CreateOrEditDocumentFileDto
                {
                    ShippingRequestTripId = shippingRequestTrip.Id
                };
                shippingRequestTrip.CreateOrEditDocumentFileDto.DocumentTypeDto = ObjectMapper.Map<DocumentTypeDto>(documentType);

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
            var documentType = await _documentTypeRepository.SingleAsync(x => x.SpecialConstant.Contains(TACHYONConsts.TripAttachmentDocumentTypeSpecialConstant));
            shippingRequestTrip.CreateOrEditDocumentFileDto.DocumentTypeDto = ObjectMapper.Map<DocumentTypeDto>(documentType);


            return shippingRequestTrip;
        }

        public async Task CreateOrEdit(CreateOrEditShippingRequestTripDto input)
        {
            await DisableTenancyFiltersIfTachyonDealer();
            var request = await GetShippingRequestByPermission(input.ShippingRequestId);

            ValidateTripDates(input, request);
            ValidateNumberOfDrops(input, request);
            ValidateTotalweight(input, request);

            if (!input.Id.HasValue)
            {
                int requestNumberOfTripsAdd = await _shippingRequestTripRepository.GetAll().Where(x => x.ShippingRequestId == input.ShippingRequestId).CountAsync() + 1;
                if (requestNumberOfTripsAdd > request.NumberOfTrips) throw new UserFriendlyException(L("The number of trips " + request.NumberOfTrips));
                await Create(input, request);
                request.TotalsTripsAddByShippier += 1;
            }
            else
            {
                await Update(input, request);
            }


        }


        public async Task<FileDto> GetTripAttachmentFileDto(int id)
        {
            DisableTenancyFilters();
            var documentFile = new DocumentFile();
            using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MustHaveTenant, AbpDataFilters.MayHaveTenant))
            {
                var user = await _userManager.FindByIdAsync(AbpSession.UserId.ToString());
                documentFile = await _documentFileRepository.GetAll()
                   .WhereIf(IsEnabled(AppFeatures.Shipper), x => x.ShippingRequestTripFk.ShippingRequestFk.TenantId == AbpSession.TenantId)
                   .WhereIf(IsEnabled(AppFeatures.Carrier), x => x.ShippingRequestTripFk.ShippingRequestFk.CarrierTenantId == AbpSession.TenantId)
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

        private void ValidateTotalweight(CreateOrEditShippingRequestTripDto input, ShippingRequest request)
        {
            if (request.TotalWeight > 0)
            {
                var totalWeight = input.RoutPoints.Where(x => x.GoodsDetailListDto != null)
                    .Sum(x => x.GoodsDetailListDto.Sum(g => g.Weight * g.Amount));
                if (totalWeight > request.TotalWeight)
                {
                    throw new UserFriendlyException(L("TheTotalWeightOfGoodsDetailsshouldNotBeGreaterThanShippingRequestWeight",
                        request.TotalWeight));
                }
            }
        }

        private void ValidateNumberOfDrops(CreateOrEditShippingRequestTripDto input, ShippingRequest request)
        {
            if (input.RoutPoints.Count(x => x.PickingType == PickingType.Dropoff) != request.NumberOfDrops)
            {
                throw new UserFriendlyException(L("The number of drop points must be" + request.NumberOfDrops));
            }
        }

        private void ValidateTripDates(CreateOrEditShippingRequestTripDto input, ShippingRequest request)
        {
            if (
                input.StartTripDate.Date > request.EndTripDate?.Date ||
                input.StartTripDate.Date < request.StartTripDate?.Date ||
                (input.EndTripDate != null && input.EndTripDate.Value.Date > request.EndTripDate?.Date) ||
                (input.EndTripDate != null && input.EndTripDate.Value.Date < request.StartTripDate?.Date)
            )
            {
                throw new UserFriendlyException(L("The trip date range must between shipping request range date"));
            }
        }

        public async Task AssignDriverAndTruckToShippmentByCarrier(AssignDriverAndTruckToShippmentByCarrierInput input)
        {
            DisableTenancyFilters();
            var trip = await _shippingRequestTripRepository.
                GetAll().
                Include(e => e.ShippingRequestFk)
                .Include(d => d.AssignedDriverUserFk)
                .Where(e => e.Id == input.Id)
                //.Where(e => e.ShippingRequestFk.CarrierTenantId == AbpSession.TenantId)
                //.Where(e => e.Status != ShippingRequestTripStatus.Delivered)
                .FirstOrDefaultAsync();
            if (trip == null) throw new UserFriendlyException(L("NoTripToAssignDriver"));

            long? oldAssignedDriverUserId = trip.AssignedDriverUserId;
            long? oldAssignedTruckId = input.AssignedTruckId;
            trip.AssignedDriverUserId = input.AssignedDriverUserId;
            trip.AssignedTruckId = input.AssignedTruckId;

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
                await _appNotifier.NotifyDriverWhenAssignToTrip(trip);
                await _firebase.PushNotificationToDriverWhenAssignTrip(new UserIdentifier(trip.AssignedDriverUserFk.TenantId, trip.AssignedDriverUserId.Value), trip.Id.ToString(), trip.WaybillNumber.ToString());
                if (oldAssignedDriverUserId.HasValue)
                {
                    //todo send specific notification    
                    await _firebase.TripChanged(new UserIdentifier(trip.AssignedDriverUserFk.TenantId, oldAssignedDriverUserId.Value), trip.Id.ToString());
                    await _appNotifier.ShipperShippingRequestTripNotifyDriverWhenUnassignedTrip(new UserIdentifier(AbpSession.TenantId, oldAssignedDriverUserId.Value), trip);
                }
            }

            if (oldAssignedTruckId != trip.AssignedTruckId)
            {
                //todo send specific notification    
                await _firebase.TripChanged(new UserIdentifier(trip.AssignedDriverUserFk.TenantId, trip.AssignedDriverUserId.Value), trip.Id.ToString());
            }
            await _appNotifier.ShipperShippingRequestTripNotifyDriverWhenAssignTrip(new UserIdentifier(AbpSession.TenantId, trip.AssignedDriverUserId.Value), trip);


            await _appNotifier.NotificationWhenTripDetailsChanged(trip, await GetCurrentUserAsync());

        }

        [AbpAuthorize(AppPermissions.Pages_ShippingRequestTrips_Create)]
        private async Task Create(CreateOrEditShippingRequestTripDto input, ShippingRequest request)
        {
            var routePoint = input.RoutPoints.OrderBy(x => x.PickingType);

            await ValidateGoodsCategory(input.RoutPoints, request.GoodCategoryId);

            ShippingRequestTrip trip = ObjectMapper.Map<ShippingRequestTrip>(input);

            //insert trip 
            var shippingRequestTripId = await _shippingRequestTripRepository.InsertAndGetIdAsync(trip);

            // add document file
            var docFileDto = input.CreateOrEditDocumentFileDto;
            if (trip.HasAttachment)
            {
                docFileDto.Name = input.CreateOrEditDocumentFileDto.DocumentTypeDto.DisplayName + "_" + shippingRequestTripId;
                docFileDto.ShippingRequestTripId = shippingRequestTripId;
                await _documentFilesAppService.CreateOrEdit(docFileDto);
            }

            //Notify Carrier with trip details
            await NotifyCarrierWithTripDetails(trip, request.CarrierTenantId, true, true, true);

        }

        [AbpAuthorize(AppPermissions.Pages_ShippingRequestTrips_Edit)]
        private async Task Update(CreateOrEditShippingRequestTripDto input, ShippingRequest request)
        {
            var trip = await GetTrip((int)input.Id, input.ShippingRequestId);
            TripCanEditOrDelete(trip);


            await ValidateGoodsCategory(input.RoutPoints, request.GoodCategoryId);
            await RemoveDeletedTripPoints(input, trip);
            await RemoveDeletedTripVases(input, trip);


            if (input.HasAttachment)
            {
                await _documentFilesAppService.CreateOrEdit(input.CreateOrEditDocumentFileDto);
            }
            else
            {
                //remove file if exists
                var documentFile = await _documentFileRepository.FirstOrDefaultAsync(x => x.ShippingRequestTripId == trip.Id);
                if (documentFile != null)
                {
                    await _documentFilesManager.DeleteDocumentFile(documentFile);
                }

            }

            var needseliveryNoteOldValue = trip.NeedsDeliveryNote;

            if (needseliveryNoteOldValue != input.NeedsDeliveryNote)
            {
            }

            ObjectMapper.Map(input, trip);
        }


        [AbpAuthorize(AppPermissions.Pages_ShippingRequestTrips_Delete)]
        public async Task Delete(EntityDto input)
        {

            var trip = await _shippingRequestTripRepository.
                FirstOrDefaultAsync(
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

        [AbpAuthorize(AppPermissions.Pages_ShippingRequestTrips_Acident_Cancel)]
        public async Task CancelByAccident(long id)
        {
            DisableTenancyFilters();
            var trip = await _shippingRequestTripRepository.GetAll().Include(x => x.ShippingRequestFk)
                     .WhereIf(IsEnabled(AppFeatures.Carrier), x => x.ShippingRequestFk.CarrierTenantId == AbpSession.TenantId && !x.IsApproveCancledByCarrier)
                     .WhereIf(IsEnabled(AppFeatures.Shipper), x => x.ShippingRequestFk.TenantId == AbpSession.TenantId && !x.IsApproveCancledByShipper)
                     .WhereIf(IsEnabled(AppFeatures.TachyonDealer), x => !x.IsApproveCancledByShipper || !x.IsApproveCancledByCarrier)
                .FirstOrDefaultAsync(x => x.Id == id && x.HasAccident);
            if (trip != null)
            {
                List<UserIdentifier> userIdentifiers = new List<UserIdentifier>();
                if (IsEnabled(AppFeatures.Shipper))
                {
                    trip.IsApproveCancledByShipper = true;
                    userIdentifiers.Add(await GetAdminTenant((int)trip.ShippingRequestFk.CarrierTenantId));
                }
                else if (IsEnabled(AppFeatures.Carrier))
                {
                    trip.IsApproveCancledByCarrier = true;
                    userIdentifiers.Add(new UserIdentifier(trip.ShippingRequestFk.TenantId, (long)trip.ShippingRequestFk.CreatorUserId));

                }
                else if (IsEnabled(AppFeatures.TachyonDealer))
                {
                    trip.IsApproveCancledByShipper = true;
                    trip.IsApproveCancledByCarrier = true;
                    userIdentifiers.Add(await GetAdminTenant((int)trip.ShippingRequestFk.CarrierTenantId));
                    userIdentifiers.Add(new UserIdentifier(trip.ShippingRequestFk.TenantId, (long)trip.ShippingRequestFk.CreatorUserId));

                }
                if (trip.IsApproveCancledByShipper && trip.IsApproveCancledByCarrier)
                {

                    if (!_shippingRequestTripRepository.GetAll().Any(x => x.Id != trip.Id && x.HasAccident))
                    {
                        var request = trip.ShippingRequestFk;
                        request.HasAccident = false;
                    }
                    trip.Status = ShippingRequestTripStatus.Canceled;
                }
                await _appNotifier.ShippingRequestTripCancelByAccident(userIdentifiers, trip, GetCurrentUser());
            }
            //await _shippingRequestRepository.DeleteAsync(input.Id);
        }

        #region Heleper
        /// <summary>
        /// Check can delete or edit trip if trip on stand by status
        /// </summary>
        /// <param name="trip"></param>
        private void TripCanEditOrDelete(ShippingRequestTrip trip)
        {
            // When Edit Or Delete
            if (trip.Status != ShippingRequestTripStatus.New)
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
                       .ThenInclude(t => t.Translations)
                .Include(x => x.RoutPoints)
                  .ThenInclude(i => i.GoodsDetails)
                    .ThenInclude(p => p.UnitOfMeasureFk)
                .Include(x => x.RoutPoints)
                    .ThenInclude(i => i.GoodsDetails)
                      .ThenInclude(p => p.DangerousGoodTypeFk)
                .Include(x => x.RoutPoints)
                    .ThenInclude(c => c.ReceiverFk)
                .Include(x => x.ShippingRequestTripVases)
                  .ThenInclude(v => v.ShippingRequestVasFk)
                    .ThenInclude(v => v.VasFk)
                .WhereIf(requestId.HasValue, x => x.ShippingRequestId == requestId)
                 .FirstOrDefaultAsync(x => x.Id == tripid);
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
                          .WhereIf(IsEnabled(AppFeatures.Carrier), x => x.CarrierTenantId == AbpSession.TenantId)
                          .WhereIf(IsEnabled(AppFeatures.Shipper), x => x.TenantId == AbpSession.TenantId)
                          .WhereIf(IsEnabled(AppFeatures.TachyonDealer), x => x.IsTachyonDeal)
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
            await GetShippingRequestByPermission(trip.ShippingRequestId);

            return ObjectMapper.Map<T>(trip);
        }

        private async Task<UserIdentifier> GetAdminTenant(int tenantId)
        {
            return new UserIdentifier(tenantId, (await _userManager.GetAdminByTenantIdAsync(tenantId)).Id);
        }

        private async Task ValidateGoodsCategory(IEnumerable<CreateOrEditRoutPointDto> routPoints, int? shippingRequestGoodCategoryId)
        {

            var goodsCategories = await _goodCategoryRepository.GetAllListAsync();

            // todo Add Localized String Here
            foreach (var goodsDetail in routPoints.Where(x => x.GoodsDetailListDto != null).SelectMany(routPoint => routPoint.GoodsDetailListDto))
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

        private async Task NotifyCarrierWithTripDetails(ShippingRequestTrip trip, int? carrierTenantId, bool hasAttachmentNotification, bool needseliverNoteNotification, bool hasAttachment)
        {
            //Notify carrier when trip has attachment or needs delivery note
            if (trip.ShippingRequestFk.CarrierTenantId != null && trip.HasAttachment && hasAttachmentNotification)
            {
                await _appNotifier.NotifyCarrierWhenTripHasAttachment(trip.Id, carrierTenantId, hasAttachment);
            }
            if (trip.ShippingRequestFk.CarrierTenantId != null && trip.NeedsDeliveryNote && needseliverNoteNotification)
            {
                await _appNotifier.NotifyCarrierWhenTripNeedsDeliverNote(trip.Id, carrierTenantId);
            }
        }

        #endregion
    }
}