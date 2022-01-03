using Abp;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.EntityHistory;
using Abp.Linq.Extensions;
using Abp.Timing;
using Abp.UI;
using AutoMapper.QueryableExtensions;
using DevExtreme.AspNet.Data.ResponseModel;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using TACHYON.DriverLocationLogs;
using TACHYON.DriverLocationLogs.dtos;
using TACHYON.EntityLogs;
using TACHYON.Features;
using TACHYON.Firebases;
using TACHYON.Goods.GoodCategories.Dtos;
using TACHYON.Mobile;
using TACHYON.Notifications;
using TACHYON.Rating;
using TACHYON.Rating.dtos;
using TACHYON.Routs.RoutPoints;
using TACHYON.Routs.RoutPoints.Dtos;
using TACHYON.Routs.RoutPoints.RoutPointSmartEnum;
using TACHYON.Shipping.Accidents.Dto;
using TACHYON.Shipping.Drivers.Dto;
using TACHYON.Shipping.ShippingRequests;
using TACHYON.Shipping.ShippingRequestTrips;
using TACHYON.Shipping.Trips;
using TACHYON.Shipping.Trips.Accidents.Dto;
using TACHYON.Trucks.TrucksTypes.Dtos;

namespace TACHYON.Shipping.Drivers
{
    [AbpAuthorize()]
    public class ShippingRequestDriverAppService : TACHYONAppServiceBase, IShippingRequestDriverAppService
    {

        private readonly IRepository<ShippingRequestTrip> _ShippingRequestTrip;
        private readonly IRepository<RoutPoint, long> _RoutPointRepository;
        private readonly IRepository<ShippingRequestTripTransition> _shippingRequestTripTransitionRepository;
        private readonly IRepository<ShippingRequestTripAccident> _shippingRequestTripAccidentRepository;
        private readonly ShippingRequestDriverManager _shippingRequestDriverManager;
        private readonly ShippingRequestManager _shippingRequestManager;
        private readonly IAppNotifier _appNotifier;
        private readonly ShippingRequestsTripManager _shippingRequestsTripManager;
        private readonly IRepository<UserOTP> _userOtpRepository;
        private readonly RatingLogManager _ratingLogManager;
        private readonly IRepository<DriverLocationLog, long> _driverLocationLogRepository;
        private readonly EntityLogManager _logManager;
        public ShippingRequestDriverAppService(
            IRepository<ShippingRequestTrip> ShippingRequestTrip,
            IRepository<RoutPoint, long> RoutPointRepository,
            IRepository<ShippingRequestTripTransition> shippingRequestTripTransitionRepository,
            ShippingRequestDriverManager shippingRequestDriverManager,
            ShippingRequestManager shippingRequestManager,
            IAppNotifier appNotifier,
            ShippingRequestsTripManager shippingRequestsTripManager,
            IRepository<UserOTP> userOtpRepository,
            IRepository<ShippingRequestTripAccident> shippingRequestTripAccidentRepository,
            RatingLogManager ratingLogManager,
            IRepository<DriverLocationLog, long> driverLocationLogRepository)
        {
            _ShippingRequestTrip = ShippingRequestTrip;
            _RoutPointRepository = RoutPointRepository;
            _shippingRequestTripTransitionRepository = shippingRequestTripTransitionRepository;
            _shippingRequestDriverManager = shippingRequestDriverManager;
            _shippingRequestManager = shippingRequestManager;
            _appNotifier = appNotifier;
            _shippingRequestsTripManager = shippingRequestsTripManager;
            _userOtpRepository = userOtpRepository;
            _shippingRequestTripAccidentRepository = shippingRequestTripAccidentRepository;
            _ratingLogManager = ratingLogManager;
            _driverLocationLogRepository = driverLocationLogRepository;
            _logManager = logManager;
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
        .WhereIf(input.Status.HasValue && input.Status == ShippingRequestTripDriverLoadStatusDto.Current, e => e.StartTripDate.Date <= Clock.Now.Date && e.Status != ShippingRequestTripStatus.Delivered && e.Status != ShippingRequestTripStatus.DeliveredAndNeedsConfirmation)
        .WhereIf(input.Status.HasValue && input.Status == ShippingRequestTripDriverLoadStatusDto.Past, e => (e.Status == ShippingRequestTripStatus.Delivered || e.Status == ShippingRequestTripStatus.DeliveredAndNeedsConfirmation))
        .WhereIf(input.Status.HasValue && input.Status == ShippingRequestTripDriverLoadStatusDto.Comming, e => e.StartTripDate.Date > Clock.Now.Date)
        .OrderBy(input.Sorting ?? "Status asc");

            //.PageBy(input);
            var pageingitem = query.PageBy(input);
            var result = ObjectMapper.Map<List<ShippingRequestTripDriverListDto>>(pageingitem.ToList());

            var incidents = _shippingRequestTripAccidentRepository.GetAll().Where(x => result.Select(y => y.Id).Contains(x.RoutPointFK.ShippingRequestTripId)).ToList();

            foreach (var item in result)
            {
                item.HasIncident = await HasIncident(item.Id);
            }

            var totalCount = await query.CountAsync();
            return new PagedResultDto<ShippingRequestTripDriverListDto>(
                totalCount, result


            );


        }

