using Abp.Configuration;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Localization;
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
                var Trips = _shippingRequestTripRepository.
                    GetAll().        
                    AsNoTracking().
                    Include(r=>r.ShippingRequestFk).
                    Include(p=>p.RoutPoints).
                        ThenInclude(r=>r.ReceiverFk).
                    Where(x => x.ShippingRequestFk.Status == Shipping.ShippingRequests.ShippingRequestStatus.PostPrice &&
                    x.Status == Shipping.Trips.ShippingRequestTripStatus.StandBy && 
                    EF.Functions.DateDiffDay(Clock.Now.Date, x.StartTripDate.Date)==-1).OrderBy(x=>x.ShippingRequestFk.TenantId).ToList();


                Trips.ForEach(t =>
                {
                    string Culture=default;
                   
                    if (t.RoutPoints.Count>0)
                    {
                        var routPoints = t.RoutPoints.ToList();
                        Culture = _settingManager.GetSettingValueForUser(LocalizationSettingNames.DefaultLanguage, t.ShippingRequestFk.TenantId, t.ShippingRequestFk.CreatorUserId.Value, true);

                        routPoints.ForEach(p =>
                        {
                           _shippingRequestManager.SendSmsToReceiver(p, Culture);
                            Task.Delay(1000);
                        });
                    }

                });
            }
              

        }
    }
}
