using Abp.Configuration;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Threading.BackgroundWorkers;
using Abp.Threading.Timers;
using Abp.Timing;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using TACHYON.Shipping.ShippingRequests;
using TACHYON.Shipping.ShippingRequestTrips;

namespace TACHYON.BackgroundWorkers.ShippingRequests
{
    public class SendShipmentCodeToReceiverWorker : PeriodicBackgroundWorkerBase, ISingletonDependency
    {
        private const int CheckPeriodAsMilliseconds = 1 * 60 * 60 * 1000 * 24; //1 day
        private readonly ISettingManager _settingManager;
        private readonly IRepository<ShippingRequestTrip> _shippingRequestTripRepository;
        private readonly ShippingRequestManager _shippingRequestManager;
        public SendShipmentCodeToReceiverWorker(AbpTimer timer, ISettingManager settingManager, IRepository<ShippingRequestTrip> shippingRequestTripRepository,
            ShippingRequestManager shippingRequestManager) : base(timer)
        {
            Timer.Period = CheckPeriodAsMilliseconds;
            _settingManager = settingManager;
            _shippingRequestTripRepository = shippingRequestTripRepository;
            _shippingRequestManager = shippingRequestManager;
        }
        [UnitOfWork]
        protected override void DoWork()
        {
            using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MustHaveTenant, AbpDataFilters.MayHaveTenant))
            {
                var routePoints = _shippingRequestTripRepository.
                    GetAll().        
                    AsNoTracking().
                    Include(p=>p.RoutPoints).
                        ThenInclude(r=>r.ReceiverFk).
                    Where(x => x.ShippingRequestFk.Status == Shipping.ShippingRequests.ShippingRequestStatus.PostPrice &&
                    x.Status == Shipping.Trips.ShippingRequestTripStatus.StandBy && x.Id !=3 &&
                    EF.Functions.DateDiffDay(Clock.Now, x.StartTripDate)==-1).Select(x=>x.RoutPoints).ToList();
            
                routePoints.ForEach(Points =>
                {
                    foreach (var p in Points)
                    {
                        _shippingRequestManager.SendSmsToReceiver(p);
                        Task.Delay(1000);
                    }
                });
            }
              //  _settingManager.GetSettingValueForUser(LocalizationSettingNames.DefaultLanguage, 1, 3, true);

        }
    }
}
