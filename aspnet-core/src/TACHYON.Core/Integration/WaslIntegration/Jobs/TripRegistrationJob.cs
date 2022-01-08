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
    public class TripRegistrationJob : BackgroundJob<int>, ITransientDependency
    {
        private readonly WaslIntegrationManager _waslIntegrationManager;

        public TripRegistrationJob(WaslIntegrationManager waslIntegrationManager)
        {
            _waslIntegrationManager = waslIntegrationManager;
        }

        [UnitOfWork]
        public override void Execute(int args)
        {
            AsyncHelper.RunSync(() => _waslIntegrationManager.TripRegistration(args));
        }
    }
}