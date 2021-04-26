using Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using TACHYON.Net.Sms;
using TACHYON.Notifications;
using TACHYON.Routs.RoutPoints;

namespace TACHYON.Shipping.ShippingRequests
{
    public class ShippingRequestManager : TACHYONDomainServiceBase
    {
        private readonly IRepository<RoutPoint,long> _routPointRepository;
        private readonly IRepository<RoutePointReceiverReceiveShipmentCode> _receiverReceiveShipmentCodeRepository;

        private readonly ISmsSender _smsSender;
        private readonly IAppNotifier _appNotifier;
        public ShippingRequestManager(ISmsSender smsSender,
            IRepository<RoutPoint, long> routPointRepository,
            IAppNotifier appNotifier,
            IRepository<RoutePointReceiverReceiveShipmentCode> receiverReceiveShipmentCodeRepository)
        {
            _smsSender = smsSender;
            _routPointRepository = routPointRepository;
            _appNotifier = appNotifier;
            _receiverReceiveShipmentCodeRepository = receiverReceiveShipmentCodeRepository;
        }

        /// <summary>
        /// Set shipping request status to post price
        /// </summary>
        /// <param name="shippingRequest"></param>
        /// <returns></returns>

        public async Task SetToPostPrice(ShippingRequest shippingRequest)
        {
            shippingRequest.Status = ShippingRequestStatus.PostPrice;
            await SendSmsToReceiversByShippingRequestId(shippingRequest.Id);
            await _appNotifier.ShippingRequestNotifyCarrirerWhenShipperAccepted(shippingRequest);
        }
        /// <summary>
        /// Call only when the shipping status post price
        /// </summary>
        /// <param name="shippingRequest"></param>
        /// <returns></returns>
        private async Task SendSmsToReceiversByShippingRequestId(long ShippingRequestId)
        {
            var Points = _routPointRepository.GetAll()
                .Include(r=>r.ReceiverFk)
                .AsTracking()
                .Where(x => x.ShippingRequestTripFk.ShippingRequestId == ShippingRequestId  && x.PickingType== PickingType.Dropoff);
            if (Points==null) return;
            foreach (var p in Points)
            {
                await SendSmsToReceiver(p);
            }
        }

        public async Task SendSmsToReceiversByTripId(long id)
        {
            var Points = _routPointRepository.GetAll()
                .Include(r => r.ReceiverFk)
                .AsTracking()
                .Where(x => x.ShippingRequestTripId == id && x.PickingType == PickingType.Dropoff);
            if (Points == null) return;
            foreach (var p in Points)
            {
                await SendSmsToReceiver(p);
            }
        }
        /// <summary>
        /// Send shipment code to receiver
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        private async Task SendSmsToReceiver(RoutPoint point)
        {
            string number= point.ReceiverPhoneNumber;
            string message = L(TACHYONConsts.SMSShippingRequestReceiverCode, point.Code);
           if (point.ReceiverFk !=null)
            {
                number = point.ReceiverFk.PhoneNumber;
            }
            if (await CheckIfReceiverReceiveShipmentCodeBefore(point.Id, number)) return;
            await _smsSender.SendAsync(number, message);

        }

        /// <summary>
        /// Check if the receiver receive shipment Code before
        /// </summary>
        /// <param name="id"></param>
        /// <param name="PhoneNumber"></param>
        /// <returns></returns>
        private async Task<bool> CheckIfReceiverReceiveShipmentCodeBefore(long id,string PhoneNumber)
        {
            if (! await _receiverReceiveShipmentCodeRepository.GetAll().AnyAsync(x=>x.PointId== id && x.ReceiverPhone == PhoneNumber))
            {
                await _receiverReceiveShipmentCodeRepository.InsertAsync(new  RoutePointReceiverReceiveShipmentCode(id, PhoneNumber));
                return false;
            }
            return true;
        }
    }
}
