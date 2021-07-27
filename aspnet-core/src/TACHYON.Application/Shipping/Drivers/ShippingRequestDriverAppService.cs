using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Abp.Timing;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using TACHYON.Features;
using TACHYON.Firebases;
using TACHYON.Goods.GoodCategories.Dtos;
using TACHYON.Mobile;
using TACHYON.Notifications;
using TACHYON.Routs.RoutPoints;
using TACHYON.Routs.RoutPoints.Dtos;
using TACHYON.Shipping.Drivers.Dto;
using TACHYON.Shipping.ShippingRequests;
using TACHYON.Shipping.ShippingRequestTrips;
using TACHYON.Shipping.Trips;
using TACHYON.Trucks.TrucksTypes.Dtos;

namespace TACHYON.Shipping.Drivers
{
    [AbpAuthorize()]
    public class ShippingRequestDriverAppService : TACHYONAppServiceBase, IShippingRequestDriverAppService
    {

        private readonly IRepository<ShippingRequestTrip> _ShippingRequestTrip;
        private readonly IRepository<RoutPoint, long> _RoutPointRepository;
        private readonly IRepository<ShippingRequestTripTransition> _shippingRequestTripTransitionRepository;
        private readonly ShippingRequestDriverManager _shippingRequestDriverManager;
        private readonly ShippingRequestManager _shippingRequestManager;
        private readonly IAppNotifier _appNotifier;
        private readonly IFirebaseNotifier _firebaseNotifier;
        private readonly ShippingRequestsTripManager _shippingRequestsTripManager;
        private readonly IRepository<UserOTP> _userOtpRepository;
        public ShippingRequestDriverAppService(
            IRepository<ShippingRequestTrip> ShippingRequestTrip,
            IRepository<RoutPoint, long> RoutPointRepository,
            IRepository<ShippingRequestTripTransition> shippingRequestTripTransitionRepository,
            ShippingRequestDriverManager shippingRequestDriverManager,
            ShippingRequestManager shippingRequestManager,
            IAppNotifier appNotifier,
            IFirebaseNotifier firebaseNotifier,
            ShippingRequestsTripManager shippingRequestsTripManager,IRepository<UserOTP> userOtpRepository)
        {
            _ShippingRequestTrip = ShippingRequestTrip;
            _RoutPointRepository = RoutPointRepository;
            _shippingRequestTripTransitionRepository = shippingRequestTripTransitionRepository;
            _shippingRequestDriverManager = shippingRequestDriverManager;
            _shippingRequestManager = shippingRequestManager;
            _appNotifier = appNotifier;
            _firebaseNotifier = firebaseNotifier;
            _shippingRequestsTripManager = shippingRequestsTripManager;
            _userOtpRepository = userOtpRepository;
        }
        /// <summary>
        /// list all trips realted with drivers
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PagedResultDto<ShippingRequestTripDriverListDto>> GetAll(ShippingRequestTripDriverFilterInput input)

        {

            DisableTenancyFilters();
            var query = _ShippingRequestTrip
        .GetAll()
        .AsNoTracking()
        .Include(i => i.ShippingRequestFk)
             .ThenInclude(r => r.DestinationCityFk)
       .Include(i => i.ShippingRequestFk)
              .ThenInclude(r => r.OriginCityFk)
       .Include(i => i.OriginFacilityFk)
       .Include(i => i.DestinationFacilityFk)
           .Where(t => t.AssignedDriverUserId == AbpSession.UserId && t.Status != ShippingRequestTripStatus.Canceled && t.DriverStatus != ShippingRequestTripDriverStatus.Rejected)
        .WhereIf(input.Status.HasValue && input.Status == ShippingRequestTripDriverLoadStatusDto.Current, e => e.StartTripDate.Date <= Clock.Now.Date && e.Status != ShippingRequestTripStatus.Delivered )
        .WhereIf(input.Status.HasValue && input.Status == ShippingRequestTripDriverLoadStatusDto.Past, e => e.Status == ShippingRequestTripStatus.Delivered)
        .WhereIf(input.Status.HasValue && input.Status == ShippingRequestTripDriverLoadStatusDto.Comming, e => e.StartTripDate.Date > Clock.Now.Date)
        .OrderBy(input.Sorting ?? "Status asc");

            //.PageBy(input);
            var pageingitem= query.PageBy(input);
            var totalCount = await query.CountAsync();
                        return new PagedResultDto<ShippingRequestTripDriverListDto>(
                            totalCount,
                            ObjectMapper.Map<List<ShippingRequestTripDriverListDto>>(pageingitem)

                        );


        }
        /// <summary>
        /// This api is for mobile deveopment team
        /// </summary>
        /// <param name="tripId"></param>
        /// <returns></returns>
        public async Task<DropOffPointDto> GetReceiverCode(int PointId)
        {
            DisableTenancyFilters();

            var Point = await _RoutPointRepository.GetAll()
                .Include(t => t.ShippingRequestTripFk)
                 .ThenInclude(r => r.ShippingRequestFk)
                    .ThenInclude(p => p.PackingTypeFk)
            .Include(i => i.FacilityFk)
               .ThenInclude(c => c.CityFk)
           .Include(i => i.ReceiverFk)
           .Include(i => i.GoodsDetails)
            .ThenInclude(i => i.UnitOfMeasureFk)
            .SingleOrDefaultAsync(t => t.Id == PointId && t.ShippingRequestTripFk.Status != ShippingRequestTripStatus.Canceled && t.ShippingRequestTripFk.AssignedDriverUserId == AbpSession.UserId && t.ShippingRequestTripFk.DriverStatus != ShippingRequestTripDriverStatus.Rejected);
            if (Point == null) throw new UserFriendlyException(L("TheTripIsNotFound"));
            var DropOff = ObjectMapper.Map<DropOffPointDto>(Point);

            return DropOff;
        }


