﻿using Abp.BackgroundJobs;
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
    public class CreateTripJop : BackgroundJob<int>, ITransientDependency
    {
        private readonly BayanIntegrationManagerV3 _bayanIntegrationManager3;

        public CreateTripJop(BayanIntegrationManagerV3 bayanIntegrationManager3)
        {
            _bayanIntegrationManager3 = bayanIntegrationManager3;
        }

        [UnitOfWork]
        [AutomaticRetry(Attempts = 2)]
        public override void Execute(int tripId)
        {

            AsyncHelper.RunSync(() => _bayanIntegrationManager3.CreateTrip(tripId));

        }
    }
}



