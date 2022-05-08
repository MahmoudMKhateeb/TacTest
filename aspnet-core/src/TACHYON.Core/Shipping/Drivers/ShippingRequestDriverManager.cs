using Abp;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Abp.Runtime.Session;
using Abp.Timing;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using System.Linq;
using System.Threading.Tasks;
using TACHYON.Authorization.Users;
using TACHYON.Common;
using TACHYON.Invoices;
using TACHYON.Notifications;
using TACHYON.Routs.RoutPoints;
using TACHYON.Shipping.RoutPoints;
using TACHYON.Shipping.ShippingRequests;
using TACHYON.Shipping.ShippingRequestTrips;
using TACHYON.Shipping.Trips;

namespace TACHYON.Shipping.Drivers
{
    public class ShippingRequestDriverManager : TACHYONDomainServiceBase
    {
        private readonly IRepository<ShippingRequestTrip> _ShippingRequestTrip;
        private readonly IRepository<RoutPoint, long> _RoutPointRepository;
        private readonly IRepository<ShippingRequest, long> _ShippingRequestRepository;
        private readonly IRepository<RoutPointStatusTransition> _routPointStatusTransitionRepository;
        private readonly IRepository<RoutPointDocument, long> _routPointDocumentRepository;

        private readonly InvoiceManager _invoiceManager;
        private readonly UserManager _userManager;
        private readonly IAppNotifier _appNotifier;
        private readonly IAbpSession _abpSession;
        private readonly IRepository<ShippingRequestTripTransition> _shippingRequestTripTransitionRepository;

        public ShippingRequestDriverManager(IRepository<ShippingRequestTrip> ShippingRequestTrip,
            IRepository<RoutPoint, long> RoutPointRepository,
            IRepository<ShippingRequest, long> ShippingRequestRepository,
            IRepository<ShippingRequestTripTransition> shippingRequestTripTransitionRepository,
            IRepository<RoutPointStatusTransition> routPointStatusTransitionRepository,
            InvoiceManager invoiceManager,
            UserManager userManager,
            IAppNotifier appNotifier,
            IAbpSession abpSession,
            IRepository<RoutPointDocument, long> routPointDocumentRepository)
        {
            _ShippingRequestTrip = ShippingRequestTrip;
            _ShippingRequestRepository = ShippingRequestRepository;
            _RoutPointRepository = RoutPointRepository;
            _shippingRequestTripTransitionRepository = shippingRequestTripTransitionRepository;
            _routPointStatusTransitionRepository = routPointStatusTransitionRepository;
            _invoiceManager = invoiceManager;
            _userManager = userManager;
            _appNotifier = appNotifier;
            _abpSession = abpSession;
            _routPointDocumentRepository = routPointDocumentRepository;
        }


        public async Task SetRoutStatusTransition(RoutPoint routPoint, RoutePointStatus Status)
        {
            //await _invoiceManager.GenertateInvoiceWhenShipmintDelivery(routPoint.ShippingRequestTripFk);
            routPoint.Status = Status;
            await _routPointStatusTransitionRepository.InsertAsync(new RoutPointStatusTransition
            {
                PointId = routPoint.Id, Status = Status
            });
        }

        public async Task NotifyCarrierWithDriverGpsOff(User user)
        {
            var carrierAdminUser = await _userManager.GetAdminByTenantIdAsync(user.TenantId.Value);
            await _appNotifier.NotifyCarrierWithDriverGpsOff(new UserIdentifier(user.TenantId, carrierAdminUser.Id),
                user);
        }
    }
}