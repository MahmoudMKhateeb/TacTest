using Abp.BackgroundJobs;
using Abp.Dependency;
using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Penalties.Jobs
{
    public class NotficationWhenViolateDetention : BackgroundJob<int[]>, ITransientDependency
    {
        public override void Execute(int[] args)
        {
            throw new NotImplementedException();
        }
    }
}
