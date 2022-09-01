using Abp.Configuration;
using System.Linq;
using System.Threading.Tasks;
using TACHYON.Configuration;

namespace TACHYON.DynamicInvoices
{
    public class DynamicInvoiceManager : TACHYONDomainServiceBase ,IDynamicInvoiceManager
    {
        public DynamicInvoiceManager()
        {
            // inject your dependencies here
        }
        // Add your business logic here 

        public async Task CalculatePrice(DynamicInvoice dynamicInvoice)
        {
            var taxVat = await SettingManager.GetSettingValueAsync<decimal>(AppSettings.HostManagement.TaxVat);
            dynamicInvoice.SubTotalAmount = dynamicInvoice.Items.Sum(x => x.Price);
            dynamicInvoice.VatAmount = dynamicInvoice.SubTotalAmount * (taxVat/100); // 15/100 => 15% || 0.15
            dynamicInvoice.TotalAmount = dynamicInvoice.SubTotalAmount + dynamicInvoice.VatAmount;
        }
    }
}