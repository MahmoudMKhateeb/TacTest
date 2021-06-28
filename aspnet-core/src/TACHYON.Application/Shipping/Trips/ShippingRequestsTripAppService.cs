using Abp;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using TACHYON.Authorization;
using TACHYON.Authorization.Users;
using TACHYON.Documents.DocumentFiles;
using TACHYON.Features;
using TACHYON.Firebases;
using TACHYON.Goods.GoodsDetails;
using TACHYON.Notifications;
using TACHYON.Routs.RoutPoints;
using TACHYON.Shipping.ShippingRequests;
using TACHYON.Shipping.ShippingRequestTrips;
using TACHYON.Shipping.Trips.Dto;
using TACHYON.Shipping.Trips.RejectReasons.Dtos;
using TACHYON.ShippingRequestTripVases;
namespace TACHYON.Shipping.Trips
{
    [AbpAuthorize(AppPermissions.Pages_ShippingRequestTrips)]
    public class ShippingRequestsTripAppService : TACHYONAppServiceBase, IShippingRequestsTripAppService
    {
        private readonly IRepository<ShippingRequestTrip> _ShippingRequestTripRepository;
        private readonly IRepository<ShippingRequest, long> _ShippingRequestRepository;
        private readonly IRepository<RoutPoint, long> _RoutPointRepository;
        private readonly IRepository<ShippingRequestTripVas, long> _ShippingRequestTripVasRepository;
        private readonly IRepository<GoodsDetail, long> _GoodsDetailRepository;
        private readonly UserManager _userManager;
        private readonly IAppNotifier _appNotifier;
        private readonly IFirebaseNotifier _firebase;
        private readonly ShippingRequestManager _shippingRequestManager;
        private readonly DocumentFilesAppService _documentFilesAppService;

        public ShippingRequestsTripAppService(
            IRepository<ShippingRequestTrip> ShippingRequestTripRepository,
            IRepository<ShippingRequest, long> ShippingRequestRepository,
            IRepository<RoutPoint, long> RoutPointRepository,
            IRepository<ShippingRequestTripVas, long> ShippingRequestTripVasRepository,
            IRepository<GoodsDetail, long> GoodsDetailRepository,
            UserManager userManager,
            IAppNotifier appNotifier,
            IFirebaseNotifier firebase,
            ShippingRequestManager shippingRequestManager, DocumentFilesAppService documentFilesAppService)
        {
            _ShippingRequestTripRepository = ShippingRequestTripRepository;
            _ShippingRequestRepository = ShippingRequestRepository;
            _RoutPointRepository = RoutPointRepository;
            _ShippingRequestTripVasRepository = ShippingRequestTripVasRepository;
            _GoodsDetailRepository = GoodsDetailRepository;
            _userManager = userManager;
            _appNotifier = appNotifier;
            _firebase = firebase;
            _shippingRequestManager = shippingRequestManager;
            _documentFilesAppService = documentFilesAppService;
        }



