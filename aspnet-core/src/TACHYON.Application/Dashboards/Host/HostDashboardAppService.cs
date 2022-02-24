using Abp.Authorization;
using Abp.Domain.Repositories;
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
using System.Linq;
using System.Threading.Tasks;
using TACHYON.Authorization;
using TACHYON.Authorization.Users;
using TACHYON.Cities;
using TACHYON.Dashboards.Host.Dto;
using TACHYON.Goods.GoodCategories;
using TACHYON.Invoices;
using TACHYON.MultiTenancy;
using TACHYON.Routs.RoutTypes;
using TACHYON.Shipping.ShippingRequests;
using TACHYON.Shipping.ShippingRequestTrips;
using TACHYON.Shipping.Trips;
using TACHYON.Trucks;
using TACHYON.Trucks.TrucksTypes;
using TACHYON.Trucks.TrucksTypes.Dtos;

namespace TACHYON.Dashboards.Host
{
    [AbpAuthorize(AppPermissions.Pages_HostDashboard)]
    public class HostDashboardAppService : TACHYONAppServiceBase, IHostDashboardAppService
    {
        private readonly IRepository<ShippingRequest, long> _shippingRequestRepository;
        private readonly IRepository<TrucksType, long> _lookup_trucksTypeRepository;
        private readonly IRepository<User, long> _usersRepository;
        private readonly IRepository<ShippingRequestTrip> _shippingRequestTripRepository;
        private readonly IRepository<Tenant> _tenantRepository;
        private readonly IRepository<Truck, long> _trucksRepository;
        private readonly IRepository<GoodCategory> _goodTypesRepository;
        private readonly IRepository<RoutType> _routeTypeRepository;
        private readonly IRepository<City> _lookup_citiesRepository;
        private readonly IRepository<Invoice, long> _invoicesRepository;

        public HostDashboardAppService(
             IRepository<ShippingRequest, long> shippingRequestRepository,
             IRepository<TrucksType, long> lookup_trucksTypeRepository,
             IRepository<User, long> usersRepository,
             IRepository<ShippingRequestTrip> shippingRequestTripRepository,
             IRepository<Tenant> tenantRepository,
             IRepository<Truck, long> trucksRepository,
             IRepository<GoodCategory> goodTypesRepository,
             IRepository<RoutType> routeTypeRepository,
             IRepository<City> lookup_citiesRepository,
             IRepository<Invoice, long> invoicesRepository

            )
        {
            _lookup_trucksTypeRepository = lookup_trucksTypeRepository;
            _shippingRequestRepository = shippingRequestRepository;
            _usersRepository = usersRepository;
            _shippingRequestTripRepository = shippingRequestTripRepository;
            _tenantRepository = tenantRepository;
            _trucksRepository = trucksRepository;
            _goodTypesRepository = goodTypesRepository;
            _routeTypeRepository = routeTypeRepository;
            _lookup_citiesRepository = lookup_citiesRepository;
            _invoicesRepository = invoicesRepository;

        }

        public async Task<List<TruckTypeAvailableTrucksDto>> GetTrucksTypeCount()
        {
            return await _lookup_trucksTypeRepository.GetAll()
            .Select(x => new TruckTypeAvailableTrucksDto()
            {
                Id = x.Id,
                TruckType = x.DisplayName,
                AvailableTrucksCount = _shippingRequestRepository.GetAll().AsNoTracking().Where(r => r.TrucksTypeId == x.Id).Count()

            }).Where(r => r.AvailableTrucksCount > 0).ToListAsync();

        }

        public async Task<List<ListPerMonthDto>> GetAccountsCountsPerMonth()
        {
            DisableTenancyFilters();
            var list = await _usersRepository.GetAll().AsNoTracking().Where(r => !r.IsDriver)
                .GroupBy(r => new { r.CreationTime.Year, r.CreationTime.Month })
                .Select(g => new ListPerMonthDto() { Year = DateTime.Now.Year, Month = g.Key.Month.ToString(), Count = g.Count() })
                .OrderBy(x => x.Year).ThenBy(x => x.Month)
             .ToListAsync();

            list.ForEach(r =>
            {
                r.Month = new DateTime(DateTime.Now.Year, Convert.ToInt32(r.Month), 1).ToString("MMM");
            });

            return list;
        }

