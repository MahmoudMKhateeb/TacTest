using Abp;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TACHYON.Trucks;

namespace TACHYON.Shipping.ShippingRequestBids
{
    public class BidDomainService : TACHYONDomainServiceBase, IBidDomainService
    {
        private readonly IRepository<Truck, long> _truckRepository;

        public BidDomainService(IRepository<Truck, long> truckRepository)
        {
            _truckRepository = truckRepository;
        }


        /// <summary>
        /// get all carriers who have this tucks filter
        /// </summary>
        /// <param name="trucksTypeId"></param>
        /// <returns></returns>
        public async Task<UserIdentifier[]> GetCarriersByTruckTypeArrayAsync(long trucksTypeId)
        {
            using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MustHaveTenant))
            {
                var carriersList = await _truckRepository.GetAll()
                    .Where(x => x.TrucksTypeId == trucksTypeId)
                    .Select(x => new UserIdentifier(x.TenantId, x.CreatorUserId.Value))
                    .Distinct()
                    .ToArrayAsync();

                return carriersList;
            }
        }
    }
}