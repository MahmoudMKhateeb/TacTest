using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Threading.BackgroundWorkers;
using Abp.Threading.Timers;
using Abp.Timing;
using Org.BouncyCastle.Math.EC.Rfc7748;
using System;
using System.Collections.Generic;
using System.Linq.Dynamic.Core;
using System.Text;
using TACHYON.Authorization.Users;

namespace TACHYON.Shipping.ShippingRequests
{
    public class TrackBidEndDateWorker : PeriodicBackgroundWorkerBase, ISingletonDependency
    {
        private const int CheckPeriodAsMilliseconds = 1 * 60 * 60 * 1000 * 24; //1 day

        private readonly IRepository<ShippingRequest, long> _shippingRequestRepository;
        private readonly UserEmailer _userEmailer;

        public TrackBidEndDateWorker(
            AbpTimer timer,
            IRepository<ShippingRequest, long> shippingRequestRepository,
            UserEmailer userEmailer) : base(timer)
        {
            shippingRequestRepository = _shippingRequestRepository;
            userEmailer = _userEmailer;
            Timer.Period = CheckPeriodAsMilliseconds;
            Timer.RunOnStart = true;
        }

        //#544
        protected override void DoWork()
        {
            using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MustHaveTenant))
            {
                var ExpiresBids = _shippingRequestRepository.GetAllList(u => u.BidEndDate < Clock.Now && u.BidEndDate != null && u.IsClosedBid == false);

                foreach(var item in ExpiresBids)
                {
                    item.IsClosedBid = true;
                    item.CloseBidDate = Clock.Now;
                    Logger.Info(item + " Expired End Bid date, the bid is closed.");
                }
                    
            }
        }
    }
}
