using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
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
            await _appNotifier.ShippingRequestNotifyCarrirerWhenShipperAccepted(shippingRequest);
        }


  
        /// <summary>
        /// Send shipment code to receiver
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public async  void SendSmsToReceiver(RoutPoint point,string Culture)
        {
            string number= point.ReceiverPhoneNumber;
            string message = L(TACHYONConsts.SMSShippingRequestReceiverCode, new CultureInfo(Culture) , point.Code);
           if (point.ReceiverFk !=null)
            {
                number = point.ReceiverFk.PhoneNumber;
            }
            await _smsSender.SendAsync(number, message);

        }

    }
}