        private async Task<bool> HasIncident(long id)
        {
            return await _shippingRequestTripAccidentRepository.CountAsync(x => x.RoutPointFK.ShippingRequestTripId == id) > 0;
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
            var otps = await _userOtpRepository.GetAll().Where(x => x.UserId == userId).ToListAsync();
            return ObjectMapper.Map<List<UserOtpDto>>(otps);
        }
        /// <summary>
        /// Get trip details rleated with drivers
        /// </summary>
        /// <param name="TripId"></param>
        /// <returns></returns>
        public async Task<ShippingRequestTripDriverDetailsDto> GetDetail(int TripId, bool IsAccepted)
        {
            DisableTenancyFilters();
            var trip = await _ShippingRequestTrip.GetAll()
            .Include(i => i.ShippingRequestFk)
               .ThenInclude(r => r.DestinationCityFk)
           .Include(i => i.ShippingRequestFk)
               .ThenInclude(r => r.OriginCityFk)
           .Include(i => i.ShippingRequestFk)
               .ThenInclude(p => p.PackingTypeFk)
           .Include(i => i.ShippingRequestFk)
               .ThenInclude(p => p.GoodCategoryFk)
               .ThenInclude(p => p.Translations)
            .Include(i => i.ShippingRequestFk)
               .ThenInclude(p => p.Tenant)
           .Include(i => i.DestinationFacilityFk)
           .Include(i => i.OriginFacilityFk)
           .Include(i => i.RoutPoints)
            .ThenInclude(r => r.FacilityFk)
             .ThenInclude(r => r.CityFk)
             .Include(x => x.AssignedTruckFk)
              .ThenInclude(t => t.TrucksTypeFk)
               .ThenInclude(t => t.Translations)
             .WhereIf(IsAccepted, t => t.DriverStatus == ShippingRequestTripDriverStatus.Accepted)
            .SingleOrDefaultAsync(t => t.Id == TripId && t.Status != ShippingRequestTripStatus.Canceled && t.AssignedDriverUserId == AbpSession.UserId);


            if (trip == null) throw new UserFriendlyException(L("TheTripIsNotFound"));
            var tripDto = ObjectMapper.Map<ShippingRequestTripDriverDetailsDto>(trip);

            // return current driver trip
            var CurrentTrip = await _ShippingRequestTrip.GetAll()
                    .Where(x => x.AssignedDriverUserId == AbpSession.UserId && x.Status == ShippingRequestTripStatus.Intransit).FirstOrDefaultAsync();

            if (CurrentTrip != null)
            {
                tripDto.CurrentTripId = CurrentTrip.Id;
            }

            if (trip.AssignedTruckFk != null) tripDto.TruckType = ObjectMapper.Map<TrucksTypeDto>(trip.AssignedTruckFk.TrucksTypeFk).TranslatedDisplayName;
            if (tripDto.TripStatus == ShippingRequestTripStatus.Intransit)
            {
                tripDto.ActionStatus = ShippingRequestTripDriverActionStatusDto.ContinueTrip;
            }


            else if (trip.StartTripDate.Date <= Clock.Now.Date && trip.Status == ShippingRequestTripStatus.New && trip.DriverStatus == ShippingRequestTripDriverStatus.Accepted)
            {
                //Check there any trip the driver still working on or not
                await _RoutPointRepository.GetAll().Where(x => x.IsActive && x.ShippingRequestTripFk.AssignedDriverUserId == AbpSession.UserId).CountAsync();
                if (CurrentTrip == null)
                    tripDto.ActionStatus = ShippingRequestTripDriverActionStatusDto.CanStartTrip;
            }

            var rate = new RatingLog();


            tripDto.IsShippingExpRated = await _ratingLogManager.IsRateDoneBefore(new RatingLog
            {
                DriverId = AbpSession.UserId,
                TripId = TripId,
                RateType = RateType.SEByDriver
            });

            foreach (var point in tripDto.RoutePoints)
            {
                point.IsFacilityRated = await _ratingLogManager.IsRateDoneBefore(new RatingLog
                {
                    DriverId = AbpSession.UserId,
                    PointId = point.Id,
                    RateType = RateType.SEByDriver,
                    FacilityId = point.FacilityId
                });
            }


            //fill incidents
            var query = _shippingRequestTripAccidentRepository
              .GetAll()
              .AsNoTracking()
              .Include(t => t.RoutPointFK)
               .ThenInclude(f => f.FacilityFk)
                .ThenInclude(c => c.CityFk)
              .Include(r => r.ResoneFK)
               .ThenInclude(t => t.Translations)
                      .Where(x => x.RoutPointFK.ShippingRequestTripId == TripId)
                      .WhereIf(AbpSession.TenantId.HasValue && IsEnabled(AppFeatures.Carrier), x => x.RoutPointFK.ShippingRequestTripFk.ShippingRequestFk.CarrierTenantId == AbpSession.TenantId)
                      .WhereIf(AbpSession.TenantId.HasValue && IsEnabled(AppFeatures.Shipper), x => x.RoutPointFK.ShippingRequestTripFk.ShippingRequestFk.TenantId == AbpSession.TenantId)
                      .WhereIf(!AbpSession.TenantId.HasValue || IsEnabled(AppFeatures.TachyonDealer), x => true)
                      .WhereIf(GetCurrentUser().IsDriver, x => x.RoutPointFK.ShippingRequestTripFk.AssignedDriverUserId == AbpSession.UserId).ToList();

            query.ForEach(q =>
            {
                if (q.ResoneFK != null && !q.ResoneFK.Key.Contains(TACHYONConsts.OthersDisplayName))
                {
                    //var reasone = await _shippingRequestReasonAccidentRepository.FirstOrDefaultAsync(x=>x.Language== CurrentLanguage || x.Language== TACHYONConsts.DefaultLanguage);
                    var reasone = ObjectMapper.Map<ShippingRequestReasonAccidentListDto>(q.ResoneFK);
                    //q.Description = reasone.Name;
                    q.OtherReasonName = reasone.Name;
                }

            });

            tripDto.ShippingRequestTripAccidentList = ObjectMapper.Map<List<ShippingRequestTripAccidentListDto>>(query);

            //tripDto.RoutePoints.ToList().ForEach(async x =>
            //x.IsFacilityRated =( await _ratingLogManager.IsRateDoneBefore(new RatingLog
            //{
            //    DriverId = AbpSession.UserId,
            //    PointId = x.Id,
            //    RateType = RateType.SEByDriver
            //})));

            //return good category name automatic from default language
            tripDto.GoodsCategory = ObjectMapper.Map<GoodCategoryDto>(trip.ShippingRequestFk.GoodCategoryFk).DisplayName;

            return tripDto;

        }


