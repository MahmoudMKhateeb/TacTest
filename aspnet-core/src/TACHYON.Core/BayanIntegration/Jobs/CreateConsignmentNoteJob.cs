// unset

using Abp.BackgroundJobs;
using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.Threading;

namespace TACHYON.BayanIntegration.Jobs
{
    public class CreateConsignmentNoteJob : BackgroundJob<int>, ITransientDependency
    {
        private readonly BayanIntegrationManager _bayanIntegrationManager;

        public CreateConsignmentNoteJob(BayanIntegrationManager bayanIntegrationManager)
        {
            _bayanIntegrationManager = bayanIntegrationManager;
        }

        [UnitOfWork]
        public override void Execute(int tripId)
        {
            AsyncHelper.RunSync(() => _bayanIntegrationManager.CreateConsignmentNote(tripId));
        }
    }
}