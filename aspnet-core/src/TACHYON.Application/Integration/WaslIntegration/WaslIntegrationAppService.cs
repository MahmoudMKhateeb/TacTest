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
            var truck = await _truckRepository.GetAsync(truckId);
            await _manager.QueueVehicleRegistrationJob(truck);
        }
        public async Task VehicleDelete(long truckId)
        {
            var truck = await _truckRepository.GetAsync(truckId);
            await _manager.QueueVehicleDeleteJob(truck);
        }
        public async Task DriverRegistration(long driverId)
        {
            using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MayHaveTenant, AbpDataFilters.MustHaveTenant))
            {
                var user = await _userRepository.GetAll().FirstAsync(x => x.Id == driverId);
                await _manager.QueueDriverRegistrationJob(user);
            }

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
            await _manager.QueueTripRegistrationJob(tripId);
        }
        public async Task TripUpdate(int tripId)
        {
            await _manager.QueueTripUpdateJob(tripId);
        }

    }
}
