using Abp.Dependency;
using Abp.Events.Bus.Handlers;
using System.Threading.Tasks;
using TACHYON.Shipping.ShippingRequestUpdate;

namespace TACHYON.Shipping.ShippingRequestUpdates.EventBus
{
    public class UpdatedShippingRequestEventHandler : IAsyncEventHandler<UpdatedShippingRequestEventData>, ITransientDependency
    {
        private readonly ShippingRequestUpdateManager _manager;

        public UpdatedShippingRequestEventHandler(ShippingRequestUpdateManager manager)
        {
            _manager = manager;
        }

        public async Task HandleEventAsync(UpdatedShippingRequestEventData eventData)
        {
            await _manager.Create(new CreateSrUpdateInputDto()
            {
                EntityLogId = eventData.EntityLogId, ShippingRequestId = eventData.ShippingRequestId
            });
        }
    }
}