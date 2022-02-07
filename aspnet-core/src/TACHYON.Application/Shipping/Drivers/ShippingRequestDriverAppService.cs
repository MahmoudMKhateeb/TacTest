using Abp;
using Abp.Application.Features;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.EntityHistory;
using Abp.Linq.Extensions;
using Abp.Runtime.Validation;
using Abp.Timing;
using Abp.UI;
using AutoMapper.QueryableExtensions;
using DevExtreme.AspNet.Data.ResponseModel;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using NUglify.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using TACHYON.AddressBook;
using TACHYON.Authorization.Users;
using TACHYON.Documents.DocumentFiles;
using TACHYON.DriverLocationLogs;
using TACHYON.DriverLocationLogs.dtos;
using TACHYON.EntityLogs;
using TACHYON.Features;
using TACHYON.Firebases;
using TACHYON.Goods.GoodCategories.Dtos;
using TACHYON.Integration.WaslIntegration;
using TACHYON.Mobile;
using TACHYON.MultiTenancy;
using TACHYON.Notifications;
using TACHYON.Rating;
using TACHYON.Rating.dtos;
using TACHYON.Routs.RoutPoints;
using TACHYON.Routs.RoutPoints.Dtos;
using TACHYON.Routs.RoutPoints.RoutPointSmartEnum;
using TACHYON.Shipping.Accidents.Dto;
using TACHYON.Shipping.Drivers.Dto;
using TACHYON.Shipping.RoutPoints;
using TACHYON.Shipping.ShippingRequests;
using TACHYON.Shipping.ShippingRequestTrips;
using TACHYON.Shipping.Trips;
using TACHYON.Shipping.Trips.Accidents.Dto;
using TACHYON.Storage;
using TACHYON.Tracking;
using TACHYON.Tracking.Dto;
using TACHYON.Tracking.Dto.WorkFlow;
using TACHYON.Trucks.TrucksTypes.Dtos;
using Z.EntityFramework.Plus;

namespace TACHYON.Shipping.Drivers
{
    [AbpAuthorize()]
    public class ShippingRequestDriverAppService : TACHYONAppServiceBase, IShippingRequestDriverAppService
    {
        private readonly IRepository<ShippingRequestTrip> _ShippingRequestTrip;
        private readonly IRepository<RoutPoint, long> _RoutPointRepository;
        private readonly IRepository<RoutPointStatusTransition> _routPointStatusTransitionRepository;
        private readonly IRepository<ShippingRequestTripTransition> _shippingRequestTripTransitionRepository;
        private readonly IRepository<ShippingRequestTripAccident> _shippingRequestTripAccidentRepository;
        private readonly ShippingRequestDriverManager _shippingRequestDriverManager;
        private readonly ShippingRequestManager _shippingRequestManager;
        private readonly IAppNotifier _appNotifier;
        private readonly IRepository<UserOTP> _userOtpRepository;
        private readonly RatingLogManager _ratingLogManager;
        private readonly IRepository<DriverLocationLog, long> _driverLocationLogRepository;
        private readonly ShippingRequestPointWorkFlowProvider _workFlowProvider;
        private readonly ITempFileCacheManager _tempFileCacheManager;

        private readonly IRepository<User, long> _userRepository;

        public ShippingRequestDriverAppService(
            IRepository<ShippingRequestTrip> ShippingRequestTrip,
            IRepository<RoutPoint, long> RoutPointRepository,
            IRepository<ShippingRequestTripTransition> shippingRequestTripTransitionRepository,
            ShippingRequestDriverManager shippingRequestDriverManager,
            ShippingRequestManager shippingRequestManager,
            IAppNotifier appNotifier,
            IRepository<UserOTP> userOtpRepository,
            IRepository<ShippingRequestTripAccident> shippingRequestTripAccidentRepository,
            RatingLogManager ratingLogManager,
            IRepository<DriverLocationLog, long> driverLocationLogRepository,
            EntityLogManager logManager,
            IFirebaseNotifier firebaseNotifier,
            ShippingRequestPointWorkFlowProvider workFlowProvider,
            IRepository<RoutPointStatusTransition> routPointStatusTransitionRepository,
            ITempFileCacheManager tempFileCacheManager,
            IRepository<User, long> userRepository)
        {
            _ShippingRequestTrip = ShippingRequestTrip;
            _RoutPointRepository = RoutPointRepository;
            _shippingRequestTripTransitionRepository = shippingRequestTripTransitionRepository;
            _shippingRequestDriverManager = shippingRequestDriverManager;
            _shippingRequestManager = shippingRequestManager;
            _appNotifier = appNotifier;
            _userOtpRepository = userOtpRepository;
            _shippingRequestTripAccidentRepository = shippingRequestTripAccidentRepository;
            _ratingLogManager = ratingLogManager;
            _driverLocationLogRepository = driverLocationLogRepository;
            _workFlowProvider = workFlowProvider;
            _routPointStatusTransitionRepository = routPointStatusTransitionRepository;
            _tempFileCacheManager = tempFileCacheManager;
            _userRepository = userRepository;
        }