        public async Task<PagedResultDto<ShippingRequestsTripListDto>> GetAll(ShippingRequestTripFilterInput Input)
        {
            DisableTenancyFilters();
            var request = await GetShippingRequestByPermission(Input.RequestId);
            var query = _ShippingRequestTripRepository
        .GetAll()
        .AsNoTracking()
            .Include(x => x.OriginFacilityFk)
            .Include(x => x.DestinationFacilityFk)
            .Include(x => x.AssignedTruckFk)
            .Include(x => x.AssignedDriverUserFk)
            .Include(x => x.ShippingRequestTripRejectReason)
                .ThenInclude(t => t.Translations)
        .Where(x => x.ShippingRequestId == request.Id)
        .WhereIf(Input.Status.HasValue, e => e.Status == Input.Status)
        .OrderBy(Input.Sorting ?? "Status asc");


            var ResultPage = await query.PageBy(Input).ToListAsync();
            ResultPage.ForEach(r =>
            {
                if (r.ShippingRequestTripRejectReason != null)
                {
                    var reasone = ObjectMapper.Map<ShippingRequestTripRejectReasonListDto>(r.ShippingRequestTripRejectReason);
                   if (reasone !=null)
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
                ObjectMapper.Map<List<ShippingRequestsTripListDto>>(ResultPage)

            );

        }

        public async Task<ShippingRequestsTripForViewDto> GetShippingRequestTripForView(int id)
        {

            return await GetShippingRequestTripForMapper<ShippingRequestsTripForViewDto>(id);
        }


        public async Task<CreateOrEditShippingRequestTripDto> GetShippingRequestTripForEdit(EntityDto input)
        {
            return await GetShippingRequestTripForMapper<CreateOrEditShippingRequestTripDto>(input.Id);

        }

        public async Task CreateOrEdit(CreateOrEditShippingRequestTripDto input)
        {
            var request = await GetShippingRequestByPermission(input.ShippingRequestId);

            if (
                input.StartTripDate.Date > request.EndTripDate?.Date ||
                input.StartTripDate.Date < request.StartTripDate?.Date ||
                input.EndTripDate.Date > request.EndTripDate?.Date ||
                input.EndTripDate.Date < request.StartTripDate?.Date
                )
            {

                throw new UserFriendlyException(L("The trip date range must between shipping request range date"));

            }

            //    int requestNumberOfDrops = request.RouteTypeId== ShippingRequestRouteType.SingleDrop ? 1: request.NumberOfDrops;

            if (input.RoutPoints.Count(x => x.PickingType == PickingType.Dropoff) != request.NumberOfDrops)
            {
                throw new UserFriendlyException(L("The number of drop points must be" + request.NumberOfDrops));
            }
            if (request.TotalWeight>0)
            {
                var TotalWeight = input.RoutPoints.Sum(x => x.GoodsDetailListDto.Sum(g => g.Weight));
                if (TotalWeight> request.TotalWeight)
                {
                    throw new UserFriendlyException(L("TheTotalWeightOfGoodsDetailsshouldNotBeGreaterThanShippingRequestWeight", request.TotalWeight));
                }
            }
            if (!input.Id.HasValue)
            {
                int requestNumberOfTripsAdd = await _ShippingRequestTripRepository.GetAll().Where(x => x.ShippingRequestId == input.ShippingRequestId).CountAsync() + 1;
                if (requestNumberOfTripsAdd > request.NumberOfTrips) throw new UserFriendlyException(L("The number of trips " + request.NumberOfTrips));
                await Create(input, request);
                request.TotalsTripsAddByShippier += 1;
            }
            else
            {

                await Update(input, request);
            }

        }

        public async Task AssignDriverAndTruckToShippmentByCarrier(AssignDriverAndTruckToShippmentByCarrierInput input)
        {
            DisableTenancyFilters();
            var trip = await _ShippingRequestTripRepository.
                GetAll().
                Include(e => e.ShippingRequestFk)
                .Include(d => d.AssignedDriverUserFk)
                .Where(e => e.Id == input.Id)
                .Where(e => e.ShippingRequestFk.CarrierTenantId == AbpSession.TenantId && e.DriverStatus != ShippingRequestTripDriverStatus.Accepted)
                .FirstOrDefaultAsync();
            if (trip == null) throw new UserFriendlyException(L("NoTripToAssignDriver"));

            long? oldAssignedDriverUserId = trip.AssignedDriverUserId;

            trip.AssignedDriverUserId = input.AssignedDriverUserId;
            trip.AssignedTruckId = input.AssignedTruckId;
            if (trip.DriverStatus != ShippingRequestTripDriverStatus.None)
            {
                trip.DriverStatus = ShippingRequestTripDriverStatus.None;
                trip.RejectedReason = string.Empty;
                trip.RejectReasonId = default(int?);
            }

            if (oldAssignedDriverUserId != trip.AssignedDriverUserId)
            {
                await _appNotifier.NotifyDriverWhenAssignToTrip(trip);
                await _firebase.PushNotificationToDriverWhenAssignTrip(new UserIdentifier(trip.AssignedDriverUserFk.TenantId, trip.AssignedDriverUserId.Value), trip.Id.ToString());
            }
            await _appNotifier.ShipperShippingRequestTripNotifyDriverWhenAssignTrip(new UserIdentifier(AbpSession.TenantId, trip.AssignedDriverUserId.Value), trip);
        }

        [AbpAuthorize(AppPermissions.Pages_ShippingRequestTrips_Create)]
        private async Task Create(CreateOrEditShippingRequestTripDto input, ShippingRequest request)
        {
            var RoutePoint = input.RoutPoints.OrderBy(x => x.PickingType);
            ShippingRequestTrip trip = ObjectMapper.Map<ShippingRequestTrip>(input);


            //insert trip 
            var shippingRequestTripId = await _ShippingRequestTripRepository.InsertAndGetIdAsync(trip);

            // add document file
            var docFileDto = input.CreateOrEditDocumentFileDto;
            if (docFileDto.UpdateDocumentFileInput != null && !docFileDto.UpdateDocumentFileInput.FileToken.IsNullOrEmpty())
            {
                docFileDto.ShippingRequestTripId = shippingRequestTripId;
                docFileDto.Name = docFileDto.Name + "_" + shippingRequestTripId.ToString();
                await _documentFilesAppService.CreateOrEdit(docFileDto);
            }
          
        }

        [AbpAuthorize(AppPermissions.Pages_ShippingRequestTrips_Edit)]
        private async Task Update(CreateOrEditShippingRequestTripDto input, ShippingRequest request)
        {
            var trip = await GetTrip((int)input.Id, input.ShippingRequestId);
            TripCanEditOrDelete(trip);


            foreach (var point in trip.RoutPoints)
            {
                if (!input.RoutPoints.Any(x => x.Id == point.Id))
                {
                    await _RoutPointRepository.DeleteAsync(point);
                }
                foreach (var g in point.GoodsDetails.Where(x => x.Id != 0))
                {
                    if (!input.RoutPoints.Any(x => x.GoodsDetailListDto.Any(d => d.Id == g.Id)))
                    {
                        await _GoodsDetailRepository.DeleteAsync(g);
                    }
                }
            }

            foreach (var vas in trip.ShippingRequestTripVases)
            {
                if (!input.ShippingRequestTripVases.Any(x => x.Id == vas.Id))
                {
                    await _ShippingRequestTripVasRepository.DeleteAsync(vas);
                }
            }

            ObjectMapper.Map(input, trip);
        }

        [AbpAuthorize(AppPermissions.Pages_ShippingRequestTrips_Delete)]
        public async Task Delete(EntityDto input)
        {

            var trip = await _ShippingRequestTripRepository.
                FirstOrDefaultAsync(
                x => x.Id == input.Id &&
                x.Status == ShippingRequestTripStatus.New);



            if (trip != null)
            {
                var Request = await GetShippingRequestByPermission(trip.ShippingRequestId);
                TripCanEditOrDelete(trip);
                Request.TotalsTripsAddByShippier -= 1;
                await _ShippingRequestTripRepository.DeleteAsync(trip);
            }
        }

        [AbpAuthorize(AppPermissions.Pages_ShippingRequestTrips_Acident_Cancel)]
        public async Task CancelByAccident(long id)
        {
            DisableTenancyFilters();
            var trip = await _ShippingRequestTripRepository.GetAll().Include(x => x.ShippingRequestFk)
                     .WhereIf(IsEnabled(AppFeatures.Carrier), x => x.ShippingRequestFk.CarrierTenantId == AbpSession.TenantId && !x.IsApproveCancledByCarrier)
                     .WhereIf(IsEnabled(AppFeatures.Shipper), x => x.ShippingRequestFk.TenantId == AbpSession.TenantId && !x.IsApproveCancledByShipper)
                     .WhereIf(IsEnabled(AppFeatures.TachyonDealer), x => !x.IsApproveCancledByShipper || !x.IsApproveCancledByCarrier)
                .FirstOrDefaultAsync(x => x.Id == id && x.HasAccident);
            if (trip != null)
            {
                List<UserIdentifier> UserIdentifiers = new List<UserIdentifier>();
                if (IsEnabled(AppFeatures.Shipper))
                {
                    trip.IsApproveCancledByShipper = true;
                    UserIdentifiers.Add(await GetAdminTenant((int)trip.ShippingRequestFk.CarrierTenantId));
                }
                else if (IsEnabled(AppFeatures.Carrier))
                {
                    trip.IsApproveCancledByCarrier = true;
                    UserIdentifiers.Add(new UserIdentifier(trip.ShippingRequestFk.TenantId, (long)trip.ShippingRequestFk.CreatorUserId));

                }
                else if (IsEnabled(AppFeatures.TachyonDealer))
                {
                    trip.IsApproveCancledByShipper = true;
                    trip.IsApproveCancledByCarrier = true;
                    UserIdentifiers.Add(await GetAdminTenant((int)trip.ShippingRequestFk.CarrierTenantId));
                    UserIdentifiers.Add(new UserIdentifier(trip.ShippingRequestFk.TenantId, (long)trip.ShippingRequestFk.CreatorUserId));

                }
                if (trip.IsApproveCancledByShipper && trip.IsApproveCancledByCarrier)
                {

                    if (!_ShippingRequestTripRepository.GetAll().Any(x => x.Id != trip.Id && x.HasAccident))
                    {
                        var request = trip.ShippingRequestFk;
                        request.HasAccident = false;
                    }
                    trip.Status = ShippingRequestTripStatus.Canceled;
                }
                await _appNotifier.ShippingRequestTripCancelByAccident(UserIdentifiers, trip, GetCurrentUser());
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
        private async Task<ShippingRequestTrip> GetTrip(int tripid, long? RequestId = null)
        {
            using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MustHaveTenant, AbpDataFilters.MayHaveTenant))
            {
                var trip = await _ShippingRequestTripRepository
                .GetAll()
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
                .Include(x => x.ShippingRequestTripVases)
                  .ThenInclude(v => v.ShippingRequestVasFk)
                    .ThenInclude(v => v.VasFk)
                .WhereIf(RequestId.HasValue, x => x.ShippingRequestId == RequestId)
                 .FirstOrDefaultAsync(x => x.Id == tripid);
                if (trip == null) throw new UserFriendlyException(L("ShippingRequestTripIsNotFound"));
                return trip;
            }

        }
        /// <summary>
        /// Return Request when the user loging as shipper or host or carrier 
        /// </summary>
        /// <param name="ShippingRequestId"></param>
        /// <returns></returns>

        private async Task<ShippingRequest> GetShippingRequestByPermission(long ShippingRequestId)
        {
            var request = await _ShippingRequestRepository.GetAll()
                          .WhereIf(IsEnabled(AppFeatures.Carrier), x => x.CarrierTenantId == AbpSession.TenantId)
                          .WhereIf(IsEnabled(AppFeatures.Shipper), x => x.TenantId == AbpSession.TenantId)
                          .WhereIf(IsEnabled(AppFeatures.TachyonDealer), x => x.IsTachyonDeal)
                .FirstOrDefaultAsync(x => x.Id == ShippingRequestId);
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

        private async Task<UserIdentifier> GetAdminTenant(int TenantId)
        {
            return new UserIdentifier(TenantId, (await _userManager.GetAdminByTenantIdAsync(TenantId)).Id);
        }
        #endregion
    }
}
