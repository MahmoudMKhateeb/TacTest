using Abp;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Abp.Timing;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using TACHYON.Authorization.Users;
using TACHYON.Net.Sms;
using TACHYON.Notifications;
using TACHYON.Routs.RoutPoints;
using TACHYON.Shipping.Drivers.Dto;
using TACHYON.Shipping.ShippingRequests;
using TACHYON.Shipping.ShippingRequestTrips;
using TACHYON.Shipping.Trips;
using TACHYON.Storage;

namespace TACHYON.Shipping.Drivers
{
    public class ShippingRequestDriverAppService : TACHYONAppServiceBase, IShippingRequestDriverAppService
    {

        private readonly IRepository<ShippingRequestTrip> _ShippingRequestTrip;
        //private readonly IRepository<RoutPoint> _ShippingRequestTripPointRepository;
        private readonly IRepository<RoutPoint, long> _RoutPointRepository;
        private readonly IRepository<ShippingRequest, long> _ShippingRequestRepository;
        private readonly ISmsSender _smsSender;
        private readonly IAppNotifier _appNotifier;

        private readonly IBinaryObjectManager _BinaryObjectManager;
        private readonly UserManager _userManager;

        public ShippingRequestDriverAppService(
            IRepository<ShippingRequestTrip> ShippingRequestTrip,
            IRepository<RoutPoint, long> RoutPointRepository,
            IRepository<ShippingRequest, long> ShippingRequestRepository,
            IBinaryObjectManager BinaryObjectManager,
            ISmsSender smsSender,
            IAppNotifier appNotifier,
            UserManager userManager)
        {
            _ShippingRequestTrip = ShippingRequestTrip;
            _ShippingRequestRepository = ShippingRequestRepository;
            _RoutPointRepository = RoutPointRepository;
            _BinaryObjectManager = BinaryObjectManager;
            _smsSender = smsSender;
            _appNotifier = appNotifier;
            _userManager = userManager;


        }
        /// <summary>
        /// list all trips realted with drivers
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PagedResultDto<ShippingRequestTripDriverListDto>> GetAll(ShippingRequestTripDriverFilterInput input)
        {
            var query = _ShippingRequestTrip
                .GetAll()
                .AsNoTracking()
                .Include(i => i.ShippingRequestFk)
                   .ThenInclude(r => r.RouteFk)
                   .ThenInclude(r => r.DestinationCityFk)
               .Include(i => i.ShippingRequestFk)
                   .ThenInclude(r => r.RouteFk)
                   .ThenInclude(r => r.OriginCityFk)
               .Include(i => i.OriginFacilityFk)
               .Include(i => i.DestinationFacilityFk)
               .Include(r => r.RoutPoints)
                   .Where(t => t.AssignedDriverUserId == AbpSession.UserId)
                .WhereIf(input.Status.HasValue, e => e.Status == input.Status)
                .OrderBy(input.Sorting ?? "Status asc")
                .PageBy(input);

            var totalCount = await query.CountAsync();
            return new PagedResultDto<ShippingRequestTripDriverListDto>(
                totalCount,
                ObjectMapper.Map<List<ShippingRequestTripDriverListDto>>(query)

            );

        }

        /// <summary>
        /// Get trip details rleated with drivers
        /// </summary>
        /// <param name="RequestId"></param>
        /// <returns></returns>
        public async Task<ShippingRequestTripDriverDetailsDto> GetDetail(long TripId)
        {
            var query = await _ShippingRequestTrip.GetAll()
                .Include(i => i.ShippingRequestFk)
                   .ThenInclude(r => r.RouteFk)
                   .ThenInclude(r => r.DestinationCityFk)
               .Include(i => i.ShippingRequestFk)
                   .ThenInclude(r => r.RouteFk)
                   .ThenInclude(r => r.OriginCityFk)
               .Include(i => i.DestinationFacilityFk)
               .Include(i => i.OriginFacilityFk)
                .SingleOrDefaultAsync(t => t.Id == TripId && t.AssignedDriverUserId == AbpSession.UserId);

            return ObjectMapper.Map<ShippingRequestTripDriverDetailsDto>(query);
        }

        /// <summary>
        /// When driver click on mobile app to start trip  Journey
        /// </summary>
        /// <param name="TripId"></param>
        /// <returns></returns>
        public async Task<bool> StartTrip(long TripId)
        {
            var trip = await _ShippingRequestTrip
                .FirstOrDefaultAsync(t => t.Id == TripId && t.AssignedDriverUserId == AbpSession.UserId && t.Status != ShippingRequestTripStatus.Finished && t.ShippingRequestFk.StartTripDate <= Clock.Now);
            if (trip != null)
            {
                if (!_ShippingRequestTrip.GetAll().Any(x => x.Id != trip.Id && x.Status != ShippingRequestTripStatus.Finished && x.AssignedDriverUserId == AbpSession.UserId))
                {
                    var RouteStart = await _RoutPointRepository.SingleAsync(x => x.ShippingRequestTripId == trip.Id && x.PickingType== PickingType.Pickup);

                    RouteStart.StartTime = Clock.Now;
                    RouteStart.IsActive = true;
                    trip.Status = ShippingRequestTripStatus.PickupWay;
                    trip.StartTripDate = Clock.Now;

                    return true;
                }


            }
            return false;
        }

