using Abp;
using Abp.Configuration;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Threading;
using Abp.Threading.BackgroundWorkers;
using Abp.Threading.Timers;
using Abp.Timing;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using TACHYON.Firebases;
using TACHYON.Shipping.ShippingRequestTrips;

namespace TACHYON.BackgroundWorkers.ShippingRequests
{/// <summary>
/// Reminder the driver You have a new trip tomorrow
/// </summary>
    public class DriverTripReminderWorker : PeriodicBackgroundWorkerBase, ISingletonDependency
    {
        private const int runEvery =  1 * 60 * 60 * 1000 * 24; //1 day
        private readonly IRepository<ShippingRequestTrip> _shippingRequestTripRepository;
        private readonly IFirebaseNotifier _firebaseNotifier;
        public DriverTripReminderWorker(
            AbpTimer timer,
            IRepository<ShippingRequestTrip> shippingRequestTripRepository,
            IFirebaseNotifier firebaseNotifier) : base(timer)
        {
            Timer.Period = runEvery;
            Timer.RunOnStart = true;
            _shippingRequestTripRepository = shippingRequestTripRepository;
            _firebaseNotifier = firebaseNotifier;
        }

        [UnitOfWork]
        protected override void DoWork()
        {
            using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MustHaveTenant, AbpDataFilters.MayHaveTenant))
            {
                var Trips = _shippingRequestTripRepository.
                    GetAll().
                    Include(d=>d.AssignedDriverUserFk).
                    AsNoTracking().
                    Where(x => x.ShippingRequestFk.Status == Shipping.ShippingRequests.ShippingRequestStatus.PostPrice &&
                    x.Status == Shipping.Trips.ShippingRequestTripStatus.New &&
                    x.DriverStatus== Shipping.Trips.ShippingRequestTripDriverStatus.Accepted &&
                    EF.Functions.DateDiffDay(Clock.Now.Date, x.StartTripDate.Date) == -1).ToList();

                Trips.ForEach(t =>
                {
                    AsyncHelper.RunSync(() => _firebaseNotifier.ReminderDriverForTrip(new UserIdentifier(t.AssignedDriverUserFk.TenantId, t.AssignedDriverUserId.Value), t.Id.ToString()));
                });
            }

        }
    }
}
