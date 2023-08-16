using Abp.Application.Features;
using Abp.Authorization;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Abp.MultiTenancy;
using Abp.Runtime.Session;
using Abp.Timing;
using Abp.UI;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using TACHYON.Authorization;
using TACHYON.Authorization.Users;
using TACHYON.Cities;
using TACHYON.Dashboards.Carrier.Dto;
using TACHYON.Dashboards.Host.Dto;
using TACHYON.Dashboards.Host.TMS_HostDto;
using TACHYON.Dashboards.Shipper.Dto;
using TACHYON.Features;
using TACHYON.Goods.GoodCategories;
using TACHYON.Invoices;
using TACHYON.Invoices.SubmitInvoices;
using TACHYON.MultiTenancy;
using TACHYON.PriceOffers;
using TACHYON.PricePackages;
using TACHYON.Routs.RoutPoints;
using TACHYON.Routs.RoutTypes;
using TACHYON.Shipping.DirectRequests;
using TACHYON.Shipping.ShippingRequests;
using TACHYON.Shipping.ShippingRequestTrips;
using TACHYON.Shipping.Trips;
using TACHYON.ShippingRequestVases;
using TACHYON.Trucks;


namespace TACHYON.Dashboards.Carrier
{
    // todo fix auth asap
    //[AbpAuthorize(AppPermissions.Pages_CarrierDashboard)]
    public class CarrierDashboardAppService : TACHYONAppServiceBase, ICarrierDashboardAppService
    {

        private readonly IRepository<User, long> _usersRepository;
        private readonly IRepository<Truck, long> _trucksRepository;
        private readonly IRepository<ShippingRequestTrip> _shippingRequestTripRepository;
        private readonly IRepository<ShippingRequestVas, long> _shippingRequestVasRepository;
        private readonly IRepository<PriceOffer, long> _priceOfferRepository;
        private readonly IRepository<SubmitInvoice, long> _submitInvoiceRepository;
        private readonly IRepository<ShippingRequestDirectRequest,long> _directRequestRepository;
        private readonly IRepository<RoutPoint,long> _routePointRepository;
        private readonly IRepository<PricePackage,long> _pricePackageRepository;
        private readonly DashboardDomainService _dashboardDomainService;


        public CarrierDashboardAppService(
             IRepository<User, long> usersRepository,
             IRepository<Truck, long> trucksRepository,
             IRepository<ShippingRequestTrip> shippingRequestTripRepository,
             IRepository<ShippingRequestVas, long> shippingRequestVasRepository,
             IRepository<PriceOffer, long> priceOfferRepository,
             IRepository<SubmitInvoice, long> submitInvoiceRepository,
             IRepository<ShippingRequestDirectRequest, long> directRequestRepository,
             IRepository<RoutPoint, long> routePointRepository,
             IRepository<PricePackage, long> pricePackageRepository,
             DashboardDomainService dashboardDomainService)
        {
            _usersRepository = usersRepository;
            _trucksRepository = trucksRepository;
            _shippingRequestTripRepository = shippingRequestTripRepository;
            _shippingRequestVasRepository = shippingRequestVasRepository;
            _priceOfferRepository = priceOfferRepository;
            _submitInvoiceRepository = submitInvoiceRepository;
            _directRequestRepository = directRequestRepository;
            _routePointRepository = routePointRepository;
            _pricePackageRepository = pricePackageRepository;
            _dashboardDomainService = dashboardDomainService;
        }


        public async Task<long> GetNumberOfGeneratedInvoices()
        {
            return await _submitInvoiceRepository.GetAll().Where(x => x.TenantId == AbpSession.TenantId)
                .LongCountAsync();
        }
        public async Task<int> GetDeliveredTripsCountForCurrentWeek()
        {
            DateTime startOfCurrentWeek = Clock.Now.StartOfWeek(DayOfWeek.Sunday).Date;
            DateTime endOfCurrentWeek = startOfCurrentWeek.AddDays(7).Date;
            DisableTenancyFilters();

            return await _shippingRequestTripRepository.GetAll()
                .Where(x => x.ShippingRequestFk.CarrierTenantId == AbpSession.TenantId)
                .CountAsync(x =>
                    x.Status == ShippingRequestTripStatus.Delivered && x.EndWorking.HasValue &&
                    (x.EndWorking.Value.Date >= startOfCurrentWeek && x.EndWorking.Value.Date <= endOfCurrentWeek));
        }

