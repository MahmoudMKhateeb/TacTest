// unset

using Abp.BackgroundJobs;
using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.Threading;
using Hangfire;
using TACHYON.Integration.BayanIntegration.V2;

namespace TACHYON.Integration.BayanIntegration.Jobs
{
    public class EditConsignmentNoteJob : BackgroundJob<int>, ITransientDependency
    {
        private readonly BayanIntegrationManagerV2 _bayanIntegrationManager;

        public EditConsignmentNoteJob(BayanIntegrationManagerV2 bayanIntegrationManager)
        {
            _bayanIntegrationManager = bayanIntegrationManager;
        }

        [UnitOfWork]
        [AutomaticRetry(Attempts = 2)]
        public override void Execute(int tripId)
        {
            AsyncHelper.RunSync(() => _bayanIntegrationManager.EditConsignmentNote(tripId));

        }
    }
}