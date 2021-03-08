using Abp;
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
using System.Text;
using TACHYON.Notifications;
using TACHYON.Routs.RoutPoints;
using TACHYON.Shipping.ShippingRequests;
using TACHYON.Shipping.Trips;

namespace TACHYON.Shipping.ShippingRequestTrips
{
    public class ShippmentStartWorker : PeriodicBackgroundWorkerBase, ISingletonDependency
    {
        private const int CheckPeriodAsMilliseconds = 1 * 60 * 60 * 1000 * 24; //1 day
        private readonly IAppNotifier _appNotifier;
        private readonly IRepository<ShippingRequestTrip> _shippingRequestTripRepository;
        private readonly IRepository<RoutPoint,long> _routPointRepository;


        public ShippmentStartWorker(
            AbpTimer timer,
            IAppNotifier appNotifier,
            IRepository<ShippingRequestTrip> shippingRequestTripRepository,
            IRepository<RoutPoint, long> routPointRepository) : base(timer)
        {
            Timer.Period = CheckPeriodAsMilliseconds;
            Timer.RunOnStart = true;
            _appNotifier = appNotifier;
            _shippingRequestTripRepository = shippingRequestTripRepository;
            _routPointRepository = routPointRepository;
        }

        [UnitOfWork]
        protected override void DoWork()
        {
            using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MustHaveTenant, AbpDataFilters.MayHaveTenant))
            {
                var StartedShippments = _shippingRequestTripRepository.GetAll()
                    .Where(x => x.StartTripDate.Date == Clock.Now.Date)
                    .Include(x => x.ShippingRequestFk)
                    .ToList();

                //get shipping request Ids for started shippments
                var shippingRequestTripIds = StartedShippments.Select(y => y.Id).ToList();

                //get all way points for started shippments
                var AllWayPoints = _routPointRepository.GetAll()
                    .Include(e => e.FacilityFk)
                    .Where(e => shippingRequestTripIds.Contains(e.ShippingRequestTripId))
                    .Where(e => e.PickingTypeId == TACHYONConsts.PickupPickingType)
                    .ToList();

                foreach (var item in StartedShippments)
                {
                    var driverUserId = item.ShippingRequestFk.AssignedDriverUserId;

                    var pickupFacility = AllWayPoints
                        .FirstOrDefault(e => e.ShippingRequestTripId == item.Id)?.FacilityFk.Name;

                    //send notification to Driver for start shippment
                    _appNotifier.StartShippment(
                        new UserIdentifier(item.ShippingRequestFk.TenantId, driverUserId.Value),
                        item.Id, pickupFacility);
                }

                CurrentUnitOfWork.SaveChanges();
            }
        }

    }
}
