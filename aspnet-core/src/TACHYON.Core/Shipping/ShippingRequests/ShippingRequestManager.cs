using Abp.Application.Features;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Abp.Runtime.Session;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using TACHYON.Features;
using TACHYON.Net.Sms;
using TACHYON.Notifications;
using TACHYON.Routs.RoutPoints;
using TACHYON.Url;

namespace TACHYON.Shipping.ShippingRequests
{
    public class ShippingRequestManager : TACHYONDomainServiceBase
    {
        private readonly IRepository<RoutPoint, long> _routPointRepository;
        private readonly IRepository<ShippingRequest, long> _shippingRequestRepository;
        private readonly IFeatureChecker _featureChecker;
        private readonly IAbpSession _abpSession;
        protected readonly IWebUrlService WebUrlService;


        private readonly ISmsSender _smsSender;
        private readonly IAppNotifier _appNotifier;
        public ShippingRequestManager(ISmsSender smsSender,
            IRepository<RoutPoint, long> routPointRepository,
            IAppNotifier appNotifier,
            IRepository<ShippingRequest, long> shippingRequestRepository, IFeatureChecker featureChecker, IAbpSession abpSession, IWebUrlService webUrlService)
        {
            _smsSender = smsSender;
            _routPointRepository = routPointRepository;
            _appNotifier = appNotifier;
            _shippingRequestRepository = shippingRequestRepository;
            _featureChecker = featureChecker;
            _abpSession = abpSession;
            WebUrlService = webUrlService;
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
        public async void SendSmsToReceiver(RoutPoint point, string Culture)
        {
            string number = point.ReceiverPhoneNumber;
            string formattedDate = point.EndTime?.ToString("dd/MM/yyyy hh:mm");
            var ratingLink = $"{L("ClickToRate")} {WebUrlService.WebSiteRootAddressFormat}account/RatingPage/{point.Code}";
            string message = L(TACHYONConsts.SMSShippingRequestReceiverCode, new CultureInfo(Culture),
                point.WaybillNumber, point.Code, ratingLink);
            if (point.ReceiverFk != null)
            {
                number = point.ReceiverFk.PhoneNumber;
            }
            await _smsSender.SendAsync(number, message);

        }

        /// <summary>
        /// Send shipment code to receiver
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public async Task SendSmsToReceiver(RoutPoint point)
        {
            string number = point.ReceiverPhoneNumber;
            string formattedDate = point.EndTime?.ToString("dd/MM/yyyy hh:mm");
            var ratingLink =
                $"{L("ClickToRate")} {WebUrlService.WebSiteRootAddressFormat}account/RatingPage/{point.Code}";
            string message = L(TACHYONConsts.SMSShippingRequestReceiverCode, point.WaybillNumber, point.Code, ratingLink);
            if (point.ReceiverFk != null)
            {
                await _smsSender.SendAsync(point.ReceiverFk.PhoneNumber, message);
            }
            if (!string.IsNullOrEmpty(number)) await _smsSender.SendAsync(number, message);

        }

        /// <summary>
        /// Send shipment code to receivers
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public async Task SendSmsToReceivers(int tripId)
        {
            var RoutePoints = await _routPointRepository.GetAll().Include(r => r.ReceiverFk).Include(x => x.ShippingRequestTripFk).Where(x => x.ShippingRequestTripId == tripId && x.PickingType == PickingType.Dropoff).ToListAsync();
            RoutePoints.ForEach(async p =>
            {
                await SendSmsToReceiver(p);
            });

        }


        public async Task<ShippingRequest> GetShippingRequestWhenNormalStatus(long ShippingRequestId)
        {

            return await _shippingRequestRepository
                                        .GetAll()
                                        .WhereIf(await _featureChecker.IsEnabledAsync(AppFeatures.Shipper), x => x.TenantId == _abpSession.TenantId && !x.IsTachyonDeal)
                                        .FirstOrDefaultAsync(r => r.Id == ShippingRequestId && (r.Status == ShippingRequestStatus.NeedsAction || r.Status == ShippingRequestStatus.PrePrice || r.Status == ShippingRequestStatus.AcceptedAndWaitingCarrier));
        }
    }
}