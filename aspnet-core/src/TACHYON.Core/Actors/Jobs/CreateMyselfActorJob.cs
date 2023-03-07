using Abp.Authorization.Users;
using Abp.BackgroundJobs;
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
            using var uow = _unitOfWorkManager.Begin();
            

            var createdMyselfActors = (from tenantId in args.TenantIds
                join tenant in _tenantRepository.GetAll() on tenantId equals tenant.Id
                from adminUser in _userRepository.GetAll().IgnoreQueryFilters().Where(x=> !x.IsDeleted)
                where adminUser.TenantId == tenantId && adminUser.UserName.Equals(AbpUserBase.AdminUserName)
                select new Actor()
                {
                    TenantId = tenantId,
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