        /// <summary>
        /// Change trip status for each points
        /// </summary>
        public async void ChangeTripStatus()
        {
            var trip = await GetActiveTrip();
            var Point = await _RoutPointRepository.FirstOrDefaultAsync(x => x.ShippingRequestTripId == trip.Id && x.IsActive);
            switch (trip.Status)
            {
                case ShippingRequestTripStatus.PickupWay:
                    trip.Status = ShippingRequestTripStatus.StartLoading;
                    break;
                case ShippingRequestTripStatus.Dropoffway:
                    trip.Status = ShippingRequestTripStatus.DropoffArrived;
                    break;
            }

        }

        /// <summary>
        /// Set new active dropoff point for trip
        /// </summary>
        /// <param name="PointId">Dropoff point id</param>

        public async void GotoNextLocation(long PointId)
        {
            var trip = await GetActiveTrip();
            if (!_RoutPointRepository.GetAll().Any(x => (x.IsActive || (x.Id == PointId && x.IsComplete)) && x.ShippingRequestTripId == trip.Id))
            {
                var Newpoint = await _RoutPointRepository.FirstOrDefaultAsync(x => x.Id == PointId);
                if (Newpoint == null) throw new UserFriendlyException(L("the trip is not exists"));
                Newpoint.StartTime = Clock.Now;
                Newpoint.IsActive = true;
                trip.Status = ShippingRequestTripStatus.Dropoffway;

            }
        }
        /// <summary>
        ///  confirm receiving the shipment
        /// </summary>
        /// <param name="Code"></param>
        /// <returns></returns>

        public async Task<bool> ConfirmReceiverCode(string Code)
        {
            var CurrentPoint = await _RoutPointRepository.FirstOrDefaultAsync(x => x.IsActive && x.Code == Code && x.ShippingRequestTripFk.AssignedDriverUserId == AbpSession.UserId);

            return CurrentPoint != null;
        }

        /// <summary>
        /// driver confirm the trip has finished 
        /// </summary>
        /// <param name="Input"></param>
        /// <param name="Code"></param>
        /// <returns></returns>

        public async Task<bool> UploadPointDeliveryDocument(ShippingRequestTripDriverDocumentDto Input, string Code)
        {

            var CurrentPoint = await _RoutPointRepository.FirstOrDefaultAsync(x => x.IsActive && x.Code == Code && x.ShippingRequestTripFk.AssignedDriverUserId == AbpSession.UserId);
            if (CurrentPoint == null) return false;

            if (Input == null || string.IsNullOrEmpty(Input.DocumentBase64)) return false;


            var fileBytes = Convert.FromBase64String(Input.DocumentBase64.Split(',')[1]);
            var fileObject = new BinaryObject(AbpSession.TenantId, fileBytes);
            await _BinaryObjectManager.SaveAsync(fileObject);

            CurrentPoint.EndTime = Clock.Now;
            CurrentPoint.DocumentContentType = Input.DocumentContentType;
            CurrentPoint.DocumentName = Input.DocumentName;
            CurrentPoint.DocumentId = fileObject.Id;
            CurrentPoint.IsActive = false;
            CurrentPoint.IsComplete = true;

            var trip = await GetActiveTrip();
            if (await _RoutPointRepository.GetAll().Where(x => x.ShippingRequestTripId == trip.Id && !x.IsComplete).CountAsync() == 0)
            {
                trip.Status = ShippingRequestTripStatus.Finished;
                trip.EndTripDate = Clock.Now;

                await ChangeShippingRequestStatusIfAllTripsDone(trip.ShippingRequestId);


            }
            else
            {
                trip.Status = ShippingRequestTripStatus.offloading;
            }
            return true;
        }
        /// <summary>
        /// The driver ask receiver to rate the trip
        /// </summary>
        /// <param name="PointId"></param>
        /// <param name="Rate"></param>
        public async void SetRating(long PointId, int Rate)
        {
            var Point = await _RoutPointRepository.FirstOrDefaultAsync(x => x.Id == PointId && x.IsComplete && x.ShippingRequestTripFk.AssignedDriverUserId == AbpSession.UserId && !x.Rating.HasValue);
            if (Point != null) Point.Rating = Rate;
        }


        #region Heleper
        /// <summary>
        /// Get current active trip for driver
        /// </summary>
        /// <returns></returns>
        private async Task<ShippingRequestTrip> GetActiveTrip()
        {
            var trip = await _ShippingRequestTrip
                            .FirstOrDefaultAsync(t => t.AssignedDriverUserId == AbpSession.UserId && (ShippingRequestTripStatus)t.Status != ShippingRequestTripStatus.Finished);
            if (trip == null) throw new UserFriendlyException(L("thetripIsNotFound"));
            return trip;
        }
        /// <summary>
        /// Get current active route point for driver
        /// </summary>
        /// <returns></returns>
        private async Task<RoutPoint> GetActivePoint()
        {
            var ActivePoint = await _RoutPointRepository.FirstOrDefaultAsync(x => x.IsActive && x.ShippingRequestTripFk.AssignedDriverUserId == AbpSession.UserId);
            if (ActivePoint == null) throw new UserFriendlyException(L("thetripIsNotFound"));

            return ActivePoint;
        }

        private async Task ChangeShippingRequestStatusIfAllTripsDone(long RequestId)
        {
            if (!_ShippingRequestTrip.GetAll().Any(x => x.AssignedDriverUserId == AbpSession.UserId && x.Status != ShippingRequestTripStatus.Finished && x.ShippingRequestId == RequestId))
            {
                var Request = await _ShippingRequestRepository.SingleAsync(x => x.Id == RequestId);
                Request.Status = ShippingRequestStatus.Finished;

                await _appNotifier.ShipperShippingRequestFinish(new UserIdentifier(Request.TenantId, _userManager.GetAdminByTenantIdAsync(Request.TenantId).Id), Request);
            }
        }


        #endregion
    }
}