        public async Task<List<ListPerMonthDto>> GetNewTripsCountPerMonth()
        {
            DisableTenancyFiltersIfHost();
            await DisableTenancyFiltersIfTachyonDealer();

            var groupedTrips = await _shippingRequestTripRepository.GetAll().AsNoTracking()
                .Where(x => x.Status == ShippingRequestTripStatus.New)
                .GroupBy(r => new { r.CreationTime.Year, r.CreationTime.Month })
                .Select(g => new ListPerMonthDto() { Year = DateTime.Now.Year, Month = g.Key.Month.ToString(), Count = g.Count() })
                .OrderBy(x => x.Year).ThenBy(x => x.Month)
             .ToListAsync();

            groupedTrips.ForEach(r =>
            {
                r.Month = new DateTime(DateTime.Now.Year, Convert.ToInt32(r.Month), 1).ToString("MMM");
            });

            return groupedTrips;
        }

        public async Task<List<GoodTypeAvailableDto>> GetGoodTypeCountPerMonth()
        {
            return await _goodTypesRepository.GetAll().AsNoTracking()
           .Select(x => new GoodTypeAvailableDto()
           {
               Id = x.Id,
               GoodType = x.Translations.FirstOrDefault() != null ? x.Translations.FirstOrDefault().DisplayName : "",
               AvailableGoodTypesCount = _shippingRequestRepository.GetAll().Where(r => r.GoodCategoryId == x.Id).Count()

           }).Where(r => r.AvailableGoodTypesCount > 0).ToListAsync();

        }
        /// <summary>
        /// FOR Reviewwwww
        /// </summary>
        /// <returns></returns>
        public async Task<List<RouteTypeAvailableDto>> GetRouteTypeCountPerMonth()
        {

            return await _shippingRequestRepository.GetAll().AsNoTracking()
                .GroupBy(x => x.RouteTypeId)
                .Select(group => new RouteTypeAvailableDto
                {
                    RouteType = group.Key.ToString(),
                    AvailableRouteTypesCount = group.Count()
                }).ToListAsync();

        }

        public async Task<long> GetOngoingTripsCount()
        {
            DisableTenancyFiltersIfHost();
            await DisableTenancyFiltersIfTachyonDealer();

            return await _shippingRequestTripRepository.GetAll().AsNoTracking()
                .Where(x => x.Status == ShippingRequestTripStatus.InTransit)
                .CountAsync();

        }

        public async Task<long> GetDeliveredTripsCount()
        {
            DisableTenancyFiltersIfHost();
            await DisableTenancyFiltersIfTachyonDealer();

            return await _shippingRequestTripRepository.GetAll().AsNoTracking()
                .Where(x => x.Status == ShippingRequestTripStatus.Delivered)
                .CountAsync();

        }

        public async Task<long> GetShippersCount()
        {
            DisableTenancyFiltersIfHost();
            await DisableTenancyFiltersIfTachyonDealer();

            return await _tenantRepository.GetAll().AsNoTracking()
                .Where(r => r.Edition.DisplayName.Equals("shipper"))
                .CountAsync();
        }

        public async Task<long> GetCarriersCount()
        {
            DisableTenancyFiltersIfHost();
            await DisableTenancyFiltersIfTachyonDealer();

            return await _tenantRepository.GetAll().AsNoTracking()
                .Where(r => r.Edition.DisplayName.Equals("carrier"))
                .CountAsync();
        }

        public async Task<long> GetDriversCount()
        {
            DisableTenancyFiltersIfHost();
            await DisableTenancyFiltersIfTachyonDealer();

            return await _usersRepository.GetAll().AsNoTracking()
                .Where(r => r.IsDriver)
                .CountAsync();
        }

        public async Task<long> GetTrucksCount()
        {
            DisableTenancyFiltersIfHost();
            await DisableTenancyFiltersIfTachyonDealer();

            return await _trucksRepository.CountAsync();
        }

