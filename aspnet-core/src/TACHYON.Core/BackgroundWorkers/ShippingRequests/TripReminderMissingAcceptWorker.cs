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
{
    public class TripReminderMissingAcceptWorker : PeriodicBackgroundWorkerBase, ISingletonDependency
    {
        private const int runEvery = 60 * 60 * 1000 ; //1 hour
        private readonly IRepository<ShippingRequestTrip> _shippingRequestTripRepository;
        private readonly IFirebaseNotifier _firebaseNotifier;
        private readonly IAppNotifier _appNotifier;
        public TripReminderMissingAcceptWorker(
            AbpTimer timer,
            IRepository<ShippingRequestTrip> shippingRequestTripRepository,
            IFirebaseNotifier firebaseNotifier, IAppNotifier appNotifier) : base(timer)
        {
            Timer.Period = runEvery;
            Timer.RunOnStart = true;
            _shippingRequestTripRepository = shippingRequestTripRepository;
            _firebaseNotifier = firebaseNotifier;
            _appNotifier = appNotifier;
        }

        [UnitOfWork]
        protected override void DoWork()
        {
            using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MustHaveTenant, AbpDataFilters.MayHaveTenant))
            {
                var Trips = _shippingRequestTripRepository
                    .GetAll()
                    .Include(d=>d.AssignedDriverUserFk)   
                    .Include(r=>r.ShippingRequestFk)
                        .ThenInclude(c=>c.CarrierTenantFk)
                    .AsNoTracking()
                    .Where(x => x.ShippingRequestFk.Status == Shipping.ShippingRequests.ShippingRequestStatus.PostPrice &&
                    x.Status == Shipping.Trips.ShippingRequestTripStatus.New &&
                    x.DriverStatus== Shipping.Trips.ShippingRequestTripDriverStatus.None &&
                    x.AssignedDriverTime.HasValue &&
                    EF.Functions.DateDiffHour(x.AssignedDriverTime,Clock.Now) <=11).ToList();

                Trips.ForEach(t =>
                {
                    var totalDiffHour = EF.Functions.DateDiffHour(t.AssignedDriverTime,Clock.Now);
                    if (totalDiffHour <= 3) // driver
                    {
                     

                        AsyncHelper.RunSync(() => _firebaseNotifier.PushNotificationToDriverWhenAssignTrip(new UserIdentifier(t.AssignedDriverUserFk.TenantId, t.AssignedDriverUserId.Value),t.Id.ToString(), t.WaybillNumber.ToString()));

                    }
                    // carrier
                    else if (totalDiffHour <= 5 || totalDiffHour <= 7 || totalDiffHour <= 9) {
                     
                        AsyncHelper.RunSync(() => _appNotifier.CarrierTripNeedAccept(t));

                    } //TMS
                    else if (totalDiffHour > 9)
                    {
                        AsyncHelper.RunSync(() => _appNotifier.TMSTripNeedAccept(t));

                    }
                });
            }

        }
    }
}
