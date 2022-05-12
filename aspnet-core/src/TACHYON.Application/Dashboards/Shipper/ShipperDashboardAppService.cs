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
using System.Threading.Tasks;
using TACHYON.Authorization;
using TACHYON.Dashboards.Host.Dto;
using TACHYON.Dashboards.Shipper;
using TACHYON.Dashboards.Shipper.Dto;
using TACHYON.Documents.DocumentFiles;
using TACHYON.Features;
using TACHYON.Invoices;
using TACHYON.MultiTenancy;
using TACHYON.Offers;
using TACHYON.PriceOffers;
using TACHYON.Routs.RoutPoints;
using TACHYON.Shipping.ShippingRequests;
using TACHYON.Shipping.ShippingRequestTrips;
using TACHYON.Shipping.Trips;
using TACHYON.Tenants.Dashboard.Dto;

namespace TACHYON.Dashboards.Shipper
{
    [AbpAuthorize(AppPermissions.Pages_ShipperDashboard)]
    public class ShipperDashboardAppService : TACHYONAppServiceBase
    {
        private readonly IRepository<ShippingRequest, long> _shippingRequestRepository;
        private readonly IRepository<ShippingRequestTrip> _shippingRequestTripRepository;
        private readonly IRepository<InvoiceTrip, long> _invoiceTripsRepository;
        private readonly IRepository<RoutPointDocument, long> _routePointDocumentRepository;
        private readonly IRepository<DocumentFile, Guid> _documentFileRepository;
        private readonly IRepository<Invoice, long> _invoiceRepository;
        private readonly IRepository<PriceOffer, long> _priceOffersRepository;

        public ShipperDashboardAppService(
             IRepository<ShippingRequest, long> shippingRequestRepository,
             IRepository<ShippingRequestTrip> shippingRequestTripRepository,
             IRepository<InvoiceTrip, long> invoiceTripsRepository,
             IRepository<RoutPointDocument, long> routePointDocumentRepository,
             IRepository<DocumentFile, Guid> documentFileRepository,
             IRepository<Invoice, long> invoiceRepository,
             IRepository<PriceOffer, long> priceOffersRepository)
        {
            _shippingRequestRepository = shippingRequestRepository;
            _shippingRequestTripRepository = shippingRequestTripRepository;
            _invoiceTripsRepository = invoiceTripsRepository;
            _routePointDocumentRepository = routePointDocumentRepository;
            _documentFileRepository = documentFileRepository;
            _invoiceRepository = invoiceRepository;
            _priceOffersRepository = priceOffersRepository;
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

        public async Task<AcceptedAndRejectedRequestsListDto> GetAcceptedAndRejectedRequests()
        {
            DisableTenancyFilters();


            var query = _priceOffersRepository
                .GetAll()
                .AsNoTracking()
                .Where(x => x.ShippingRequestFk.TenantId == AbpSession.TenantId)
                .Where(x => x.CreationTime.Year == Clock.Now.Year)
                .Select(x => new { x.Status, x.CreationTime.Month });

            var accepted = await query
                .Where(x => x.Status == PriceOfferStatus.Accepted)
                .GroupBy(x => x.Month)
                .Select(g => new ChartCategoryPairedValuesDto
                {
                    X = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(g.Key),
                    Y = g.Count()
                })
                .ToListAsync();

            var rejected = await query
                .Where(x => x.Status == PriceOfferStatus.Rejected)
                .GroupBy(x => x.Month)
                .Select(g => new ChartCategoryPairedValuesDto
                {
                    X = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(g.Key),
                    Y = g.Count()
                })
                .ToListAsync();

            return new AcceptedAndRejectedRequestsListDto
            {
                AcceptedOffers = accepted,
                RejectedOffers = rejected
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

        public async Task<CompletedTripVsPodListDto> GetCompletedTripVsPod()
        {
            DisableTenancyFilters();



            var query = _shippingRequestTripRepository
                .GetAll()
                .AsNoTracking()
                .Where(x => x.ShippingRequestFk.Tenant.Id == AbpSession.TenantId)
                .Where(x => x.Status == ShippingRequestTripStatus.Delivered);


            var podTrips = (await query
                    .Where(x => x.RoutPoints.Any(p => p.IsPodUploaded))
                    .ToListAsync())
                .GroupBy(x => x.CreationTime.Month)
                .OrderBy(x => x.Key)
                .Select(g => new ChartCategoryPairedValuesDto
                {
                    X = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(g.Key),
                    Y = g.Count()

                })
                .ToList();

            var total = (await query
                .ToListAsync())
                .GroupBy(x => x.CreationTime.Month)
                .OrderBy(x => x.Key)
                .Select(g => new ChartCategoryPairedValuesDto
                {
                    X = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(g.Key),
                    Y = g.Count()

                })
                .ToList();

            return new CompletedTripVsPodListDto
            {
                CompletedTrips = total,
                PODTrips = podTrips
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
                        NumberOfRequests = g.Count()
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
                        NumberOfRequests = g.Count()
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


        public async Task<long> GetInvoiceDueDateInDays()
        {
            DisableTenancyFilters();

            return await _invoiceRepository.GetAll()
            .AsNoTracking()
            .Where(r => r.TenantId == AbpSession.TenantId)
            .Where(r => !r.IsPaid)
            .Where(r => r.DueDate <= Clock.Now.Date.AddDays(5)).CountAsync();

        }

        // Tracking Map
        public async Task<PagedResultDto<TrackingMapDto>> GetTrackingMap(GetTripsForTrackingInput input)
        {
            DisableTenancyFilters();
            var trips = _shippingRequestTripRepository.GetAll()
            .Include(r => r.ShippingRequestFk)
            .ThenInclude(r => r.Tenant)
            .Include(r => r.RoutPoints)
            .ThenInclude(r => r.FacilityFk)
            .Include(x => x.OriginFacilityFk)
            .ThenInclude(x => x.CityFk)
            .Include(x => x.DestinationFacilityFk)
            .ThenInclude(x => x.CityFk)
            .AsNoTracking()
            .WhereIf(await IsEnabledAsync(AppFeatures.Carrier), x => x.ShippingRequestFk.CarrierTenantId == AbpSession.TenantId)
            .WhereIf(await IsEnabledAsync(AppFeatures.Shipper), x => x.ShippingRequestFk.TenantId == AbpSession.TenantId)
            .Where(r => r.Status == ShippingRequestTripStatus.InTransit && r.CreationTime.Year == Clock.Now.Year)
            .Select(s => new TrackingMapDto()
            {
                DestinationCity = s.DestinationFacilityFk.Name,
                OriginCity = s.OriginFacilityFk.Name,
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
                }).ToList()

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