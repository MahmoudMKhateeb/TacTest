using Abp.BackgroundJobs;
using Abp.Dependency;
using Abp.Threading;
using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Penalties.Jobs
{
    public class NotficationBeforeViolateDetention : BackgroundJob<(int shipperId,long pointId)>, ITransientDependency
    {
        private readonly PenaltyManager _penaltyManager;

        public NotficationBeforeViolateDetention(PenaltyManager penaltyManager)
        {
            _penaltyManager = penaltyManager;
        }

        public override void Execute((int shipperId, long pointId) args)
        {
            AsyncHelper.RunSync(() => _penaltyManager.SendNotficationBeforeViolateDetention(args.shipperId, args.pointId));
        }
    }
}