        public async Task<List<UserOtpDto>> GetUserOtps(long userId)
        {
            var otps=await _userOtpRepository.GetAll().Where(x => x.UserId == userId).ToListAsync();
            return ObjectMapper.Map<List<UserOtpDto>>(otps);
        }
        /// <summary>
        /// Get trip details rleated with drivers
        /// </summary>
        /// <param name="TripId"></param>
        /// <returns></returns>
        public async Task<ShippingRequestTripDriverDetailsDto> GetDetail(long TripId,bool IsAccepted)
        {
            DisableTenancyFilters();
                var trip = await _ShippingRequestTrip.GetAll()
                .Include(i => i.ShippingRequestFk)
                   .ThenInclude(r => r.DestinationCityFk)
               .Include(i => i.ShippingRequestFk)
                   .ThenInclude(r => r.OriginCityFk)
               .Include(i => i.ShippingRequestFk)
                   .ThenInclude(p=>p.PackingTypeFk)
               .Include(i => i.ShippingRequestFk)
                   .ThenInclude(p => p.GoodCategoryFk)
                   .ThenInclude(p => p.Translations)
               .Include(i => i.DestinationFacilityFk)
               .Include(i => i.OriginFacilityFk)
               .Include(i=>i.RoutPoints)
                .ThenInclude(r=>r.FacilityFk)
                 .ThenInclude(r => r.CityFk)
                 .Include(x=>x.AssignedTruckFk)
                  .ThenInclude(t => t.TrucksTypeFk)
                   .ThenInclude(t=>t.Translations)
                 .WhereIf(IsAccepted,t=>t.DriverStatus== ShippingRequestTripDriverStatus.Accepted)
                .SingleOrDefaultAsync(t => t.Id == TripId && t.Status != ShippingRequestTripStatus.Canceled && t.AssignedDriverUserId == AbpSession.UserId);

            
            if (trip==null) throw new UserFriendlyException(L("TheTripIsNotFound"));
            var tripDto = ObjectMapper.Map<ShippingRequestTripDriverDetailsDto>(trip);

            // return current driver trip
            var CurrentTrip = await _ShippingRequestTrip.GetAll()
                    .Where(x => x.AssignedDriverUserId == AbpSession.UserId && x.Status == ShippingRequestTripStatus.Intransit).FirstOrDefaultAsync();

            if (CurrentTrip != null)
            {
                tripDto.CurrentTripId = CurrentTrip.Id;
            }

            if (trip.AssignedTruckFk !=null) tripDto.TruckType = ObjectMapper.Map<TrucksTypeDto>(trip.AssignedTruckFk.TrucksTypeFk).TranslatedDisplayName;
            if (tripDto.TripStatus == ShippingRequestTripStatus.Intransit)
            {
                tripDto.ActionStatus = ShippingRequestTripDriverActionStatusDto.ContinueTrip;
            }


            else if (trip.StartTripDate.Date <= Clock.Now.Date && trip.Status == ShippingRequestTripStatus.New && trip.DriverStatus== ShippingRequestTripDriverStatus.Accepted)
            {
                //Check there any trip the driver still working on or not
                    await _RoutPointRepository.GetAll().Where(x => x.IsActive && x.ShippingRequestTripFk.AssignedDriverUserId == AbpSession.UserId).CountAsync();
                if (CurrentTrip == null)
                    tripDto.ActionStatus = ShippingRequestTripDriverActionStatusDto.CanStartTrip;
            }

            //return good category name automatic from default language
            tripDto.GoodsCategory = ObjectMapper.Map<GoodCategoryDto>(trip.ShippingRequestFk.GoodCategoryFk).DisplayName;
            return tripDto;

        }


