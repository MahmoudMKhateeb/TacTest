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


        public async Task<List<ListPerMonthDto>> GetCompletedTripsCountPerMonth()
        {
            DisableTenancyFilters();
            var groupedTrips = await _shippingRequestTripRepository.GetAll().AsNoTracking()
                .Where(x => x.Status == ShippingRequestTripStatus.Delivered)
                .WhereIf(IsEnabled(AppFeatures.Carrier),
                    x => x.ShippingRequestFk.CarrierTenantId == AbpSession.TenantId)
                .WhereIf(IsEnabled(AppFeatures.Shipper), x => x.ShippingRequestFk.TenantId == AbpSession.TenantId)
                .GroupBy(r => new { r.CreationTime.Year, r.CreationTime.Month })
                .Select(g =>
                    new ListPerMonthDto()
                    {
                        Year = DateTime.Now.Year, Month = g.Key.Month.ToString(), Count = g.Count()
                    })
                .OrderBy(x => x.Year).ThenBy(x => x.Month)
                .ToListAsync();

            groupedTrips.ForEach(r =>
            {
                r.Month = new DateTime(DateTime.Now.Year, Convert.ToInt32(r.Month), 1).ToString("MMM");
            });

            return groupedTrips;
        }


        public async Task<AcceptedAndRejectedRequestsListDto> GetAcceptedAndRejectedRequests()
        {
            DisableTenancyFilters();

            var list = new AcceptedAndRejectedRequestsListDto();

            var acceptedPricedRequests = await _shippingRequestRepository.GetAll().AsNoTracking()
                .Where(x => x.Status == ShippingRequestStatus.PostPrice)
                .WhereIf(IsEnabled(AppFeatures.Carrier), x => x.CarrierTenantId == AbpSession.TenantId)
                .WhereIf(IsEnabled(AppFeatures.Shipper), x => x.TenantId == AbpSession.TenantId)
                .GroupBy(r => new { r.CreationTime.Year, r.CreationTime.Month })
                .Select(g =>
                    new RequestsListPerMonthDto() { Year = DateTime.Now.Year, Month = g.Key.Month, Count = g.Count() })
                .OrderBy(x => x.Year).ThenBy(x => x.Month)
                .ToListAsync();

            var rejectedRequests = await _shippingRequestRepository.GetAll().AsNoTracking()
                .Where(x => x.Status == ShippingRequestStatus.Cancled || x.Status == ShippingRequestStatus.Expired)
                .WhereIf(IsEnabled(AppFeatures.Carrier), x => x.CarrierTenantId == AbpSession.TenantId)
                .WhereIf(IsEnabled(AppFeatures.Shipper), x => x.TenantId == AbpSession.TenantId)
                .GroupBy(r => new { r.CreationTime.Year, r.CreationTime.Month })
                .Select(g =>
                    new RequestsListPerMonthDto() { Year = DateTime.Now.Year, Month = g.Key.Month, Count = g.Count() })
                .OrderBy(x => x.Year).ThenBy(x => x.Month)
                .ToListAsync();

            list.AcceptedRequests = acceptedPricedRequests;
            list.RejectedRequests = rejectedRequests;
            return list;
        }

        public async Task<List<MostCarriersWorksListDto>> GetMostWorkedWithCarriers()
        {
            DisableTenancyFilters();

            return await _shippingRequestRepository.GetAll().AsNoTracking()
                .Include(r => r.CarrierTenantFk)
                .WhereIf(IsEnabled(AppFeatures.Shipper), x => x.TenantId == AbpSession.TenantId)
                .Where(x => x.CarrierTenantId != null)
                .GroupBy(r => new { r.CarrierTenantId, r.CarrierTenantFk.Name, r.CarrierTenantFk.Rate })
                .Select(carrier => new MostCarriersWorksListDto()
                {
                    Id = carrier.Key.CarrierTenantId,
                    CarrierName = carrier.Key.Name,
                    CarrierRating = carrier.Key.Rate,
                    NumberOfTrips = _shippingRequestTripRepository.GetAll().AsNoTracking().Where(r =>
                        r.ShippingRequestFk.TenantId == AbpSession.TenantId &&
                        r.ShippingRequestFk.CarrierTenantId == carrier.Key.CarrierTenantId).Count(),
                    Count = carrier.Count()
                })
                .OrderByDescending(r => r.Count).Take(5).ToListAsync();
        }

        public async Task<CompletedTripVsPodListDto> GetCompletedTripVsPod()
        {
            DisableTenancyFilters();

            var list = new CompletedTripVsPodListDto();

            var completedTrips = await _shippingRequestTripRepository.GetAll().AsNoTracking()
                .Where(x => x.Status == ShippingRequestTripStatus.Delivered)
                .WhereIf(IsEnabled(AppFeatures.Shipper), x => x.ShippingRequestFk.TenantId == AbpSession.TenantId)
                .GroupBy(r => new { r.CreationTime.Year, r.CreationTime.Month })
                .Select(g =>
                    new RequestsListPerMonthDto() { Year = DateTime.Now.Year, Month = g.Key.Month, Count = g.Count() })
                .OrderBy(x => x.Year).ThenBy(x => x.Month)
                .ToListAsync();

            var podTrips = await _routePointDocumentRepository.GetAll().AsNoTracking()
                .Include(r => r.RoutPointFk)
                .ThenInclude(r => r.ShippingRequestTripFk)
                .ThenInclude(r => r.ShippingRequestFk)
                .ThenInclude(r => r.Tenant)
                .Where(r => r.RoutPointFk.Status == RoutePointStatus.DeliveryConfirmation &&
                            r.RoutePointDocumentType == RoutePointDocumentType.POD)
                .Where(x => x.RoutPointFk.ShippingRequestTripFk.ShippingRequestFk.TenantId == AbpSession.TenantId)
                .GroupBy(r => new { r.CreationTime.Year, r.CreationTime.Month })
                .Select(g =>
                    new RequestsListPerMonthDto() { Year = DateTime.Now.Year, Month = g.Key.Month, Count = g.Count() })
                .OrderBy(x => x.Year).ThenBy(x => x.Month)
                .ToListAsync();

            list.CompletedTrips = completedTrips;
            list.PODTrips = podTrips;
            return list;
        }

        public async Task<InvoicesVsPaidInvoicesDto> GetInvoicesVSPaidInvoices()
        {
            DisableTenancyFilters();

            var list = new InvoicesVsPaidInvoicesDto();

            var invoices = await _invoiceTripsRepository.GetAll().AsNoTracking()
                .Include(x => x.InvoiceFK)
                .Where(x => x.InvoiceFK.TenantId == AbpSession.TenantId)
                .GroupBy(r => new { r.InvoiceFK.CreationTime.Year, r.InvoiceFK.CreationTime.Month })
                .Select(g =>
                    new RequestsListPerMonthDto() { Year = DateTime.Now.Year, Month = g.Key.Month, Count = g.Count() })
                .OrderBy(x => x.Year).ThenBy(x => x.Month)
                .ToListAsync();

            var paidInvoices = await _invoiceTripsRepository.GetAll().AsNoTracking()
                .Include(x => x.InvoiceFK)
                .Where(x => x.InvoiceFK.TenantId == AbpSession.TenantId)
                .Where(x => x.InvoiceFK.IsPaid == true)
                .GroupBy(r => new { r.InvoiceFK.CreationTime.Year, r.InvoiceFK.CreationTime.Month })
                .Select(g =>
                    new RequestsListPerMonthDto() { Year = DateTime.Now.Year, Month = g.Key.Month, Count = g.Count() })
                .OrderBy(x => x.Year).ThenBy(x => x.Month)
                .ToListAsync();

            list.ShipperInvoices = invoices;
            list.PaidInvoices = paidInvoices;
            return list;
        }

        public async Task<List<RequestsInMarketpalceDto>> GetRequestsInMarketpalce()
        {
            DisableTenancyFilters();

            return await _shippingRequestRepository.GetAll().AsNoTracking()
                .Where(r => r.RequestType == ShippingRequestType.Marketplace
                            && r.TenantId == AbpSession.TenantId
                            && r.CarrierTenantId == null
                            && r.BidEndDate != null
                            && r.BidEndDate.Value.Date <= Clock.Now.Date)
                .Select(request => new RequestsInMarketpalceDto()
                {
                    RequestReference = request.ReferenceNumber,
                    BiddingEndDate = request.BidEndDate,
                    NumberOfOffers = request.TotalOffers
                }).OrderBy(r => r.BiddingEndDate).ToListAsync();
        }


        public async Task<List<MostUsedOriginsDto>> GetMostUsedOrigins()
        {
            DisableTenancyFilters();

            return await _shippingRequestRepository.GetAll().AsNoTracking()
                .Include(r => r.OriginCityFk)
                .WhereIf(IsEnabled(AppFeatures.Shipper), x => x.TenantId == AbpSession.TenantId)
                .GroupBy(r => new { r.OriginCityId, r.OriginCityFk.DisplayName })
                .Select(res => new MostUsedOriginsDto()
                {
                    CityName = res.Key.DisplayName, NumberOfRequests = res.Count()
                })
                .OrderByDescending(r => r.NumberOfRequests).Take(5).ToListAsync();
        }

        public async Task<List<MostUsedOriginsDto>> GetMostUsedDestinatiions()
        {
            DisableTenancyFilters();

            return await _shippingRequestRepository.GetAll().AsNoTracking()
                .Include(r => r.DestinationCityFk)
                .WhereIf(IsEnabled(AppFeatures.Shipper), x => x.TenantId == AbpSession.TenantId)
                .GroupBy(r => new { r.DestinationCityId, r.DestinationCityFk.DisplayName })
                .Select(res => new MostUsedOriginsDto()
                {
                    CityName = res.Key.DisplayName, NumberOfRequests = res.Count()
                })
                .OrderByDescending(r => r.NumberOfRequests).Take(5).ToListAsync();
        }


        public async Task<long> GetDocumentsDueDateInDays()
        {
            DisableTenancyFilters();

            var query = _documentFileRepository.GetAll().AsNoTracking()
                .Include(r => r.TenantFk)
                .WhereIf(IsEnabled(AppFeatures.Shipper), x => x.TenantId == AbpSession.TenantId)
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

            var query = _invoiceRepository.GetAll().AsNoTracking()
                .Include(r => r.Tenant)
                .WhereIf(IsEnabled(AppFeatures.Shipper), x => x.TenantId == AbpSession.TenantId)
                .WhereIf(IsEnabled(AppFeatures.Carrier), x => x.TenantId == AbpSession.TenantId)
                .Where(r => r.IsPaid == false
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
            return await _shippingRequestTripRepository.GetAll().AsNoTracking()
                .Include(r => r.ShippingRequestFk)
                .ThenInclude(r => r.Tenant)
                .Include(r => r.RoutPoints)
                .ThenInclude(r => r.FacilityFk)
                .Include(x => x.OriginFacilityFk)
                .ThenInclude(x => x.CityFk)
                .Include(x => x.DestinationFacilityFk)
                .ThenInclude(x => x.CityFk)
                .WhereIf(IsEnabled(AppFeatures.Carrier),
                    x => x.ShippingRequestFk.CarrierTenantId == AbpSession.TenantId)
                .WhereIf(IsEnabled(AppFeatures.Shipper), x => x.ShippingRequestFk.TenantId == AbpSession.TenantId)
                .Where(r => r.Status == ShippingRequestTripStatus.Intransit)
                .Select(s => new TrackingMapDto()
                {
                    DestinationCity = s.DestinationFacilityFk.Name,
                    OriginCity = s.OriginFacilityFk.Name,
                    DestinationLongitude =
                        (s.DestinationFacilityFk.Location != null ? s.DestinationFacilityFk.Location.X : 0),
                    DestinationLatitude =
                        (s.DestinationFacilityFk.Location != null ? s.DestinationFacilityFk.Location.Y : 0),
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
                .ToListAsync();
        }
    }
}