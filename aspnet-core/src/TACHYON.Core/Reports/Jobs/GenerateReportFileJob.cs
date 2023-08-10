using Abp.BackgroundJobs;
using Abp.Dependency;
using Abp.Domain.Uow;
using System;
using System.Threading.Tasks;

namespace TACHYON.Reports.Jobs
{
    public class GenerateReportFileJob : AsyncBackgroundJob<Guid>, ITransientDependency
    {
        private readonly IReportManager _reportManager;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public GenerateReportFileJob(IReportManager reportManager, IUnitOfWorkManager unitOfWorkManager)
        {
            _reportManager = reportManager;
            _unitOfWorkManager = unitOfWorkManager;
        }

        protected override async Task ExecuteAsync(Guid reportId)
        {
            using var uow = _unitOfWorkManager.Begin();
            await _reportManager.GenerateReportFile(reportId);
            await uow.CompleteAsync();
        }
    }
}