        public async Task<RoutDropOffDto> GetDropOffDetail(long PointId)
        {
            DisableTenancyFilters();

            var Point = await _RoutPointRepository.GetAll()
                .Include(t => t.ShippingRequestTripFk)
                 .ThenInclude(r => r.ShippingRequestFk)
                    .ThenInclude(p => p.PackingTypeFk)
                .Include(t => t.RoutPointDocuments)
            .Include(i => i.FacilityFk)
               .ThenInclude(c => c.CityFk)
           .Include(i => i.ReceiverFk)
           .Include(i => i.GoodsDetails)
            .ThenInclude(i => i.UnitOfMeasureFk)
            .SingleOrDefaultAsync(t => t.Id == PointId && t.ShippingRequestTripFk.Status != ShippingRequestTripStatus.Canceled && t.ShippingRequestTripFk.AssignedDriverUserId == AbpSession.UserId && t.ShippingRequestTripFk.DriverStatus != ShippingRequestTripDriverStatus.Rejected);
            if (Point == null) throw new UserFriendlyException(L("TheTripIsNotFound"));
            var DropOff = ObjectMapper.Map<RoutDropOffDto>(Point);

            DropOff.IsFacilityRated = await _ratingLogManager.IsRateDoneBefore(new RatingLog
            {
                DriverId = AbpSession.UserId,
                PointId = PointId,
                RateType = RateType.FacilityByDriver,
                FacilityId = Point.FacilityId
            });
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
        }

