//using Abp;
//using Abp.BackgroundJobs;
//using Abp.Domain.Repositories;
//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Threading.Tasks;
//using TACHYON.Features;
//using TACHYON.Invoices.Balances;
//using TACHYON.Notifications;
//using TACHYON.Shipping.ShippingRequests;
//using TACHYON.TachyonPriceOffers;

//namespace TACHYON.Shipping.ShippingRequestBids
//{
//    public class ShippingRequestBidManager : TACHYONDomainServiceBase
//    {
//        private readonly IRepository<ShippingRequestBid, long> _shippingRequestBidsRepository;
//        private readonly BalanceManager _balanceManager;
//        private readonly ShippingRequestManager _shippingRequestManager;
//        private readonly BackgroundJobManager _backgroundJobManager;
//        IAppNotifier _appNotifier;

//        public ShippingRequestBidManager(IRepository<ShippingRequestBid, long> shippingRequestBidsRepository,
//            BalanceManager balanceManager,
//            ShippingRequestManager shippingRequestManager, BackgroundJobManager backgroundJobManager,
//            IAppNotifier appNotifier)
//        {
//            _shippingRequestBidsRepository = shippingRequestBidsRepository;
//            _balanceManager = balanceManager;
//            _shippingRequestManager = shippingRequestManager;
//            _backgroundJobManager = backgroundJobManager;
//            _appNotifier = appNotifier;
//        }

//        public async Task AcceptBidAndGoToPostPriceAsync(ShippingRequestBid bid)
//        {
//            bid.IsAccepted = true;
//            bid.ShippingRequestFk.BidStatus = ShippingRequestBidStatus.Closed;
//            if(!bid.ShippingRequestFk.IsTachyonDeal)
//            {
//                await _balanceManager.ShipperCanAcceptPrice(bid.ShippingRequestFk.TenantId, bid.price, bid.ShippingRequestId);
//                AssignShippingRequestInfo(bid.ShippingRequestFk, bid);
//                await _shippingRequestManager.SetToPostPrice(bid.ShippingRequestFk);
//            }

//            //Reject the other bids of this shipping request by background job
//            await _backgroundJobManager.EnqueueAsync<RejectOtherBidsJob, RejectOtherBidsJobArgs>
//                (new RejectOtherBidsJobArgs { AcceptedBidId = bid.Id, ShippingReuquestId = bid.ShippingRequestId });

//            await _appNotifier.AcceptShippingRequestBid(new UserIdentifier(bid.TenantId, bid.CreatorUserId.Value), bid.ShippingRequestId);
//        }

//        private void AssignShippingRequestInfo(ShippingRequest shippingRequestItem, ShippingRequestBid bid)
//        {
//            shippingRequestItem.CarrierTenantId = bid.TenantId;
//            shippingRequestItem.Price = bid.price; //bid price that biddingCommission added to the base
//                                                   //  shippingRequestItem.Status = ShippingRequestStatus.PostPrice;
//            shippingRequestItem.CarrierPriceType = CarrierPriceType.ShipperBidding;
//            shippingRequestItem.ActualCommissionValue = bid.ActualCommissionValue;
//            shippingRequestItem.ActualPercentCommission = bid.ActualPercentCommission;
//            shippingRequestItem.ActualMinCommissionValue = bid.ActualMinCommissionValue;
//            shippingRequestItem.TotalCommission = bid.TotalCommission;
//            shippingRequestItem.VatAmount = bid.VatAmount;
//            shippingRequestItem.SubTotalAmount = bid.PriceSubTotal;
//        }

//    }
//}

