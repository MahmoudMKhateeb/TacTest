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

namespace TACHYON.Integration.BayanIntegration.V3.Jobs
{
    public class UpdateVehicleOrDriverJobArgs
    {


        public int TripId { get; set; }
        public long DriverId { get; set; }
        public long TruckId { get; set; }
    }

    public class UpdateVehicleOrDriverJob : BackgroundJob<UpdateVehicleOrDriverJobArgs>, ITransientDependency
    {
        private readonly BayanIntegrationManagerV3 _bayanIntegrationManager3;

        public UpdateVehicleOrDriverJob(BayanIntegrationManagerV3 bayanIntegrationManager3)
        {
            _bayanIntegrationManager3 = bayanIntegrationManager3;
        }

        [UnitOfWork]
        [AutomaticRetry(Attempts = 2)]
        public override void Execute(UpdateVehicleOrDriverJobArgs args)
        {

            AsyncHelper.RunSync(() => _bayanIntegrationManager3.UpdateVehicleOrDriver(args));

        }
    }
}




