using Abp.Authorization.Users;
using Abp.Collections.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using TACHYON.Actors;
using TACHYON.EntityFrameworkCore;
using TACHYON.Features;

namespace TACHYON.Migrations.Seed.Tenants
{
    public class DefaultActorBuilder
    {
        private readonly TACHYONDbContext _dbContext;

        public DefaultActorBuilder(TACHYONDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void BuildTenantsDefaultActor()
        {
            

            string trueVal = true.ToString().ToLower();
            string falseVal = false.ToString().ToLower();

            var editionIds = _dbContext.EditionFeatureSettings.Where(i =>
                    i.Name.Contains(AppFeatures.CMS) && i.Value.ToLower().Equals(trueVal))
                .Select(x => x.EditionId).ToList();

            int[] excludedTenantIds = (from tenant in _dbContext.Tenants
                where _dbContext.TenantFeatureSettings.Any(i =>
                    i.TenantId == tenant.Id && i.Name.Contains(AppFeatures.CMS) && i.Value.ToLower().Equals(falseVal))
                select tenant.Id).ToArray();

            int[] tenantIds = (from tenant in _dbContext.Tenants
                where _dbContext.TenantFeatureSettings.Any(i => i.TenantId == tenant.Id 
                && i.Name.Contains(AppFeatures.CMS) && i.Value.ToLower().Equals(trueVal)) ||
                (editionIds.Contains(tenant.EditionId.Value) && !excludedTenantIds.Contains(tenant.Id)) select tenant.Id).ToArray();
            
            
            if (tenantIds.IsNullOrEmpty()) return;
            
            var createdMyselfActors = (
                from tenant in _dbContext.Tenants
                where tenantIds.Contains(tenant.Id) 
                      && _dbContext.Actors.Where(x=> x.TenantId == tenant.Id).All(x=> x.ActorType != ActorTypesEnum.MySelf)
                from adminUser in _dbContext.Users.IgnoreQueryFilters().Where(x=> !x.IsDeleted)
                    where adminUser.TenantId == tenant.Id && AbpUserBase.AdminUserName.Contains(adminUser.UserName)
                select new Actor()
                {
                    TenantId = tenant.Id,
                    ActorType = ActorTypesEnum.MySelf,
                    CompanyName = TACHYONConsts.MyselfCompanyName,
                    MoiNumber = tenant.MoiNumber ?? string.Empty,
                    Email = adminUser.EmailAddress,
                    Address = tenant.Address ?? string.Empty,
                    MobileNumber = adminUser.PhoneNumber ?? string.Empty,
                    IsActive = true,
                }).ToList();

            if (createdMyselfActors.IsNullOrEmpty()) return;
            
            _dbContext.Actors.AddRange(createdMyselfActors);
            _dbContext.SaveChanges();


        }
    }
}