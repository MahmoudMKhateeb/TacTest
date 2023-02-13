using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Abp.Timing;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TACHYON.Actors;
using TACHYON.Dashboards.Shipper.Dto;
using TACHYON.Routs.RoutPoints;
using TACHYON.Shipping.ShippingRequests;
using TACHYON.Shipping.ShippingRequestTrips;
using TACHYON.Shipping.Trips;

namespace TACHYON.Dashboards.Broker
{
    // todo: add permission here (broker)
    [AbpAuthorize()]
    public class BrokerDashboardAppService : TACHYONAppServiceBase
    {
        private readonly IRepository<Actor> _actorRepository;
        private readonly IRepository<ShippingRequestTrip> _tripRepository;
        private readonly IRepository<RoutPoint,long> _routePointRepository;

        public BrokerDashboardAppService(
            IRepository<Actor> actorRepository,
            IRepository<ShippingRequestTrip> tripRepository,
            IRepository<RoutPoint, long> routePointRepository)
        {
            _actorRepository = actorRepository;
            _tripRepository = tripRepository;
            _routePointRepository = routePointRepository;
        }

        public async Task<ActorsNumbersDto> GetNumbersOfActors()
        {
            int carrierActorsCount = await _actorRepository.GetAll()
                .CountAsync(x => x.ActorType == ActorTypesEnum.Carrier);
            int shipperActorsCount = await _actorRepository.GetAll()
                    .CountAsync(x => x.ActorType == ActorTypesEnum.Shipper);

            int totalActorsCount = carrierActorsCount + shipperActorsCount;

            return new ActorsNumbersDto()
            {
                CarrierActorPercentage = (carrierActorsCount / totalActorsCount) * 100,
                ShipperActorPercentage = (shipperActorsCount / totalActorsCount) * 100,
                TotalActors = totalActorsCount
            };
        }

        public async Task<ActorsForCurrentMonthDto> GetNewActorsStatistics()
        {
            int carrierActorsCount = await _actorRepository.GetAll()
                .Where(x=> x.CreationTime.Month == Clock.Now.Month)
                .CountAsync(x => x.ActorType == ActorTypesEnum.Carrier);
            int shipperActorsCount = await _actorRepository.GetAll()
                .Where(x=> x.CreationTime.Month == Clock.Now.Month)
                .CountAsync(x => x.ActorType == ActorTypesEnum.Shipper);

            int totalActorsForCurrentMonth = carrierActorsCount + shipperActorsCount;
            
            // todo add GrowthChangePercentage

            return new ActorsForCurrentMonthDto()
            {
                CarrierActorsPercentage = (carrierActorsCount / totalActorsForCurrentMonth) * 100,
                ShipperActorsPercentage = (shipperActorsCount / totalActorsForCurrentMonth) * 100,
                TotalActorsForCurrentMonth = totalActorsForCurrentMonth,
                // todo GrowthChangePercentage = 
            };
        }

        public async Task<ListResultDto<ActiveActorDto>> GetMostActiveActors(GetMostActiveActorsInput input)
        {
            if (input.RangeType == DateRangeType.CustomRange && (!input.FromDate.HasValue || !input.ToDate.HasValue))
                throw new UserFriendlyException(L("FromAndToDateMustHaveAValueWhenSelectingCustomRange"));
            
            DisableTenancyFilters();

            var tripsQuery =  _tripRepository.GetAll()
                .Where(x => x.ShippingRequestFk.TenantId == AbpSession.TenantId ||
                            x.ShippingRequestFk.CarrierTenantId == AbpSession.TenantId)
                .Where(x =>
                    (input.ActorType == ActorTypesEnum.Carrier && x.ShippingRequestFk.CarrierActorId.HasValue) ||
                    (input.ActorType == ActorTypesEnum.Shipper && x.ShippingRequestFk.ShipperActorId.HasValue))
                .WhereIf(input.RangeType == DateRangeType.ThisMonth, x => x.CreationTime.Month == Clock.Now.Month)
                .WhereIf(input.RangeType == DateRangeType.LastMonth,
                    x => x.CreationTime.Month == Clock.Now.AddMonths(-1).Month)
                .WhereIf(input.RangeType == DateRangeType.LastYear,
                    x => x.CreationTime.Year == Clock.Now.AddYears(-1).Year);

            var activeActors = await (from trip in tripsQuery
                    group trip by (input.ActorType == ActorTypesEnum.Carrier
                        ? trip.ShippingRequestFk.CarrierActorFk.CompanyName
                        : trip.ShippingRequestFk.ShipperActorFk.CompanyName)
                    into tripsGroup
                    select new ActiveActorDto { ActorName = tripsGroup.Key, NumberOfTrips = tripsGroup.Count() })
                .ToListAsync();

            return new ListResultDto<ActiveActorDto>() { Items = activeActors };
        }
        
