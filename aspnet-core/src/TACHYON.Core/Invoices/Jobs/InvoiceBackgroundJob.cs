using Abp.BackgroundJobs;
using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.Threading;
using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Invoices.Jobs
{
    public class InvoiceBackgroundJob : BackgroundJob<int>, ITransientDependency
    {
        private InvoiceManager _InvoiceManager { get; set; }

        public InvoiceBackgroundJob(InvoiceManager InvoiceManager)
        {
            _InvoiceManager = InvoiceManager;
        }

        [UnitOfWork]
        public override void Execute(int args)
        {
            AsyncHelper.RunSync(() => _InvoiceManager.GenerateInvoice(args));
        }
    }
}