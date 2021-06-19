using Abp.BackgroundJobs;
using Abp.Dependency;
using Abp.Domain.Uow;
using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Invoices.Jobs
{
    public class InvoiceBackgroundJob : BackgroundJob<int>, ITransientDependency
    {
        private  InvoiceManager _InvoiceManager { get; set; }

        public InvoiceBackgroundJob(InvoiceManager InvoiceManager)
        {
            _InvoiceManager = InvoiceManager;
        }

        [UnitOfWork]
        public async override void Execute(int args)
        {
           await _InvoiceManager.GenerateInvoice(args);
        }
    }
}
