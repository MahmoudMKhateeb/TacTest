using Abp;
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
using TACHYON.Shipping.ShippingRequests;

namespace TACHYON.BackgroundWorkers.ShippingRequests
{
    public class ShipperReminderToCompelteTripsWroker : PeriodicBackgroundWorkerBase, ISingletonDependency
    {
        private const int runEvery = 1 * 60 * 60 * 1000 * 24; //1 day
        private readonly IRepository<ShippingRequest, long> _shippingRequestRepository;
        private readonly IFirebaseNotifier _firebaseNotifier;
        private readonly IAppNotifier _appNotifier;
        public ShipperReminderToCompelteTripsWroker(
            AbpTimer timer,
            IRepository<ShippingRequest, long> shippingRequestRepository,
            IFirebaseNotifier firebaseNotifier,
            IAppNotifier appNotifier) : base(timer)
        {
            Timer.Period = runEvery;
            Timer.RunOnStart = true;
            _shippingRequestRepository = shippingRequestRepository;
            _firebaseNotifier = firebaseNotifier;
            _appNotifier = appNotifier;
        }

        [UnitOfWork]
        protected override void DoWork()
        {
            using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MustHaveTenant, AbpDataFilters.MayHaveTenant))
            {
                var request = _shippingRequestRepository.
                    GetAll().
                    AsNoTracking().
                    Where
                    (
                            x => x.Status == Shipping.ShippingRequests.ShippingRequestStatus.PostPrice &&
                            x.TotalsTripsAddByShippier < x.NumberOfTrips &&
                            EF.Functions.DateDiffDay(Clock.Now.Date, x.EndTripDate.Value.Date) >= -5/* before 5 days*/
                    ).ToList();

                request.ForEach(r =>
                {
                    var user = new UserIdentifier(r.TenantId, r.CreatorUserId.Value);
                    AsyncHelper.RunSync
                    (
                        () => _firebaseNotifier.General
                            (
                                user,
                                new System.Collections.Generic.Dictionary<string, string>() { ["id"] = r.Id.ToString() },
                                "",
                                "ShipperReminderToCompelteTrips"
                            )

                    );
                    AsyncHelper.RunSync(() => _appNotifier.ShipperReminderToCompelteTrips(user, r));

                });
            }

        }
    }
}