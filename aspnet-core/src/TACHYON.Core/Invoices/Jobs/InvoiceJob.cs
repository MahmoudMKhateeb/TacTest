using Abp.Dependency;
using Abp.Domain.Repositories;
using Quartz;
using System;
using System.Threading.Tasks;
using TACHYON.Invoices;
using TACHYON.Invoices.Periods;

namespace TACHYON.Core.Invoices.Jobs
{
    public class InvoiceJob : IJob, ITransientDependency
    {
        public InvoicePeriodType PeriodType { private get; set; }
        public int PeriodId { private get; set; }
        public InvoiceManager _InvoiceManager { get; set; }



        public InvoiceJob(InvoiceManager InvoiceManager)
        {
            _InvoiceManager = InvoiceManager;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            JobKey key = context.JobDetail.Key;
            JobDataMap dataMap = context.JobDetail.JobDataMap;
            _InvoiceManager.GenerateInvoice(dataMap.GetInt("PeriodId"));
            await Console.Error.WriteLineAsync("");
        }




    }
}