        public async Task<List<ListUsersHaveMostRequests>> GetShippersHaveMostRequests()
        {
            DisableTenancyFiltersIfHost();

            return await _tenantRepository.GetAll().AsNoTracking().Include(r => r.Edition).Where(r => r.Edition.DisplayName.Contains("shipper"))
            .Select(tenant => new ListUsersHaveMostRequests()
            {
                Id = tenant.Id,
                Name = tenant.Name,
                Rating = tenant.Rate,
                NumberOfRequests = _shippingRequestRepository.GetAll().AsNoTracking().Include(r => r.Tenant).AsNoTracking().Where(r => r.TenantId == tenant.Id && r.Status == ShippingRequestStatus.Completed).Count()
            }).Where(r => r.NumberOfRequests > 0).OrderByDescending(r => r.NumberOfRequests).Take(3).ToListAsync();
        }

        public async Task<List<ListUsersHaveMostRequests>> GetCarriersHaveMostRequests()
        {
            DisableTenancyFiltersIfHost();

            return await _tenantRepository.GetAll().AsNoTracking().Include(r => r.Edition).Where(r => r.Edition.DisplayName.Contains("carrier"))
           .Select(tenant => new ListUsersHaveMostRequests()
           {
               Id = tenant.Id,
               Name = tenant.Name,
               Rating = tenant.Rate,
               NumberOfRequests = _shippingRequestRepository.GetAll().AsNoTracking().Include(r => r.Tenant).AsNoTracking().Where(r => r.TenantId == tenant.Id && r.Status == ShippingRequestStatus.Completed).Count()
           }).Where(r => r.NumberOfRequests > 0).OrderByDescending(r => r.NumberOfRequests).Take(3).ToListAsync();
        }

        public async Task<List<ListUsersHaveMostRequests>> GetTopRatedShippers()
        {
            DisableTenancyFiltersIfHost();

            return await _tenantRepository.GetAll().AsNoTracking().Include(r => r.Edition).Where(r => r.Edition.DisplayName.Contains("shipper"))
            .Select(tenant => new ListUsersHaveMostRequests()
            {
                Id = tenant.Id,
                Name = tenant.Name,
                Rating = tenant.Rate,
                NumberOfRequests = _shippingRequestRepository.GetAll().AsNoTracking().Include(r => r.Tenant).AsNoTracking().Where(r => r.TenantId == tenant.Id).Count()
            }).Where(r => r.Rating > 0).OrderByDescending(r => r.Rating).Take(5).ToListAsync();
        }

        public async Task<List<ListUsersHaveMostRequests>> GetTopRatedCarriers()
        {
            DisableTenancyFiltersIfHost();

            return await _tenantRepository.GetAll().AsNoTracking().Include(r => r.Edition).Where(r => r.Edition.DisplayName.Contains("carrier"))
            .Select(tenant => new ListUsersHaveMostRequests()
            {
                Id = tenant.Id,
                Name = tenant.Name,
                Rating = tenant.Rate,
                NumberOfRequests = _shippingRequestRepository.GetAll().AsNoTracking().Include(r => r.Tenant).AsNoTracking().Where(r => r.TenantId == tenant.Id).Count()
            }).Where(r => r.Rating > 0).OrderByDescending(r => r.Rating).Take(5).ToListAsync();
        }

        public async Task<List<ListUsersHaveMostRequests>> GetWorstRatedShippers()
        {
            DisableTenancyFiltersIfHost();

            return await _tenantRepository.GetAll().AsNoTracking().Include(r => r.Edition).Where(r => r.Edition.DisplayName.Contains("shipper"))
            .Select(tenant => new ListUsersHaveMostRequests()
            {
                Id = tenant.Id,
                Name = tenant.Name,
                Rating = tenant.Rate,
                NumberOfRequests = _shippingRequestRepository.GetAll().AsNoTracking().Include(r => r.Tenant).AsNoTracking().Where(r => r.TenantId == tenant.Id).Count()
            }).OrderBy(r => r.Rating).Take(5).ToListAsync();
        }

