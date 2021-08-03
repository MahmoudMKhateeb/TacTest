using Abp.BackgroundJobs;
using Abp.Dependency;
using Abp.Quartz;
using Abp.Threading;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TACHYON.Authorization.Users;

namespace TACHYON.Documents
{
    public class ExpiredDocumentsReportJob : IJob, ITransientDependency
    {
        private readonly IUserEmailer _userEmailer;
        private readonly DocumentFilesManager _documentFilesManager;
        public readonly IBackgroundJobManager _backgroundJobManager;


        public ExpiredDocumentsReportJob(IUserEmailer userEmailer, DocumentFilesManager documentFilesManager, IBackgroundJobManager backgroundJobManager)
        {
            _userEmailer = userEmailer;
            _documentFilesManager = documentFilesManager;
            _backgroundJobManager = backgroundJobManager;
        }

        public Task Execute(IJobExecutionContext context)
        {
            // var documents = AsyncHelper.RunSync(() => _documentFilesManager.GetAllTenantDriverAndTruckDocumentFilesListAsync());
            // var documents = await _documentFilesManager.GetAllTenantDriverAndTruckDocumentFilesListAsync();
            _backgroundJobManager.EnqueueAsync<ExpiredDocumentsReportBackgroundJob, int>(5);
            //AsyncHelper.RunSync(() =>_userEmailer.SendDocumentsExpiredInfoAsyn(documents, documents.FirstOrDefault().TenantId.Value));
            //await _userEmailer.SendDocumentsExpiredInfoAsyn(await _documentFilesManager.GetAllTenantDriverAndTruckDocumentFilesListAsync(),4);
            //await Console.Error.WriteLineAsync("tasneemJob");
            return Task.CompletedTask;
        }
    }
}
