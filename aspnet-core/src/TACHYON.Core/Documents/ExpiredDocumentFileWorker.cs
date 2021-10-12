using Abp;
using Abp.BackgroundJobs;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Net.Mail;
using Abp.Threading;
using Abp.Threading.BackgroundWorkers;
using Abp.Threading.Timers;
using Abp.Timing;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TACHYON.Authorization.Users;
using TACHYON.Documents.DocumentFiles;
using TACHYON.Notifications;

namespace TACHYON.Documents
{
    public class ExpiredDocumentFileWorker : PeriodicBackgroundWorkerBase, ISingletonDependency
    {
        private const int CheckPeriodAsMilliseconds = 1 * 60 * 60 * 1000 * 24; //1 day

        private readonly IRepository<DocumentFile, Guid> _documentFileRepository;
        private readonly IAppNotifier _appNotifier;
        private readonly IUserEmailer _userEmailer;
        private readonly IBackgroundJobManager _backgroundJobManager;

        public ExpiredDocumentFileWorker(AbpTimer timer, IRepository<DocumentFile, Guid> documentFileRepository, IAppNotifier appNotifier, IUserEmailer userEmailer, IBackgroundJobManager backgroundJobManager) : base(timer)
        {
            _documentFileRepository = documentFileRepository;
            _appNotifier = appNotifier;
            Timer.Period = CheckPeriodAsMilliseconds;
            Timer.RunOnStart = true;
            _userEmailer = userEmailer;
            _backgroundJobManager = backgroundJobManager;
        }

        [UnitOfWork]
        protected override void DoWork()
        {
            AsyncHelper.RunSync(() => _backgroundJobManager.EnqueueAsync<ExpiredDocumentFileBackgroundJob, int>(0));
        }
    }
}