        /// <summary>
        /// this service for broker actors data only
        /// </summary>
        /// <returns></returns>
        public async Task<List<UpcomingTripsOutput>> GetUpcomingTrips()
        {
            DateTime currentDay = Clock.Now.Date;
            DateTime endOfCurrentWeek = currentDay.AddDays(7).Date;

            DisableTenancyFilters();

            var trips = await (from trip in _tripRepository.GetAll().AsNoTracking()
                    .Include(x => x.OriginFacilityFk).ThenInclude(x => x.CityFk)
                    .Include(x => x.DestinationFacilityFk).ThenInclude(x => x.CityFk)
                where (trip.ShippingRequestFk.TenantId == AbpSession.TenantId || trip.ShippingRequestFk.CarrierTenantId == AbpSession.TenantId) &&
                      (trip.ShippingRequestFk.CarrierActorId.HasValue || trip.ShippingRequestFk.ShipperActorId.HasValue) &&
                      trip.Status == ShippingRequestTripStatus.New && trip.StartTripDate.Date >= currentDay &&
                      trip.StartTripDate.Date <= endOfCurrentWeek
                select new
                {
                    trip.Id,
                    Origin =
                        trip.OriginFacilityId.HasValue ? trip.OriginFacilityFk.CityFk.DisplayName : string.Empty,
                    Destinations = trip.RoutPoints.Where(x=> x.PickingType == PickingType.Dropoff)
                        .Select(x=> x.FacilityFk.CityFk.DisplayName).Distinct().ToList(),
                        trip.WaybillNumber,
                    TripType = trip.ShippingRequestFk.ShippingRequestFlag == ShippingRequestFlag.Dedicated
                        ? LocalizationSource.GetString("Dedicated")
                        : trip.ShippingRequestFk.IsSaas() ? LocalizationSource.GetString("Saas")
                            : LocalizationSource.GetString("TruckAggregation"), trip.StartTripDate
                }).ToListAsync();
            
           var upcomingTrips = (from trip in trips
                group trip by trip.StartTripDate.Date
                into upcomingTripsGroup
                select new UpcomingTripsOutput()
                {
                    Date = upcomingTripsGroup.Key,
                    Trips = upcomingTripsGroup.Select(x=> new UpcomingTripItemDto()
                    {
                        Id = x.Id,Origin = x.Origin,
                        Destinations = x.Destinations,
                        WaybillNumber = x.WaybillNumber,
                        TripType = x.TripType
                    }).ToList()
                }).ToList();

            return upcomingTrips;
        }
        
        public async Task<List<NeedsActionTripDto>> GetNeedsActionTrips()
        {
             DisableTenancyFilters();

             var trips = await (from point in _routePointRepository.GetAll()
                 where (point.ShippingRequestTripFk.ShippingRequestFk.TenantId == AbpSession.TenantId ||
                        point.ShippingRequestTripFk.ShippingRequestFk.CarrierTenantId == AbpSession.TenantId) &&
                       (point.ShippingRequestTripFk.ShippingRequestFk.CarrierActorId.HasValue ||
                        point.ShippingRequestTripFk.ShippingRequestFk.ShipperActorId.HasValue) &&
                       (!point.ShippingRequestTripFk.ShippingRequestFk.CarrierActorId.HasValue &&
                        !point.ShippingRequestTripFk.ShippingRequestFk.ShipperActorId.HasValue)
                       && point.ShippingRequestTripFk.Status == ShippingRequestTripStatus.DeliveredAndNeedsConfirmation
                       && point.PickingType == PickingType.Dropoff && !point.IsComplete &&
                       (!point.IsPodUploaded || !point.ShippingRequestTripFk.EndWorking.HasValue)
                     
                select new NeedsActionTripDto()
                {
                    Origin = point.ShippingRequestTripFk.OriginFacilityFk.CityFk.DisplayName,
                    Destinations = point.ShippingRequestTripFk.RoutPoints
                        .Where(x => x.PickingType == PickingType.Dropoff)
                        .Select(x => x.FacilityFk.CityFk.DisplayName).Distinct().ToList(),
                    WaybillNumber = point.ShippingRequestTripFk.RouteType.HasValue
                        ? (point.ShippingRequestTripFk.RouteType == ShippingRequestRouteType.SingleDrop
                            ? point.ShippingRequestTripFk.WaybillNumber
                            : point.WaybillNumber)
                        : (point.ShippingRequestTripFk.ShippingRequestFk.RouteTypeId ==
                           ShippingRequestRouteType.SingleDrop
                            ? point.ShippingRequestTripFk.WaybillNumber
                            : point.WaybillNumber),
                    NeedsPod = !point.IsPodUploaded,
                    NeedsReceiverCode = !point.ShippingRequestTripFk.EndWorking.HasValue
                }).ToListAsync();

            return trips;
        }

        public async Task<ActiveAndNonActiveActorsDto> GetNumberOfActiveAndNonActiveActors()
        {
            var actorsLookup = await _actorRepository.GetAll().Select(x => new { x.IsActive, x.ActorType }).ToListAsync();

            return new ActiveAndNonActiveActorsDto()
            {
                ActiveCarrierActorsCount = actorsLookup.Count(x => x.IsActive && x.ActorType == ActorTypesEnum.Carrier),
                ActiveShipperActorsCount = actorsLookup.Count(x => x.IsActive && x.ActorType == ActorTypesEnum.Shipper),
                NonActiveCarrierActorsCount =
                    actorsLookup.Count(x => !x.IsActive && x.ActorType == ActorTypesEnum.Carrier),
                NonActiveShipperActorsCount =
                    actorsLookup.Count(x => !x.IsActive && x.ActorType == ActorTypesEnum.Shipper),
            };
        }

        public async Task<List<MostUsedOriginsDto>> GetMostUsedOrigins()
        {
            DisableTenancyFilters();

            return await (from trip in _tripRepository.GetAll()
                where (trip.ShippingRequestFk.TenantId == AbpSession.TenantId ||
                       trip.ShippingRequestFk.CarrierTenantId == AbpSession.TenantId) &&
                      (trip.ShippingRequestFk.CarrierActorId.HasValue ||
                       trip.ShippingRequestFk.ShipperActorId.HasValue)
                       group trip by trip.OriginFacilityFk.CityFk.DisplayName into tripsGroup
                    select new MostUsedOriginsDto{CityName = tripsGroup.Key, NumberOfRequests = tripsGroup.Count()})
                .OrderByDescending(x=> x.NumberOfRequests).ToListAsync();
        }
    }
}