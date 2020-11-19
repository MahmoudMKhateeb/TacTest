using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
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


        public ExpiredDocumentFileWorker(AbpTimer timer, IRepository<DocumentFile, Guid> documentFileRepository, IAppNotifier appNotifier) : base(timer)
        {
            _documentFileRepository = documentFileRepository;
            _appNotifier = appNotifier;
            Timer.Period = CheckPeriodAsMilliseconds;
        }

        [UnitOfWork]
        protected override void DoWork()
        {
            using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MayHaveTenant))
            {

                var docs = _documentFileRepository.GetAll()
                    .Include(x => x.DocumentTypeFk)
                    .Where(x => x.DocumentTypeFk.HasExpirationDate)
                    .Where(x => x.IsAccepted)
                    .ToList();




                foreach (DocumentFile documentFile in docs)
                {
                    //ExpirationAlertDays 
                    var expirationAlertDays = documentFile.DocumentTypeFk.ExpirationAlertDays;
                    if (expirationAlertDays != null && documentFile.ExpirationDate != null)
                    {
                        var alertDate = documentFile.ExpirationDate.Value.Date.Subtract(TimeSpan.FromDays(Convert.ToDouble(expirationAlertDays))).Date;
                        if (alertDate == Clock.Now.Date)
                        {

                            var user = new UserIdentifier(documentFile.TenantId, documentFile.CreatorUserId.Value);
                            _appNotifier.DocumentFileBeforExpiration(user, documentFile.Id, expirationAlertDays.Value);

                        }
                    }

                    if (documentFile.ExpirationDate != null && documentFile.ExpirationDate.Value.Date == DateTime.Now.Date)
                    {
                        var user = new UserIdentifier(documentFile.TenantId, documentFile.CreatorUserId.Value);
                        _appNotifier.DocumentFileExpiration(user, documentFile.Id);
                    }

                }


                CurrentUnitOfWork.SaveChanges();
            }
        }
    }
}