using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Net.Mail;
using Abp.Threading;
using Abp.Threading.BackgroundWorkers;
using Abp.Threading.Timers;
using Abp.Timing;
using Microsoft.EntityFrameworkCore;
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

        public ExpiredDocumentFileWorker(AbpTimer timer, IRepository<DocumentFile, Guid> documentFileRepository, IAppNotifier appNotifier, IUserEmailer userEmailer) : base(timer)
        {
            _documentFileRepository = documentFileRepository;
            _appNotifier = appNotifier;
            Timer.Period = CheckPeriodAsMilliseconds;
            _userEmailer = userEmailer;
            timer.RunOnStart = true;
        }

        [UnitOfWork]
        protected override void DoWork()
        {
            using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MayHaveTenant))
            {

                var docs = _documentFileRepository.GetAll()
                    .Include(x => x.DocumentTypeFk)
                    .Include(x=>x.TenantFk)
                    .Where(x => x.DocumentTypeFk.HasExpirationDate)
                    .Where(x => x.IsAccepted)
                    .ToList();




                foreach (DocumentFile documentFile in docs)
                {
                    if (documentFile.ExpirationDate == null)
                    {
                        continue;
                    }

                    //AlertDays 
                    var expirationAlertDays = documentFile.DocumentTypeFk.ExpirationAlertDays;
                    if (expirationAlertDays != null)
                    {
                        var alertDate = documentFile.ExpirationDate.Value.AddDays(-1 * expirationAlertDays.Value).Date;
                        if (alertDate == Clock.Now.Date)
                        {

                            var user = new UserIdentifier(documentFile.TenantId, documentFile.CreatorUserId.Value);
                            AsyncHelper.RunSync(() => _appNotifier.DocumentFileBeforExpiration(user, documentFile.Id, expirationAlertDays.Value));

                        }
                    }

                    //Expiration
                    if (documentFile.ExpirationDate.Value.Date == Clock.Now.Date)
                    {
                        var user = new UserIdentifier(documentFile.TenantId, documentFile.CreatorUserId.Value);
                        AsyncHelper.RunSync(() => _appNotifier.DocumentFileExpiration(user, documentFile.Id));
                        Logger.Info(documentFile + "ExpiredDocumentFileWorker logger.");

                        //Send email with expired documents
                        AsyncHelper.RunSync(() => _userEmailer.SendExpiredDateDocumentsAsyn(documentFile.TenantFk, documentFile.Name));
                    }

                }


                CurrentUnitOfWork.SaveChanges();
            }
        }
    }
}