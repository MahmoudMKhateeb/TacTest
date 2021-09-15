// unset

using Abp.Dependency;
using Abp.Events.Bus.Entities;
using Abp.Events.Bus.Handlers;
using Abp.Extensions;
using Abp.Threading;
using NPOI.HSSF.Record;
using TACHYON.BayanIntegration;
using TACHYON.Notifications;
using TACHYON.Shipping.ShippingRequestTrips;

namespace TACHYON.Shipping.Trips
{
    public class TripUpdatedEventHandler : IEventHandler<EntityUpdatedEventData<ShippingRequestTrip>>, ITransientDependency
    {
        private readonly BayanIntegrationManager _bayanIntegrationManager;

        public TripUpdatedEventHandler(BayanIntegrationManager bayanIntegrationManager)
        {

            _bayanIntegrationManager = bayanIntegrationManager;
        }

        public void HandleEvent(EntityUpdatedEventData<ShippingRequestTrip> eventData)
        {
            //todo send notification for driver and shipper and carrier using eventBus


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
    }
}