using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Threading.BackgroundWorkers;
using Abp.Threading.Timers;
using Abp.Timing;
using Microsoft.EntityFrameworkCore;
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
                ExpiredRentedDedicatedRequests();

                //Track drivers statuses
                DriverStartOrEndRentalPeriodStatusesChange();

                //Track trucks statuses
                TrucksStartOrEndRentalPeriodStatusesChange();
            }
        }

        private void TrucksStartOrEndRentalPeriodStatusesChange()
        {
            var trucksList = _dedicatedShippingRequestTruck.GetAll()
                                .Include(x => x.Truck)
                                .Include(x => x.ShippingRequest.ReferenceNumber)
                                .ToList();

            var activeTrucksStart = trucksList.Where(x => x.ShippingRequest.RentalStartDate.Value.Date >= Clock.Now.Date &&
                x.Status != WorkingStatus.Busy);
            foreach (var truck in activeTrucksStart)
            {
                truck.Status = WorkingStatus.Busy;
            }

            var busyTrucksEnded = trucksList.Where(x => x.ShippingRequest.RentalEndDate.Value.Date < Clock.Now.Date &&
                x.Status == WorkingStatus.Busy);
            foreach (var truck in activeTrucksStart)
            {
                truck.Status = WorkingStatus.Active;
            }
        }

        private void DriverStartOrEndRentalPeriodStatusesChange()
        {
            var driversList = _dedicatedShippingRequestDriver.GetAll()
                .Include(x => x.DriverUser)
                .Include(x => x.ShippingRequest.ReferenceNumber)
                .ToList();

            var activeDriversStart = driversList.Where(x => x.ShippingRequest.RentalStartDate.Value.Date >= Clock.Now.Date &&
                x.Status != WorkingStatus.Busy);
            foreach (var driver in activeDriversStart)
            {
                driver.Status = WorkingStatus.Busy;
            }

            var busyDriversEnded = driversList.Where(x => x.ShippingRequest.RentalEndDate.Value.Date < Clock.Now.Date &&
                x.Status == WorkingStatus.Busy);
            foreach (var driver in busyDriversEnded)
            {
                driver.Status = WorkingStatus.Active;
            }
        }

        private void ExpiredRentedDedicatedRequests()
        {
            var expiredRentalShippingRequests = _shippingRequestRepository.GetAll()
                                .Where(x => x.ShippingRequestFlag == ShippingRequestFlag.Dedicated &&
                                 x.RentalEndDate != null && x.RentalEndDate.Value.Date > Clock.Now.Date
                                 && x.Status != ShippingRequestStatus.Expired)
                                .ToList();

            foreach (var request in expiredRentalShippingRequests)
            {
                request.Status = ShippingRequestStatus.Expired;
            }
        }
    }
}