        /// <summary>
        /// Change trip status for each points
        /// </summary>
        public async Task ChangeTripStatus()
        {
            DisableTenancyFilters();
            var trip = await _shippingRequestDriverManager.GetActiveTrip();
            if (trip.RoutePointStatus == RoutePointStatus.FinishOffLoadShipment) throw new UserFriendlyException(L("TripNotFound"));
            var Point = await _RoutPointRepository.GetAll().Include(t => t.ShippingRequestTripFk).ThenInclude(r => r.ShippingRequestFk).FirstOrDefaultAsync(x => x.ShippingRequestTripId == trip.Id && x.IsActive == true);
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
                    if (Point.PickingType == PickingType.Pickup)
                    {
                        Point.ActualPickupOrDeliveryDate = trip.ActualPickupDate = Clock.Now;
                    }
                    break;
                case RoutePointStatus.StartedMovingToOfLoadingLocation:
                    trip.RoutePointStatus = RoutePointStatus.ArrivedToDestination;
                    break;
                case RoutePointStatus.ArrivedToDestination:
                    trip.RoutePointStatus = RoutePointStatus.StartOffloading;

                    break;
                case RoutePointStatus.StartOffloading:
                    trip.RoutePointStatus = RoutePointStatus.FinishOffLoadShipment;
                    if (Point.PickingType == PickingType.Dropoff)
                    {
                        Point.ActualPickupOrDeliveryDate = Clock.Now;
                    }

                    //if all points delivered, actual delivery date of last point will be stored in trip
                    if (!trip.RoutPoints.Any(x => x.ActualPickupOrDeliveryDate == null && x.Id != Point.Id))
                    {
                        trip.ActualDeliveryDate = Clock.Now;
                    }

                    //update completed status
                    Point.CompletedStatus = RoutePointCompletedStatus.CompletedAndMissingReceiverCode;
                    //check if all trip points completed, change the trip status from intransit to delivered and needs confirmation
                    if (trip.RoutPoints.Where(x => x.Id != Point.Id).All(x => x.CompletedStatus > RoutePointCompletedStatus.NotCompleted))
                    {
                        Point.IsActive = false;
                        trip.Status = ShippingRequestTripStatus.DeliveredAndNeedsConfirmation;
                    }

                    break;
            }