        public async Task<int> GetInTransitTripsCount()
        {
            DisableTenancyFilters();

            return await _shippingRequestTripRepository.GetAll()
                .Where(x => x.ShippingRequestFk.CarrierTenantId == AbpSession.TenantId)
                .CountAsync(x => x.Status == ShippingRequestTripStatus.InTransit);
        }
        
        public async Task<List<UpcomingTripsOutput>> GetUpcomingTrips()
        {
            DateTime currentDay = Clock.Now.Date;
            DateTime endOfCurrentWeek = currentDay.AddDays(7).Date; 
            
            DisableTenancyFilters();

            var trips = await (from trip in _shippingRequestTripRepository.GetAll().AsNoTracking()
                    .Include(x => x.OriginFacilityFk).ThenInclude(x => x.CityFk)
                    .Include(x => x.DestinationFacilityFk).ThenInclude(x => x.CityFk)
                where trip.ShippingRequestFk.CarrierTenantId == AbpSession.TenantId &&
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
        
        public async Task<List<NeedsActionTripDto>> GetNeedsActionTrips(DateRangeInput input)
        {
            DisableTenancyFilters();
            var trips = await (from point in _routePointRepository.GetAll()
                where point.ShippingRequestTripFk.ShippingRequestFk.CarrierTenantId == AbpSession.TenantId
                      && point.ShippingRequestTripFk.Status == ShippingRequestTripStatus.DeliveredAndNeedsConfirmation
                      && point.PickingType == PickingType.Dropoff && !point.IsComplete && (!point.IsPodUploaded || !point.ShippingRequestTripFk.EndWorking.HasValue
                      && point.ShippingRequestTripFk.CreationTime.Date > input.StartDate && point.ShippingRequestTripFk.CreationTime.Date < input.EndDate)
                select new NeedsActionTripDto()
                {
                    Origin = point.ShippingRequestTripFk.OriginFacilityFk.CityFk.DisplayName,
                    Destinations = GetDistinctDestinations(point.ShippingRequestTripFk.RoutPoints
                            .Where(x => x.PickingType == PickingType.Dropoff).Select(c => c.FacilityFk.CityFk)),
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
        public async Task<List<NewDirectRequestListDto>> GetNewDirectRequest()
        {
            DisableTenancyFilters();

            var directRequests = await _directRequestRepository.GetAll()
                .Where(x => x.CarrierTenantId == AbpSession.TenantId &&
                            x.Status == ShippingRequestDirectRequestStatus.New)
                .Select(x => new NewDirectRequestListDto()
                {
                    ReferenceNumber = x.ShippingRequestFK.ReferenceNumber,
                    CompanyName = x.ShippingRequestFK.RequestType == ShippingRequestType.TachyonManageService
                        ? LocalizationSource.GetString("TachyonManageService")
                        : x.Tenant.companyName,
                    ShippingRequestId = x.ShippingRequestId,
                    CreationTime = x.CreationTime
                }).OrderByDescending(x=>x.CreationTime).ToListAsync();

            return directRequests;
        }

        public async Task<List<ChartCategoryPairedValuesDto>> GetCompletedTripsCountPerMonth(GetDataByDateFilterInput input)
        {
            DisableTenancyFilters();



            //daily => default before 30 day
            if (input.DatePeriod == FilterDatePeriod.Daily)
            {
                return await GetCompletedTripsIfDaily(input);
            }
            else if (input.DatePeriod == FilterDatePeriod.Monthly)
            {
                return await GetCompletedTripsIfMonthly(input);
            }

            return new List<ChartCategoryPairedValuesDto>();
        }


        private async Task<List<ChartCategoryPairedValuesDto>> GetCompletedTripsIfMonthly(GetDataByDateFilterInput input)
        {
            var year = Clock.Now.Year;
            var trips = await _shippingRequestTripRepository
                .GetAll()
                .AsNoTracking()
                .Where(x => x.Status == ShippingRequestTripStatus.Delivered && x.CreationTime.Year == Clock.Now.Year)
                .Where(x => x.ShippingRequestFk.CarrierTenantId == AbpSession.TenantId)
                .ToListAsync();

            var result = trips
                        .GroupBy(r => r.CreationTime.Month)
                        .Select(g => new ChartCategoryPairedValuesDto
                        {
                            Y = g.Count(),

                            X = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(g.Key)
                        }).ToList();

            return result;

        }


        private async Task<List<ChartCategoryPairedValuesDto>> GetCompletedTripsIfDaily(GetDataByDateFilterInput input)
        {

            var tripsDailyList = await _shippingRequestTripRepository.GetAll()
                .AsNoTracking()
            .Where(x => x.Status == ShippingRequestTripStatus.Delivered && x.CreationTime > Clock.Now.AddDays(-30))
            .Where(x => x.ShippingRequestFk.CarrierTenantId == AbpSession.TenantId)
            .ToListAsync();

            var result = tripsDailyList
                .GroupBy(r =>

                    r.CreationTime.Date
                )
                .Select(s => new ChartCategoryPairedValuesDto
                {
                    Y = s.Count(),
                    X = s.Key.ToString("dd/MM/yyyy"),

                }).OrderBy(r => r.Y).ToList();
            return result;

        }



        public async Task<ActivityItemsDto> GetDriversActivity()
        {
            DisableTenancyFilters();
            var query = _usersRepository
                .GetAll()
                .AsNoTracking()
                .Where(r => r.TenantId == AbpSession.TenantId)
                .Where(x => x.IsDriver);

            var inTransitTrucks = await _shippingRequestTripRepository
                .GetAll()
                .Where(x => x.Status == ShippingRequestTripStatus.InTransit)
                .Where(x => x.ShippingRequestFk.CarrierTenantId == AbpSession.TenantId || x.CarrierTenantId == AbpSession.TenantId)
                .Select(x => x.AssignedDriverUserId)
                .Distinct()
                .CountAsync();

            var activeItems = await query.CountAsync(r => r.IsActive);  
            var notActiveItems = await query.CountAsync(r => !r.IsActive);


            return new ActivityItemsDto()
            {
                ActiveItems = activeItems,
                NotActiveItems = notActiveItems,
                InTransitItems = inTransitTrucks,
                TotalItemsCount = activeItems + notActiveItems
            };
        }
        
        public async Task<CompletedTripVsPodListDto> GetCompletedTripVsPod(FilterDatePeriod period)
        {
            DisableTenancyFilters();

            DateTime startOfCurrentWeek = Clock.Now.StartOfWeek(DayOfWeek.Sunday).Date;
            DateTime endOfCurrentWeek = startOfCurrentWeek.AddDays(7).Date;

            var query = _shippingRequestTripRepository
                .GetAll()
                .AsNoTracking()
                .Where(x=> x.ShippingRequestId.HasValue ? x.ShippingRequestFk.CarrierTenantId == AbpSession.TenantId: x.CarrierTenantId == AbpSession.TenantId)
                .WhereIf(period == FilterDatePeriod.Daily,x=>  x.CreationTime.Date >= startOfCurrentWeek && x.CreationTime.Date <= endOfCurrentWeek )
                .WhereIf(period == FilterDatePeriod.Weekly,x=>  x.CreationTime.Year == Clock.Now.Year && x.CreationTime.Month == Clock.Now.Month )
                .WhereIf(period == FilterDatePeriod.Monthly,x=>  x.CreationTime.Year == Clock.Now.Year )
                .Where(x => x.Status == ShippingRequestTripStatus.Delivered || x.Status == ShippingRequestTripStatus.DeliveredAndNeedsConfirmation);


            var podTrips = await query
                    .Where(x=>x.Status == ShippingRequestTripStatus.DeliveredAndNeedsConfirmation)
                    .Where(x => x.RoutPoints.Where(y=>y.PickingType == PickingType.Dropoff).Any(p => !p.IsPodUploaded))
                    .Select(x=> new {x.CreationTime,x.Id}).ToListAsync();
            
            var total = await query.Where(x=> x.Status == ShippingRequestTripStatus.Delivered).Select(x=> new {x.CreationTime,x.Id}).ToListAsync();
                
            
            var podTripsList = new List<ChartCategoryPairedValuesDto>();
            var totalList = new List<ChartCategoryPairedValuesDto>();
            if (period == FilterDatePeriod.Daily)
            {
                podTripsList = podTrips.GroupBy(x => x.CreationTime.DayOfWeek)
                    .Select(x => new ChartCategoryPairedValuesDto() { X = x.Key.ToString(), Y = x.Count() })
                    .ToList();
                
                totalList = total.GroupBy(x => x.CreationTime.DayOfWeek)
                    .Select(x => new ChartCategoryPairedValuesDto() { X = x.Key.ToString(), Y = x.Count() })
                    .ToList();
            }

            if (period == FilterDatePeriod.Monthly)
            {
                var GroupedPodTrips = podTrips.GroupBy(x => x.CreationTime.Month);

                var GroupedTotal = total.GroupBy(x => x.CreationTime.Month);

                foreach (var date in _dashboardDomainService.GetYearMonthsEndWithCurrent())
                {
                    if (GroupedPodTrips.Select(x => x.Key).ToList().Contains(date.Month))
                    {
                        podTripsList.Add(GroupedPodTrips.Where(x => x.Key == date.Month)
                    .Select(x => new ChartCategoryPairedValuesDto() { X = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(x.Key), Y = x.Count() })
                    .FirstOrDefault());
                    }
                    else
                    {
                        podTripsList.Add(new ChartCategoryPairedValuesDto { X = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(date.Month), Y = 0 });
                    }

                    if (GroupedTotal.Select(x => x.Key).ToList().Contains(date.Month))
                    {
                        totalList.Add(GroupedTotal.Where(x => x.Key == date.Month)
                    .Select(x => new ChartCategoryPairedValuesDto() { X = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(x.Key), Y = x.Count() })
                    .FirstOrDefault());
                    }
                    else
                    {
                        totalList.Add(new ChartCategoryPairedValuesDto { X = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(date.Month), Y = 0 });
                    }
                }
            }
            
            if (period == FilterDatePeriod.Weekly)
            {
                DateTime firstMonthDay = new DateTime(Clock.Now.Year, Clock.Now.Month, 1);
                DateTime firstMonthSunday = firstMonthDay.AddDays((DayOfWeek.Sunday + 7 - firstMonthDay.DayOfWeek) % 7);
                
                podTripsList = podTrips.GroupBy(x => ((x.CreationTime.Day - firstMonthSunday.Day) / 7) +1)
                    .Select(x => new ChartCategoryPairedValuesDto() { X = $"Week {x.Key}",Y = x.Count() })
                    .ToList();
                
                totalList = total.GroupBy(x => ((x.CreationTime.Day - firstMonthSunday.Day) / 7) +1)
                    .Select(x => new ChartCategoryPairedValuesDto() { X = $"Week {x.Key}", Y = x.Count() })
                    .ToList();
            }

            return new CompletedTripVsPodListDto
            {
                CompletedTrips = totalList,
                PODTrips = podTripsList
            };



        }

        public async Task<ActivityItemsDto> GetTrucksActivity()
        {
            DisableTenancyFilters();
            var query = _trucksRepository
                .GetAll()
                .AsNoTracking()
                .Where(r => r.TenantId == AbpSession.TenantId);

            var inTransitTrucks = await _shippingRequestTripRepository
                .GetAll()
                .Where(x => x.Status == ShippingRequestTripStatus.InTransit)
                .Where(x => x.ShippingRequestFk.CarrierTenantId == AbpSession.TenantId || x.CarrierTenantId == AbpSession.TenantId)
                .Select(x => x.AssignedTruckId)
                .Distinct()
                .CountAsync();

            var activeItems = await query.CountAsync(r => r.TruckStatusId == 1);  
            var notActiveItems = await query.CountAsync(r => r.TruckStatusId == 2);



            return new ActivityItemsDto()
            {
                ActiveItems = activeItems,
                NotActiveItems = notActiveItems,
                InTransitItems = inTransitTrucks,
                TotalItemsCount = activeItems + notActiveItems
            };
        }

        public async Task<List<MostTenantWorksListDto>> GetMostWorkedWithShippers()
        {
       
                DisableTenancyFilters();

                var trips = await _shippingRequestTripRepository
                     .GetAll()
                     .Include(r => r.ShippingRequestFk)
                     .ThenInclude(x => x.Tenant)
                     .AsNoTracking()
                     .Where(t => t.ShippingRequestFk.CarrierTenantId == AbpSession.TenantId )
                     .Select
                     (
                         x => new
                         {
                             shipperId = x.ShippingRequestFk.Tenant.Id,
                             shipperName = x.ShippingRequestFk.Tenant.TenancyName,
                             shipperRating = x.ShippingRequestFk.Tenant.Rate
                         }
                     )
                     .ToListAsync();

                return trips
                    .GroupBy(x => x.shipperId)
                    .Select(
                        g => new MostTenantWorksListDto()
                        {
                            Id = g.Key,
                            Name = g.FirstOrDefault()?.shipperName,
                            Rating = g.FirstOrDefault()?.shipperRating,
                            NumberOfTrips = g.Count(),
                        })
                 .OrderByDescending(x => x.NumberOfTrips)
                 .Take(5)
                 .ToList();


            


        }

        public async Task<List<ChartCategoryPairedValuesDto>> GetMostVasesUsedByShippers()
        {
            DisableTenancyFilters();

             var shippingRequestVases =  await _shippingRequestVasRepository
                    .GetAll()
                    .AsNoTracking()
                    .Include(r => r.VasFk).ThenInclude(x=> x.Translations)
                    .Where(x => x.ShippingRequestFk.CarrierTenantId == AbpSession.TenantId)
                    .ToListAsync();
                var mostVasesUsed = (from srVas in shippingRequestVases
                        group srVas by srVas.VasId into vasGroup
                        let vasFk = vasGroup.Select(x=> x.VasFk).FirstOrDefault(x=> x.Id == vasGroup.Key)
                        let vasTranslation = vasFk.Translations.FirstOrDefault(x=> x.Language.Contains(CultureInfo.CurrentUICulture.Name))
                        select new ChartCategoryPairedValuesDto()
                        {
                            X = vasTranslation != null ? vasTranslation.DisplayName : vasFk.Key,
                            Y = vasGroup.Count()
                        }).Distinct()
                    .OrderByDescending(r => r.Y)
                .Take(5)
                .ToList();

             return mostVasesUsed;
        }

                public async Task<AcceptedAndRejectedRequestsListDto> GetAcceptedAndRejectedRequests(FilterDatePeriod period)
        {
            DisableTenancyFilters();

        DateTime startOfCurrentWeek = Clock.Now.StartOfWeek(DayOfWeek.Sunday).Date;
        DateTime endOfCurrentWeek = startOfCurrentWeek.AddDays(7).Date;
        
            var query = _priceOfferRepository
                .GetAll()
                .AsNoTracking()
                .Where(x => x.TenantId == AbpSession.TenantId)
                .WhereIf(period == FilterDatePeriod.Daily,x=>  x.CreationTime.Date >= startOfCurrentWeek && x.CreationTime.Date <= endOfCurrentWeek )
                .WhereIf(period == FilterDatePeriod.Weekly,x=>  x.CreationTime.Year == Clock.Now.Year && x.CreationTime.Month == Clock.Now.Month )
                .WhereIf(period == FilterDatePeriod.Monthly,x=>  x.CreationTime.Year == Clock.Now.Year )
                .Select(x => new { x.Status, x.CreationTime });

            var accepted =  await query.Where(x => x.Status == PriceOfferStatus.Accepted).ToListAsync();
            var rejected = await query.Where(x => x.Status == PriceOfferStatus.Rejected).ToListAsync();

            var acceptedOffers = new List<ChartCategoryPairedValuesDto>();
            var rejectedOffers = new List<ChartCategoryPairedValuesDto>();
            if (period == FilterDatePeriod.Daily)
            {
                acceptedOffers = accepted.GroupBy(x => x.CreationTime.DayOfWeek)
                    .Select(x => new ChartCategoryPairedValuesDto() { X = x.Key.ToString(), Y = x.Count() })
                    .ToList();
                
                rejectedOffers = rejected.GroupBy(x => x.CreationTime.DayOfWeek)
                    .Select(x => new ChartCategoryPairedValuesDto() { X = x.Key.ToString(), Y = x.Count() })
                    .ToList();
            }

            if (period == FilterDatePeriod.Monthly)
            {
                acceptedOffers = accepted.GroupBy(x => x.CreationTime.Month)
                    .Select(x => new ChartCategoryPairedValuesDto() { X = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(x.Key ),Y = x.Count() })
                    .ToList();
                
                rejectedOffers = rejected.GroupBy(x => x.CreationTime.Month)
                    .Select(x => new ChartCategoryPairedValuesDto() { X = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(x.Key), Y = x.Count() })
                    .ToList();
            }
            
            if (period == FilterDatePeriod.Weekly)
            {
                int dayOfCurrentMonth = Clock.Now.Day;
                DateTime firstMonthDay = new DateTime(Clock.Now.Year, Clock.Now.Month, 1);
                DateTime firstMonthSunday = firstMonthDay.AddDays((DayOfWeek.Sunday + 7 - firstMonthDay.DayOfWeek) % 7);
                
                acceptedOffers = accepted.GroupBy(x => ((x.CreationTime.Day - firstMonthSunday.Day) / 7) +1)
                    .Select(x => new ChartCategoryPairedValuesDto() { X = $"Week {x.Key}",Y = x.Count() })
                    .ToList();
                
                rejectedOffers = rejected.GroupBy(x => ((x.CreationTime.Day - firstMonthSunday.Day) / 7) +1)
                    .Select(x => new ChartCategoryPairedValuesDto() { X = $"Week {x.Key}", Y = x.Count() })
                    .ToList();
            }

            if (period == FilterDatePeriod.Yearly)
            {
                var allYears = new int[(Clock.Now.Year - 2020) + 1];

                int Year = 2020;
                for (var i = 0; i <= Clock.Now.Year - 2020; i++)
                {
                    allYears[i] = Year;
                    Year++;
                }

                foreach (var year in allYears)
                {
                    var groupedPaid = accepted.GroupBy(x => x.CreationTime.Date.Year);
                    var groupedRejected = rejected.GroupBy(x => x.CreationTime.Year);

                    if (groupedPaid.Select(x => x.Key).ToList().Contains(year))
                    {
                        acceptedOffers.Add(groupedPaid.Where(x => x.Key == year)
                    .Select(x => new ChartCategoryPairedValuesDto() { X = $"{x.Key}", Y = x.Count() })
                    .FirstOrDefault());
                    }
                    else
                    {
                        acceptedOffers.Add(new ChartCategoryPairedValuesDto { X = $"{year}", Y = 0 });
                    }
                    if (groupedRejected.Select(x => x.Key).ToList().Contains(year))
                    {
                        rejectedOffers.Add(groupedRejected.Where(x => x.Key == year)
                    .Select(x => new ChartCategoryPairedValuesDto() { X = $"{x.Key}", Y = x.Count() })
                    .FirstOrDefault());
                    }
                    else
                    {
                        rejectedOffers.Add(new ChartCategoryPairedValuesDto { X = $"{year}", Y = 0 });
                    }
                }
            }

            return new AcceptedAndRejectedRequestsListDto
            {
                AcceptedOffers = acceptedOffers,
                RejectedOffers = rejectedOffers
            };
        }


        public async Task<GetCarrierInvoicesDetailsOutput> GetCarrierInvoicesDetails(FilterDatePeriod period)
        {
            DisableTenancyFilters();


            var query = _submitInvoiceRepository
                .GetAll()
                .AsNoTracking()
                .Where(x => x.TenantId == AbpSession.TenantId && ((Clock.Now.Month == 12 && x.CreationTime.Year == Clock.Now.Year) ||
                (Clock.Now.Month != 12 && (x.CreationTime.Year == Clock.Now.Year || x.CreationTime.Year == Clock.Now.AddYears(-1).Year))));

            var paidList = await query
                    .Where(x => x.Status == SubmitInvoiceStatus.Paid)
                    .ToListAsync();


            var claimedList = await query
               .Where(x => x.Status == SubmitInvoiceStatus.Claim)
               .ToListAsync();


            var newInvoicesList = await query
                 .Where(x => x.Status == SubmitInvoiceStatus.New)
                 .ToListAsync();


            var totalList = await query
                    .ToListAsync();

            var paid = new List<ChartCategoryPairedValuesDto>();
            var claimed = new List<ChartCategoryPairedValuesDto>();
            var newInvoices = new List<ChartCategoryPairedValuesDto>();
            var total = new List<ChartCategoryPairedValuesDto>();

            if (period == FilterDatePeriod.Daily)
            {
                paid = paidList.GroupBy(x => x.CreationTime.DayOfWeek)
                    .Select(x => new ChartCategoryPairedValuesDto() { X = x.Key.ToString(), Y = x.Count() })
                .ToList();

                claimed = claimedList.GroupBy(x => x.CreationTime.DayOfWeek)
                    .Select(x => new ChartCategoryPairedValuesDto() { X = x.Key.ToString(), Y = x.Count() })
                    .ToList();

                newInvoices = newInvoicesList.GroupBy(x => x.CreationTime.DayOfWeek)
                    .Select(x => new ChartCategoryPairedValuesDto() { X = x.Key.ToString(), Y = x.Count() })
                .ToList();

                total = totalList.GroupBy(x => x.CreationTime.DayOfWeek)
                    .Select(x => new ChartCategoryPairedValuesDto() { X = x.Key.ToString(), Y = x.Count() })
                    .ToList();
            }

            if (period == FilterDatePeriod.Monthly)
            {
                var groupedPaidList = paidList.GroupBy(x => x.CreationTime.Month);
                var groupedClaimList = claimedList.GroupBy(x => x.CreationTime.Month);
                var groupedNewInvoiceList = newInvoicesList.GroupBy(x => x.CreationTime.Month);
                var groupedTotolList = totalList.GroupBy(x=>x.CreationTime.Month);

                foreach (var date in _dashboardDomainService.GetYearMonthsEndWithCurrent())
                {
                    if (groupedPaidList.Select(x => x.Key).ToList().Contains(date.Month))
                    {
                        paid.Add(groupedPaidList.Where(x => x.Key == date.Month)
                    .Select(x => new ChartCategoryPairedValuesDto() { X = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(x.Key), Y = x.Count() })
                    .FirstOrDefault());
                    }
                    else
                    {
                        paid.Add(new ChartCategoryPairedValuesDto { X = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(date.Month), Y = 0 });
                    }

                    if (groupedClaimList.Select(x => x.Key).ToList().Contains(date.Month))
                    {
                        claimed.Add(groupedClaimList.Where(x => x.Key == date.Month)
                    .Select(x => new ChartCategoryPairedValuesDto() { X = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(x.Key), Y = x.Count() })
                    .FirstOrDefault());
                    }
                    else
                    {
                        claimed.Add(new ChartCategoryPairedValuesDto { X = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(date.Month), Y = 0 });
                    }

                    if (groupedNewInvoiceList.Select(x => x.Key).ToList().Contains(date.Month))
                    {
                        newInvoices.Add(groupedNewInvoiceList.Where(x => x.Key == date.Month)
                    .Select(x => new ChartCategoryPairedValuesDto() { X = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(x.Key), Y = x.Count() })
                    .FirstOrDefault());
                    }
                    else
                    {
                        newInvoices.Add(new ChartCategoryPairedValuesDto { X = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(date.Month), Y = 0 });
                    }


                    if (groupedTotolList.Select(x => x.Key).ToList().Contains(date.Month))
                    {
                        total.Add(groupedTotolList.Where(x => x.Key == date.Month)
                    .Select(x => new ChartCategoryPairedValuesDto() { X = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(x.Key), Y = x.Count() })
                    .FirstOrDefault());
                    }
                    else
                    {
                        total.Add(new ChartCategoryPairedValuesDto { X = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(date.Month), Y = 0 });
                    }
                }
            }

            if (period == FilterDatePeriod.Weekly)
            {
                int dayOfCurrentMonth = Clock.Now.Day;
                DateTime firstMonthDay = new DateTime(Clock.Now.Year, Clock.Now.Month, 1);
                DateTime firstMonthSunday = firstMonthDay.AddDays((DayOfWeek.Sunday + 7 - firstMonthDay.DayOfWeek) % 7);

                paid = paidList.GroupBy(x => ((x.CreationTime.Day - firstMonthSunday.Day) / 7) + 1)
                    .Select(x => new ChartCategoryPairedValuesDto() { X = $"Week {x.Key}", Y = x.Count() })
                    .ToList();

                claimed = claimedList.GroupBy(x => ((x.CreationTime.Day - firstMonthSunday.Day) / 7) + 1)
                    .Select(x => new ChartCategoryPairedValuesDto() { X = $"Week {x.Key}", Y = x.Count() })
                    .ToList();

                newInvoices = newInvoicesList.GroupBy(x => ((x.CreationTime.Day - firstMonthSunday.Day) / 7) + 1)
                    .Select(x => new ChartCategoryPairedValuesDto() { X = $"Week {x.Key}", Y = x.Count() })
                    .ToList();

                total = totalList.GroupBy(x => ((x.CreationTime.Day - firstMonthSunday.Day) / 7) + 1)
                    .Select(x => new ChartCategoryPairedValuesDto() { X = $"Week {x.Key}", Y = x.Count() })
                    .ToList();
            }

            if (period == FilterDatePeriod.Yearly)
            {
                var allYears = new int[(Clock.Now.Year - 2020) + 1];

                int Year = 2020;
                for (var i = 0; i <= Clock.Now.Year - 2020; i++)
                {
                    allYears[i] = Year;
                    Year++;
                }

                foreach (var year in allYears)
                {
                    var groupedPaid = paidList.GroupBy(x => x.CreationTime.Date.Year);
                    var groupedClaimed = claimedList.GroupBy(x => x.CreationTime.Year);
                    var groupedNewInvoices = newInvoicesList.GroupBy(x => x.CreationTime.Year);
                    var groupedTotal = totalList.GroupBy(x => x.CreationTime.Year);

                    if (groupedPaid.Select(x => x.Key).ToList().Contains(year))
                    {
                        paid.Add(groupedPaid.Where(x => x.Key == year)
                    .Select(x => new ChartCategoryPairedValuesDto() { X = $"{x.Key}", Y = x.Count() })
                    .FirstOrDefault());
                    }
                    else
                    {
                        paid.Add(new ChartCategoryPairedValuesDto { X = $"{year}", Y = 0 });
                    }
                    if (groupedClaimed.Select(x => x.Key).ToList().Contains(year))
                    {
                        claimed.Add(groupedClaimed.Where(x => x.Key == year)
                    .Select(x => new ChartCategoryPairedValuesDto() { X = $"{x.Key}", Y = x.Count() })
                    .FirstOrDefault());
                    }
                    else
                    {
                        claimed.Add(new ChartCategoryPairedValuesDto { X = $"{year}", Y = 0 });
                    }

                    if (groupedNewInvoices.Select(x => x.Key).ToList().Contains(year))
                    {
                        newInvoices.Add(groupedNewInvoices.Where(x => x.Key == year)
                    .Select(x => new ChartCategoryPairedValuesDto() { X = $"{x.Key}", Y = x.Count() })
                    .FirstOrDefault());
                    }
                    else
                    {
                        newInvoices.Add(new ChartCategoryPairedValuesDto { X = $"{year}", Y = 0 });
                    }

                    if (groupedTotal.Select(x => x.Key).ToList().Contains(year))
                    {
                        total.Add(groupedTotal.Where(x => x.Key == year)
                    .Select(x => new ChartCategoryPairedValuesDto() { X = $"{x.Key}", Y = x.Count() })
                    .FirstOrDefault());
                    }
                    else
                    {
                        total.Add(new ChartCategoryPairedValuesDto { X = $"{year}", Y = 0 });
                    }
                }
            }


            return new GetCarrierInvoicesDetailsOutput
            {
                PaidInvoices = paid,
                NewInvoices = newInvoices,
                Total = total,
                Claimed = claimed
            };
        }


        public async Task<List<ChartCategoryPairedValuesDto>> GetMostPricePackageByShippers()
        {
            //DisableTenancyFilters();

            return (await _pricePackageRepository
                    .GetAll()
                    .AsNoTracking()
                    //.Include(r => r.Tenant)
                    .ToListAsync())
                .GroupBy(r => new { r.DisplayName })
                .Select(vas => new ChartCategoryPairedValuesDto()
                {
                    X = vas.Key.DisplayName,
                    Y = vas.Count()
                }).Distinct()
                .OrderByDescending(r => r.Y)
                .Take(5)
                .ToList();
        }

        private static List<string> GetDistinctDestinations(IEnumerable<City> cities)
        {
            return cities.Select(x => x.DisplayName).Distinct().ToList();
        }

    }

    public class GetCarrierInvoicesDetailsOutput
    {
        public List<ChartCategoryPairedValuesDto> PaidInvoices { get; set; }
        public List<ChartCategoryPairedValuesDto> NewInvoices { get; set; }
        public List<ChartCategoryPairedValuesDto> Total { get; set; }
        public List<ChartCategoryPairedValuesDto> Claimed { get; set; }
    }
}