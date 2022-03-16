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
             var drivers = await _usersRepository.GetAll().AsNoTracking()
                 .Where(r => r.IsDriver && r.TenantId == AbpSession.TenantId).ToListAsync();
            return new ActivityItemsDto()
            {
                ActiveItems = drivers.Where(r => r.IsActive).Count(),
                NotActiveItems = drivers.Where(r => !r.IsActive).Count()
            };
        }

        public async Task<ActivityItemsDto> GetTrucksActivity()
        {
            DisableTenancyFilters();
            var trucks = await _trucksRepository.GetAll().AsNoTracking()
                       .Where(r => r.TenantId == AbpSession.TenantId).ToListAsync();
            return new ActivityItemsDto()
            {
                ActiveItems = trucks.Where(r => r.TruckStatusId == 1).Count(),
                NotActiveItems = trucks.Where(r => r.TruckStatusId == 2).Count()
            };
        }

        public async Task<List<MostShippersWorksListDto>> GetMostWorkedWithShippers()
        {
            DisableTenancyFilters();
            var requests = await _shippingRequestRepository.GetAll().Include(r => r.Tenant).Include(r => r.CarrierTenantFk).AsNoTracking()
                 .Where(x => x.CarrierTenantId != null && x.CarrierTenantId == AbpSession.TenantId)
                 .ToListAsync();
            var shippersIdsList = requests.Select(x => x.Id).ToList();
            var trips = await _shippingRequestTripRepository.GetAll()
                             .Include(r => r.ShippingRequestFk).ThenInclude(r => r.Tenant).AsNoTracking()
                             .Where(r => shippersIdsList.Contains(r.ShippingRequestFk.TenantId)).Distinct().ToListAsync();
            return requests.Select(shipper => new MostShippersWorksListDto()
            {
                Id = shipper.TenantId,
                ShipperName = shipper.Tenant.TenancyName,
                ShipperRating = shipper.Tenant.Rate,
                NumberOfTrips = trips.Where(r => r.ShippingRequestFk != null && r.ShippingRequestFk.TenantId == shipper.TenantId).Count(),
            }).OrderByDescending(r => r.NumberOfTrips).Take(5).ToList();
            
        }

        public async Task<List<VasTypeDto>> GetMostVasesUsedByShippers()
        {
            DisableTenancyFilters();

            return await _shippingRequestVasRepository.GetAll().AsNoTracking()
                .Include(r => r.ShippingRequestFk)
                 .ThenInclude(r => r.Tenant)
                .Include(r => r.VasFk)
                .Where(x => x.ShippingRequestFk.CarrierTenantId != null && x.ShippingRequestFk.CarrierTenantId == AbpSession.TenantId)
                .GroupBy(r => new { r.VasFk.Name })
                .Select(vas => new VasTypeDto()
                {
                    VasType = vas.Key.Name,
                    AvailableVasTypeCount = vas.Count()
                }).Distinct().OrderByDescending(r => r.AvailableVasTypeCount).Take(5).ToListAsync();
        }




    }
}