        public async Task<RoutDropOffDto> GetDropOffDetail(long PointId)
        {
            DisableTenancyFilters();

            var Point = await _RoutPointRepository.GetAll()
                .Include(t=>t.ShippingRequestTripFk)
                 .ThenInclude(r=>r.ShippingRequestFk)
                    .ThenInclude(p=>p.PackingTypeFk)
            .Include(i => i.FacilityFk)
               .ThenInclude(c => c.CityFk)
           .Include(i => i.ReceiverFk)
           .Include(i => i.GoodsDetails)
            .ThenInclude(i=>i.UnitOfMeasureFk)
            .SingleOrDefaultAsync(t => t.Id == PointId && t.ShippingRequestTripFk.Status != ShippingRequestTripStatus.Canceled && t.ShippingRequestTripFk.AssignedDriverUserId == AbpSession.UserId && t.ShippingRequestTripFk.DriverStatus != ShippingRequestTripDriverStatus.Rejected);
            if (Point == null) throw new UserFriendlyException(L("TheTripIsNotFound"));
            var DropOff = ObjectMapper.Map<RoutDropOffDto>(Point);

            return DropOff;
        }

        /// <summary>
        /// When driver click on mobile app to start trip  Journey
        /// </summary>
        /// <param name="Input"></param>
        /// <returns></returns>
        public async Task StartTrip(ShippingRequestTripDriverStartInputDto Input)
        {
            await _shippingRequestsTripManager.Start(Input);
            //DisableTenancyFilters();
            //var trip = await _ShippingRequestTrip
            //   .FirstOrDefaultAsync(
            //        t => t.Id == Input.Id && 
            //        t.AssignedDriverUserId == AbpSession.UserId && 
            //        t.Status == ShippingRequestTripStatus.New && 
            //        t.ShippingRequestFk.StartTripDate.Value.Date <= Clock.Now.Date &&
            //        t.DriverStatus == ShippingRequestTripDriverStatus.Accepted);

            //if (trip == null) throw new UserFriendlyException(L("YouCannotStartWithTheTripSelected"));

            //if (!_ShippingRequestTrip.GetAll().Any(x => 
            //x.Id != trip.Id &&
            //x.Status == ShippingRequestTripStatus.Intransit && 
            //x.AssignedDriverUserId == AbpSession.UserId
            //))
            //{
            //    var RouteStart = await _RoutPointRepository.GetAll().Include(x => x.FacilityFk).SingleAsync(x => x.ShippingRequestTripId == trip.Id && x.PickingType == PickingType.Pickup);

            //    RouteStart.StartTime = Clock.Now;
            //    RouteStart.IsActive = true;
            //    trip.Status = ShippingRequestTripStatus.Intransit;
            //    trip.RoutePointStatus = RoutePointStatus.StartedMovingToLoadingLocation;

            //    trip.StartTripDate = Clock.Now;
            //    await _shippingRequestDriverManager.StartTransition(RouteStart, new Point(Input.lat, Input.lng));
            //}
            //else
            //{
            //    throw new UserFriendlyException(L("YouCanNotStartNewTripWhenYouHaveAnotherTripStillNotFinish"));
            //}



        }

        /// <summary>
        /// Change trip status for each points
        /// </summary>
        public async Task ChangeTripStatus()
        {
            DisableTenancyFilters();
            var trip = await _shippingRequestDriverManager.GetActiveTrip();
            if (trip.RoutePointStatus == RoutePointStatus.FinishOffLoadShipment) throw new UserFriendlyException(L("TripNotFound"));
            var Point = await _RoutPointRepository.GetAll().Include(t=>t.ShippingRequestTripFk).ThenInclude(r=>r.ShippingRequestFk).FirstOrDefaultAsync(x => x.ShippingRequestTripId == trip.Id && x.IsActive==true );
            switch (trip.RoutePointStatus)
            {
                case RoutePointStatus.StartedMovingToLoadingLocation:
                    trip.RoutePointStatus = RoutePointStatus.ArriveToLoadingLocation;
                    break;
                case RoutePointStatus.ArriveToLoadingLocation:
                    trip.RoutePointStatus = RoutePointStatus.StartLoading;
                    break;
                case RoutePointStatus.StartLoading:
                    trip.RoutePointStatus = RoutePointStatus.FinishLoading;
                    await _shippingRequestManager.SendSmsToReceivers(trip.Id);
                    break;
                case RoutePointStatus.StartedMovingToOfLoadingLocation:
                    trip.RoutePointStatus = RoutePointStatus.ArrivedToDestination;
                    break;
                case RoutePointStatus.ArrivedToDestination:
                    trip.RoutePointStatus = RoutePointStatus.StartOffloading;

                    break;
                case RoutePointStatus.StartOffloading:
                    trip.RoutePointStatus = RoutePointStatus.FinishOffLoadShipment;

                    break;
            }

           await  _shippingRequestDriverManager.SetRoutStatusTransition(Point,trip.RoutePointStatus);
           await _shippingRequestsTripManager.NotificationWhenPointChanged(Point, GetCurrentUser());


        }

