using Abp.BackgroundJobs;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TACHYON.Shipping.ShippingRequestBids
{
    public class RejectOtherBidsJob : BackgroundJob<RejectOtherBidsJobArgs>, ITransientDependency
    {
        private readonly IRepository<ShippingRequestBid, long> _shippingRequestBidsRepository;

        public RejectOtherBidsJob(IRepository<ShippingRequestBid, long> shippingRequestBidsRepository)
        {
            _shippingRequestBidsRepository = shippingRequestBidsRepository;
        }

        public override void Execute(RejectOtherBidsJobArgs args)
        {
            using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MustHaveTenant))
            {
                var otherBids = _shippingRequestBidsRepository.GetAll()
                    .Where(x => x.ShippingRequestId == args.ShippingReuquestId)
                    .Where(x => x.Id != args.AcceptedBidId);

                foreach (var item in otherBids)
                {
                    item.IsRejected = true;
                }
            }
        }
    }
}