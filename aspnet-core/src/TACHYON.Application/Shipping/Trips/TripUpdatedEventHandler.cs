// unset

using Abp;
using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.Events.Bus.Entities;
using Abp.Events.Bus.Handlers;
using Abp.Extensions;
using Abp.Threading;
using NPOI.HSSF.Record;
using System;
using System.Threading.Tasks;
using TACHYON.BayanIntegration;
using TACHYON.Firebases;
using TACHYON.Notifications;
using TACHYON.Shipping.ShippingRequestTrips;

namespace TACHYON.Shipping.Trips
{
    public class TripUpdatedEventHandler : IEventHandler<EntityUpdatedEventData<ShippingRequestTrip>>, ITransientDependency
    {
        private readonly BayanIntegrationManager _bayanIntegrationManager;
        private readonly IAppNotifier _appNotifier;
        private readonly IFirebaseNotifier _firebaseNotifier;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public TripUpdatedEventHandler(BayanIntegrationManager bayanIntegrationManager, IAppNotifier appNotifier, IFirebaseNotifier firebaseNotifier, IUnitOfWorkManager unitOfWorkManager)
        {
            _bayanIntegrationManager = bayanIntegrationManager;
            _appNotifier = appNotifier;
            _firebaseNotifier = firebaseNotifier;
            _unitOfWorkManager = unitOfWorkManager;
        }

        public void HandleEvent(EntityUpdatedEventData<ShippingRequestTrip> eventData)
        {
            //todo send notification for driver and shipper and carrier using eventBus

            AsyncHelper.RunSync(async () => await NotifyTripUpdated(eventData.Entity));

            // Add BayanIntegration Jobs To the Queue
            if (eventData.Entity.AssignedDriverUserId.HasValue && eventData.Entity.AssignedTruckId.HasValue)
            {
                if (eventData.Entity.BayanId.IsNullOrEmpty())
                {
                    AsyncHelper.RunSync(() => _bayanIntegrationManager.QueueCreateConsignmentNote(eventData.Entity.Id));
                }
                else
                {
                    AsyncHelper.RunSync(() => _bayanIntegrationManager.QueueEditConsignmentNote(eventData.Entity.Id));
                }
            }
        }

        // todo add localization for notifications messages 
        private async Task NotifyTripUpdated(ShippingRequestTrip trip)
        {
            var shipperTenantId = trip.ShippingRequestFk.TenantId;
            var carrierTenantId = trip.ShippingRequestFk.CarrierTenantId;


            if (carrierTenantId is null)
            {
                await _appNotifier.NotifyShipperWhenTripUpdated(shipperTenantId, trip.Id);
                await _appNotifier.NotifyTachyonDealWhenTripUpdated(trip.Id);
            }
            else
            {
                await _appNotifier.NotifyShipperCarrierAndTachyonDealerWhenTripUpdated
                    (shipperTenantId, carrierTenantId.Value, trip.Id);

                var driverId = trip.AssignedDriverUserId;
                if (driverId is null || trip.DriverStatus != ShippingRequestTripDriverStatus.Accepted) return;

                var driverIdentifier = new UserIdentifier(carrierTenantId, driverId.Value);
                using (var unitOfWork = _unitOfWorkManager.Begin())
                {
                    await _firebaseNotifier.TripChanged(driverIdentifier, trip.Id.ToString());
                    await unitOfWork.CompleteAsync();
                }
            }
        }
    }
}