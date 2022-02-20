using Abp.BackgroundJobs;
using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TACHYON.Authorization.Users;

namespace TACHYON.Documents
{
    public class ExpiredDocumentsReportBackgroundJob : AsyncBackgroundJob<int?>, ITransientDependency
    {
        private readonly IUserEmailer _userEmailer;
        private readonly DocumentFilesManager _documentFilesManager;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public ExpiredDocumentsReportBackgroundJob(IUserEmailer userEmailer, DocumentFilesManager documentFilesManager, IUnitOfWorkManager unitOfWorkManager)
        {
            _userEmailer = userEmailer;
            _documentFilesManager = documentFilesManager;
            _unitOfWorkManager = unitOfWorkManager;
        }

        [UnitOfWork]
        protected override async Task ExecuteAsync(int? args)
        {
            using var uow = _unitOfWorkManager.Begin();
            await _documentFilesManager.SendDocumentsExpiredStatusMonthlyReport();
            await uow.CompleteAsync();
        }
    }
}
