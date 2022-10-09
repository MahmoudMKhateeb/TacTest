using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Threading.BackgroundWorkers;
using Abp.Threading.Timers;
using Abp.Timing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using TACHYON.Shipping.Dedicated;

namespace TACHYON.Shipping.ShippingRequests
{
    public class TrackDedicatedTrucksStatuses : PeriodicBackgroundWorkerBase, ISingletonDependency
    {
        private const int CheckPeriodAsMilliseconds = 1 * 60 * 60 * 1000 * 24; //1 day
        private readonly IRepository<ShippingRequest, long> _shippingRequestRepository;
        private readonly IRepository<DedicatedShippingRequestDriver, long> _dedicatedShippingRequestDriver;
        private readonly IRepository<DedicatedShippingRequestTruck, long> _dedicatedShippingRequestTruck;

        public TrackDedicatedTrucksStatuses(AbpTimer timer,
            IRepository<ShippingRequest, long> shippingRequestRepository,
            IRepository<DedicatedShippingRequestDriver, long> dedicatedShippingRequestDriver,
            IRepository<DedicatedShippingRequestTruck, long> dedicatedShippingRequestTruck) : base(timer)
        {
            Timer.Period = CheckPeriodAsMilliseconds;
            Timer.RunOnStart = true;
            _shippingRequestRepository = shippingRequestRepository;
            _dedicatedShippingRequestDriver = dedicatedShippingRequestDriver;
            _dedicatedShippingRequestTruck = dedicatedShippingRequestTruck;
        }

        [UnitOfWork]
        protected override void DoWork()
        {
            using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MustHaveTenant, AbpDataFilters.MayHaveTenant))
            {
                var expiredRentalShippingRequests = _shippingRequestRepository.GetAll()
                    .Where(x => x.ShippingRequestFlag == ShippingRequestFlag.Dedicated &&
                     x.RentalEndDate != null && x.RentalEndDate.Value.Date > Clock.Now.Date
                     && x.Status != ShippingRequestStatus.Expired)
                    .ToList();

                foreach(var request in expiredRentalShippingRequests)
                {
                    request.Status=ShippingRequestStatus.Expired;
                }

                //Track drivers statuses
                var activeDriversStart = _dedicatedShippingRequestDriver.GetAll()
                    .Where(x => x.ShippingRequest.RentalStartDate.Value.Date >= Clock.Now.Date &&
                    x.Status != DedicatedShippingRequestTruckOrDriverStatus.Busy)
                    .ToList();

                foreach(var driver in activeDriversStart)
                {
                    driver.Status = DedicatedShippingRequestTruckOrDriverStatus.Busy;
                }

                //Track trucks statuses
                var activeTrucksStart = _dedicatedShippingRequestTruck.GetAll()
                    .Where(x => x.ShippingRequest.RentalStartDate.Value.Date >= Clock.Now.Date &&
                    x.Status != DedicatedShippingRequestTruckOrDriverStatus.Busy)
                    .ToList();

                foreach (var truck in activeTrucksStart)
                {
                    truck.Status = DedicatedShippingRequestTruckOrDriverStatus.Busy;
                }
            }
        }
    }
}
