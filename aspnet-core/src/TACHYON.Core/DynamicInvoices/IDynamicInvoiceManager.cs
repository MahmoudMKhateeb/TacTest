using Abp.Domain.Services;
using System.Threading.Tasks;

namespace TACHYON.DynamicInvoices
{
    public interface IDynamicInvoiceManager : IDomainService
    {
        Task CalculatePrice(DynamicInvoice dynamicInvoice);
    }
}