        /// <summary>
        /// Set new active dropoff point for trip
        /// </summary>
        /// <param name="PointId">Dropoff point id</param>

        public async Task GotoNextLocation(long PointId)
        {
            DisableTenancyFilters();
            var CurrentPoint = await _shippingRequestDriverManager.GetActivePoint();
            ShippingRequestTrip trip = default;
            if (CurrentPoint !=null)
            {
                 trip = CurrentPoint.ShippingRequestTripFk;

                if (CurrentPoint.PickingType == PickingType.Pickup && trip.RoutePointStatus != RoutePointStatus.FinishLoading) throw new UserFriendlyException(L("TheTripIsNotFound"));

                if (trip.RoutePointStatus == RoutePointStatus.FinishLoading)
                {
                    CurrentPoint.IsActive = false;
                    CurrentPoint.IsComplete = true;
                    CurrentPoint.EndTime = Clock.Now;
                }
            }
            else
            {
                trip = await _shippingRequestDriverManager.GetActiveTrip();
            }

               var Count= await _RoutPointRepository.GetAll()
                .Where(x=> (
                (x.IsActive &&  x.PickingType == PickingType.Dropoff && x.Id != PointId) ||
(x.Id == PointId && ( x.IsComplete || x.IsActive))) &&
                x.ShippingRequestTripId == trip.Id).CountAsync();

            if (Count>0) throw new UserFriendlyException(L("ThereIsAnotherActivePointStillNotClose"));


            var Newpoint = await _RoutPointRepository.GetAll().Include(x=>x.FacilityFk).FirstOrDefaultAsync(x => x.Id == PointId);
                if (Newpoint == null) throw new UserFriendlyException(L("the trip is not exists"));
                Newpoint.StartTime = Clock.Now;
                Newpoint.IsActive = true;
                trip.RoutePointStatus = RoutePointStatus.StartedMovingToOfLoadingLocation;

            await _shippingRequestDriverManager.ChangeTransition(Newpoint, RoutePointStatus.StartedMovingToOfLoadingLocation);
            await _shippingRequestsTripManager.NotificationWhenPointChanged(Newpoint, GetCurrentUser());


        }
        /// <summary>
        ///  confirm receiving the shipment
        /// </summary>
        /// <param name="Code"></param>
        /// <returns></returns>

        public async Task ConfirmReceiverCode(string Code)
        {
            DisableTenancyFilters();
            var CurrentPoint = await _RoutPointRepository.GetAll()
                .Include(t=>t.ShippingRequestTripFk)
                    .ThenInclude(x => x.ShippingRequestTripVases)
                .Include(x => x.ShippingRequestTripFk)
                    .ThenInclude(x => x.ShippingRequestFk)
                     .ThenInclude(x => x.Tenant)
                .FirstOrDefaultAsync(
                                    x => x.IsActive && x.ShippingRequestTripFk.RoutePointStatus == RoutePointStatus.FinishOffLoadShipment &&
                                    x.Code == Code &&
                                    x.ShippingRequestTripFk.AssignedDriverUserId == AbpSession.UserId
                                    );
            if (CurrentPoint == null) throw new UserFriendlyException(L("TheReceiverCodeIsIncorrect"));

            CurrentPoint.ShippingRequestTripFk.RoutePointStatus = RoutePointStatus.ReceiverConfirmed;
           await _shippingRequestDriverManager.SetRoutStatusTransition(CurrentPoint, RoutePointStatus.ReceiverConfirmed);
            await _shippingRequestsTripManager.NotificationWhenPointChanged(CurrentPoint, GetCurrentUser());
        }