            await _shippingRequestDriverManager.SetRoutStatusTransition(Point, trip.RoutePointStatus);
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
            if (CurrentPoint != null)
            {
                trip = CurrentPoint.ShippingRequestTripFk;

                if (CurrentPoint.PickingType == PickingType.Pickup && trip.RoutePointStatus != RoutePointStatus.FinishLoading) throw new UserFriendlyException(L("TheTripIsNotFound"));

                if (trip.RoutePointStatus == RoutePointStatus.FinishLoading || trip.RoutePointStatus == RoutePointStatus.DeliveryConfirmation)
                {
                    CurrentPoint.IsActive = false;
                    CurrentPoint.IsComplete = true;
                    CurrentPoint.EndTime = Clock.Now;
                    CurrentPoint.CompletedStatus = RoutePointCompletedStatus.Completed;

                }

                if (CurrentPoint.PickingType == PickingType.Dropoff &&
                    (trip.RoutePointStatus == RoutePointStatus.FinishOffLoadShipment ||
                    trip.RoutePointStatus == RoutePointStatus.ReceiverConfirmed))
                {
                    //can go to next point if not confirming receiverd code nor uploading POD
                    CurrentPoint.IsActive = false;
                }

                await CurrentUnitOfWork.SaveChangesAsync();
            }
            else
            {
                // when we don't have current active point in same point trip??
                trip = await _shippingRequestDriverManager.GetActiveTrip();
            }

            var Count = await _RoutPointRepository.GetAll()
             .Where(x => (
             (x.IsActive && x.PickingType == PickingType.Dropoff && x.Id != PointId) ||
             //check if the selected point is completed or active, wether it pickup or drop
             (x.Id == PointId && (x.IsComplete || x.IsActive))) &&
             x.ShippingRequestTripId == trip.Id).CountAsync();

            if (Count > 0) throw new UserFriendlyException(L("ThereIsAnotherActivePointStillNotClose"));


            var Newpoint = await _RoutPointRepository.GetAll().Include(x => x.FacilityFk)
                .Include(x => x.ShippingRequestTripFk)
                .ThenInclude(x => x.ShippingRequestFk)
                .FirstOrDefaultAsync(x => x.Id == PointId);
            if (Newpoint == null) throw new UserFriendlyException(L("the point is not exists"));
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

        public async Task ConfirmReceiverCode(string Code, long? PointId)
        {
            DisableTenancyFilters();
            //get point that send, if null get active point
            var CurrentPoint = await _RoutPointRepository.GetAll()
                .Include(t => t.ShippingRequestTripFk)
                    .ThenInclude(x => x.ShippingRequestTripVases)
                .Include(x => x.ShippingRequestTripFk)
                    .ThenInclude(x => x.ShippingRequestFk)
                     .ThenInclude(x => x.Tenant)
                .WhereIf(PointId == null, x => x.IsActive)
                .WhereIf(PointId != null, x => x.Id == PointId)
                .FirstOrDefaultAsync(
                                    x => x.Status == RoutePointStatus.FinishOffLoadShipment &&
                                    x.Code == Code &&
                                    x.ShippingRequestTripFk.AssignedDriverUserId == AbpSession.UserId
                                    );
            if (CurrentPoint == null) throw new UserFriendlyException(L("TheReceiverCodeIsIncorrect"));

            CurrentPoint.ShippingRequestTripFk.RoutePointStatus = RoutePointStatus.ReceiverConfirmed;
            CurrentPoint.CompletedStatus = RoutePointCompletedStatus.CompletedAndMissingPOD;

            await _shippingRequestDriverManager.SetRoutStatusTransition(CurrentPoint, RoutePointStatus.ReceiverConfirmed);
            await _shippingRequestsTripManager.NotificationWhenPointChanged(CurrentPoint, GetCurrentUser());
        }

        /// <summary>
        /// The driver rate facility after drop finished
        /// </summary>
        /// <param name="input"></param>
        public async Task SetRating(long PointId, int Rate, string Note)
        {
            //DisableTenancyFilters();
            //var Point = await _RoutPointRepository.FirstOrDefaultAsync(x => x.Id == PointId && x.ShippingRequestTripFk.Status != ShippingRequestTripStatus.Canceled && x.IsComplete && x.ShippingRequestTripFk.AssignedDriverUserId == AbpSession.UserId && !x.Rating.HasValue);
            //if (Point != null) {
            //    Point.Rating = Rate;
            //    Point.ReceiverNote = Note;
            //} 
            var input = new CreateFacilityRateByDriverDto();
            input.PointId = PointId;
            input.Rate = Rate;
            input.Note = Note;
            await _ratingLogManager.ValidateAndCreateRating(input, RateType.FacilityByDriver);
        }

