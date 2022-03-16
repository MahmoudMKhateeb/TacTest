using Abp.Authorization;
using Abp.Domain.Repositories;
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
using TACHYON.Routs.RoutPoints;
using TACHYON.Shipping.ShippingRequests;
using TACHYON.Shipping.ShippingRequestTrips;
using TACHYON.Shipping.Trips;
using TACHYON.Tenants.Dashboard.Dto;

namespace TACHYON.Dashboards.Shipper
{
    [AbpAuthorize(AppPermissions.Pages_ShipperDashboard)]
    public class ShipperDashboardAppService : TACHYONAppServiceBase, IShipperDashboardAppService
    {
        private readonly IRepository<ShippingRequest, long> _shippingRequestRepository;
        private readonly IRepository<ShippingRequestTrip> _shippingRequestTripRepository;
        private readonly IRepository<InvoiceTrip, long> _invoiceTripsRepository;
        private readonly IRepository<RoutPointDocument, long> _routePointDocumentRepository;
        private readonly IRepository<DocumentFile, Guid> _documentFileRepository;
        private readonly IRepository<Invoice, long> _invoiceRepository;

        public ShipperDashboardAppService(
             IRepository<ShippingRequest, long> shippingRequestRepository,
             IRepository<ShippingRequestTrip> shippingRequestTripRepository,
             IRepository<InvoiceTrip, long> invoiceTripsRepository,
             IRepository<RoutPointDocument, long> routePointDocumentRepository,
             IRepository<DocumentFile, Guid> documentFileRepository,
             IRepository<Invoice, long> invoiceRepository

            )
        {
            _shippingRequestRepository = shippingRequestRepository;
            _shippingRequestTripRepository = shippingRequestTripRepository;
            _invoiceTripsRepository = invoiceTripsRepository;
            _routePointDocumentRepository = routePointDocumentRepository;
            _documentFileRepository = documentFileRepository;
            _invoiceRepository = invoiceRepository;
        }


        public async Task<List<ListPerMonthDto>> GetCompletedTripsCountPerMonth(GetDataByDateFilterInput input)
        {
            DisableTenancyFilters();

            var groupedTripsList = new List<ListPerMonthDto>();
            
            //daily => default before 30 day
            if (input.DatePeriod == FilterDatePeriod.Daily)
            {
                var TripsDailyList = await _shippingRequestTripRepository.GetAll().AsNoTracking()
                .Where(x => x.Status == ShippingRequestTripStatus.Delivered && x.CreationTime.Year == Clock.Now.Year && x.CreationTime > Clock.Now.AddDays(-30))
                .WhereIf(await IsEnabledAsync(AppFeatures.Carrier), x => x.ShippingRequestFk.CarrierTenantId == AbpSession.TenantId)
                .WhereIf(await IsEnabledAsync(AppFeatures.Shipper), x => x.ShippingRequestFk.TenantId == AbpSession.TenantId)
                .ToListAsync();
                var grouped = TripsDailyList
                    .GroupBy(r => new { r.CreationTime.Day, r.CreationTime.Year, r.CreationTime.Month })
                    .Select(s => new
                    {
                        list = s.ToList(),
                        Year = s.Key.Year,
                        Day = s.Key.Day,
                        Month = s.Key.Month.ToString()
                    }).ToList();
                groupedTripsList = grouped.Select(g => new ListPerMonthDto
                {
                    Year = g.Year,
                    Month = g.Month,
                    Day = g.Day,
                    Count = g.list.Count(),
                }).OrderBy(r => r.Day).ToList();
            }
            if (input.DatePeriod == FilterDatePeriod.Weekly)
            {
                DateTime firstDay = new DateTime(DateTime.Now.Year, 1, 1);

                var TripsWeeklyList = (from u in _shippingRequestTripRepository.GetAll().AsNoTracking().AsEnumerable()
                                       where u.Status == ShippingRequestTripStatus.Delivered && u.CreationTime.Year == Clock.Now.Year
                                       group u by new { u.CreationTime.Year, WeekNumber = (u.CreationTime - new DateTime(DateTime.Now.Year, 1, 1)).Days / 7 } into ut
                                       select new { list = ut.ToList(), Year = ut.Key.Year, Week = ut.Key.WeekNumber }).ToList();


                groupedTripsList = TripsWeeklyList.Select(x => new ListPerMonthDto
                {
                    Year = x.Year,
                    Week = x.Week,
                    Count = x.list.Count()
                }).OrderBy(r => r.Week).Distinct().ToList();

            }
            if (input.DatePeriod == FilterDatePeriod.Monthly)
            {
                var TripsMonthlyList = await _shippingRequestTripRepository.GetAll().AsNoTracking()
                .Where(x => x.Status == ShippingRequestTripStatus.Delivered && x.CreationTime.Year == Clock.Now.Year && x.CreationTime > Clock.Now.AddDays(-30))
                .WhereIf(await IsEnabledAsync(AppFeatures.Carrier), x => x.ShippingRequestFk.CarrierTenantId == AbpSession.TenantId)
                .WhereIf(await IsEnabledAsync(AppFeatures.Shipper), x => x.ShippingRequestFk.TenantId == AbpSession.TenantId)
                .ToListAsync();

                var grouped2 = TripsMonthlyList
            .GroupBy(r => new { r.CreationTime.Year, r.CreationTime.Month })
            .Select(s => new
            {
                list = s.ToList(),
                Year = s.Key.Year,
                Month = s.Key.Month.ToString()
            }).ToList();
                groupedTripsList = grouped2.Select(g => new ListPerMonthDto
                {
                    Year = g.Year,
                    Month = new DateTime(g.Year, Convert.ToInt16(g.Month), 1).ToString("MMM"),
                    Count = g.list.Count(),
                }).ToList();
            }
           
            return groupedTripsList;
        }

