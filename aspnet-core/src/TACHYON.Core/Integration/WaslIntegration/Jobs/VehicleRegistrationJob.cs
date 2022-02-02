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
using TACHYON.Integration.WaslIntegration.Modules;
using TACHYON.Trucks;

namespace TACHYON.Integration.WaslIntegration.Jobs
{
    public class VehicleRegistrationJob : BackgroundJob<long>, ITransientDependency
    {
        private readonly WaslIntegrationManager _waslIntegrationManager;

        public VehicleRegistrationJob(WaslIntegrationManager waslIntegrationManager)
        {
            _waslIntegrationManager = waslIntegrationManager;
        }

        [UnitOfWork]
        [AutomaticRetry(Attempts = 2)]
        public override void Execute(long truckId)
        {
            AsyncHelper.RunSync(() => _waslIntegrationManager.VehicleRegistration(truckId));
        }
    }
}