        /// <summary>
        /// The driver rate shipping Experience after trip delivered
        /// </summary>
        /// <param name="input"></param>
        public async Task SetShippingExpRating(int tripId, int rate, string note)
        {
            var input = new CreateShippingExpRateByDriverDto();
            input.TripId = tripId;
            input.Rate = rate;
            input.Note = note;
            await _ratingLogManager.ValidateAndCreateRating(input, RateType.SEByDriver);
        }




        public async Task Accepted(int TripId)
        {
            await _shippingRequestsTripManager.Accepted(TripId);
        }

        #region Location tracking
        public async Task NotifyCarrierWithOffGps()
        {
            var user = GetCurrentUser();
            await _shippingRequestDriverManager.NotifyCarrierWithDriverGpsOff(user);
            Logger.Info("GPS for driver name:" + user.Name + " is off");
        }

        public async Task CreateDriverLocationLog(CreateDriverLocationLogInput input)
        {
            var point = new Point(input.Longitude, input.Latitude)
            {
                SRID = 4326
            };

            var output = ObjectMapper.Map<DriverLocationLog>(input);
            output.Location = point;

            await _driverLocationLogRepository.InsertAsync(output);
        }


        public async Task<LoadResult> GetAllDriverLocationLogs(GetAllDriverLocationLogsInput input)
        {

            DisableTenancyFiltersIfHost();

            var filteredLocations = _driverLocationLogRepository.GetAll()
                .Where(x => x.CreatorUserId == input.DriverId)
                .WhereIf(input.TripId.HasValue, x => x.TripId == input.TripId)
                .ProjectTo<DriverLocationLogDto>(AutoMapperConfigurationProvider);

            var result = await LoadResultAsync(filteredLocations, input.Filter);

            return result;
        }

        #endregion

        /// <summary>
        /// This service id for developer
        /// </summary>
        /// <param name="TripId"></param>
        /// <returns></returns>
        [AbpAllowAnonymous]
        public async Task Reset(int TripId)
        {
            DisableTenancyFilters();

            var trip = await _ShippingRequestTrip.GetAll()
                .Include(x => x.ShippingRequestFk)
                .Include(x => x.RoutPoints)
                .ThenInclude(x => x.RoutPointDocuments)
                .Include(x => x.RatingLogs)
                .Include(x => x.RoutPoints)
                .ThenInclude(x => x.RatingLogs)
                .FirstOrDefaultAsync(x => x.Id == TripId);
            await ResetTripStatus(trip);

        }


        /// <summary>
        /// Carrier or tachyon dealer can reset trip to go back to New
        /// </summary>
        /// <param name="TripId"></param>
        /// <returns></returns>
        public async Task ResetTrip(int TripId)
        {
            if (!IsEnabled(AppFeatures.Carrier) && !IsEnabled(AppFeatures.TachyonDealer))
            {
                throw new UserFriendlyException(L("YouDonnotHavePermission"));
            }
            DisableTenancyFilters();

            var trip = await _ShippingRequestTrip.GetAll()
                .WhereIf(IsEnabled(AppFeatures.Carrier), x => x.ShippingRequestFk.CarrierTenantId == AbpSession.TenantId)
                .WhereIf(IsEnabled(AppFeatures.TachyonDealer), x => x.ShippingRequestFk.IsTachyonDeal == true)
                .Include(x => x.ShippingRequestFk)
                .Include(x => x.RoutPoints)
                .ThenInclude(x => x.RoutPointDocuments)
                .Include(x => x.RoutPoints)
                .ThenInclude(x => x.RatingLogs)
                .Include(x => x.RatingLogs)
                .FirstOrDefaultAsync(x => x.Id == TripId);

            await ResetTripStatus(trip);
        }

