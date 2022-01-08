using Abp.BackgroundJobs;
using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TACHYON.Integration.WaslIntegration.Modules;

namespace TACHYON.Integration.WaslIntegration.Jobs
{
    public class VehicleDeleteJob : BackgroundJob<WaslVehicleRoot>, ITransientDependency
    {
        private readonly WaslIntegrationManager _waslIntegrationManager;

        public VehicleDeleteJob(WaslIntegrationManager waslIntegrationManager)
        {
            _waslIntegrationManager = waslIntegrationManager;
        }

        [UnitOfWork]
        public override void Execute(WaslVehicleRoot args)
        {
            AsyncHelper.RunSync(() => _waslIntegrationManager.VehicleDelete(args));
        }
    }
}