        public async Task<AcceptedAndRejectedRequestsListDto> GetAcceptedAndRejectedRequests()
        {
            DisableTenancyFilters();

            var list = new AcceptedAndRejectedRequestsListDto();
            
            var acceptedPricedRequests = await _shippingRequestRepository.GetAll().AsNoTracking()
                .Where(x => x.Status == ShippingRequestStatus.PostPrice && x.CreationTime.Year == Clock.Now.Year)
                .WhereIf(await IsEnabledAsync(AppFeatures.Carrier), x => x.CarrierTenantId == AbpSession.TenantId)
                .WhereIf(await IsEnabledAsync(AppFeatures.Shipper), x => x.TenantId == AbpSession.TenantId)
                .GroupBy(r => new { r.CreationTime.Year, r.CreationTime.Month })
                .Select(g => new RequestsListPerMonthDto() { Year = DateTime.Now.Year, Month = g.Key.Month })
                .Distinct().ToListAsync();

            list.AcceptedRequests = acceptedPricedRequests.Select(g => new RequestsListPerMonthDto
            {
                Year = g.Year,
                Month = g.Month,
                Count = acceptedPricedRequests.Where(m => m.Month == g.Month).Count(),
            }).OrderBy(r => r.Month).ToList();

            var rejectedRequests = await _shippingRequestRepository.GetAll().AsNoTracking()
                .Where(x => (x.Status == ShippingRequestStatus.Cancled || x.Status == ShippingRequestStatus.Expired) && x.CreationTime.Year == Clock.Now.Year)
                .WhereIf(await IsEnabledAsync(AppFeatures.Carrier), x => x.CarrierTenantId == AbpSession.TenantId)
                .WhereIf(await IsEnabledAsync(AppFeatures.Shipper), x => x.TenantId == AbpSession.TenantId)
                .GroupBy(r => new { r.CreationTime.Year, r.CreationTime.Month })
                .Select(g => new RequestsListPerMonthDto() { Year = DateTime.Now.Year, Month = g.Key.Month })
                .Distinct().ToListAsync();

            list.RejectedRequests = rejectedRequests.Select(g => new RequestsListPerMonthDto
            {
                Year = g.Year,
                Month = g.Month,
                Count = rejectedRequests.Where(m => m.Month == g.Month).Count(),
            }).OrderBy(r => r.Month).ToList();

            return list;
        }

