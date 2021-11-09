using Abp.Dependency;
using Abp.EntityHistory;
using Abp.Events.Bus.Entities;
using Abp.Events.Bus.Handlers;
using Abp.Threading;
using System.Linq;
using TACHYON.EntityLogs;
using TACHYON.Shipping.ShippingRequestTrips;
using EntityHistoryHelper = TACHYON.EntityHistory.EntityHistoryHelper;

namespace TACHYON.Routs.RoutPoints
{
    public class EntityChangedCreatedEventHandler : IEventHandler<EntityCreatedEventData<EntityChange>>, ITransientDependency
    {
        private readonly EntityLogManager _logManager;

        public EntityChangedCreatedEventHandler(EntityLogManager logManager)
        {
            _logManager = logManager;
        }

        public void HandleEvent(EntityCreatedEventData<EntityChange> eventData)
        {
            // I Think it's no need for this check because if the entity change reference to untracked
            // entity then why entity change created !?
            // if (EntityHistoryHelper.TrackedTypes.Any(x => x.FullName.Equals(eventData.Entity.EntityTypeFullName)))
            AsyncHelper.RunSync(() => _logManager.CreateEntityLog(eventData.Entity));
        }
    }
}