using Abp.Authorization.Users;
using Abp.BackgroundJobs;
using Abp.Collections.Extensions;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using TACHYON.Authorization.Users;
using TACHYON.MultiTenancy;

namespace TACHYON.Actors.Jobs
{
    public class CreateMyselfActorJob : IBackgroundJob<CreateMyselfActorJobArgs>, ITransientDependency
    {
        private readonly IRepository<Actor> _actorRepository;
        private readonly IRepository<Tenant> _tenantRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IRepository<User,long> _userRepository;

        public CreateMyselfActorJob(
            IRepository<Actor> actorRepository,
            IUnitOfWorkManager unitOfWorkManager,
            IRepository<Tenant> tenantRepository,
            IRepository<User, long> userRepository)
        {
            _actorRepository = actorRepository;
            _unitOfWorkManager = unitOfWorkManager;
            _tenantRepository = tenantRepository;
            _userRepository = userRepository;
        }

        public void Execute(CreateMyselfActorJobArgs args)
        {
            if (args.TenantIds.IsNullOrEmpty()) return;
            using var uow = _unitOfWorkManager.Begin();

            var createdMyselfActors = (from tenant in _tenantRepository.GetAll() 
                where args.TenantIds.Contains(tenant.Id)
                join adminUser in _userRepository.GetAll().IgnoreQueryFilters().Where(x => !x.IsDeleted)
                    on tenant.Id equals adminUser.TenantId
                where adminUser.UserName.Equals(AbpUserBase.AdminUserName)
                 && _actorRepository.GetAll().Where(x => x.TenantId == tenant.Id)
                          .All(x => x.ActorType != ActorTypesEnum.MySelf)
                select new Actor()
                {
                    TenantId = tenant.Id,
                    ActorType = ActorTypesEnum.MySelf,
                    CompanyName = TACHYONConsts.MyselfCompanyName,
                    MoiNumber = tenant.MoiNumber,
                    Email = adminUser.EmailAddress ?? string.Empty,
                    Address = tenant.Address,
                    MobileNumber = adminUser.PhoneNumber ?? string.Empty,
                    IsActive = true,
                }).ToList();

            foreach (Actor myselfActor in createdMyselfActors)
            {
                _actorRepository.Insert(myselfActor);
            }
            
            uow.Complete();
        }
    }
}