        public async Task<List<ListUsersHaveMostRequests>> GetWorstRatedCarriers()
        {
            DisableTenancyFiltersIfHost();

            return await _tenantRepository.GetAll().AsNoTracking().Include(r => r.Edition).Where(r => r.Edition.DisplayName.Contains("carrier"))
            .Select(tenant => new ListUsersHaveMostRequests()
            {
                Id = tenant.Id,
                Name = tenant.Name,
                Rating = tenant.Rate,
                NumberOfRequests = _shippingRequestRepository.GetAll().AsNoTracking().Include(r => r.Tenant).AsNoTracking().Where(r => r.TenantId == tenant.Id).Count()
            }).OrderBy(r => r.Rating).Take(5).ToListAsync();
        }

        public async Task<List<ListRequestsUnPricedMarketPlace>> GetUnpricedRequestsInMarketplace(GetUnpricedRequestsInMarketplaceInput input)
        {
            DisableTenancyFiltersIfHost();

            var t = await _shippingRequestRepository.GetAll().AsNoTracking().Include(r => r.Tenant).Where(r => r.RequestType == ShippingRequestType.Marketplace
                                                                                           && r.Status == ShippingRequestStatus.PrePrice
                                                                                           && r.BidEndDate != null
                                                                                           && r.BidEndDate.Value.Date <= Clock.Now.Date)
                                                                                   .WhereIf(input.StartDate != null && input.EndDate != null, r => r.BidStartDate >= input.StartDate && r.BidEndDate <= input.EndDate)
            .Select(request => new ListRequestsUnPricedMarketPlace()
            {
                Id = request.Id,
                ShipperName = request.Tenant.Name,
                BiddingEndDate = request.BidEndDate,
                RequestReference = request.ReferenceNumber
            }).OrderBy(r => r.BiddingEndDate).Take(10).ToListAsync();
            return t;
        }

        public async Task<List<ListRequestsByCityDto>> GetNumberOfRequestsForEachCity()
        {
            DisableTenancyFiltersIfHost();

            var result = await _shippingRequestRepository.GetAll().AsNoTracking().Include(r => r.DestinationCityFk)
                        .GroupBy(p => p.DestinationCityId,
                        (k, c) => new ListRequestsByCityDto()
                        {
                            Id = k,
                            NumberOfRequests = c.Count(),
                            CityName = _lookup_citiesRepository.GetAll().AsNoTracking().Where(e => e.Id == k.Value).FirstOrDefault().DisplayName
                        }
                        ).ToListAsync();

            result.ForEach(x =>
            {
                x.minimumValueOfRequests = result.Select(x => x.NumberOfRequests).Min();
                x.maximumValueOfRequests = result.Select(x => x.NumberOfRequests).Max();
            });

            return result;
        }

        /**
        * Get Count of Request Being Priced before bid end date
        **/
        public async Task<long> GetRequestBeingPricedBeforeBidEndDateCount()
        {
            DisableTenancyFiltersIfHost();
            await DisableTenancyFiltersIfTachyonDealer();

            return await _shippingRequestRepository.GetAll().AsNoTracking()
                .Where(x => x.Status == ShippingRequestStatus.PostPrice
                         && x.BidEndDate != null
                         && x.BidEndDate.Value.Date <= Clock.Now.Date)
                .CountAsync();
        }

        /**
         * Get Count of Requests pricings that were Aceepted 
         */
        public async Task<long> GetRequestsPricingAcceptedCount()
        {
            DisableTenancyFiltersIfHost();
            await DisableTenancyFiltersIfTachyonDealer();

            return await _shippingRequestRepository.GetAll().AsNoTracking()
                .Where(x => x.Status == ShippingRequestStatus.PostPrice)
                .CountAsync();
        }

        /**
         * Get Count of Invoices Paid by Shippers before thier due date. 
         */
        public async Task<long> GetInvoicesPaidBeforeDueDatePercentageCount()
        {
            DisableTenancyFiltersIfHost();
            await DisableTenancyFiltersIfTachyonDealer();

            return await _invoicesRepository.GetAll().AsNoTracking()
                .Where(x => x.AccountType == InvoiceAccountType.AccountReceivable
                         && x.IsPaid == true
                         && x.DueDate.Date > Clock.Now.Date)
                .CountAsync();
        }


    }
}