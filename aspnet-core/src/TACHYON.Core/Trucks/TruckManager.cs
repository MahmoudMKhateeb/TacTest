using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.UI;

namespace TACHYON.Trucks
{
    /// <summary>
    /// Truck manager.
    /// Used to implement domain logic for trucks.
    /// </summary>
    public class TruckManager : TACHYONDomainServiceBase
    {
        private readonly IRepository<Truck, long> _truckRepository;

        public TruckManager(IRepository<Truck, long> truckRepository)
        {
            _truckRepository = truckRepository;
        }

        public async Task<Truck> CreateAsync(Truck truck)
        {
            var existedTruck = await _truckRepository.FirstOrDefaultAsync(x => x.PlateNumber == truck.PlateNumber);
            if (existedTruck != null)
            {
                throw new UserFriendlyException(L("truck.DuplicatePlateNumber"));
            }

            return await _truckRepository.InsertAsync(truck);
        }
    }
}