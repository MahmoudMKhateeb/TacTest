using Abp;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TACHYON.Trucks;

namespace TACHYON.Shipping.ShippingRequestBids
{
    public class BidDomainService: TACHYONDomainServiceBase, IBidDomainService
    {
        private readonly IRepository<Truck,Guid> _truckRepository;

        public BidDomainService(IRepository<Truck, Guid> truckRepository)
        {
            _truckRepository = truckRepository;
        }

        public UserIdentifier[] GetCarriersByTruckTypeArray(long trucksTypeId)
        {
            using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MustHaveTenant))
            {
                var carriersList = _truckRepository.GetAll()
                    .Where(x => x.TrucksTypeId == trucksTypeId)
                    .Select(x => new UserIdentifier(x.TenantId, x.CreatorUserId.Value))
                    .Distinct()
                    .ToArray();

                return carriersList;
            }
        }


    }
}
