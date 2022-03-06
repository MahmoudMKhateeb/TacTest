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
using System.Globalization;
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
using TACHYON.Tenants.Dashboard.Dto;
using TACHYON.Trucks;
using TACHYON.Trucks.TrucksTypes;
using TACHYON.Trucks.TrucksTypes.Dtos;

namespace TACHYON.Dashboards.Host
{
    [AbpAuthorize(AppPermissions.Pages_HostDashboard)]
    public class HostDashboardAppService : TACHYONAppServiceBase, IHostDashboardAppService
    {
        private const string DateFormat = "yyyy-MM-dd";
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
                AvailableTrucksCount = _shippingRequestRepository.GetAll().AsNoTracking().Where(r => r.TrucksTypeId == x.Id && r.CreationTime.Year == DateTime.Now.Year).Count()

            }).Where(r => r.AvailableTrucksCount > 0).ToListAsync();

        }
        //Get Data For Current year by default
        public async Task<List<ListPerMonthDto>> GetAccountsStatistics(GetDataByDateFilterInput input)
        {
            DisableTenancyFiltersIfHost();
            await DisableTenancyFiltersIfTachyonDealer();
            var list = await _usersRepository.GetAll().AsNoTracking()
                .Where(r => !r.IsDriver && r.CreationTime.Year == DateTime.Now.Year)
                .WhereIf(input.SalesSummaryDatePeriod == SalesSummaryDatePeriod.Daily, r=> r.CreationTime > DateTime.Now.AddDays(-30) && r.CreationTime < DateTime.Now)
                .GroupBy(r => new { r.CreationTime.Year, r.CreationTime.Month,r.CreationTime.Day })
                .Select(g => new ListPerMonthDto() { Year = g.Key.Year, Month = g.Key.Month.ToString(),Day = g.Key.Day, Count = g.Count() })
                .OrderBy(x => x.Year).ThenBy(x => x.Month)
             .ToListAsync();

            list.ForEach(r =>
            {
                r.Month = new DateTime(r.Year, Convert.ToInt32(r.Month), r.Day).ToString("MMM");
            });
            if(input.SalesSummaryDatePeriod == SalesSummaryDatePeriod.Weekly)
            {
                DateTime firstDay = new DateTime(DateTime.Now.Year,1,1);

                var query = (from u in _usersRepository.GetAll().AsNoTracking().AsEnumerable()
                       where u.IsDriver == false && u.CreationTime.Year == DateTime.Now.Year
                       group u by new { u.CreationTime.Year, WeekNumber = (u.CreationTime - new DateTime(DateTime.Now.Year, 1, 1)).Days / 7 } into ut
                       select new ListPerMonthDto { Year = ut.Key.Year, Week = ut.Key.WeekNumber, Count = ut.Count() });
                list = query.OrderBy(r=>r.Week).ToList();
                
            }
            if (input.SalesSummaryDatePeriod == SalesSummaryDatePeriod.Monthly)
            {

                list = await _usersRepository.GetAll().AsNoTracking()
                .Where(r => !r.IsDriver && r.CreationTime.Year == DateTime.Now.Year)
                .GroupBy(r => new {
                    year = r.CreationTime.Year,
                    month = r.CreationTime.Month
                })
                .Select(g => new ListPerMonthDto() { Year = g.Key.year, Month = g.Key.month.ToString(), Count = g.Count() })
                .OrderBy(x => x.Year).ThenBy(x => x.Month)
                .ToListAsync();

                list.ForEach(r =>
                {
                    r.Month = new DateTime(r.Year, Convert.ToInt32(r.Month), 1).ToString("MMM");
                });
            }
            return list;
        }

        public async Task<List<ListPerMonthDto>> GetNewTripsStatistics(GetDataByDateFilterInput input)
        {
            DisableTenancyFiltersIfHost();
            await DisableTenancyFiltersIfTachyonDealer();

            var groupedTrips = await _shippingRequestTripRepository.GetAll().AsNoTracking()
                .Where(x => x.Status == ShippingRequestTripStatus.New && x.CreationTime.Year == Clock.Now.Year)
                .WhereIf(input.SalesSummaryDatePeriod == SalesSummaryDatePeriod.Daily, r => r.CreationTime > DateTime.Now.AddDays(-30) && r.CreationTime < DateTime.Now)
                .GroupBy(r => new { r.CreationTime.Year, r.CreationTime.Month , r.CreationTime.Day })
                .Select(g => new ListPerMonthDto() { Year = g.Key.Year, Month = g.Key.Month.ToString(), Day = g.Key.Day, Count = g.Count() })
                .OrderBy(x => x.Day)
             .ToListAsync();

            groupedTrips.ForEach(r =>
            {
                r.Month = new DateTime(r.Year, Convert.ToByte(r.Month),1).ToString("MMM");
            });

            if (input.SalesSummaryDatePeriod == SalesSummaryDatePeriod.Weekly)
            {
                DateTime firstDay = new DateTime(DateTime.Now.Year, 1, 1);

                var query = (from u in _shippingRequestTripRepository.GetAll().AsNoTracking().AsEnumerable()
                             where u.Status == ShippingRequestTripStatus.New && u.CreationTime.Year == Clock.Now.Year
                             group u by new { u.CreationTime.Year, WeekNumber = (u.CreationTime - new DateTime(DateTime.Now.Year, 1, 1)).Days / 7 } into ut
                             select new ListPerMonthDto { Year = ut.Key.Year, Week = ut.Key.WeekNumber, Count = ut.Count() });
                groupedTrips = query.OrderBy(r => r.Week).ToList();
            }
            if (input.SalesSummaryDatePeriod == SalesSummaryDatePeriod.Monthly)
            {

                groupedTrips = await _shippingRequestTripRepository.GetAll().AsNoTracking()
                .Where(x => x.Status == ShippingRequestTripStatus.New && x.CreationTime.Year == Clock.Now.Year)
                .GroupBy(r => new {
                    year = r.CreationTime.Year,
                    month = r.CreationTime.Month
                })
                .Select(g => new ListPerMonthDto() { Year = g.Key.year, Month = g.Key.month.ToString(), Count = g.Count() })
                .OrderBy(x => x.Year).ThenBy(x => x.Month)
                .ToListAsync();

                groupedTrips.ForEach(r =>
                {
                    r.Month = new DateTime(r.Year, Convert.ToByte(r.Month), 1).ToString("MMM");
                });
            }

            return groupedTrips;
        }

        public async Task<List<GoodTypeAvailableDto>> GetGoodTypeCountPerMonth()
        {
            return await _goodTypesRepository.GetAll().AsNoTracking()
           .Select(x => new GoodTypeAvailableDto()
           {
               Id = x.Id,
               GoodType = x.Translations.FirstOrDefault() != null ? x.Translations.FirstOrDefault().DisplayName : "",
               AvailableGoodTypesCount = _shippingRequestRepository.GetAll().Where(r => r.GoodCategoryId == x.Id && r.CreationTime.Year == DateTime.Now.Year).Count()

           }).Where(r => r.AvailableGoodTypesCount > 0).ToListAsync();

        }

        public async Task<List<RouteTypeAvailableDto>> GetRouteTypeCountPerMonth()
        {

            return await _shippingRequestRepository.GetAll().AsNoTracking().Where(r=>r.RouteTypeId != null && r.RouteTypeId != 0 && r.CreationTime.Year == DateTime.Now.Year)
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
                .Where(x => x.Status == ShippingRequestTripStatus.Intransit && x.CreationTime.Year == DateTime.Now.Year)
                .CountAsync();

        }

        public async Task<long> GetDeliveredTripsCount()
        {
            DisableTenancyFiltersIfHost();
            await DisableTenancyFiltersIfTachyonDealer();

            return await _shippingRequestTripRepository.GetAll().AsNoTracking()
                .Where(x => x.Status == ShippingRequestTripStatus.Delivered && x.CreationTime.Year == DateTime.Now.Year)
                .CountAsync();

        }

        public async Task<long> GetShippersCount()
        {
            DisableTenancyFiltersIfHost();
            await DisableTenancyFiltersIfTachyonDealer();

            return await _tenantRepository.GetAll().AsNoTracking()
                .Where(r => r.Edition.DisplayName.Equals("shipper") && r.CreationTime.Year == DateTime.Now.Year)
                .CountAsync();
        }

        public async Task<long> GetCarriersCount()
        {
            DisableTenancyFiltersIfHost();
            await DisableTenancyFiltersIfTachyonDealer();

            return await _tenantRepository.GetAll().AsNoTracking()
                .Where(r => r.Edition.DisplayName.Equals("carrier") && r.CreationTime.Year == DateTime.Now.Year)
                .CountAsync();
        }

        public async Task<long> GetDriversCount()
        {
            DisableTenancyFiltersIfHost();
            await DisableTenancyFiltersIfTachyonDealer();

            return await _usersRepository.GetAll().AsNoTracking()
                .Where(r => r.IsDriver && r.CreationTime.Year == DateTime.Now.Year)
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
                NumberOfRequests = _shippingRequestRepository.GetAll().AsNoTracking().Include(r => r.Tenant).AsNoTracking().Where(r => r.TenantId == tenant.Id && r.Status == ShippingRequestStatus.Completed && r.CreationTime.Year == DateTime.Now.Year).Count()
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
               NumberOfRequests = _shippingRequestRepository.GetAll().AsNoTracking().Include(r => r.Tenant).AsNoTracking().Where(r => r.TenantId == tenant.Id && r.Status == ShippingRequestStatus.Completed && r.CreationTime.Year == DateTime.Now.Year).Count()
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
                NumberOfRequests = _shippingRequestRepository.GetAll().AsNoTracking().Include(r => r.Tenant).AsNoTracking().Where(r => r.TenantId == tenant.Id && r.CreationTime.Year == DateTime.Now.Year).Count()
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
                NumberOfRequests = _shippingRequestRepository.GetAll().AsNoTracking().Include(r => r.Tenant).AsNoTracking().Where(r => r.TenantId == tenant.Id && r.CreationTime.Year == DateTime.Now.Year).Count()
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
                NumberOfRequests = _shippingRequestRepository.GetAll().AsNoTracking().Include(r => r.Tenant).AsNoTracking().Where(r => r.TenantId == tenant.Id && r.CreationTime.Year == DateTime.Now.Year).Count()
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
                NumberOfRequests = _shippingRequestRepository.GetAll().AsNoTracking().Include(r => r.Tenant).AsNoTracking().Where(r => r.TenantId == tenant.Id && r.CreationTime.Year == DateTime.Now.Year).Count()
            }).OrderBy(r => r.Rating).Take(5).ToListAsync();
        }

        public async Task<List<ListRequestsUnPricedMarketPlace>> GetUnpricedRequestsInMarketplace(GetDataByDateFilterInput input)
        {
            DisableTenancyFiltersIfHost();

            var list = await _shippingRequestRepository.GetAll().AsNoTracking()
                .Include(r => r.Tenant)
                .Where(r => r.RequestType == ShippingRequestType.Marketplace
                    && r.Status == ShippingRequestStatus.PrePrice
                    && r.BidEndDate != null
                    && r.BidEndDate.Value.Date <= Clock.Now.Date && r.CreationTime.Year == Clock.Now.Year)
                .WhereIf(input.SalesSummaryDatePeriod == SalesSummaryDatePeriod.Daily, r => r.CreationTime > DateTime.Now.AddDays(-30) && r.CreationTime < DateTime.Now)
                .Select(request => new ListRequestsUnPricedMarketPlace()
                {
                    Id = request.Id,
                    ShipperName = request.Tenant.Name,
                    BiddingEndDate = request.BidEndDate,
                    RequestReference = request.ReferenceNumber
                }).OrderBy(r => r.BiddingEndDate).Take(10).ToListAsync();

            if (input.SalesSummaryDatePeriod == SalesSummaryDatePeriod.Weekly)
            {
                DateTime firstDay = new DateTime(DateTime.Now.Year, 1, 1);

                var query = (from r in _shippingRequestRepository.GetAll().AsNoTracking().AsEnumerable()
                             where  r.RequestType == ShippingRequestType.Marketplace
                                    && r.Status == ShippingRequestStatus.PrePrice
                                    && r.BidEndDate != null
                                    && r.BidEndDate.Value.Date <= Clock.Now.Date && r.CreationTime.Year == Clock.Now.Year
                             group r by new
                             {
                                 Id = r.Id,
                                 ShipperName = r.Tenant.Name,
                                 BiddingEndDate = r.BidEndDate,
                                 RequestReference = r.ReferenceNumber,
                                 r.CreationTime.Year,
                                 WeekNumber = (r.CreationTime - new DateTime(DateTime.Now.Year, 1, 1)).Days / 7 } into ut
                             select new ListRequestsUnPricedMarketPlace {
                                 Id = ut.Key.Id,
                                 ShipperName = ut.Key.ShipperName,
                                 BiddingEndDate = ut.Key.BiddingEndDate,
                                 RequestReference = ut.Key.RequestReference
                             });
                list = query.ToList();
               
            }
            if (input.SalesSummaryDatePeriod == SalesSummaryDatePeriod.Monthly)
            {

             list = await _shippingRequestRepository.GetAll().AsNoTracking()
               .Include(r => r.Tenant)
               .Where(r => r.RequestType == ShippingRequestType.Marketplace
                   && r.CreationTime.Year == DateTime.Now.Year
                   && r.Status == ShippingRequestStatus.PrePrice
                   && r.BidEndDate != null
                   && r.BidEndDate.Value.Date <= Clock.Now.Date && r.CreationTime.Year == Clock.Now.Year)
                .GroupBy(r => new {
                    Id = r.Id,
                    ShipperName = r.Tenant.Name,
                    BiddingEndDate = r.BidEndDate,
                    RequestReference = r.ReferenceNumber,
                    year = r.CreationTime.Year,
                    month = r.CreationTime.Month
                })
               .Select(request => new ListRequestsUnPricedMarketPlace()
               {
                   Id = request.Key.Id,
                   ShipperName = request.Key.ShipperName,
                   BiddingEndDate = request.Key.BiddingEndDate,
                   RequestReference = request.Key.RequestReference
               }).OrderBy(r => r.BiddingEndDate).ToListAsync();

            }

            return list;
        }

        public async Task<List<ListRequestsByCityDto>> GetNumberOfRequestsForEachCity()
        {
            DisableTenancyFiltersIfHost();

            var result = await _shippingRequestRepository.GetAll().AsNoTracking().Include(r => r.DestinationCityFk).Where(r=>r.CreationTime.Year == DateTime.Now.Year)
                        .GroupBy(p => p.DestinationCityId,
                        (k, c) => new ListRequestsByCityDto()
                        {
                            Id = k,
                            NumberOfRequests = c.Count(),
                            CityName = _lookup_citiesRepository.GetAll().AsNoTracking().Where(e => e.Id == k.Value).FirstOrDefault().DisplayName
                        }
                        ).Where(r=>r.Id != null).ToListAsync();

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
                         && x.CreationTime.Year == DateTime.Now.Year
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
                .Where(x => x.Status == ShippingRequestStatus.PostPrice && x.CreationTime.Year == DateTime.Now.Year)
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
                         && x.CreationTime.Year == DateTime.Now.Year
                         && x.DueDate.Date > Clock.Now.Date)
                .CountAsync();
        }


    }
}