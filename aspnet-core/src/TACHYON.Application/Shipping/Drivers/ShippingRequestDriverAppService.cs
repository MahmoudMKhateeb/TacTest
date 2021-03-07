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
using TACHYON.Net.Sms;
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
        private readonly IRepository<ShippingRequestTripPoint> _ShippingRequestTripPointRepository;
        private readonly IRepository<RoutPoint, long> _RoutPointRepository;
        private readonly IRepository<ShippingRequest, long> _ShippingRequestRepository;
        private readonly ISmsSender _smsSender;
        private readonly IBinaryObjectManager _BinaryObjectManager;

        public ShippingRequestDriverAppService(
            IRepository<ShippingRequestTrip> ShippingRequestTrip,
            IRepository<ShippingRequestTripPoint> ShippingRequestTripPointRepository,
            IRepository<RoutPoint, long> RoutPointRepository,
            IRepository<ShippingRequest, long> ShippingRequestRepository,
            IBinaryObjectManager BinaryObjectManager,
            ISmsSender smsSender)
        {
            _ShippingRequestTrip = ShippingRequestTrip;
            _ShippingRequestTripPointRepository = ShippingRequestTripPointRepository;
            _ShippingRequestRepository = ShippingRequestRepository;
            _RoutPointRepository = RoutPointRepository;
            _BinaryObjectManager = BinaryObjectManager;
            _smsSender = smsSender;


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
               .Include(i => i.ShippingRequestFk)
                   .ThenInclude(r => r.RouteFk)
                   .ThenInclude(r => r.DestinationFacilityFk)
               .Include(i => i.ShippingRequestFk)
                   .ThenInclude(r => r.RouteFk)
                   .ThenInclude(r => r.OriginFacilityFk)
                   .Where(t => t.AssignedDriverUserId == AbpSession.UserId)
                .WhereIf(input.Status.HasValue, e => (ShippingRequestTripStatus)e.TripStatusId == input.Status)
                .OrderBy(input.Sorting ?? "TripStatusId asc")
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
        public async Task<ShippingRequestTripDriverDetailsDto> GetDetail(long RequestId)
        {
            var query = await _ShippingRequestRepository.GetAll().AsNoTracking()
                .Include(rt => rt.RoutPoints)
                .SingleAsync(t => t.Id == RequestId && t.AssignedDriverUserId == AbpSession.UserId);
            if (query != null)
            {

            }
            return null;
        }

        /// <summary>
        /// When driver click on mobile app to start trip  Journey
        /// </summary>
        /// <param name="TripId"></param>
        /// <returns></returns>
        public async Task<bool> StartTrip(long TripId)
        {
            var trip = await _ShippingRequestTrip
                .SingleAsync(t => t.Id == TripId && t.AssignedDriverUserId == AbpSession.UserId && (ShippingRequestTripStatus)t.TripStatusId != ShippingRequestTripStatus.Finished && t.ShippingRequestFk.StartTripDate <= Clock.Now);
            if (trip != null)
            {
                if (!_ShippingRequestTrip.GetAll().Any(x => x.Id != trip.Id && (ShippingRequestTripStatus)x.TripStatusId != ShippingRequestTripStatus.Finished && x.AssignedDriverUserId == AbpSession.UserId))
                {
                    foreach (var point in _RoutPointRepository.GetAll().Where(p => p.ShippingRequestId == trip.ShippingRequestId))
                    {
                        var trippoint = new ShippingRequestTripPoint();
                        trippoint.PointId = point.Id;
                        trippoint.TripId = trip.Id;
                        trip.Code = trip.Code + point.Code;
                        if (point.ParentId.HasValue)
                        {
                            trippoint.StartTime = Clock.Now;
                            trippoint.IsActive = true;
                        }
                        await _ShippingRequestTripPointRepository.InsertAsync(trippoint);
                    }
                    trip.TripStatusId = (int)ShippingRequestTripStatus.PickupWay;
                    trip.StartTripDate = Clock.Now;

                    return true ;
                }


            }
            return false;
        }

        /// <summary>
        /// Change trip status for each points
        /// </summary>
        public async void ChangeTripStatus()
        {
            var trip =await GetActiveTrip();
            var Point = await _ShippingRequestTripPointRepository.SingleAsync(x => x.TripId == trip.Id && x.IsActive);
           switch ((ShippingRequestTripStatus)trip.TripStatusId)
            {
                case ShippingRequestTripStatus.PickupWay:
                    trip.TripStatusId = (int)ShippingRequestTripStatus.StartLoading;
                    break;
                case ShippingRequestTripStatus.Dropoffway:
                    trip.TripStatusId = (int)ShippingRequestTripStatus.DropoffArrived;
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
            if (!_ShippingRequestTripPointRepository.GetAll().Any(x => (x.IsActive || (x.PointId== PointId && x.IsComplete)) && x.TripId == trip.Id))
            {
                var Newpoint = await _ShippingRequestTripPointRepository.SingleAsync(x => x.PointId == PointId);
                if (Newpoint==null) throw new UserFriendlyException(L("the trip is not exists"));
                Newpoint.StartTime = Clock.Now;
                Newpoint.IsActive = true;
                trip.TripStatusId = (int)ShippingRequestTripStatus.Dropoffway;

            }
        }
        /// <summary>
        ///  confirm receiving the shipment
        /// </summary>
        /// <param name="Code"></param>
        /// <returns></returns>

        public async Task<bool> ConfirmReceiverCode(string Code)
        {
            var CurrentPoint = await _ShippingRequestTripPointRepository.SingleAsync(x => x.IsActive && x.Code == Code && x.Trip.AssignedDriverUserId== AbpSession.UserId);

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

            var CurrentPoint = await _ShippingRequestTripPointRepository.SingleAsync(x => x.IsActive && x.Code == Code && x.Trip.AssignedDriverUserId == AbpSession.UserId);
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
           if (await _ShippingRequestTripPointRepository.GetAll().Where(x => x.TripId == trip.Id && !x.IsComplete).CountAsync()==0)
            {
                trip.TripStatusId = (int)ShippingRequestTripStatus.Finished;
                trip.EndTripDate = Clock.Now;

                await ChangeShippingRequestStatusIfAllTripsDone(trip.ShippingRequestId);


            }
           else
            {
                trip.TripStatusId = (int)ShippingRequestTripStatus.offloading;
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
            var Point = await _ShippingRequestTripPointRepository.SingleAsync(x => x.PointId== PointId && x.IsComplete && x.Trip.AssignedDriverUserId== AbpSession.UserId && !x.Rating.HasValue);
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
                            .SingleAsync(t => t.AssignedDriverUserId == AbpSession.UserId && (ShippingRequestTripStatus)t.TripStatusId != ShippingRequestTripStatus.Finished);    
            if (trip==null) throw new UserFriendlyException(L("thetripIsNotFound"));
            return trip;
        }
        /// <summary>
        /// Get current active route point for driver
        /// </summary>
        /// <returns></returns>
        private async Task<ShippingRequestTripPoint> GetActivePoint()
        {
            var ActivePoint= await _ShippingRequestTripPointRepository.SingleAsync(x => x.IsActive && x.Trip.AssignedDriverUserId== AbpSession.UserId);
            if (ActivePoint == null) throw new UserFriendlyException(L("thetripIsNotFound"));

            return ActivePoint;
        }

        private async Task ChangeShippingRequestStatusIfAllTripsDone(long RequestId)
        {
            if (!_ShippingRequestTrip.GetAll().Any(x => x.AssignedDriverUserId == AbpSession.UserId && x.TripStatusId != (int)ShippingRequestTripStatus.Finished && x.ShippingRequestId == RequestId))
            {
                var Request = await _ShippingRequestRepository.SingleAsync(x=>x.Id== RequestId);
                Request.ShippingRequestStatusId =(int) ShippingRequestStatus.Finished;
            }
        }
        #endregion
    }
}
