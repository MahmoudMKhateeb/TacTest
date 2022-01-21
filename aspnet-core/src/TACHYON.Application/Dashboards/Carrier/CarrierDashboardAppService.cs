using Abp.Authorization;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
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
using TACHYON.Dashboards.Carrier.Dto;
using TACHYON.Goods.GoodCategories;
using TACHYON.MultiTenancy;
using TACHYON.Routs.RoutTypes;
using TACHYON.Shipping.ShippingRequests;
using TACHYON.Shipping.ShippingRequestTrips;
using TACHYON.Shipping.Trips;
using TACHYON.Trucks;
using TACHYON.Trucks.TrucksTypes;
using TACHYON.Trucks.TrucksTypes.Dtos;

namespace TACHYON.Dashboards.Carrier
{
    [AbpAuthorize(AppPermissions.Pages_CarrierDashboard)]
    public class CarrierDashboardAppService : TACHYONAppServiceBase, ICarrierDashboardAppService
    {
        private readonly IRepository<User, long> _usersRepository;
        private readonly IRepository<Truck, long> _trucksRepository;


        public CarrierDashboardAppService(
            IRepository<User, long> usersRepository,
            IRepository<Truck, long> trucksRepository
        )
        {
            _usersRepository = usersRepository;
            _trucksRepository = trucksRepository;
        }

        public async Task<ActivityItemsDto> GetDriversActivity()
        {
            DisableTenancyFilters();
            var drivers = _usersRepository.GetAll().AsNoTracking()
                .Where(r => r.IsDriver);
            return new ActivityItemsDto()
            {
                ActiveItems = await drivers.Where(r => r.IsActive).CountAsync(),
                NotActiveItems = await drivers.Where(r => !r.IsActive).CountAsync()
            };
        }

        public async Task<ActivityItemsDto> GetTrucksActivity()
        {
            DisableTenancyFilters();
            var trucks = _trucksRepository.GetAll().AsNoTracking();
            return new ActivityItemsDto()
            {
                ActiveItems = await trucks.Where(r => r.TruckStatusId == 1).CountAsync(),
                NotActiveItems = await trucks.Where(r => r.TruckStatusId == 2).CountAsync()
            };
        }
    }
}