        public async Task<List<MostCarriersWorksListDto>> GetMostWorkedWithCarriers()
        {
            DisableTenancyFilters();
            var requests = await _shippingRequestRepository
                .GetAll().Include(r => r.Tenant).Include(r => r.CarrierTenantFk).AsNoTracking()
                .Where(x => x.TenantId == AbpSession.TenantId && x.CarrierTenantId != null)
                    .ToListAsync();
            var shippersIdsList = requests.Select(x => x.Id).ToList();
            var trips = await _shippingRequestTripRepository.GetAll()
                              .Include(r=>r.ShippingRequestFk).ThenInclude(r=>r.Tenant).AsNoTracking()
                             .Where(r => shippersIdsList.Contains(r.ShippingRequestFk.TenantId)).Distinct().ToListAsync();
            return requests.Select(carrier => new MostCarriersWorksListDto()
            {
                Id = carrier.CarrierTenantId,
                CarrierName = carrier.Tenant.TenancyName,
                CarrierRating = carrier.Tenant.Rate,
                NumberOfTrips = trips.Where(r =>r.ShippingRequestFk != null && r.ShippingRequestFk.CarrierTenantId == carrier.CarrierTenantId).Count(),
            }).OrderByDescending(r => r.NumberOfTrips).Take(5).ToList();
        }

        public async Task<CompletedTripVsPodListDto> GetCompletedTripVsPod()
        {
            DisableTenancyFilters();

            var list = new CompletedTripVsPodListDto();

            var completedTrips = await _shippingRequestTripRepository.GetAll().Include(r=>r.ShippingRequestFk).ThenInclude(r=>r.Tenant).AsNoTracking()
                .Where(x => x.Status == ShippingRequestTripStatus.Delivered && x.CreationTime.Year == Clock.Now.Year
                         && x.ShippingRequestFk.TenantId == AbpSession.TenantId)
                .GroupBy(r => new { r.CreationTime.Year, r.CreationTime.Month })
                .Select(g => new RequestsListPerMonthDto() { Year = DateTime.Now.Year, Month = g.Key.Month})
                .OrderBy(x => x.Year).ThenBy(x => x.Month)
                .Distinct().ToListAsync();

            list.CompletedTrips = completedTrips.Select(g => new RequestsListPerMonthDto
            {
                Year = g.Year,
                Month = g.Month,
                Count = completedTrips.Where(m => m.Month == g.Month).Count(),
            }).OrderBy(r => r.Month).ToList();

            var podTrips = await _routePointDocumentRepository.GetAll()
                .Include(r => r.RoutPointFk)
                .ThenInclude(r => r.ShippingRequestTripFk)
                .ThenInclude(r => r.ShippingRequestFk)
                .ThenInclude(r => r.Tenant)
                .AsNoTracking()
                .Where(r => r.RoutPointFk.Status == RoutePointStatus.DeliveryConfirmation && r.RoutePointDocumentType == RoutePointDocumentType.POD)
                .Where(x => x.RoutPointFk.ShippingRequestTripFk.ShippingRequestFk.TenantId == AbpSession.TenantId && x.CreationTime.Year == Clock.Now.Year)
                .GroupBy(r => new { r.CreationTime.Year, r.CreationTime.Month })
                .Select(g => new RequestsListPerMonthDto() { Year = DateTime.Now.Year, Month = g.Key.Month, Count = g.Count() })
                .OrderBy(x => x.Year).ThenBy(x => x.Month)
              .Distinct().ToListAsync();

            list.PODTrips = podTrips.Select(g => new RequestsListPerMonthDto
            {
                Year = g.Year,
                Month = g.Month,
                Count = podTrips.Where(m => m.Month == g.Month).Count(),
            }).OrderBy(r => r.Month).ToList();

            return list;

        }

