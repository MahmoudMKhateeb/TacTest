using Abp.BackgroundJobs;
using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.Threading;
using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Documents
{
    public class ExpiredDocumentFileBackgroundJob : BackgroundJob<int>, ITransientDependency
    {
        private readonly DocumentFilesManager _documentFilesManager;

        public ExpiredDocumentFileBackgroundJob(DocumentFilesManager documentFilesManager)
        {
            _documentFilesManager = documentFilesManager;
        }

        [UnitOfWork]
        public override void Execute(int args)
        {
            AsyncHelper.RunSync(() => _documentFilesManager.NotifyExpiredDocumentFile());
        }
    }
}