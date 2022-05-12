using Abp.BackgroundJobs;
using Abp.EntityHistory;
using System.Threading.Tasks;


namespace TACHYON.EntityLogs.Jobs
{
    public class EntityLogHelperManager : TACHYONDomainServiceBase
    {
        private readonly IBackgroundJobManager _jobManager;

        public EntityLogHelperManager(IBackgroundJobManager jobManager)
        {
            _jobManager = jobManager;
        }

        public async Task EnqueueEntityLog(EntityChange entityChange)
        {
            await _jobManager.EnqueueAsync<EntityLogJob, EntityLogJobArguments>(
                new EntityLogJobArguments(entityChange.Id));
        }
    }
}