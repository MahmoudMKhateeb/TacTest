using Abp.BackgroundJobs;
using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TACHYON.Integration.WaslIntegration.Jobs
{
    public class TripUpdateJob : BackgroundJob<int>, ITransientDependency
    {
        private readonly WaslIntegrationManager _waslIntegrationManager;

        public TripUpdateJob(WaslIntegrationManager waslIntegrationManager)
        {
            _waslIntegrationManager = waslIntegrationManager;
        }

        [UnitOfWork]
        public override void Execute(int TripId)
        {
            AsyncHelper.RunSync(() => _waslIntegrationManager.TripUpdate(TripId));
        }
    }

}