        public async Task<InvoicesVsPaidInvoicesDto> GetInvoicesVSPaidInvoices()
        {
            DisableTenancyFilters();

            var list = new InvoicesVsPaidInvoicesDto();

            var invoices = await _invoiceTripsRepository.GetAll()
                .Include(x => x.InvoiceFK)
                .AsNoTracking()
                .Where(x => x.InvoiceFK.TenantId == AbpSession.TenantId && x.InvoiceFK.CreationTime.Year == Clock.Now.Year)
                .GroupBy(r => new { r.InvoiceFK.CreationTime.Year, r.InvoiceFK.CreationTime.Month })
                .Select(g => new RequestsListPerMonthDto() { Year = DateTime.Now.Year, Month = g.Key.Month })
                .OrderBy(x => x.Year).ThenBy(x => x.Month)
             .Distinct().ToListAsync();

            list.ShipperInvoices = invoices.Select(g => new RequestsListPerMonthDto
            {
                Year = g.Year,
                Month = g.Month,
                Count = invoices.Where(m => m.Month == g.Month).Count(),
            }).OrderBy(r => r.Month).ToList();

            var paidInvoices = await _invoiceTripsRepository.GetAll()
                .Include(x => x.InvoiceFK)
                .AsNoTracking()
                .Where(x => x.InvoiceFK.TenantId == AbpSession.TenantId && x.InvoiceFK.CreationTime.Year == Clock.Now.Year)
                .Where(x => x.InvoiceFK.IsPaid == true)
                .GroupBy(r => new { r.InvoiceFK.CreationTime.Year, r.InvoiceFK.CreationTime.Month })
                .Select(g => new RequestsListPerMonthDto() { Year = DateTime.Now.Year, Month = g.Key.Month })
                .OrderBy(x => x.Year).ThenBy(x => x.Month)
              .Distinct().ToListAsync();

            list.PaidInvoices = paidInvoices.Select(g => new RequestsListPerMonthDto
            {
                Year = g.Year,
                Month = g.Month,
                Count = paidInvoices.Where(m => m.Month == g.Month).Count(),
            }).OrderBy(r => r.Month).ToList();

            return list;
        }

        public async Task<List<RequestsInMarketpalceDto>> GetRequestsInMarketpalce(GetDataByDateFilterInput input)
        {
            DisableTenancyFilters();

            var query = _shippingRequestRepository.GetAll().AsNoTracking()
                                                             .Where(r => r.RequestType == ShippingRequestType.Marketplace
                                                                      && r.TenantId == AbpSession.TenantId && r.CreationTime.Year == Clock.Now.Year
                                                                      && r.CarrierTenantId == null

                                                                      && ((r.BidEndDate != null && r.BidEndDate.Value.Date <= Clock.Now.Date) || r.BidEndDate == null));
            var list = new List<RequestsInMarketpalceDto>();
            if (input.DatePeriod == FilterDatePeriod.Daily)
            {
                list = await query
                .Where(r => r.CreationTime > Clock.Now.AddDays(-30))
                .GroupBy(r => new
                {
                    r.CreationTime.Year,
                    r.CreationTime.Month,
                    r.CreationTime.Day,
                    RequestReference = r.ReferenceNumber,
                    BiddingEndDate = r.BidEndDate,
                    NumberOfOffers = r.TotalOffers
                })
                .Select(request => new RequestsInMarketpalceDto()
                {
                    RequestReference = request.Key.RequestReference,
                    BiddingEndDate = request.Key.BiddingEndDate,
                    NumberOfOffers = request.Key.NumberOfOffers
                }).OrderByDescending(r => r.BiddingEndDate).Take(10).ToListAsync();
            }
            if (input.DatePeriod == FilterDatePeriod.Weekly)
            {

                var query2 = from u in query.AsEnumerable()
                             group u by new { u.CreationTime.Year,u.ReferenceNumber,u.BidEndDate , u.TotalOffers, WeekNumber = (u.CreationTime - new DateTime(DateTime.Now.Year, 1, 1)).Days / 7 } into ut
                             select new RequestsInMarketpalceDto
                             {
                               RequestReference = ut.Key.ReferenceNumber,
                               BiddingEndDate = ut.Key.BidEndDate,
                               NumberOfOffers = ut.Key.TotalOffers
                             };
                list = query2.OrderByDescending(r => r.BiddingEndDate).Take(10).ToList();
               
            }

            if (input.DatePeriod == FilterDatePeriod.Monthly)
            {
                list = await query.GroupBy(r => new {
                        r.CreationTime.Year,
                        r.CreationTime.Month,
                        RequestReference = r.ReferenceNumber,
                        BiddingEndDate = r.BidEndDate,
                        NumberOfOffers = r.TotalOffers
                    })
                .Select(request => new RequestsInMarketpalceDto()
                {
                    RequestReference = request.Key.RequestReference,
                    BiddingEndDate = request.Key.BiddingEndDate,
                    NumberOfOffers = request.Key.NumberOfOffers
                }).OrderByDescending(r => r.BiddingEndDate).Take(10).ToListAsync();
                
            }
            return list;

        }