        /// <summary>
        /// list all trips realted with drivers
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PagedResultDto<ShippingRequestTripDriverListDto>> GetAll(
            ShippingRequestTripDriverFilterInput input)

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
                .Where(t => t.AssignedDriverUserId == AbpSession.UserId &&
                            t.Status != ShippingRequestTripStatus.Canceled &&
                            t.DriverStatus != ShippingRequestTripDriverStatus.Rejected)
                .WhereIf(input.Status.HasValue && input.Status == ShippingRequestTripDriverLoadStatusDto.Current,
                    e => e.StartTripDate.Date <= Clock.Now.Date && e.Status != ShippingRequestTripStatus.Delivered &&
                         e.Status != ShippingRequestTripStatus.DeliveredAndNeedsConfirmation)
                .WhereIf(input.Status.HasValue && input.Status == ShippingRequestTripDriverLoadStatusDto.Past,
                    e => (e.Status == ShippingRequestTripStatus.Delivered ||
                          e.Status == ShippingRequestTripStatus.DeliveredAndNeedsConfirmation))
                .WhereIf(input.Status.HasValue && input.Status == ShippingRequestTripDriverLoadStatusDto.Comming,
                    e => e.StartTripDate.Date > Clock.Now.Date)
                .OrderBy(input.Sorting ?? "Status asc");

            //.PageBy(input);
            var pageingitem = query.PageBy(input);
            var result = ObjectMapper.Map<List<ShippingRequestTripDriverListDto>>(pageingitem.ToList());

            var incidents = _shippingRequestTripAccidentRepository.GetAll()
                .Where(x => result.Select(y => y.Id).Contains(x.RoutPointFK.ShippingRequestTripId)).ToList();

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
            return await _shippingRequestTripAccidentRepository.CountAsync(x =>
                x.RoutPointFK.ShippingRequestTripId == id) > 0;
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
                .SingleOrDefaultAsync(t =>
                    t.Id == PointId && t.ShippingRequestTripFk.Status != ShippingRequestTripStatus.Canceled &&
                    t.ShippingRequestTripFk.AssignedDriverUserId == AbpSession.UserId &&
                    t.ShippingRequestTripFk.DriverStatus != ShippingRequestTripDriverStatus.Rejected);
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
                .SingleOrDefaultAsync(t =>
                    t.Id == TripId && t.Status != ShippingRequestTripStatus.Canceled &&
                    t.AssignedDriverUserId == AbpSession.UserId);


            if (trip == null) throw new UserFriendlyException(L("TheTripIsNotFound"));
            var tripDto = ObjectMapper.Map<ShippingRequestTripDriverDetailsDto>(trip);

            // return current driver trip
            var CurrentTrip = await _ShippingRequestTrip.GetAll()
                .Where(x => x.AssignedDriverUserId == AbpSession.UserId &&
                            x.Status == ShippingRequestTripStatus.Intransit).FirstOrDefaultAsync();

            if (CurrentTrip != null)
                tripDto.CurrentTripId = CurrentTrip.Id;

            if (trip.AssignedTruckFk != null)
                tripDto.TruckType = ObjectMapper.Map<TrucksTypeDto>(trip.AssignedTruckFk.TrucksTypeFk)
                    .TranslatedDisplayName;

