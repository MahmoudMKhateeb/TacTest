using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Abp.Runtime.Session;
using Abp.Timing;
using Abp.UI;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using TACHYON.Authorization;
using TACHYON.Dashboards.Host.Dto;
using TACHYON.Dashboards.Shipper;
using TACHYON.Dashboards.Shipper.Dto;
using TACHYON.Documents.DocumentFiles;
using TACHYON.Dto;
using TACHYON.Features;
using TACHYON.Invoices;
using TACHYON.Invoices.SubmitInvoices;
using TACHYON.MultiTenancy;
using TACHYON.Offers;
using TACHYON.PriceOffers;
using TACHYON.Routs.RoutPoints;
using TACHYON.Shipping.RoutPoints;
using TACHYON.Shipping.ShippingRequests;
using TACHYON.Shipping.ShippingRequestTrips;
using TACHYON.Shipping.Trips;
using TACHYON.Tenants.Dashboard.Dto;
using TACHYON.Tracking;
using TACHYON.Trucks.TrucksTypes;

namespace TACHYON.Dashboards.Shipper
{
    [AbpAuthorize(AppPermissions.Pages_ShipperDashboard)]
    public class ShipperDashboardAppService : TACHYONAppServiceBase
    {
        private readonly IRepository<ShippingRequest, long> _shippingRequestRepository;
        private readonly IRepository<ShippingRequestTrip> _shippingRequestTripRepository;
        private readonly IRepository<DocumentFile, Guid> _documentFileRepository;
        private readonly IRepository<Invoice, long> _invoiceRepository;
        private readonly IRepository<PriceOffer, long> _priceOffersRepository;
        private readonly IRepository<ShippingRequestTripAccident> _accidentRepository;
        private readonly IRepository<TrucksType,long> _truckTypesRepository;
        private readonly IRepository<RoutPoint,long> _routePointRepository;
        private readonly IRepository<RoutPointStatusTransition> _transitionRepository;
        private readonly IRepository<SubmitInvoice, long> _submitInvoiceRepository;

        public ShipperDashboardAppService(
             IRepository<ShippingRequest, long> shippingRequestRepository,
             IRepository<ShippingRequestTrip> shippingRequestTripRepository,
             IRepository<DocumentFile, Guid> documentFileRepository,
             IRepository<Invoice, long> invoiceRepository,
             IRepository<PriceOffer, long> priceOffersRepository,
             IRepository<ShippingRequestTripAccident> accidentRepository,
             IRepository<TrucksType, long> truckTypesRepository,
             IRepository<RoutPoint, long> routePointRepository,
             IRepository<RoutPointStatusTransition> transitionRepository,
             IRepository<SubmitInvoice, long> submitInvoiceRepository)
        {
            _shippingRequestRepository = shippingRequestRepository;
            _shippingRequestTripRepository = shippingRequestTripRepository;
            _documentFileRepository = documentFileRepository;
            _invoiceRepository = invoiceRepository;
            _priceOffersRepository = priceOffersRepository;
            _accidentRepository = accidentRepository;
            _truckTypesRepository = truckTypesRepository;
            _routePointRepository = routePointRepository;
            _transitionRepository = transitionRepository;
            _submitInvoiceRepository = submitInvoiceRepository;
        }



        public async Task<int> GetDeliveredTripsCountForCurrentWeek()
        {
            bool isBroker = await FeatureChecker.IsEnabledAsync(AppFeatures.CMS);
            
            DateTime startOfCurrentWeek = Clock.Now.StartOfWeek(DayOfWeek.Sunday).Date;
            DateTime endOfCurrentWeek = startOfCurrentWeek.AddDays(7).Date;
            DisableTenancyFilters();
            
            return await (from trip in _shippingRequestTripRepository.GetAll()
                where ((!isBroker && trip.ShippingRequestFk.TenantId == AbpSession.TenantId) || (isBroker &&
                          (trip.ShippingRequestFk.TenantId == AbpSession.TenantId ||
                           trip.ShippingRequestFk.CarrierTenantId == AbpSession.TenantId)))
                      && trip.Status == ShippingRequestTripStatus.Delivered
                let lastWorkingDate = _transitionRepository.GetAll()
                    .Where(x => x.RoutPointFK.ShippingRequestTripId == trip.Id && !x.IsReset)
                    .OrderByDescending(x => x.CreationTime)
                    .Select(x => x.CreationTime).FirstOrDefault()
                where lastWorkingDate >= startOfCurrentWeek && lastWorkingDate <= endOfCurrentWeek
                select trip).CountAsync();
        }
        