        public async Task<List<MostUsedOriginsDto>> GetMostUsedOrigins()
        {
            DisableTenancyFilters();

            return await _shippingRequestRepository.GetAll()
                .Include(r => r.OriginCityFk)
                .AsNoTracking()
                .Where(x => x.TenantId == AbpSession.TenantId)
                .GroupBy(r => new { r.OriginCityId, r.OriginCityFk.DisplayName })
                .Select(res => new MostUsedOriginsDto()
                {
                    CityName = res.Key.DisplayName,
                    NumberOfRequests = res.Count()
                })
                .OrderByDescending(r => r.NumberOfRequests).Take(5).ToListAsync();
        }

        public async Task<List<MostUsedOriginsDto>> GetMostUsedDestinatiions()
        {
            DisableTenancyFilters();

            return await _shippingRequestRepository.GetAll()
                .Include(r => r.DestinationCityFk)
                .AsNoTracking()
                .Where(x => x.TenantId == AbpSession.TenantId)
                .GroupBy(r => new { r.DestinationCityId, r.DestinationCityFk.DisplayName })
                .Select(res => new MostUsedOriginsDto()
                {
                    CityName = res.Key.DisplayName,
                    NumberOfRequests = res.Count()
                })
                .OrderByDescending(r => r.NumberOfRequests).Take(5).ToListAsync();
        }


        public async Task<long> GetDocumentsDueDateInDays()
        {
            DisableTenancyFilters();

            var query = _documentFileRepository.GetAll()
            .Include(r => r.TenantFk)
            .AsNoTracking()
            .Where(x => x.TenantId == AbpSession.TenantId)
            .Where(r => (r.IsAccepted == false || r.IsRejected == true)
                    && r.ExpirationDate != null
                    && r.ExpirationDate.Value.Date > Clock.Now.Date);

            return (await query.ToListAsync()).Select(t => new
            {
                days = t.ExpirationDate.Value.Date.Subtract(Clock.Now.Date).TotalDays
            }).Where(r => r.days <= 5)
           .Count();

        }


        public async Task<long> GetInvoiceDueDateInDays()
        {
            DisableTenancyFilters();

            var query = _invoiceRepository.GetAll()
            .Include(r => r.Tenant)
            .AsNoTracking()
            .Where(r =>
                    r.TenantId == AbpSession.TenantId
                    && r.IsPaid == false
                    && r.DueDate != null
                    && r.DueDate.Date > Clock.Now.Date);

            return (await query.ToListAsync()).Select(t => new
            {
                days = t.DueDate.Date.Subtract(Clock.Now.Date).TotalDays
            }).Where(r => r.days <= 5)
           .Count();

        }

        // Tracking Map
        public async Task<List<TrackingMapDto>> GetTrackingMap()
        {
            DisableTenancyFilters();
            return await _shippingRequestTripRepository.GetAll()
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
            .Where(r => r.Status == ShippingRequestTripStatus.Intransit && r.CreationTime.Year == Clock.Now.Year)
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

            })
            .OrderByDescending(r=>r.Id).Take(10).ToListAsync();
        }
    }
}