            if (tripDto.TripStatus == ShippingRequestTripStatus.Intransit)

                tripDto.ActionStatus = ShippingRequestTripDriverActionStatusDto.ContinueTrip;

            else if (trip.StartTripDate.Date <= Clock.Now.Date && trip.Status == ShippingRequestTripStatus.New &&
                     trip.DriverStatus == ShippingRequestTripDriverStatus.Accepted)
                tripDto.ActionStatus = ShippingRequestTripDriverActionStatusDto.CanStartTrip;

            var rate = new RatingLog();

            tripDto.IsShippingExpRated = await _ratingLogManager.IsRateDoneBefore
            (
                new RatingLog
                {
                    DriverId = AbpSession.UserId,
                    TripId = TripId,
                    RateType = RateType.SEByDriver
                }
            );

            //  Need Review This 
            foreach (var point in tripDto.RoutePoints)
            {
                point.IsFacilityRated = await _ratingLogManager.IsRateDoneBefore
                (
                    new RatingLog
                    {
                        DriverId = AbpSession.UserId,
                        PointId = point.Id,
                        RateType = RateType.FacilityByDriver,
                        FacilityId = point.FacilityId
                    }
                );
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
                .WhereIf
                (
                    AbpSession.TenantId.HasValue && IsEnabled(AppFeatures.Carrier),
                    x => x.RoutPointFK.ShippingRequestTripFk.ShippingRequestFk.CarrierTenantId == AbpSession.TenantId
                )
                .WhereIf
                (
                    AbpSession.TenantId.HasValue && IsEnabled(AppFeatures.Shipper),
                    x => x.RoutPointFK.ShippingRequestTripFk.ShippingRequestFk.TenantId == AbpSession.TenantId
                )
                .WhereIf(!AbpSession.TenantId.HasValue || IsEnabled(AppFeatures.TachyonDealer), x => true)
                .WhereIf
                (
                    GetCurrentUser().IsDriver,
                    x => x.RoutPointFK.ShippingRequestTripFk.AssignedDriverUserId == AbpSession.UserId
                ).ToList();



            tripDto.ShippingRequestTripAccidentList = ObjectMapper.Map<List<ShippingRequestTripAccidentListDto>>(query);

            //tripDto.RoutePoints.ToList().ForEach(async x =>
            //x.IsFacilityRated =( await _ratingLogManager.IsRateDoneBefore(new RatingLog
            //{
            //    DriverId = AbpSession.UserId,
            //    PointId = x.Id,
            //    RateType = RateType.SEByDriver
            //})));







            //return good category name automatic from default language
            tripDto.GoodsCategory =
                            ObjectMapper.Map<GoodCategoryDto>(trip.ShippingRequestFk.GoodCategoryFk).DisplayName;
            return tripDto;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">TripId</param>
        /// <returns></returns>
        public async Task<DriverRoutPoint> GetRoutPointForMobile(int id)
        {
            DisableTenancyFilters();
            var routes = await _RoutPointRepository.GetAll()
             .Include(t => t.RoutPointStatusTransitions)
             .Include(z => z.FacilityFk)
             .Where(x => x.ShippingRequestTripId == id)
             .Select(x => new RoutPointsMobileDto
             {
                 Id = x.Id,
                 ShippingRequestTripId = x.ShippingRequestTripId,
                 CanGoToNextLocation = x.CanGoToNextLocation,
                 IsActive = x.IsActive,
                 IsResolve = x.IsResolve,
                 IsComplete = x.IsComplete,
                 Status = x.Status,
                 lat = x.FacilityFk.Location.Y,
                 lng = x.FacilityFk.Location.X,
                 PickingType = x.PickingType,
                 IsGoodPictureUploaded = x.IsGoodPictureUploaded,
                 IsPodUploaded = x.IsPodUploaded,
                 AvailableTransactions = !x.IsResolve ? new List<PointTransactionDto>() : _workFlowProvider.GetTransactionsByStatus(x.WorkFlowVersion, x.RoutPointStatusTransitions.Where(c => !c.IsReset).Select(v => v.Status).ToList(), x.Status)
             }).ToListAsync();
            if (routes == null) throw new UserFriendlyException(L("TheTripIsNotFound"));
            routes.ForEach(x => x.StatusTitle = L(x.Status.ToString()));
            var trip = _ShippingRequestTrip.Get(id);
            return new DriverRoutPoint { TripStatus = trip.Status , TripId = trip.Id,WaybillNumber = trip.WaybillNumber, RoutPoint = routes };
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
                .SingleOrDefaultAsync(t =>
                    t.Id == PointId && t.ShippingRequestTripFk.Status != ShippingRequestTripStatus.Canceled &&
                    t.ShippingRequestTripFk.AssignedDriverUserId == AbpSession.UserId &&
                    t.ShippingRequestTripFk.DriverStatus != ShippingRequestTripDriverStatus.Rejected);

            if (Point == null) throw new UserFriendlyException(L("TheTripIsNotFound"));

            var DropOff = ObjectMapper.Map<RoutDropOffDto>(Point);

            var statuses = await _routPointStatusTransitionRepository.GetAll()
                .Where(x => x.PointId == PointId && !x.IsReset)
                .Select(s => s.Status).ToListAsync();

            DropOff.IsFacilityRated = await _ratingLogManager.IsRateDoneBefore(new RatingLog
            {
                DriverId = AbpSession.UserId,
                PointId = PointId,
                RateType = RateType.FacilityByDriver,
                FacilityId = Point.FacilityId
            });
            DropOff.AvailableTransactions = !Point.IsResolve
                ? new List<PointTransactionDto>()
                : _workFlowProvider.GetTransactionsByStatus(Point.WorkFlowVersion, statuses, Point.Status);

            return DropOff;
        }

        /// <summary>
        /// When driver click on mobile app to start trip  Journey
        /// </summary>
        /// <param name="Input"></param>
        /// <returns></returns>
        public async Task StartTrip(ShippingRequestTripDriverStartInputDto Input)
        {
            await _workFlowProvider.Start(Input);
        }

        /// <summary>
        /// Set new active dropoff point for trip
        /// </summary>
        /// <param name="PointId">Dropoff point id</param>
        public async Task GotoNextLocation(long pointId)
        {
            await _workFlowProvider.GoToNextLocation(pointId);
        }

        /// <summary>
        /// The driver rate facility after drop finished
        /// </summary>
        /// <param name="pointId"></param>
        /// <param name="rate"></param>
        /// <param name="note"></param>
        [RequiresFeature(AppFeatures.Carrier)]
        public async Task SetRating(long pointId,
            int rate,
            string note)
        {
            await CheckCurrentUserIsDriver();

            var ratingLog = new RatingLog() { PointId = pointId, Rate = rate, Note = note };
            var pointFacilityId = await _RoutPointRepository.GetAll().AsNoTracking()
                .Where(x => x.Id == ratingLog.PointId)
                .Select(x => x.FacilityId).FirstOrDefaultAsync();
            ratingLog.DriverId = AbpSession.UserId;
            ratingLog.FacilityId = pointFacilityId;
            ratingLog.RateType = RateType.FacilityByDriver;

            await _ratingLogManager.CreateRating(ratingLog);
        }

        private async Task CheckCurrentUserIsDriver()
        {
            bool isCurrentUserDriver = await _userRepository.GetAll().AsNoTracking()
                .AnyAsync(x => x.IsDriver && x.Id == AbpSession.UserId);
            if (!isCurrentUserDriver)
                throw new AbpValidationException(L("YouMustBeDriverToRateFacility"));
        }

        /// <summary>
        /// The driver rate shipping Experience after trip delivered
        /// </summary>
        [RequiresFeature(AppFeatures.Carrier)]
        public async Task SetShippingExpRating(int tripId,
            int rate,
            string note)
        {
            await CheckCurrentUserIsDriver();
            var input = new RatingLog() { TripId = tripId, RateType = RateType.SEByDriver, Rate = rate, Note = note, DriverId = AbpSession.UserId };
            await _ratingLogManager.CreateRating(input);
        }

        public async Task InvokeStatus(InvokeStatusInputDto input)
        {
            CheckIfCanAccessService(true, AppFeatures.TachyonDealer, AppFeatures.Carrier);
            var args = new PointTransactionArgs { PointId = input.Id, Code = input.Code };
            await _workFlowProvider.Invoke(args, input.Action);
        }

        public async Task Accepted(int TripId)
        {
            await _workFlowProvider.Accepted(TripId);
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
            var point = new Point(input.Longitude, input.Latitude) { SRID = 4326 };

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
                .Include(x => x.RoutPoints)
                .ThenInclude(x => x.RoutPointStatusTransitions)
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
                .WhereIf(IsEnabled(AppFeatures.Carrier),
                    x => x.ShippingRequestFk.CarrierTenantId == AbpSession.TenantId)
                .WhereIf(IsEnabled(AppFeatures.TachyonDealer), x => x.ShippingRequestFk.IsTachyonDeal == true)
                .Include(x => x.ShippingRequestFk)
                .Include(x => x.RoutPoints)
                .ThenInclude(x => x.RoutPointDocuments)
                .Include(x => x.RoutPoints)
                .ThenInclude(x => x.RoutPointStatusTransitions)
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
                    item.IsResolve = false;
                    item.Status = RoutePointStatus.StandBy;
                    item.IsDeliveryNoteUploaded = false;
                    item.RoutPointDocuments.Clear();
                    item.ActualPickupOrDeliveryDate = null;
                    item.CompletedStatus = RoutePointCompletedStatus.NotCompleted;
                    item.StartTime = item.EndTime = null;
                    item.CanGoToNextLocation = false;
                    item.IsGoodPictureUploaded = false;
                    item.RoutPointStatusTransitions.Where(s => !s.IsReset).ForEach(x => x.IsReset = true);
                    item.IsPodUploaded = false;
                    //item.RatingLogs.Where(x => x.RateType != RateType.CarrierTripBySystem && x.RateType != RateType.ShipperTripBySystem).ToList().Clear();
                    //item.ShippingRequestTripAccidents.Clear();
                    //item.ShippingRequestTripTransitions.Clear();
                });
                var request = trip.ShippingRequestFk;
                request.Status = ShippingRequestStatus.PostPrice;

