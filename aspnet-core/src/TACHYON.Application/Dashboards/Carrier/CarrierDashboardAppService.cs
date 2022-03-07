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
using TACHYON.Features;
using TACHYON.Goods.GoodCategories;
using TACHYON.MultiTenancy;
using TACHYON.Routs.RoutTypes;
using TACHYON.Shipping.ShippingRequests;
using TACHYON.Shipping.ShippingRequestTrips;
using TACHYON.Shipping.Trips;
using TACHYON.ShippingRequestVases;
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
        private readonly IRepository<ShippingRequest, long> _shippingRequestRepository;
        private readonly IRepository<ShippingRequestTrip> _shippingRequestTripRepository;
        private readonly IRepository<ShippingRequestVas,long> _shippingRequestVasRepository;

        public CarrierDashboardAppService(
             IRepository<User, long> usersRepository,
             IRepository<Truck, long> trucksRepository,
             IRepository<ShippingRequest, long> shippingRequestRepository,
             IRepository<ShippingRequestTrip> shippingRequestTripRepository,
             IRepository<ShippingRequestVas, long> shippingRequestVasRepository
            )
        {
            _usersRepository = usersRepository;
            _trucksRepository = trucksRepository;
            _shippingRequestRepository = shippingRequestRepository;
            _shippingRequestTripRepository = shippingRequestTripRepository;
            _shippingRequestVasRepository = shippingRequestVasRepository;
        }

        public async Task<ActivityItemsDto> GetDriversActivity()
        {
            DisableTenancyFilters();
            var drivers = _usersRepository.GetAll().AsNoTracking()
                 .Where(r => r.IsDriver && r.TenantId == AbpSession.TenantId);
            return new ActivityItemsDto()
            {
                ActiveItems = await drivers.Where(r => r.IsActive).CountAsync(),
                NotActiveItems = await drivers.Where(r => !r.IsActive).CountAsync()
            };
        }

        public async Task<ActivityItemsDto> GetTrucksActivity()
        {
            DisableTenancyFilters();
            var trucks = _trucksRepository.GetAll().AsNoTracking()
                       .Where(r => r.TenantId == AbpSession.TenantId);
            return new ActivityItemsDto()
            {
                ActiveItems = await trucks.Where(r => r.TruckStatusId == 1).CountAsync(),
                NotActiveItems = await trucks.Where(r => r.TruckStatusId == 2).CountAsync()
            };
        }

        public async Task<List<MostShippersWorksListDto>> GetMostWorkedWithShippers()
        {
            DisableTenancyFilters();

            return await _shippingRequestRepository.GetAll().AsNoTracking()
                .Include(r => r.CarrierTenantFk)
                .WhereIf(IsEnabled(AppFeatures.Carrier), x => x.CarrierTenantId != null && x.CarrierTenantId == AbpSession.TenantId)
                .GroupBy(r => new { r.TenantId, r.Tenant.Name, r.Tenant.Rate })
                .Select(shipper => new MostShippersWorksListDto()
                {
                    Id = shipper.Key.TenantId,
                    ShipperName = shipper.Key.Name,
                    ShipperRating = shipper.Key.Rate,
                    NumberOfTrips = _shippingRequestTripRepository.GetAll().AsNoTracking().Where(r => r.ShippingRequestFk.TenantId == AbpSession.TenantId && r.ShippingRequestFk.TenantId == shipper.Key.TenantId).Count(),
                    Count = shipper.Count()
                })
                .OrderByDescending(r => r.Count).Take(5).ToListAsync();
        }

        public async Task<List<VasTypeDto>> GetMostVasesUsedByShippers()
        {
            DisableTenancyFilters();

            return await _shippingRequestVasRepository.GetAll().AsNoTracking()
                .Include(r => r.ShippingRequestFk)
                 .ThenInclude(r => r.Tenant)
                .Include(r => r.VasFk)
                .WhereIf(IsEnabled(AppFeatures.Carrier), x => x.ShippingRequestFk.CarrierTenantId != null && x.ShippingRequestFk.CarrierTenantId == AbpSession.TenantId)
                .GroupBy(r => new { r.VasFk.Name })
                .Select(vas => new VasTypeDto()
                {
                    VasType = vas.Key.Name,
                    AvailableVasTypeCount = vas.Count()
                }).OrderByDescending(r => r.AvailableVasTypeCount).Take(5).ToListAsync();
        }




    }
}