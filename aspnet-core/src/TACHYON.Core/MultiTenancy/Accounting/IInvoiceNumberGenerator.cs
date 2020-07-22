using Abp.Dependency;
using System.Threading.Tasks;

namespace TACHYON.MultiTenancy.Accounting
{
    public interface IInvoiceNumberGenerator : ITransientDependency
    {
        Task<string> GetNewInvoiceNumber();
    }
}