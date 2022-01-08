// unset

using Abp.Dependency;
using Abp.Events.Bus.Entities;
using Abp.Events.Bus.Handlers;
using Abp.Extensions;
using Abp.Threading;
using NPOI.HSSF.Record;
using TACHYON.BayanIntegration;
using TACHYON.Integration.BayanIntegration;
using TACHYON.Integration.WaslIntegration;
using TACHYON.Notifications;
using TACHYON.Shipping.ShippingRequestTrips;

namespace TACHYON.Shipping.Trips
{
    public class TripUpdatedEventHandler : IEventHandler<EntityUpdatedEventData<ShippingRequestTrip>>, ITransientDependency
    {
        private readonly BayanIntegrationManager _bayanIntegrationManager;
        private readonly WaslIntegrationManager _waslIntegrationManager;

        public TripUpdatedEventHandler(BayanIntegrationManager bayanIntegrationManager, WaslIntegrationManager waslIntegrationManager)
        {
            _bayanIntegrationManager = bayanIntegrationManager;
            _waslIntegrationManager = waslIntegrationManager;
        }

        public void HandleEvent(EntityUpdatedEventData<ShippingRequestTrip> eventData)
        {
            //todo send notification for driver and shipper and carrier using eventBus


            // Add Integration Jobs To the Queue
            if (eventData.Entity.AssignedDriverUserId.HasValue && eventData.Entity.AssignedTruckId.HasValue)
            {
                //Bayan integration
                if (eventData.Entity.BayanId.IsNullOrEmpty())
                {
                    AsyncHelper.RunSync(() => _bayanIntegrationManager.QueueCreateConsignmentNote(eventData.Entity.Id));
                }
                else
                {
                    AsyncHelper.RunSync(() => _bayanIntegrationManager.QueueEditConsignmentNote(eventData.Entity.Id));
                }

                //wasl integration
                if (eventData.Entity.StartWorking.HasValue)
                {
                    if (!eventData.Entity.IsWaslIntegrated)
                    {
                        AsyncHelper.RunSync(() => _waslIntegrationManager.QueueTripRegistrationJob(eventData.Entity.Id));

                    }
                    if (eventData.Entity.ActualDeliveryDate.HasValue)
                    {
                        AsyncHelper.RunSync(() => _waslIntegrationManager.QueueTripUpdateJob(eventData.Entity.Id));
                    }
                }
            }
        }
    }
}