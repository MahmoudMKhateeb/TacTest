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

        public async Task<bool> SetPointToDelivery(IHasDocument document, long? pointId)
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
                x.ShippingRequestTripFk.RoutePointStatus == RoutePointStatus.ReceiverConfirmed &&
                x.ShippingRequestTripFk.AssignedDriverUserId == _abpSession.UserId);
            if (CurrentPoint == null) throw new UserFriendlyException(L("TheTripIsNotFound"));

            if (CurrentPoint.ShippingRequestTripFk.NeedsDeliveryNote && !CurrentPoint.IsDeliveryNoteUploaded)
            {
                throw new UserFriendlyException(L("YouNeedToUploadDeliveryNoteBefore"));
            }
            CurrentPoint.EndTime = Clock.Now;
            //CurrentPoint.DocumentContentType = "image/jpeg";
            //CurrentPoint.DocumentName = document.DocumentName;
            //CurrentPoint.DocumentId = document.DocumentId;


            await InsertRoutePointDocument(CurrentPoint.Id, document, RoutePointDocumentType.POD);


            CurrentPoint.IsActive = false;
            CurrentPoint.IsComplete = true;
            CurrentPoint.CompletedStatus = RoutePointCompletedStatus.Completed;
            await SetRoutStatusTransition(CurrentPoint, RoutePointStatus.DeliveryConfirmation);
            var trip = CurrentPoint.ShippingRequestTripFk;
            trip.RoutePointStatus = RoutePointStatus.DeliveryConfirmation;

            //check if all points finished, then set trip to delivered
            if (await _RoutPointRepository.GetAll().Where(x => x.ShippingRequestTripId == trip.Id && x.IsComplete == false && x.Id != CurrentPoint.Id).CountAsync() == 0)
            {
                trip.Status = ShippingRequestTripStatus.Delivered;
                trip.RoutePointStatus = RoutePointStatus.Delivered;
                trip.EndTripDate = Clock.Now;

                await Done(trip.ShippingRequestId, trip.Id);
                await _invoiceManager.GenertateInvoiceWhenShipmintDelivery(trip);

            }
            else
            {
                trip.RoutePointStatus = RoutePointStatus.DeliveryConfirmation;

            }
            return true;


        }


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
                x.ShippingRequestTripFk.RoutePointStatus == RoutePointStatus.ReceiverConfirmed &&
                x.ShippingRequestTripFk.AssignedDriverUserId == _abpSession.UserId);
            if (CurrentPoint == null) throw new UserFriendlyException(L("TheTripIsNotFound"));

            if (!CurrentPoint.ShippingRequestTripFk.NeedsDeliveryNote) throw new UserFriendlyException(L("TripDidnnotNeedsDeliveryNote"));
            CurrentPoint.IsDeliveryNoteUploaded = true;
            await InsertRoutePointDocument(CurrentPoint.Id, document, RoutePointDocumentType.DeliveryNote);

            return true;


        }

        /// <summary>
        /// Get current active trip for driver
        /// </summary>
        /// <returns></returns>
        public async Task<ShippingRequestTrip> GetActiveTrip()
        {
            var trip = await _ShippingRequestTrip
                            .FirstOrDefaultAsync(t => t.AssignedDriverUserId == _abpSession.UserId && t.Status == ShippingRequestTripStatus.Intransit);
            if (trip == null) throw new UserFriendlyException(L("TheTripIsNotFound"));
            return trip;
        }
        /// <summary>
        /// Get current active route point for driver
        /// </summary>
        /// <returns></returns>
        public async Task<RoutPoint> GetActivePoint()
        {
            var ActivePoint = await _RoutPointRepository.GetAll().Include(x => x.ShippingRequestTripFk).ThenInclude(r => r.ShippingRequestFk).FirstOrDefaultAsync(x => x.IsActive && x.ShippingRequestTripFk.Status != ShippingRequestTripStatus.Canceled && x.ShippingRequestTripFk.AssignedDriverUserId == _abpSession.UserId);
            //if (ActivePoint == null) throw new UserFriendlyException(L("TheTripIsNotFound"));

            return ActivePoint;
        }




        /// <summary>
        /// When start pikup way
        /// </summary>
        /// <param name="routPoint">first location</param>
        /// <param name="FromLocation">location map address from start rout</param>
        /// <returns></returns>
        public async Task StartTransition(RoutPoint routPoint, Point FromLocation)
        {
            FromLocation.SRID = 4326;
            ShippingRequestTripTransition tripTransition = new ShippingRequestTripTransition();
            tripTransition.FromLocation = FromLocation;
            tripTransition.ToPointId = routPoint.Id;
            tripTransition.ToLocation = routPoint.FacilityFk.Location;
            await _shippingRequestTripTransitionRepository.InsertAsync(tripTransition);
            await SetRoutStatusTransition(routPoint, RoutePointStatus.StartedMovingToLoadingLocation);

        }
        /// <summary>
        /// Change transition from old point to new next point
        /// </summary>
        /// <param name="routPoint"></param>
        /// <returns></returns>
        public async Task ChangeTransition(RoutPoint routPoint, RoutePointStatus Status)
        {
            ShippingRequestTripTransition tripTransition = new ShippingRequestTripTransition();

            var oldPointTransition = await GetLastTransitionInComplete(routPoint.ShippingRequestTripId);
            oldPointTransition.IsComplete = true;

            tripTransition.FromLocation = oldPointTransition.ToLocation;
            tripTransition.FromPointId = oldPointTransition.ToPointId;
            tripTransition.ToPointId = routPoint.Id;
            tripTransition.ToLocation = routPoint.FacilityFk.Location;

            await _shippingRequestTripTransitionRepository.InsertAsync(tripTransition);
            await SetRoutStatusTransition(routPoint, Status);

        }
        /// <summary>
        /// Call when the trip is finish
        /// </summary>
        /// <param name="RequestId"></param>
        /// <param name="TripId"></param>
        /// <returns></returns>
        private async Task Done(long RequestId, int TripId)
        {
            await ChangeShippingRequestStatusIfAllTripsDone(RequestId);
            await CloseLastTransitionInComplete(TripId);
        }

        public async Task ChangeShippingRequestStatusIfAllTripsDone(long RequestId)
        {
            if (!_ShippingRequestTrip.GetAll().Any(x => x.AssignedDriverUserId == _abpSession.UserId && x.Status != ShippingRequestTripStatus.Delivered && x.ShippingRequestId == RequestId))
            {
                var Request = await _ShippingRequestRepository.SingleAsync(x => x.Id == RequestId);
                Request.Status = ShippingRequestStatus.Completed;
                await _appNotifier.ShipperShippingRequestFinish(new UserIdentifier(Request.TenantId, _userManager.GetAdminByTenantIdAsync(Request.TenantId).Id), Request);
            }
        }



        public async Task<ShippingRequestTrip> GetTripWhenAccepedOrRejectedByDriver(int id, long userId)
        {
            var trip = await _ShippingRequestTrip
                .GetAllIncluding(o => o.OriginFacilityFk, d => d.DestinationFacilityFk, x => x.ShippingRequestFk)
                .Include(x => x.ShippingRequestTripVases)
                                .FirstOrDefaultAsync(t => t.Id == id &&
                                t.DriverStatus == ShippingRequestTripDriverStatus.None &&
                                t.Status == ShippingRequestTripStatus.New &&
                                t.AssignedDriverUserId == userId);
            if (trip == null) throw new UserFriendlyException(L("TheTripIsNotFound"));
            return trip;
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



        /// <summary>
        /// Get the last transition for trip is not complete
        /// </summary>
        /// <param name="TripId"></param>
        /// <returns></returns>
        private async Task<ShippingRequestTripTransition> GetLastTransitionInComplete(int TripId)
        {
            return await _shippingRequestTripTransitionRepository.SingleAsync(x => x.ToPoint.ShippingRequestTripId == TripId && !x.IsComplete);

        }

        /// <summary>
        /// Close the last transition for trip by set field IsComplete to value true
        /// </summary>
        /// <param name="TripId"></param>
        /// <returns></returns>
        private async Task CloseLastTransitionInComplete(int TripId)
        {
            var Last = await GetLastTransitionInComplete(TripId);
            Last.IsComplete = true;

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