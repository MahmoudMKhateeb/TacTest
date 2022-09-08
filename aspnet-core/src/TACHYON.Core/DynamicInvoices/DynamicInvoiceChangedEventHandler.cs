using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.Events.Bus.Entities;
using Abp.Events.Bus.Handlers;
using System.Threading.Tasks;
using TACHYON.DynamicInvoices.DynamicInvoiceItems;

namespace TACHYON.DynamicInvoices
{
    public class DynamicInvoiceChangedEventHandler : IAsyncEventHandler<EntityChangedEventData<DynamicInvoice>>,
        IAsyncEventHandler<EntityChangedEventData<DynamicInvoiceItem>>, ITransientDependency
    {
        private readonly IDynamicInvoiceManager _dynamicInvoiceManager;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public DynamicInvoiceChangedEventHandler(
            IDynamicInvoiceManager dynamicInvoiceManager,
            IUnitOfWorkManager unitOfWorkManager)
        {
            _dynamicInvoiceManager = dynamicInvoiceManager;
            _unitOfWorkManager = unitOfWorkManager;
        }

        
        public async Task HandleEventAsync(EntityChangedEventData<DynamicInvoice> eventData)
        {
            if (eventData.Entity.IsDeleted) return;
            await TriggerPriceCalculator(eventData.Entity.Id);
        }

        public async Task HandleEventAsync(EntityChangedEventData<DynamicInvoiceItem> eventData)
        {
            await TriggerPriceCalculator(eventData.Entity.DynamicInvoiceId);
        }

        #region Helper

        private async Task TriggerPriceCalculator(long dynamicInvoiceId)
        {
            

            using (var uow = _unitOfWorkManager.Begin())
            {
                await _dynamicInvoiceManager.CalculatePrice(dynamicInvoiceId);
                await uow.CompleteAsync();
            }
        }

        #endregion
        
    }
}