                await _shippingRequestTripTransitionRepository.DeleteAsync(x =>
                    x.ToPoint.ShippingRequestTripId == trip.Id);

                trip.HasAccident = false;
                //to save current trip incident
                await CurrentUnitOfWork.SaveChangesAsync();

                if (await CheckRequestTripsHasNoIncidents(request))
                {
                    request.HasAccident = false;
                }

                await _shippingRequestTripAccidentRepository.DeleteAsync(x =>
                    x.RoutPointFK.ShippingRequestTripId == trip.Id);

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
                    var driverId = trip.RatingLogs.FirstOrDefault(x => x.RateType == RateType.DriverByReceiver)
                        ?.DriverId;
                    var facilityId = trip.RatingLogs.FirstOrDefault(x => x.RateType == RateType.FacilityByDriver)
                        ?.FacilityId;

                    await _ratingLogManager.DeleteAllTripAndPointsRatingAsync(tripRate);

                    await CurrentUnitOfWork.SaveChangesAsync();

                    // recalculate rating For Shipper
                    await _ratingLogManager.RecalculateRatingById(trip.ShippingRequestFk.TenantId, typeof(Tenant));

                    // recalculate rating For Carrier
                    if (trip.ShippingRequestFk.CarrierTenantId.HasValue)
                        await _ratingLogManager.RecalculateRatingById(trip.ShippingRequestFk.CarrierTenantId.Value, typeof(Tenant));

                    // recalculate rating For Driver
                    if (driverId != null)
                        await _ratingLogManager.RecalculateRatingById(driverId.Value, typeof(User));

                    // recalculate rating For Facility
                    if (facilityId != null)
                        await _ratingLogManager.RecalculateRatingById(facilityId.Value, typeof(Facility));

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

        public async Task PushNotification(int id, string waybillNumber)
        {
            if (AbpSession.UserId != null)
                await _appNotifier.NotifyDriverOnlyWhenTripUpdated(id, waybillNumber,
                    new UserIdentifier(AbpSession.TenantId, AbpSession.UserId.Value));
        }
    }
}