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
using TACHYON.Notifications;
using TACHYON.Shipping.ShippingRequestTrips;

namespace TACHYON.BackgroundWorkers.ShippingRequests
{/// <summary>
/// Reminder the driver You have a new trip tomorrow
/// </summary>
    public class DriverTripReminderWorker : PeriodicBackgroundWorkerBase, ISingletonDependency
    {
        private const int runEvery = 1 * 60 * 60 * 1000 * 24; //1 day
        private readonly IRepository<ShippingRequestTrip> _shippingRequestTripRepository;
        private readonly IAppNotifier _appNotifier;
        public DriverTripReminderWorker(
            AbpTimer timer,
            IRepository<ShippingRequestTrip> shippingRequestTripRepository,
            IAppNotifier appNotifier) : base(timer)
        {
            Timer.Period = runEvery;
            Timer.RunOnStart = true;
            _shippingRequestTripRepository = shippingRequestTripRepository;
            _appNotifier = appNotifier;
        }
        // we need to search for a way to use async method in our workers

        [UnitOfWork]
        protected override void DoWork()
        {
            using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MustHaveTenant, AbpDataFilters.MayHaveTenant))
            {
                var drivers = _shippingRequestTripRepository.
                    GetAll().
                    Include(d => d.AssignedDriverUserFk).
                    AsNoTracking().
                    Where(x => x.ShippingRequestFk.Status == Shipping.ShippingRequests.ShippingRequestStatus.PostPrice &&
                    x.Status == Shipping.Trips.ShippingRequestTripStatus.New &&
                    x.DriverStatus == Shipping.Trips.ShippingRequestTripDriverStatus.Accepted &&
                    EF.Functions.DateDiffDay(Clock.Now.Date, x.StartTripDate.Date) == -1)
                    .Select(x=> new
                    {
                        Driver = new UserIdentifier(x.AssignedDriverUserFk.TenantId,x.AssignedDriverUserFk.Id),
                        TripId = x.Id
                    }).ToList();

                // I can Send A lot of Message to many users in Firebase Notifier Without any For loop... FYI
                // but with app Notifier we can't do that, Every Msg Must Sent As A Single Notification  
                
                drivers.ForEach(t =>
                {
                    AsyncHelper.RunSync(() => _appNotifier.DriverReminderForTrip(t.Driver,t.TripId.ToString()));
                });
            }

        }
    }
}