        /// <summary>
        /// The driver ask receiver to rate the trip
        /// </summary>
        /// <param name="PointId"></param>
        /// <param name="Rate"></param>
        public async Task SetRating(long PointId, double Rate,string Note)
        {
            DisableTenancyFilters();
            var Point = await _RoutPointRepository.FirstOrDefaultAsync(x => x.Id == PointId && x.ShippingRequestTripFk.Status != ShippingRequestTripStatus.Canceled && x.IsComplete && x.ShippingRequestTripFk.AssignedDriverUserId == AbpSession.UserId && !x.Rating.HasValue);
            if (Point != null) {
                Point.Rating = Rate;
                Point.ReceiverNote = Note;
            } 
        }
        public async Task Accepted(int TripId)
        {
           await _shippingRequestsTripManager.Accepted(TripId);
            //DisableTenancyFilters();
            //var trip = await _shippingRequestDriverManager.GetTripWhenAccepedOrRejectedByDriver(TripId, AbpSession.UserId.Value);
            //await _appNotifier.DriverAcceptTrip(trip, GetCurrentUser().FullName);
            //trip.DriverStatus = ShippingRequestTripDriverStatus.Accepted;
            //await _shippingRequestsTripManager.GeneratePrices(trip);
        }
        //public async Task Rejected(CreateShippingRequestTripDriverRejectDto Input)
        //{
        //    DisableTenancyFilters();
        //    var trip = await _shippingRequestDriverManager.GetTripWhenAccepedOrRejectedByDriver(Input.Id, AbpSession.UserId.Value);
        //    trip.DriverStatus = ShippingRequestTripDriverStatus.Rejected;
        //    trip.RejectReasonId = !Input.ReasoneId.HasValue || Input.ReasoneId == 0 ? default(int?) : Input.ReasoneId.Value;
        //    trip.RejectedReason = Input.Description;
        //    await _appNotifier.DriverRejectTrip(trip,GetCurrentUser().FullName);
        //}
        [AbpAllowAnonymous]
        public async Task Reset(int TripId)
        {
            DisableTenancyFilters();

            var trip = await _ShippingRequestTrip.GetAll().Include(x=>x.ShippingRequestFk).Include(x=>x.RoutPoints).FirstOrDefaultAsync(x => x.Id == TripId);
            if (trip != null)
            {
                trip.Status = ShippingRequestTripStatus.New;
                trip.RoutePointStatus = RoutePointStatus.StandBy;
                trip.DriverStatus = ShippingRequestTripDriverStatus.None;
                trip.RejectedReason = string.Empty;
                trip.RejectReasonId = default(int?);
                trip.RoutPoints.ToList().ForEach(item =>
                {
                    item.IsActive = false;
                    item.IsComplete = false;
                    item.Status = RoutePointStatus.StandBy;
                });
                var request = trip.ShippingRequestFk;
                request.Status = ShippingRequestStatus.PostPrice;

               await  _shippingRequestTripTransitionRepository.DeleteAsync(x => x.ToPoint.ShippingRequestTripId == TripId);
            }

        }


        public async Task ResetTrip(int TripId)
        {
            DisableTenancyFilters();

            var trip = await _ShippingRequestTrip.GetAll()
                .WhereIf(IsEnabled(AppFeatures.Carrier), x=>x.ShippingRequestFk.CarrierTenantId==AbpSession.TenantId)
                .WhereIf(IsEnabled(AppFeatures.TachyonDealer), x=>x.ShippingRequestFk.IsTachyonDeal==true)
                .Include(x => x.ShippingRequestFk)
                .Include(x => x.RoutPoints)
                .FirstOrDefaultAsync(x => x.Id == TripId);
            if (trip != null)
            {
                trip.Status = ShippingRequestTripStatus.New;
                trip.RoutePointStatus = RoutePointStatus.StandBy;
                trip.DriverStatus = ShippingRequestTripDriverStatus.None;
                trip.RejectedReason = string.Empty;
                trip.RejectReasonId = default(int?);
                trip.RoutPoints.ToList().ForEach(item =>
                {
                    item.IsActive = false;
                    item.IsComplete = false;
                    item.Status = RoutePointStatus.StandBy;
                });
                var request = trip.ShippingRequestFk;
                request.Status = ShippingRequestStatus.PostPrice;

                await _shippingRequestTripTransitionRepository.DeleteAsync(x => x.ToPoint.ShippingRequestTripId == TripId);
            }
            else
            {
                throw new UserFriendlyException(L("TripCannotFound"));
            }

        }

        public async Task PushNotification(int id)
        {
            
            await _firebaseNotifier.TripChanged(new Abp.UserIdentifier(AbpSession.TenantId, AbpSession.UserId.Value), id.ToString());
        }


    }
}
