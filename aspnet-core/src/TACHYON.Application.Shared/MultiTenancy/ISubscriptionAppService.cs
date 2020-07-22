using Abp.Application.Services;
using System.Threading.Tasks;

namespace TACHYON.MultiTenancy
{
    public interface ISubscriptionAppService : IApplicationService
    {
        Task DisableRecurringPayments();

        Task EnableRecurringPayments();
    }
}