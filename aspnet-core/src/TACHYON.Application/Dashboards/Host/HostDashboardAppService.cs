using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.MultiTenancy;
using Abp.Runtime.Session;
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
using TACHYON.Dashboards.Host.Dto;
using TACHYON.Goods.GoodCategories;
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
        private readonly IRepository<RoutType> _routeTypeRepository;

        public HostDashboardAppService(
             IRepository<ShippingRequest, long> shippingRequestRepository,
             IRepository<TrucksType, long> lookup_trucksTypeRepository,
             IRepository<User, long> usersRepository,
             IRepository<ShippingRequestTrip> shippingRequestTripRepository,
             IRepository<Tenant> tenantRepository,
             IRepository<Truck, long> trucksRepository,
             IRepository<GoodCategory> goodTypesRepository,
             IRepository<RoutType> routeTypeRepository
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
        }

        public async Task<List<TruckTypeAvailableTrucksDto>> GetTrucksTypeCount()
        {
            DisableTenancyFiltersIfHost();
            await DisableTenancyFiltersIfTachyonDealer();
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
            DisableTenancyFiltersIfHost();
            await DisableTenancyFiltersIfTachyonDealer();
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
            DisableTenancyFiltersIfHost();
            await DisableTenancyFiltersIfTachyonDealer();
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
            DisableTenancyFiltersIfHost();
            await DisableTenancyFiltersIfTachyonDealer();
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
                .Where(x => x.Status == ShippingRequestTripStatus.Intransit)
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



    }
}