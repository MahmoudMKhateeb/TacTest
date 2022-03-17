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
    [AbpAuthorize(AppPermissions.Pages_HostDashboard, AppPermissions.App_TachyonDealer)]

    public class HostDashboardAppService : TACHYONAppServiceBase, IHostDashboardAppService
    {
        private readonly IRepository<ShippingRequest, long> _shippingRequestRepository;
        private readonly IRepository<TrucksType, long> _lookup_trucksTypeRepository;
        private readonly IRepository<User, long> _usersRepository;
        private readonly IRepository<ShippingRequestTrip> _shippingRequestTripRepository;
        private readonly IRepository<Tenant> _tenantRepository;
        private readonly IRepository<Truck, long> _trucksRepository;
        private readonly IRepository<GoodCategory> _goodTypesRepository;
        private readonly IRepository<Invoice, long> _invoicesRepository;

        public HostDashboardAppService(
             IRepository<ShippingRequest, long> shippingRequestRepository,
             IRepository<TrucksType, long> lookup_trucksTypeRepository,
             IRepository<User, long> usersRepository,
             IRepository<ShippingRequestTrip> shippingRequestTripRepository,
             IRepository<Tenant> tenantRepository,
             IRepository<Truck, long> trucksRepository,
             IRepository<GoodCategory> goodTypesRepository,
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
            _invoicesRepository = invoicesRepository;

        }

        public async Task<List<TruckTypeAvailableTrucksDto>> GetTrucksTypeCount()
        {
            DisableTenancyFiltersIfHost();
            await DisableTenancyFiltersIfTachyonDealer();

            var truckTypes = await _lookup_trucksTypeRepository.GetAll().ToListAsync();
            var truckIdsList = truckTypes.Select(x => x.Id).ToList();
            var requests = await _shippingRequestRepository.GetAll()
                .Where(x => truckIdsList.Contains(x.TrucksTypeId.Value))
                .ToListAsync();
            return truckTypes.Select(x => new TruckTypeAvailableTrucksDto
            {
                Id=x.Id,
                TruckType=x.DisplayName,
                AvailableTrucksCount= requests.Where(y=>y.TrucksTypeId == x.Id).Count()
            }).Where(r => r.AvailableTrucksCount > 0).ToList();
        }
        //Get Data For Current year by default
        public async Task<List<ListPerMonthDto>> GetAccountsStatistics(GetDataByDateFilterInput input)
        {
            DisableTenancyFiltersIfHost();
            await DisableTenancyFiltersIfTachyonDealer();
            var groupedAccountList = new List<ListPerMonthDto>();
            groupedAccountList = await GetAccountsIfDaily(input, groupedAccountList);
            groupedAccountList = GetAccountsIfWeekly(input, groupedAccountList);
            groupedAccountList = await GetAccountsIfMonthly(input, groupedAccountList);
            
            return groupedAccountList;
        }

        public async Task<List<ListPerMonthDto>> GetNewTripsStatistics(GetDataByDateFilterInput input)
        {
            DisableTenancyFiltersIfHost();
            await DisableTenancyFiltersIfTachyonDealer();
            var groupedTripsList = new List<ListPerMonthDto>();
            groupedTripsList = await GetTripsIfDaily(input, groupedTripsList);
            groupedTripsList = GetTripsIfWeekly(input, groupedTripsList);
            groupedTripsList = await GetTripsIfMonthly(input, groupedTripsList);
            return groupedTripsList;
        }

        public async Task<List<GoodTypeAvailableDto>> GetGoodTypeCount()
        {
            DisableTenancyFiltersIfHost();
            await DisableTenancyFiltersIfTachyonDealer();
            var goodTypes = await _goodTypesRepository.GetAll().ToListAsync();
            var goodTypesIdsList = goodTypes.Select(x => x.Id).ToList();
            var requests = await _shippingRequestRepository.GetAll()
                .Where(x => x.GoodCategoryId != null && goodTypesIdsList.Contains(x.GoodCategoryId.Value))
                .ToListAsync();
            return goodTypes.Select(x => new GoodTypeAvailableDto
            {
                Id = x.Id,
                GoodType = x.Translations != null ? x.Translations.FirstOrDefault().DisplayName : x.Key,
                AvailableGoodTypesCount = requests.Where(y => y.GoodCategoryId == x.Id).Count()
            }).Where(r => r.AvailableGoodTypesCount > 0).ToList();
        }

        public async Task<List<RouteTypeAvailableDto>> GetRouteTypeCount()
        {
            DisableTenancyFiltersIfHost();
            await DisableTenancyFiltersIfTachyonDealer();

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
                .Where(r => r.Edition.DisplayName.Equals(TACHYONConsts.ShipperEdtionName) && r.CreationTime.Year == DateTime.Now.Year)
                .CountAsync();
        }

        public async Task<long> GetCarriersCount()
        {
            DisableTenancyFiltersIfHost();
            await DisableTenancyFiltersIfTachyonDealer();

            return await _tenantRepository.GetAll().AsNoTracking()
                .Where(r => r.Edition.DisplayName.Equals(TACHYONConsts.CarrierEdtionName) && r.CreationTime.Year == DateTime.Now.Year)
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
            await DisableTenancyFiltersIfTachyonDealer();
            var shippers = await _tenantRepository.GetAll().Include(r => r.Edition).AsNoTracking().Where(r => r.Edition.DisplayName.Contains(TACHYONConsts.ShipperEdtionName)).ToListAsync();
            var shippersIdsList = shippers.Select(x => x.Id).ToList();
            var requests = await _shippingRequestRepository.GetAll().Include(r => r.Tenant).AsNoTracking()
                .Where(r => r.Status == ShippingRequestStatus.Completed && r.CreationTime.Year == DateTime.Now.Year && shippersIdsList.Contains(r.TenantId)).ToListAsync();
            return shippers.Select(tenant => new ListUsersHaveMostRequests()
            {
                Id = tenant.Id,
                Name = tenant.TenancyName,
                Rating = tenant.Rate,
                NumberOfRequests = requests.Where(r=>r.TenantId == tenant.Id).Count()
            }).Where(r => r.NumberOfRequests > 0).OrderByDescending(r => r.NumberOfRequests).Take(3).ToList();

        }

        public async Task<List<ListUsersHaveMostRequests>> GetCarriersHaveMostRequests()
        {
            DisableTenancyFiltersIfHost();
            await DisableTenancyFiltersIfTachyonDealer();

            DisableTenancyFiltersIfHost();
            await DisableTenancyFiltersIfTachyonDealer();
            var carriers = await _tenantRepository.GetAll().Include(r => r.Edition).AsNoTracking().Where(r => r.Edition.DisplayName.Contains(TACHYONConsts.CarrierEdtionName)).ToListAsync();
            var carriersIdsList = carriers.Select(x => x.Id).ToList();
            var requests = await _shippingRequestRepository.GetAll().Include(r => r.Tenant).AsNoTracking()
                .Where(r => r.Status == ShippingRequestStatus.Completed && r.CreationTime.Year == DateTime.Now.Year && carriersIdsList.Contains(r.TenantId)).ToListAsync();
            return carriers.Select(tenant => new ListUsersHaveMostRequests()
            {
                Id = tenant.Id,
                Name = tenant.TenancyName,
                Rating = tenant.Rate,
                NumberOfRequests = requests.Where(r => r.TenantId == tenant.Id).Count()
            }).Where(r => r.NumberOfRequests > 0).OrderByDescending(r => r.NumberOfRequests).Take(3).ToList();
        }

        public async Task<List<ListUsersHaveMostRequests>> GetTopRatedShippers()
        {
            DisableTenancyFiltersIfHost();
            await DisableTenancyFiltersIfTachyonDealer();
            var shippers = await _tenantRepository.GetAll().Include(r => r.Edition).AsNoTracking().Where(r => r.Edition.DisplayName.Contains(TACHYONConsts.ShipperEdtionName)).ToListAsync();
            var shippersIdsList = shippers.Select(x => x.Id).ToList();
            var requests = await _shippingRequestRepository.GetAll().Include(r => r.Tenant).AsNoTracking()
                .Where(r => r.Status == ShippingRequestStatus.Completed && r.CreationTime.Year == DateTime.Now.Year && shippersIdsList.Contains(r.TenantId)).ToListAsync();
            return shippers.Select(tenant => new ListUsersHaveMostRequests()
            {
                Id = tenant.Id,
                Name = tenant.TenancyName,
                Rating = tenant.Rate,
                NumberOfRequests = requests.Where(r => r.TenantId == tenant.Id).Count()
            }).Where(r => r.Rating > 0).OrderByDescending(r => r.Rating).Take(5).ToList();
        }

        public async Task<List<ListUsersHaveMostRequests>> GetTopRatedCarriers()
        {
            DisableTenancyFiltersIfHost();
            await DisableTenancyFiltersIfTachyonDealer();
            var carriers = await _tenantRepository.GetAll().Include(r => r.Edition).AsNoTracking().Where(r => r.Edition.DisplayName.Contains(TACHYONConsts.CarrierEdtionName)).ToListAsync();
            var carriersIdsList = carriers.Select(x => x.Id).ToList();
            var requests = await _shippingRequestRepository.GetAll().Include(r => r.Tenant).AsNoTracking()
                .Where(r => r.Status == ShippingRequestStatus.Completed && r.CreationTime.Year == DateTime.Now.Year && carriersIdsList.Contains(r.TenantId)).ToListAsync();
            return carriers.Select(tenant => new ListUsersHaveMostRequests()
            {
                Id = tenant.Id,
                Name = tenant.TenancyName,
                Rating = tenant.Rate,
                NumberOfRequests = requests.Where(r => r.TenantId == tenant.Id).Count()
            }).Where(r => r.Rating > 0).OrderByDescending(r => r.Rating).Take(5).ToList();
        }

        public async Task<List<ListUsersHaveMostRequests>> GetWorstRatedShippers()
        {
            DisableTenancyFiltersIfHost();
            await DisableTenancyFiltersIfTachyonDealer();
            var shippers = await _tenantRepository.GetAll().Include(r => r.Edition).AsNoTracking().Where(r => r.Edition.DisplayName.Contains(TACHYONConsts.ShipperEdtionName)).ToListAsync();
            var shippersIdsList = shippers.Select(x => x.Id).ToList();
            var requests = await _shippingRequestRepository.GetAll().Include(r => r.Tenant).AsNoTracking()
                .Where(r => r.Status == ShippingRequestStatus.Completed && r.CreationTime.Year == DateTime.Now.Year && shippersIdsList.Contains(r.TenantId)).ToListAsync();
            return shippers.Select(tenant => new ListUsersHaveMostRequests()
            {
                Id = tenant.Id,
                Name = tenant.TenancyName,
                Rating = tenant.Rate,
                NumberOfRequests = requests.Where(r => r.TenantId == tenant.Id).Count()
            }).OrderBy(r => r.Rating).Take(5).ToList();
        }

        public async Task<List<ListUsersHaveMostRequests>> GetWorstRatedCarriers()
        {
            DisableTenancyFiltersIfHost();
            await DisableTenancyFiltersIfTachyonDealer();
            var carriers = await _tenantRepository.GetAll().Include(r => r.Edition).AsNoTracking().Where(r => r.Edition.DisplayName.Contains(TACHYONConsts.CarrierEdtionName)).ToListAsync();
            var carriersIdsList = carriers.Select(x => x.Id).ToList();
            var requests = await _shippingRequestRepository.GetAll().Include(r => r.Tenant).AsNoTracking()
                .Where(r => r.Status == ShippingRequestStatus.Completed && r.CreationTime.Year == DateTime.Now.Year && carriersIdsList.Contains(r.TenantId)).ToListAsync();
            return carriers.Select(tenant => new ListUsersHaveMostRequests()
            {
                Id = tenant.Id,
                Name = tenant.TenancyName,
                Rating = tenant.Rate,
                NumberOfRequests = requests.Where(r => r.TenantId == tenant.Id).Count()
            }).OrderBy(r => r.Rating).Take(5).ToList();
        }

        public async Task<List<ListRequestsUnPricedMarketPlace>> GetUnpricedRequestsInMarketplace(GetDataByDateFilterInput input)
        {
            DisableTenancyFiltersIfHost();
            await DisableTenancyFiltersIfTachyonDealer();

            var query = _shippingRequestRepository.GetAll()
                .Include(r => r.Tenant)
                .AsNoTracking()
                .Where(r => r.RequestType == ShippingRequestType.Marketplace
                    && r.Status == ShippingRequestStatus.PrePrice
                    && ((r.BidEndDate != null && r.BidEndDate.Value.Date <= Clock.Now.Date) || r.BidEndDate == null));
            var list = new List<ListRequestsUnPricedMarketPlace>();
            list = await GetRequestsDaily(input, query, list);
            list = GetRequestsWeekly(input, query, list);
            list = await GetRequestsMonthly(input, query, list);

            return list;
        }

        public async Task<List<ListRequestsByCityDto>> GetNumberOfRequestsForEachCity()
        {
            DisableTenancyFiltersIfHost();
            await DisableTenancyFiltersIfTachyonDealer();
            var requests = await _shippingRequestRepository
                .GetAll()
                .Include(r => r.DestinationCityFk).AsNoTracking().Where(r => r.CreationTime.Year == DateTime.Now.Year).ToListAsync();
            var groupedCities = requests
                    .GroupBy(r => new { r.DestinationCityId })
                    .Select((c,k) => new
                    {
                        key = c.Key.DestinationCityId,
                        list = c.ToList()
                    }).ToList();

             var result = groupedCities.Select(g => new ListRequestsByCityDto
             {
                 Id = g.key,
                 NumberOfRequests = g.list.Count(),
                 CityName = g.list.Select(r=>r.DestinationCityFk.DisplayName).FirstOrDefault()
             }).OrderBy(r => r.Id).ToList();

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
                         && x.CreationTime.Year == DateTime.Now.Year
                         && ((x.BidEndDate != null && x.BidEndDate.Value.Date <= Clock.Now.Date) || x.BidEndDate == null))
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

        #region Heplpers
        private async Task<List<ListPerMonthDto>> GetAccountsIfMonthly(GetDataByDateFilterInput input, List<ListPerMonthDto> groupedAccountList)
        {
            if (input.DatePeriod == FilterDatePeriod.Monthly)
            {
                var AccountsMonthlyList = await _usersRepository.GetAll().AsNoTracking()
                    .Where(r => !r.IsDriver && r.CreationTime.Year == Clock.Now.Year).ToListAsync();

                var grouped2 = AccountsMonthlyList
            .GroupBy(r => new { r.CreationTime.Year, r.CreationTime.Month })
            .Select(s => new
            {
                list = s.ToList(),
                Year = s.Key.Year,
                Month = s.Key.Month.ToString()
            }).ToList();
                groupedAccountList = grouped2.Select(g => new ListPerMonthDto
                {
                    Year = g.Year,
                    Month = new DateTime(g.Year, Convert.ToInt16(g.Month), 1).ToString("MMM"),
                    Count = g.list.Count(),
                }).ToList();
            }

            return groupedAccountList;
        }

        private List<ListPerMonthDto> GetAccountsIfWeekly(GetDataByDateFilterInput input, List<ListPerMonthDto> groupedAccountList)
        {
            if (input.DatePeriod == FilterDatePeriod.Weekly)
            {
                DateTime firstDay = new DateTime(DateTime.Now.Year, 1, 1);

                var AccountsWeeklyList = (from u in _usersRepository.GetAll().AsNoTracking().AsEnumerable()
                                          where !u.IsDriver && u.CreationTime.Year == Clock.Now.Year
                                          group u by new { u.CreationTime.Year, WeekNumber = (u.CreationTime - new DateTime(DateTime.Now.Year, 1, 1)).Days / 7 } into ut
                                          select new { list = ut.ToList(), Year = ut.Key.Year, Week = ut.Key.WeekNumber }).ToList();


                groupedAccountList = AccountsWeeklyList.Select(x => new ListPerMonthDto
                {
                    Year = x.Year,
                    Week = x.Week,
                    Count = x.list.Count()
                }).OrderBy(r => r.Week).Distinct().ToList();

            }

            return groupedAccountList;
        }

        private async Task<List<ListPerMonthDto>> GetAccountsIfDaily(GetDataByDateFilterInput input, List<ListPerMonthDto> groupedAccountList)
        {
            //daily => default before 30 day
            if (input.DatePeriod == FilterDatePeriod.Daily)
            {
                var AccountsDailyList = await _usersRepository.GetAll().AsNoTracking()
                .Where(r => !r.IsDriver &&
                r.CreationTime.Year == Clock.Now.Year &&
                r.CreationTime > Clock.Now.AddDays(-30)).ToListAsync();
                var grouped = AccountsDailyList
                    .GroupBy(r => new { r.CreationTime.Day, r.CreationTime.Year, r.CreationTime.Month })
                    .Select(s => new
                    {
                        list = s.ToList(),
                        Year = s.Key.Year,
                        Day = s.Key.Day,
                        Month = s.Key.Month.ToString()
                    }).ToList();
                groupedAccountList = grouped.Select(g => new ListPerMonthDto
                {
                    Year = g.Year,
                    Month = g.Month,
                    Day = g.Day,
                    Count = g.list.Count(),
                }).OrderBy(r => r.Day).ToList();
            }

            return groupedAccountList;
        }

        private async Task<List<ListPerMonthDto>> GetTripsIfMonthly(GetDataByDateFilterInput input, List<ListPerMonthDto> groupedTripsList)
        {
            if (input.DatePeriod == FilterDatePeriod.Monthly)
            {
                var TripsMonthlyList = await _shippingRequestTripRepository.GetAll().AsNoTracking()
                    .Where(r => r.Status == ShippingRequestTripStatus.New && r.CreationTime.Year == Clock.Now.Year).ToListAsync();

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

        private List<ListPerMonthDto> GetTripsIfWeekly(GetDataByDateFilterInput input, List<ListPerMonthDto> groupedTripsList)
        {
            if (input.DatePeriod == FilterDatePeriod.Weekly)
            {
                DateTime firstDay = new DateTime(DateTime.Now.Year, 1, 1);

                var TripsWeeklyList = (from u in _shippingRequestTripRepository.GetAll().AsNoTracking().AsEnumerable()
                                       where u.Status == ShippingRequestTripStatus.New && u.CreationTime.Year == Clock.Now.Year
                                       group u by new { u.CreationTime.Year, WeekNumber = (u.CreationTime - new DateTime(DateTime.Now.Year, 1, 1)).Days / 7 } into ut
                                       select new { list = ut.ToList(), Year = ut.Key.Year, Week = ut.Key.WeekNumber }).ToList();


                groupedTripsList = TripsWeeklyList.Select(x => new ListPerMonthDto
                {
                    Year = x.Year,
                    Week = x.Week,
                    Count = x.list.Count()
                }).OrderBy(r => r.Week).Distinct().ToList();

            }

            return groupedTripsList;
        }

        private async Task<List<ListPerMonthDto>> GetTripsIfDaily(GetDataByDateFilterInput input, List<ListPerMonthDto> groupedTripsList)
        {
            //daily => default before 30 day
            if (input.DatePeriod == FilterDatePeriod.Daily)
            {
                var TripsDailyList = await _shippingRequestTripRepository.GetAll().AsNoTracking()
                .Where(r => r.Status == ShippingRequestTripStatus.New &&
                r.CreationTime.Year == Clock.Now.Year &&
                r.CreationTime > Clock.Now.AddDays(-30)).ToListAsync();
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

            return groupedTripsList;
        }


        private static async Task<List<ListRequestsUnPricedMarketPlace>> GetRequestsMonthly(GetDataByDateFilterInput input, IQueryable<ShippingRequest> query, List<ListRequestsUnPricedMarketPlace> list)
        {
            if (input.DatePeriod == FilterDatePeriod.Monthly)
            {

                list = await query
                       .GroupBy(r => new
                       {
                           Id = r.Id,
                           ShipperName = r.Tenant.TenancyName,
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
                  }).OrderBy(r => r.BiddingEndDate).Take(10).ToListAsync();

            }

            return list;
        }

        private static List<ListRequestsUnPricedMarketPlace> GetRequestsWeekly(GetDataByDateFilterInput input, IQueryable<ShippingRequest> query, List<ListRequestsUnPricedMarketPlace> list)
        {
            if (input.DatePeriod == FilterDatePeriod.Weekly)
            {
                DateTime firstDay = new DateTime(DateTime.Now.Year, 1, 1);

                var query2 = (from r in query.AsEnumerable()
                              group r by new
                              {
                                  Id = r.Id,
                                  ShipperName = r.Tenant.TenancyName,
                                  BiddingEndDate = r.BidEndDate,
                                  RequestReference = r.ReferenceNumber,
                                  r.CreationTime.Year,
                                  WeekNumber = (r.CreationTime - new DateTime(DateTime.Now.Year, 1, 1)).Days / 7
                              } into ut
                              select new ListRequestsUnPricedMarketPlace
                              {
                                  Id = ut.Key.Id,
                                  ShipperName = ut.Key.ShipperName,
                                  BiddingEndDate = ut.Key.BiddingEndDate,
                                  RequestReference = ut.Key.RequestReference
                              });
                list = query2.OrderBy(r => r.BiddingEndDate).Take(10).ToList();

            }

            return list;
        }

        private static async Task<List<ListRequestsUnPricedMarketPlace>> GetRequestsDaily(GetDataByDateFilterInput input, IQueryable<ShippingRequest> query, List<ListRequestsUnPricedMarketPlace> list)
        {
            if (input.DatePeriod == FilterDatePeriod.Daily)
            {
                list = await query.WhereIf(input.DatePeriod == FilterDatePeriod.Daily, r => r.CreationTime > DateTime.Now.AddDays(-30))
                   .Select(request => new ListRequestsUnPricedMarketPlace()
                   {
                       Id = request.Id,
                       ShipperName = request.Tenant.TenancyName,
                       BiddingEndDate = request.BidEndDate,
                       RequestReference = request.ReferenceNumber
                   }).OrderBy(r => r.BiddingEndDate).Take(10).ToListAsync();
            }

            return list;
        }

        #endregion


    }
}