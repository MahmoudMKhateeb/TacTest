using Abp.BackgroundJobs;
using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.Threading;
using Hangfire;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TACHYON.Authorization.Users;
using TACHYON.Integration.WaslIntegration.Modules;

namespace TACHYON.Integration.WaslIntegration.Jobs
{
    public class DriverRegistrationJob : BackgroundJob<long>, ITransientDependency
    {
        private readonly WaslIntegrationManager _waslIntegrationManager;

        public DriverRegistrationJob(WaslIntegrationManager waslIntegrationManager)
        {
            _waslIntegrationManager = waslIntegrationManager;
        }

        [UnitOfWork]
        [AutomaticRetry(Attempts = 2)]
        public override void Execute(long driverId)
        {
            AsyncHelper.RunSync(() => _waslIntegrationManager.DriverRegistration(driverId));
        }
    }
}