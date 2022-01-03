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
using TACHYON.Notifications;
using TACHYON.Shipping.ShippingRequests;

namespace TACHYON.BackgroundWorkers.ShippingRequests
{
    public class ShipperReminderToCompelteTripsWroker : PeriodicBackgroundWorkerBase, ISingletonDependency
    {
        private const int runEvery = 1 * 60 * 60 * 1000 * 24; //1 day
        private readonly IRepository<ShippingRequest, long> _shippingRequestRepository;
        private readonly IAppNotifier _appNotifier;
        public ShipperReminderToCompelteTripsWroker(
            AbpTimer timer,
            IRepository<ShippingRequest, long> shippingRequestRepository,
            IAppNotifier appNotifier) : base(timer)
        {
            Timer.Period = runEvery;
            Timer.RunOnStart = true;
            _shippingRequestRepository = shippingRequestRepository;
            _appNotifier = appNotifier;
        }

        [UnitOfWork]
        protected override void DoWork()
        {
            using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MustHaveTenant, AbpDataFilters.MayHaveTenant))
            {
                var shippingRequestsToRemind = _shippingRequestRepository.
                    GetAll().
                    AsNoTracking().
                    Where
                    (
                            x => x.Status == Shipping.ShippingRequests.ShippingRequestStatus.PostPrice &&
                            x.TotalsTripsAddByShippier < x.NumberOfTrips &&
                            EF.Functions.DateDiffDay(Clock.Now.Date, x.EndTripDate.Value.Date) >= -5/* before 5 days*/
                    ).Select(x=> new
                        {UserIdentifier = new UserIdentifier(x.TenantId, x.CreatorUserId.Value), ShippingRequestId = x.Id})
                    .ToList();

                foreach (var item in shippingRequestsToRemind)
                    AsyncHelper.RunSync(() =>_appNotifier.ShipperReminderToCompleteTrips(item.ShippingRequestId, item.UserIdentifier));
            }

        }
    }
}