using Abp.BackgroundJobs;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Threading;
using Quartz;
using System;
using System.Threading.Tasks;
using TACHYON.Invoices;
using TACHYON.Invoices.Jobs;
using TACHYON.Invoices.Periods;

namespace TACHYON.Core.Invoices.Jobs
{
    public class InvoiceJob : IJob, ITransientDependency
    {
        public InvoicePeriodType PeriodType { private get; set; }
        public int PeriodId { private get; set; }
        public readonly IBackgroundJobManager _backgroundJobManager;

        public InvoiceJob(IBackgroundJobManager backgroundJobManager)
        {
            _backgroundJobManager = backgroundJobManager;
        }

        public  Task Execute(IJobExecutionContext context)
        {
            JobKey key = context.JobDetail.Key;
            //JobDataMap dataMap = context.JobDetail.JobDataMap;
            JobDataMap dataMap = context.Trigger.JobDataMap;
            int PeriodId = dataMap.GetInt("PeriodId");
            // AsyncHelper.RunSync(() => _InvoiceManager.GenerateInvoice(PeriodId));
            _backgroundJobManager.Enqueue<InvoiceBackgroundJob, int>(PeriodId);

           // Console.Error.WriteLineAsync("abdulllah");
            return Task.CompletedTask;
        }




    }
}
