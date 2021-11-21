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
            IAbpSession abpSession, IRepository<RoutPointDocument, long> routPointDocumentRepository)
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

        /// <summary>
        /// driver confirm the trip has finished 
        /// </summary>
        /// <param name="document"></param>
        /// <param name="receiverCode"></param>
        /// <returns></returns>
        public async Task<bool> UploadDeliveryNote(IHasDocument document, long? pointId)
        {
            DisableTenancyFilters();
            var CurrentPoint = await _RoutPointRepository.GetAll().
                Include(x => x.ShippingRequestTripFk)
                    .ThenInclude(x => x.ShippingRequestTripVases)
                .Include(x => x.ShippingRequestTripFk)
                    .ThenInclude(x => x.ShippingRequestFk)
                     .ThenInclude(x => x.Tenant)
                .WhereIf(pointId == null, x => x.IsActive)
                .WhereIf(pointId != null, x => x.Id == pointId)
                .FirstOrDefaultAsync(x =>
                x.ShippingRequestTripFk.AssignedDriverUserId == _abpSession.UserId);
            if (CurrentPoint == null) throw new UserFriendlyException(L("TheTripIsNotFound"));

            if (!CurrentPoint.ShippingRequestTripFk.NeedsDeliveryNote) throw new UserFriendlyException(L("TripDidnnotNeedsDeliveryNote"));
            CurrentPoint.IsDeliveryNoteUploaded = true;
            await InsertRoutePointDocument(CurrentPoint.Id, document, RoutePointDocumentType.DeliveryNote);
            return true;


        }
        public async Task SetRoutStatusTransition(RoutPoint routPoint, RoutePointStatus Status)
        {
            //await _invoiceManager.GenertateInvoiceWhenShipmintDelivery(routPoint.ShippingRequestTripFk);
            routPoint.Status = Status;
            await _routPointStatusTransitionRepository.InsertAsync(new RoutPointStatusTransition
            {
                PointId = routPoint.Id,
                Status = Status
            });
        }
        public async Task NotifyCarrierWithDriverGpsOff(User user)
        {
            var carrierAdminUser = await _userManager.GetAdminByTenantIdAsync(user.TenantId.Value);
            await _appNotifier.NotifyCarrierWithDriverGpsOff(new UserIdentifier(user.TenantId, carrierAdminUser.Id), user);
        }
        private async Task InsertRoutePointDocument(long PointId, IHasDocument document, RoutePointDocumentType documentType)
        {
            var routePointDocument = new RoutPointDocument();
            routePointDocument.RoutPointId = PointId;
            routePointDocument.RoutePointDocumentType = documentType;
            routePointDocument.DocumentContentType = "image/jpeg";
            routePointDocument.DocumentName = document.DocumentName;
            routePointDocument.DocumentId = document.DocumentId;
            await _routPointDocumentRepository.InsertAsync(routePointDocument);
        }
    }
}