        private async Task ResetTripStatus(ShippingRequestTrip trip)
        {
            if (trip != null)
            {
                trip.Status = ShippingRequestTripStatus.New;
                trip.RoutePointStatus = RoutePointStatus.StandBy;
                trip.DriverStatus = ShippingRequestTripDriverStatus.None;
                trip.RejectedReason = string.Empty;
                trip.RejectReasonId = default(int?);
                trip.ActualDeliveryDate = trip.ActualPickupDate = null;
                // trip.RatingLogs.Where(x => x.RateType != RateType.CarrierTripBySystem && x.RateType != RateType.ShipperTripBySystem).ToList().Clear();
                trip.RoutPoints.ToList().ForEach(item =>
                {
                    item.IsActive = false;
                    item.IsComplete = false;
                    item.Status = RoutePointStatus.StandBy;
                    item.IsDeliveryNoteUploaded = false;
                    item.RoutPointDocuments.Clear();
                    item.ActualPickupOrDeliveryDate = null;
                    item.CompletedStatus = RoutePointCompletedStatus.NotCompleted;
                    item.StartTime = item.EndTime = null;
                    //item.RatingLogs.Where(x => x.RateType != RateType.CarrierTripBySystem && x.RateType != RateType.ShipperTripBySystem).ToList().Clear();
                    //item.ShippingRequestTripAccidents.Clear();
                    //item.ShippingRequestTripTransitions.Clear();
                });
                var request = trip.ShippingRequestFk;
                request.Status = ShippingRequestStatus.PostPrice;

                await _shippingRequestTripTransitionRepository.DeleteAsync(x => x.ToPoint.ShippingRequestTripId == trip.Id);

                trip.HasAccident = false;
                //to save current trip incident
                await CurrentUnitOfWork.SaveChangesAsync();

                if (await CheckRequestTripsHasNoIncidents(request))
                {
                    request.HasAccident = false;
                }
                await _shippingRequestTripAccidentRepository.DeleteAsync(x => x.RoutPointFK.ShippingRequestTripId == trip.Id);

                //delete ratings
                var tripRate = trip.RatingLogs.FirstOrDefault();

                if (tripRate == null)
                {
                    tripRate = trip.RoutPoints.FirstOrDefault().RatingLogs.FirstOrDefault();
                }

                //tripRate may by trip Id or point Id, which is rated .. we need to delete all trip rates and its points and then recalculate,
                //check if trip has rating, to delete and recalculate
                if (tripRate != null)
                {
                    var DriverId = trip.RatingLogs.Where(x => x.RateType == RateType.DriverByReceiver).FirstOrDefault()?.DriverId;
                    var FacilityId = trip.RatingLogs.Where(x => x.RateType == RateType.FacilityByDriver).FirstOrDefault()?.FacilityId;

                    await _ratingLogManager.DeleteAllTripAndPointsRatingAsync(tripRate);

                    await CurrentUnitOfWork.SaveChangesAsync();

                    //recalculate rating
                    await _ratingLogManager.RecalculateCarrierRatingByCarrierTenantId(trip.ShippingRequestFk.CarrierTenantId.Value);
                    await _ratingLogManager.RecalculateShipperRatingByShipperTenantId(trip.ShippingRequestFk.TenantId);
                    if (DriverId != null)
                        await _ratingLogManager.RecaculateDriverRating(DriverId.Value);

                    if (FacilityId != null)
                        await _ratingLogManager.RecalculateFacilityRating(FacilityId.Value);
                }
            }
            else
            {
                throw new UserFriendlyException(L("TripCannotFound"));
            }
        }

        private async Task<bool> CheckRequestTripsHasNoIncidents(ShippingRequest request)
        {
            return await _ShippingRequestTrip.CountAsync(x => x.ShippingRequestId == request.Id && x.HasAccident) == 0;
        }

        public async Task PushNotification(int id,string waybillNumber)
        {
            if (AbpSession.UserId != null ) 
                await _appNotifier.NotifyDriverOnlyWhenTripUpdated(id,waybillNumber,
                    new UserIdentifier(AbpSession.TenantId, AbpSession.UserId.Value));
        }


    }
}