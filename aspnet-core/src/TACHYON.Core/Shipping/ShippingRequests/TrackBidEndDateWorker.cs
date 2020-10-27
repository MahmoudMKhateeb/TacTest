using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Threading.BackgroundWorkers;
using Abp.Threading.Timers;
using Abp.Timing;
using Org.BouncyCastle.Math.EC.Rfc7748;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using Microsoft.EntityFrameworkCore;
using TACHYON.Authorization.Users;

namespace TACHYON.Shipping.ShippingRequests
{
    public class TrackBidEndDateWorker : PeriodicBackgroundWorkerBase, ISingletonDependency
    {
        private const int CheckPeriodAsMilliseconds = 1 * 60 * 60 * 1000 * 24; //1 day

        private readonly IRepository<ShippingRequest, long> _shippingRequestRepository;

        public TrackBidEndDateWorker(
            AbpTimer timer,
            IRepository<ShippingRequest, long> shippingRequestRepository
            ) : base(timer)
        {
            _shippingRequestRepository = shippingRequestRepository;
            Timer.Period = CheckPeriodAsMilliseconds;
            Timer.RunOnStart = true;
        }

        //#544
        protected override void DoWork()
        {
            using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MustHaveTenant))
            {
                var expiresBids = _shippingRequestRepository.GetAll()
                    .Where(u => u.BidEndDate != null)
                    .Where(u => u.ShippingRequestStatusId == TACHYONConsts.Closed)
                    .Where(u => u.BidEndDate.Value.Date == Clock.Now.Date)
                    .ToList();


                foreach (var item in expiresBids)
                {
                    item.ShippingRequestStatusId = TACHYONConsts.Closed;
                    item.CloseBidDate = Clock.Now;
                    Logger.Info(item + " Expired End Bid date, the bid is closed.");
                }

                //todo add notification here 



                //Open standBy Bids
                var onGoingBids = _shippingRequestRepository.GetAll()
                        .Where(x => x.BidStartDate != null)
                        .Where(x => x.ShippingRequestStatusId == TACHYONConsts.StandBy)
                        .Where(x => x.BidStartDate.Value.Date == Clock.Now.Date)
                        .ToList();

                foreach(var item in onGoingBids)
                {
                    item.ShippingRequestStatusId = TACHYONConsts.OnGoing;
                    Logger.Info(item + " The bid is already started.");
                }
                //todo add notification here 
            }
        }
    }
}
