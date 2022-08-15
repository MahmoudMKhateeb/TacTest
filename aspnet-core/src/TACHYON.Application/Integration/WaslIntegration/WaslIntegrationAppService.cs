using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TACHYON.Authorization.Users;
using TACHYON.Features;
using TACHYON.Trucks;

namespace TACHYON.Integration.WaslIntegration
{
    [AbpAuthorize]
    public class WaslIntegrationAppService : TACHYONAppServiceBase
    {
        private readonly WaslIntegrationManager _manager;
        private readonly IRepository<Truck, long> _truckRepository;
        private readonly IRepository<User, long> _userRepository;



        public WaslIntegrationAppService(WaslIntegrationManager manager, IRepository<Truck, long> truckRepository, IRepository<User, long> userRepository)
        {
            _manager = manager;
            _truckRepository = truckRepository;
            _userRepository = userRepository;
        }

        public async Task VehicleRegistration(long truckId)
        {
            await _manager.QueueVehicleRegistrationJob(truckId);
        }
        public async Task VehicleDelete(long truckId)
        {
            DisableTenancyFiltersIfHost();

            var truck = await _truckRepository.GetAsync(truckId);

            if (await FeatureChecker.IsEnabledAsync(truck.TenantId, AppFeatures.IntegrationWslVehicleRegistration))
            {
                 await _manager.QueueVehicleDeleteJob(truck);

            }
           
        }
        public async Task DriverRegistration(long driverId)
        {
            await _manager.QueueDriverRegistrationJob(driverId);

        }
        public async Task DriverDelete(long driverId)
        {
            using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MayHaveTenant, AbpDataFilters.MustHaveTenant))
            {
                var user = await _userRepository.GetAll().FirstAsync(x => x.Id == driverId);
                await _manager.QueueDriverDeleteJob(user);
            }


        }
        public async Task TripRegistration(int tripId)
        {
            DisableTenancyFiltersIfHost();

            await _manager.QueueTripRegistrationJob(tripId);
        }
        public async Task TripUpdate(int tripId)
        {
            DisableTenancyFiltersIfHost();

            await _manager.QueueTripUpdateJob(tripId);
        }

        public async Task BulkVehicleRegistration(int tenantId)
        {
            DisableTenancyFiltersIfHost();
            var trucks = await _truckRepository.GetAll()
                .Where(x => x.TenantId == tenantId)
                .Where(x => !x.IsWaslIntegrated)
                .ToListAsync();

            foreach (var truck in trucks)
            {
                //Wasl integration 
                if ( await FeatureChecker.IsEnabledAsync(truck.TenantId, AppFeatures.IntegrationWslVehicleRegistration))
                {
                    await _manager.QueueVehicleRegistrationJob(truck.Id);

                }

            }
        }


        public async Task BulkDriverRegistration(int tenantId)
        {
            DisableTenancyFiltersIfHost();
            var drivers = await _userRepository.GetAll()
                .Where(x => x.TenantId == tenantId)
                .Where(x => x.IsDriver)
                .Where(x => !x.IsWaslIntegrated)
                .ToListAsync();

            foreach (var driver in drivers)
            {
                if (driver.TenantId != null && await FeatureChecker.IsEnabledAsync(driver.TenantId.Value, AppFeatures.IntegrationWslVehicleRegistration))
                {
                     await _manager.QueueDriverRegistrationJob(driver.Id);
                }
               
            }
        }

    }
}
