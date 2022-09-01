using Abp.Dependency;
using Abp.Events.Bus.Entities;
using Abp.Events.Bus.Handlers;
using System.Threading.Tasks;

namespace TACHYON.DynamicInvoices
{
    public class DynamicInvoiceChangedEventHandler : IAsyncEventHandler<EntityChangedEventData<DynamicInvoice>>, ITransientDependency
    {
        private readonly IDynamicInvoiceManager _dynamicInvoiceManager;

        public DynamicInvoiceChangedEventHandler(IDynamicInvoiceManager dynamicInvoiceManager)
        {
            _dynamicInvoiceManager = dynamicInvoiceManager;
        }

        public async Task HandleEventAsync(EntityChangedEventData<DynamicInvoice> eventData)
        {
            if (eventData.Entity.IsDeleted) return;

            await _dynamicInvoiceManager.CalculatePrice(eventData.Entity);
        }
    }
}