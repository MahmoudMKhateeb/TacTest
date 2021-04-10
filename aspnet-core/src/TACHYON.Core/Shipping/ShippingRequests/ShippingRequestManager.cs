using Abp.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TACHYON.Net.Sms;
using TACHYON.Routs.RoutPoints;
using TACHYON.Shipping.ShippingRequestTrips;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using TACHYON.Notifications;

namespace TACHYON.Shipping.ShippingRequests
{
    public class ShippingRequestManager : TACHYONDomainServiceBase
    {
        private readonly IRepository<RoutPoint,long> _routPointRepository;

        private readonly ISmsSender _smsSender;
        private readonly IAppNotifier _appNotifier;
        public ShippingRequestManager(ISmsSender smsSender,
            IRepository<RoutPoint, long> routPointRepository,
            IAppNotifier appNotifier)
        {
            _smsSender = smsSender;
            _routPointRepository = routPointRepository;
            _appNotifier = appNotifier;
        }

        /// <summary>
        /// Set shipping request status to post price
        /// </summary>
        /// <param name="shippingRequest"></param>
        /// <returns></returns>

        public async Task SetToPostPrice(ShippingRequest shippingRequest)
        {
            shippingRequest.Status = ShippingRequestStatus.PostPrice;
            await SendSmsToReceivers(shippingRequest.Id);
            await _appNotifier.ShippingRequestNotifyCarrirerWhenShipperAccepted(shippingRequest);
        }
        /// <summary>
        /// Call only when the shipping status post price
        /// </summary>
        /// <param name="shippingRequest"></param>
        /// <returns></returns>
        private async Task SendSmsToReceivers(long ShippingRequestId)
        {
            var Points = _routPointRepository.GetAll()
                .Include(r=>r.ReceiverFk)
                .AsTracking()
                .Where(x => x.ShippingRequestTripFk.ShippingRequestId == ShippingRequestId  && x.PickingType== PickingType.Dropoff);
            foreach (var p in Points)
            {
                await SendSmsToReceiver(p);
            }
        }

        private async Task SendSmsToReceiver(RoutPoint point)
        {
            string number= point.ReceiverPhoneNumber;
            string message = L("ShippingRequestReceiverCode", point.Code);
           if (point.ReceiverFk !=null)
            {
                number = point.ReceiverFk.PhoneNumber;
            }
            try { await _smsSender.SendAsync(number, message); }
            catch { }

        }
    }
}
