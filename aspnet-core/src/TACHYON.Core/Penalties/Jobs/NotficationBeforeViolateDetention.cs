using Abp.BackgroundJobs;
using Abp.Dependency;
using Abp.Threading;
using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Penalties.Jobs
{
    public class NotficationBeforeViolateDetention : BackgroundJob<int[]>, ITransientDependency
    {
        private readonly PenaltyManager _penaltyManager;

        public NotficationBeforeViolateDetention(PenaltyManager penaltyManager)
        {
            _penaltyManager = penaltyManager;
        }

        public override void Execute(int[] args)
        {
            AsyncHelper.RunSync(() => _penaltyManager.SendNotficationBeforeViolateDetention(args[0], args[1]));
        }
    }
}