        public async Task<int> GetInTransitTripsCount()
        {
            DisableTenancyFilters();
            
            bool isBroker = await FeatureChecker.IsEnabledAsync(AppFeatures.CMS);
            
            return await _shippingRequestTripRepository.GetAll()
                .WhereIf(!isBroker,x=> x.ShippingRequestFk.TenantId == AbpSession.TenantId)
                .WhereIf(isBroker,x=> x.ShippingRequestFk.TenantId == AbpSession.TenantId || x.ShippingRequestFk.CarrierTenantId == AbpSession.TenantId)
                .CountAsync(x =>x.Status == ShippingRequestTripStatus.InTransit);
        }
        
        public async Task<List<UpcomingTripsOutput>> GetUpcomingTrips()
        {
            DateTime currentDay = Clock.Now.Date;
            DateTime endOfCurrentWeek = currentDay.AddDays(7).Date; 
            bool isBroker = await FeatureChecker.IsEnabledAsync(AppFeatures.CMS);
            
            DisableTenancyFilters();

            // todo hide actors date when broker is request this service
            var trips = await (from trip in _shippingRequestTripRepository.GetAll().AsNoTracking()
                    .Include(x => x.OriginFacilityFk).ThenInclude(x => x.CityFk)
                    .Include(x => x.DestinationFacilityFk).ThenInclude(x => x.CityFk)
                where ((!isBroker && trip.ShippingRequestFk.TenantId == AbpSession.TenantId) || (isBroker &&
                          (trip.ShippingRequestFk.TenantId == AbpSession.TenantId ||
                           trip.ShippingRequestFk.CarrierTenantId == AbpSession.TenantId))) &&
                      (!trip.ShippingRequestFk.CarrierActorId.HasValue && !trip.ShippingRequestFk.ShipperActorId.HasValue)&&
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
            bool isBroker = await FeatureChecker.IsEnabledAsync(AppFeatures.CMS);
            if (isBroker) DisableTenancyFilters();
            
            var trips = await (from point in _routePointRepository.GetAll()
                where ((!isBroker && point.ShippingRequestTripFk.ShippingRequestFk.TenantId == AbpSession.TenantId) || (isBroker &&
                    (point.ShippingRequestTripFk.ShippingRequestFk.TenantId == AbpSession.TenantId ||
                     point.ShippingRequestTripFk.ShippingRequestFk.CarrierTenantId == AbpSession.TenantId)))&&
                    (!point.ShippingRequestTripFk.ShippingRequestFk.CarrierActorId.HasValue && !point.ShippingRequestTripFk.ShippingRequestFk.ShipperActorId.HasValue)
                      && point.ShippingRequestTripFk.Status == ShippingRequestTripStatus.DeliveredAndNeedsConfirmation
                      && point.PickingType == PickingType.Dropoff && !point.IsComplete && (!point.IsPodUploaded || !point.ShippingRequestTripFk.EndWorking.HasValue)
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

        public async Task<List<NewPriceOfferListDto>> GetNewPriceOffers()
        {
            DisableTenancyFilters();
            var offers = await _priceOffersRepository.GetAll()
                .Where(x => x.ShippingRequestFk.TenantId == AbpSession.TenantId && x.Status == PriceOfferStatus.New
                    && (x.ShippingRequestFk.Status == ShippingRequestStatus.NeedsAction))
                .Select(x => new NewPriceOfferListDto()
                {
                    ReferenceNumber = x.ShippingRequestFk.ReferenceNumber,
                    CompanyName = x.ShippingRequestFk.RequestType == ShippingRequestType.TachyonManageService
                        ? LocalizationSource.GetString("TachyonManageService")
                        : x.Tenant.companyName,
                    ShippingRequestId = x.ShippingRequestId
                }).ToListAsync();

            return offers;
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

        public async Task<AcceptedAndRejectedRequestsListDto> GetAcceptedAndRejectedRequests(FilterDatePeriod period)
        {
            DisableTenancyFilters();

            bool isBroker = await FeatureChecker.IsEnabledAsync(AppFeatures.CMS);
            DateTime startOfCurrentWeek = Clock.Now.StartOfWeek(DayOfWeek.Sunday).Date;
            DateTime endOfCurrentWeek = startOfCurrentWeek.AddDays(7).Date;
        
            var query = _priceOffersRepository
                .GetAll()
                .AsNoTracking()
                .WhereIf(!isBroker,x=> x.ShippingRequestFk.TenantId == AbpSession.TenantId)
                .WhereIf(isBroker,x=> x.ShippingRequestFk.TenantId == AbpSession.TenantId || x.ShippingRequestFk.CarrierTenantId == AbpSession.TenantId)
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

            return new AcceptedAndRejectedRequestsListDto
            {
                AcceptedOffers = acceptedOffers,
                RejectedOffers = rejectedOffers
            };
        }

        public async Task<List<MostTenantWorksListDto>> GetMostWorkedWithCarriers()
        {
            DisableTenancyFilters();

            var trips = await _shippingRequestTripRepository
                 .GetAll()
                 .Include(r => r.ShippingRequestFk)
                 .ThenInclude(x => x.CarrierTenantFk)
                 .AsNoTracking()
                 .Where(t => t.ShippingRequestFk.TenantId == AbpSession.TenantId && t.ShippingRequestFk.CarrierTenantId.HasValue)
                 .Select
                 (
                     x => new
                     {
                         CarrierId = x.ShippingRequestFk.CarrierTenantFk.Id,
                         CarrierName = x.ShippingRequestFk.CarrierTenantFk.TenancyName,
                         CarrierRating = x.ShippingRequestFk.CarrierTenantFk.Rate
                     }
                 )
                 .ToListAsync();

            return trips
                .GroupBy(x => x.CarrierId)
                .Select(
                    g => new MostTenantWorksListDto()
                    {
                        Id = g.Key,
                        Name = g.FirstOrDefault()?.CarrierName,
                        Rating = g.FirstOrDefault()?.CarrierRating,
                        NumberOfTrips = g.Count(),
                    })
             .OrderByDescending(x => x.NumberOfTrips)
             .Take(5)
             .ToList();


        }

        public async Task<CompletedTripVsPodListDto> GetCompletedTripVsPod(FilterDatePeriod period)
        {
            DisableTenancyFilters();

            bool isBroker = await FeatureChecker.IsEnabledAsync(AppFeatures.CMS);

            DateTime startOfCurrentWeek = Clock.Now.StartOfWeek(DayOfWeek.Sunday).Date;
            DateTime endOfCurrentWeek = startOfCurrentWeek.AddDays(7).Date;

            var query = _shippingRequestTripRepository
                .GetAll()
                .AsNoTracking()
                .WhereIf(!isBroker,x=> x.ShippingRequestFk.TenantId == AbpSession.TenantId)
                .WhereIf(isBroker,x=> x.ShippingRequestFk.TenantId == AbpSession.TenantId || x.ShippingRequestFk.CarrierTenantId == AbpSession.TenantId)
                .WhereIf(period == FilterDatePeriod.Daily,x=>  x.CreationTime.Date >= startOfCurrentWeek && x.CreationTime.Date <= endOfCurrentWeek )
                .WhereIf(period == FilterDatePeriod.Weekly,x=>  x.CreationTime.Year == Clock.Now.Year && x.CreationTime.Month == Clock.Now.Month )
                .WhereIf(period == FilterDatePeriod.Monthly,x=>  x.CreationTime.Year == Clock.Now.Year )
                .Where(x => x.Status == ShippingRequestTripStatus.Delivered);


            var podTrips = await query
                    .Where(x => x.RoutPoints.Any(p => p.IsPodUploaded))
                    .Select(x=> new {x.CreationTime,x.Id}).ToListAsync();
            
            var total = await query.Select(x=> new {x.CreationTime,x.Id}).ToListAsync();
                
            
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
                podTripsList = podTrips.GroupBy(x => x.CreationTime.Month)
                    .Select(x => new ChartCategoryPairedValuesDto() { X = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(x.Key ),Y = x.Count() })
                    .ToList();
                
                totalList = total.GroupBy(x => x.CreationTime.Month)
                    .Select(x => new ChartCategoryPairedValuesDto() { X = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(x.Key), Y = x.Count() })
                    .ToList();
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

        public async Task<InvoicesVsPaidInvoicesDto> GetInvoicesVSPaidInvoices()
        {
            DisableTenancyFilters();


            var query = _invoiceRepository
                .GetAll()
                .AsNoTracking()
                .Where(x => x.TenantId == AbpSession.TenantId);

            var paid = (await query
                    .Where(x => x.IsPaid)
                    .ToListAsync())
                .GroupBy(x => x.CreationTime.Date.Month)
                    .Select(g => new ChartCategoryPairedValuesDto
                    {
                        X = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(g.Key),
                        Y = g.Count()
                    })
                    .OrderBy(x => x.X)
                    .ToList();

            var total = (await query
                    .ToListAsync())
                    .GroupBy(x => x.CreationTime.Date.Month)
                    .Select(g => new ChartCategoryPairedValuesDto
                    {
                        X = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(g.Key),
                        Y = g.Count()
                    })
                    .OrderBy(x => x.X)
                    .ToList();


            return new InvoicesVsPaidInvoicesDto
            {
                PaidInvoices = paid,
                ShipperInvoices = total
            };
        }

        public async Task<List<RequestsInMarketpalceDto>> GetRequestsInMarketpalce()
        {
            DisableTenancyFilters();


            var query = _shippingRequestRepository
                .GetAll()
                .Where(r => r.RequestType == ShippingRequestType.Marketplace)
                .Where(r => r.TenantId == AbpSession.TenantId)
                .Where(r => r.TenantId == AbpSession.TenantId)
                .Where(r => !r.CarrierTenantId.HasValue)
                .Where(r => (r.BidEndDate.HasValue && r.BidEndDate.Value.Date >= Clock.Now.Date) || !r.BidEndDate.HasValue);


            return await query.Select(x => new RequestsInMarketpalceDto
            {
                BiddingEndDate = x.BidEndDate,
                NumberOfOffers = x.TotalOffers,
                RequestReference = x.ReferenceNumber
            }).ToListAsync();


        }


        public async Task<List<MostUsedOriginsDto>> GetMostUsedOrigins()
        {
            DisableTenancyFilters();

            return (await _shippingRequestTripRepository
                    .GetAll()
                    .AsNoTracking()
                    .Where(x => x.ShippingRequestFk.TenantId == AbpSession.TenantId)
                    .Select(x => new
                    {
                        cityDisplayName = x.OriginFacilityFk.CityFk.DisplayName,
                        x.Id
                    })
                    .ToListAsync())
                    .GroupBy(r => r.cityDisplayName)
                    .Select(g => new MostUsedOriginsDto()
                    {
                        CityName = g.Key,
                        NumberOfRequests = g.Count()  //number of trips
                    })
                    .OrderByDescending(r => r.NumberOfRequests)
                    .Take(5)
                    .ToList();
        }

        public async Task<List<MostUsedOriginsDto>> GetMostUsedDestinatiions()
        {
            DisableTenancyFilters();

            return (await _shippingRequestTripRepository
                    .GetAll()
                    .AsNoTracking()
                    .Where(x => x.ShippingRequestFk.TenantId == AbpSession.TenantId)
                                        .Select(x => new
                                        {
                                            cityDisplayName = x.DestinationFacilityFk.CityFk.DisplayName,
                                            x.Id
                                        })
                    .ToListAsync())
                    .GroupBy(r => r.cityDisplayName)
                    .Select(g => new MostUsedOriginsDto()
                    {
                        CityName = g.Key,
                        NumberOfRequests = g.Count() //number of trips
                    })
                    .OrderByDescending(r => r.NumberOfRequests)
                    .Take(5)
                    .ToList();
        }


        public async Task<long> GetDocumentsDueDateInDays()
        {
            DisableTenancyFilters();

            return await _documentFileRepository
                .GetAll()
                .AsNoTracking()
                .Where(x => x.TenantId == AbpSession.TenantId)
                .Where(x => x.IsAccepted)
                .Where(x => x.ExpirationDate.HasValue)
                .Where(x => x.ExpirationDate.Value.Date <= Clock.Now.Date.AddDays(5))
                .CountAsync();

        }


        public async Task<long> GetInvoiceDueDateInDays(BrokerInvoiceType? invoiceType)
        {
            DisableTenancyFilters();

            bool isBroker = await FeatureChecker.IsEnabledAsync(AppFeatures.CMS);

            return isBroker switch
            {
                true when !invoiceType.HasValue => throw new UserFriendlyException(L("YouMustProvideInvoiceType")),
                true when invoiceType == BrokerInvoiceType.SubmitInvoice => await _submitInvoiceRepository.GetAll()
                    .Where(x => x.TenantId == AbpSession.TenantId && x.DueDate.HasValue &&
                                x.DueDate <= Clock.Now.Date.AddDays(5)).CountAsync(),
                _ => await _invoiceRepository.GetAll()
                    .AsNoTracking()
                    .Where(r => r.TenantId == AbpSession.TenantId && !r.IsPaid && r.DueDate <= Clock.Now.Date.AddDays(5)).CountAsync()
            };
        }

        // Tracking Map
        public async Task<PagedResultDto<TrackingMapDto>> GetTrackingMap(GetTripsForTrackingInput input)
        {
            DisableTenancyFilters();
            var trips = _shippingRequestTripRepository.GetAll()
            .Include(r => r.ShippingRequestFk)
            .ThenInclude(r => r.Tenant)
            .Include(r => r.ShippingRequestFk).ThenInclude(x=> x.TrucksTypeFk)
            .ThenInclude(x=> x.Translations)
            .Include(r => r.RoutPoints)
            .ThenInclude(r => r.FacilityFk)
            .Include(x => x.OriginFacilityFk)
            .ThenInclude(x => x.CityFk).ThenInclude(x=> x.Translations)
            .Include(x => x.DestinationFacilityFk)
            .ThenInclude(x => x.CityFk).ThenInclude(x=> x.Translations)
            .AsNoTracking()
            .WhereIf(await IsEnabledAsync(AppFeatures.Carrier), x => x.ShippingRequestFk.CarrierTenantId == AbpSession.TenantId)
            .WhereIf(await IsEnabledAsync(AppFeatures.Shipper), x => x.ShippingRequestFk.TenantId == AbpSession.TenantId)
            .WhereIf(!input.WaybillNumber.IsNullOrEmpty(),x=> x.WaybillNumber.HasValue && x.WaybillNumber.ToString().Contains(input.WaybillNumber))
            .WhereIf(input.TruckTypeId.HasValue,x=> x.ShippingRequestFk.TrucksTypeId == input.TruckTypeId)
            .WhereIf(input.RouteType.HasValue,x=> x.ShippingRequestFk.RouteTypeId == input.RouteType)
            .WhereIf(input.SourceCityId.HasValue,x=> x.OriginFacilityFk.CityId == input.SourceCityId )
            .WhereIf(!string.IsNullOrEmpty(input.ContainerNumber), x => x.ContainerNumber == input.ContainerNumber)
            .WhereIf(input.DestinationCityId.HasValue,x=> x.DestinationFacilityFk.CityId == input.DestinationCityId )
            .WhereIf(!input.DriverName.IsNullOrEmpty(),x=> x.AssignedDriverUserFk.Name.Contains(input.DriverName) || x.AssignedDriverUserFk.Surname.Contains(input.DriverName))
            .Where(r => r.Status == ShippingRequestTripStatus.InTransit && r.CreationTime.Year == Clock.Now.Year)
            .Select(s => new TrackingMapDto()
            {
                DestinationCity = s.DestinationFacilityFk.CityFk.Translations.FirstOrDefault(t=> t.Language.Contains(CultureInfo.CurrentUICulture.Name)) == null
                    ? s.DestinationFacilityFk.CityFk.DisplayName: s.DestinationFacilityFk.CityFk.Translations
                        .FirstOrDefault(t=> t.Language.Contains(CultureInfo.CurrentUICulture.Name)).TranslatedDisplayName ,
                OriginCity = s.OriginFacilityFk.CityFk.Translations.FirstOrDefault(t=> t.Language.Contains(CultureInfo.CurrentUICulture.Name)) == null
                    ? s.OriginFacilityFk.CityFk.DisplayName: s.OriginFacilityFk.CityFk.Translations
                        .FirstOrDefault(t=> t.Language.Contains(CultureInfo.CurrentUICulture.Name)).TranslatedDisplayName,
                DestinationLongitude = (s.DestinationFacilityFk.Location != null ? s.DestinationFacilityFk.Location.X : 0),
                DestinationLatitude = (s.DestinationFacilityFk.Location != null ? s.DestinationFacilityFk.Location.Y : 0),
                OriginLongitude = (s.OriginFacilityFk.Location != null ? s.OriginFacilityFk.Location.X : 0),
                OriginLatitude = (s.OriginFacilityFk.Location != null ? s.OriginFacilityFk.Location.Y : 0),
                Id = s.Id,
                TripStatus = s.Status,
                WayBillNumber = s.WaybillNumber.ToString(),
                RoutPoints = s.RoutPoints.Select(rp => new RoutePointsTripDto()
                {
                    Id = rp.Id,
                    Facility = rp.FacilityFk.Name,
                    PickingType = rp.PickingType.GetEnumDescription(),
                    WaybillNumber = rp.WaybillNumber,
                    Longitude = (rp.FacilityFk.Location != null ? rp.FacilityFk.Location.X : 0),
                    Latitude = (rp.FacilityFk.Location != null ? rp.FacilityFk.Location.Y : 0)
                }).ToList(),
                HasIncident = _accidentRepository.GetAll().Any(a=> a.RoutPointFK.ShippingRequestTripId == s.Id),
                TruckType = s.ShippingRequestFk.TrucksTypeFk.Translations.FirstOrDefault(t=> t.Language.Contains(CultureInfo.CurrentUICulture.Name)) == null
                    ? s.ShippingRequestFk.TrucksTypeFk.Key : s.ShippingRequestFk.TrucksTypeFk.Translations
                        .FirstOrDefault(t=> t.Language.Contains(CultureInfo.CurrentUICulture.Name)).DisplayName,
                DriverName = $"{s.AssignedDriverUserFk.Name} {s.AssignedDriverUserFk.Surname}",
                ExpectedDeliveryTime = s.ExpectedDeliveryTime.HasValue ? s.ExpectedDeliveryTime.ToString() : String.Empty,
                ContainerNumber = s.ContainerNumber
            });

            var pagedAndFilteredTrips = trips
               .OrderByDescending(r => r.Id)
               .PageBy(input);
          
            var totalCount = await trips.CountAsync();

            return new PagedResultDto<TrackingMapDto>(
                totalCount,
                await pagedAndFilteredTrips.ToListAsync()
            );

        }

        [AbpAuthorize]
        public async Task<List<SelectItemDto>> GetAllTruckTypesForDropdown()
        {
            var truckTypesList = await ( from truckType in _truckTypesRepository.GetAll().AsNoTracking()
                let truckTypeTrans = truckType.Translations.FirstOrDefault(t=> t.Language.Contains(CultureInfo.CurrentUICulture.Name))
                select new SelectItemDto()
                {
                    Id = truckType.Id.ToString(),
                    DisplayName = truckTypeTrans != null ? truckTypeTrans.DisplayName : truckType.Key
                }).ToListAsync();

            return truckTypesList;
        }

        #region Helpers
        private async Task<List<ChartCategoryPairedValuesDto>> GetCompletedTripsIfMonthly(GetDataByDateFilterInput input)
        {
            var year = Clock.Now.Year;
            var trips = await _shippingRequestTripRepository
                .GetAll()
                .AsNoTracking()
                .Where(x => x.Status == ShippingRequestTripStatus.Delivered && x.CreationTime.Year == Clock.Now.Year)
                .WhereIf(await IsEnabledAsync(AppFeatures.Carrier), x => x.ShippingRequestFk.CarrierTenantId == AbpSession.TenantId)
                .WhereIf(await IsEnabledAsync(AppFeatures.Shipper), x => x.ShippingRequestFk.TenantId == AbpSession.TenantId)
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
            .WhereIf(await IsEnabledAsync(AppFeatures.Carrier), x => x.ShippingRequestFk.CarrierTenantId == AbpSession.TenantId)
            .WhereIf(await IsEnabledAsync(AppFeatures.Shipper), x => x.ShippingRequestFk.TenantId == AbpSession.TenantId)
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

        #endregion

    }
}