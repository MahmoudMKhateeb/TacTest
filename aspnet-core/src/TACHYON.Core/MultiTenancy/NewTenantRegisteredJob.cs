using Abp.BackgroundJobs;
using Abp.Dependency;
using Abp.Threading;
using System.Threading.Tasks;
using TACHYON.Notifications;

namespace TACHYON.MultiTenancy
{
    public class NewTenantRegisteredJob : AsyncBackgroundJob<string>, ITransientDependency
    {
        private readonly AppNotifier _notifier;

        public NewTenantRegisteredJob(AppNotifier notifier)
        {
            _notifier = notifier;
        }

        protected override async Task ExecuteAsync(string tenancyName)
            => await _notifier.NewTenantRegisteredAsync(tenancyName);
    }
}