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
using System.Threading.Tasks;
using Abp;
using Microsoft.EntityFrameworkCore;
using TACHYON.Authorization.Users;
using TACHYON.Notifications;
using TACHYON.Shipping.ShippingRequestBids;

namespace TACHYON.Shipping.ShippingRequests
{
    public class TrackBidEndDateWorker : PeriodicBackgroundWorkerBase, ISingletonDependency
    {
        private const int CheckPeriodAsMilliseconds = 1 * 60 * 60 * 1000 * 24; //1 day

        private readonly IRepository<ShippingRequest, long> _shippingRequestRepository;
        private readonly IAppNotifier _appNotifier;
        private readonly BidDomainService _bidDomainService;


        public TrackBidEndDateWorker(
            AbpTimer timer,
            IRepository<ShippingRequest, long> shippingRequestRepository,
            IAppNotifier appNotifier,
            BidDomainService bidDomainService

            ) : base(timer)
        {
            _shippingRequestRepository = shippingRequestRepository;
            Timer.Period = CheckPeriodAsMilliseconds;
            Timer.RunOnStart = true;
            _appNotifier = appNotifier;
        }

        //#544
        protected override void DoWork()
        {
            using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MustHaveTenant))
            {
                var expiresBids = _shippingRequestRepository.GetAll()
                    .Where(u => u.BidEndDate != null)
                    .Where(u => u.ShippingRequestStatusId == TACHYONConsts.ShippingRequestStatusClosed)
                    .Where(u => u.BidEndDate.Value.Date == Clock.Now.Date)
                    .ToList();


                foreach (var item in expiresBids)
                {
                    item.ShippingRequestStatusId = TACHYONConsts.ShippingRequestStatusClosed;
                    item.CloseBidDate = Clock.Now;
                }

                //todo add notification here 



                //Open standBy Bids
                var onGoingBids = _shippingRequestRepository.GetAll()
                        .Where(x => x.BidStartDate != null)
                        .Where(x => x.ShippingRequestStatusId == TACHYONConsts.ShippingRequestStatusStandBy)
                        .Where(x => x.BidStartDate.Value.Date == Clock.Now.Date)
                        .ToList();

                foreach (var item in onGoingBids)
                {
                    item.ShippingRequestStatusId = TACHYONConsts.ShippingRequestStatusOnGoing;
                    var users = Task.Run<UserIdentifier[]>(async () => await _bidDomainService.GetCarriersByTruckTypeArrayAsync(item.TrucksTypeId)).Result;
                    // to carrier
                    _appNotifier.ShippingRequestAsBidWithSameTruckAsync(users, item.Id);
                      //todo add notification here to shipper

                }
               
            }
        }
    }
}
