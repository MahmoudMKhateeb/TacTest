using Abp.Configuration;
using Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using TACHYON.Configuration;

namespace TACHYON.DynamicInvoices
{
    public class DynamicInvoiceManager : TACHYONDomainServiceBase ,IDynamicInvoiceManager
    {
        private readonly IRepository<DynamicInvoice, long> _dynamicInvoiceRepository;
        public DynamicInvoiceManager(IRepository<DynamicInvoice, long> dynamicInvoiceRepository)
        {
            _dynamicInvoiceRepository = dynamicInvoiceRepository;
            // inject your dependencies here
        }
        // Add your business logic here 

        public async Task CalculatePrice(long dynamicInvoiceId)
        {
            var dynamicInvoice = await _dynamicInvoiceRepository.GetAllIncluding(x => x.Items)
                .FirstOrDefaultAsync(x => x.Id == dynamicInvoiceId);
            var taxVat = await SettingManager.GetSettingValueAsync<decimal>(AppSettings.HostManagement.TaxVat);
            dynamicInvoice.SubTotalAmount = dynamicInvoice.Items.Sum(x => x.Price);
            dynamicInvoice.VatAmount = dynamicInvoice.SubTotalAmount * (taxVat/100); // 15/100 => 15% || 0.15
            dynamicInvoice.TotalAmount = dynamicInvoice.SubTotalAmount + dynamicInvoice.VatAmount;
        }
    }
}