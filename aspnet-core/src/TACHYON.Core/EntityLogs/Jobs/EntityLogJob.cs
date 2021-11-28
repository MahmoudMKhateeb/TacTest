using Abp.BackgroundJobs;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.EntityHistory;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace TACHYON.EntityLogs.Jobs
{
    public class EntityLogJob : AsyncBackgroundJob<EntityLogJobArguments>
    {
        private readonly EntityLogManager _logManager;
        private readonly IRepository<EntityChange, long> _repository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public EntityLogJob(EntityLogManager logManager, IRepository<EntityChange, long> repository, IUnitOfWorkManager unitOfWorkManager)
        {
            _logManager = logManager;
            _repository = repository;
            _unitOfWorkManager = unitOfWorkManager;
        }

        [UnitOfWork]
        protected override async Task ExecuteAsync(EntityLogJobArguments args)
        {
            using (var uow = _unitOfWorkManager.Begin())
            {

                var entity = await _repository.GetAll().IgnoreQueryFilters()
                        .AsNoTracking()
                        .Include(x => x.PropertyChanges)
                        .SingleOrDefaultAsync(x => x.Id == args.EntityChangeId);

                if (entity != null)
                    await _logManager.CreateEntityLog(entity);

                await uow.CompleteAsync();
            }

        }
    }
}