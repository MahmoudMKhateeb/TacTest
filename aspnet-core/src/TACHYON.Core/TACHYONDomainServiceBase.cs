using Abp.Domain.Services;
using Abp.Domain.Uow;
using Abp.Runtime.Session;
using Abp.Threading;
using System;
using System.Threading.Tasks;
using TACHYON.Authorization.Users;
using TACHYON.MultiTenancy;

namespace TACHYON
{
    public abstract class TACHYONDomainServiceBase : DomainService
    {
        public TenantManager TenantManager { get; set; }
        public UserManager UserManager { get; set; }

        /* Add your common members for all your domain services. */
        protected TACHYONDomainServiceBase()
        {
            LocalizationSourceName = TACHYONConsts.LocalizationSourceName;
        }

        protected virtual void DisableTenancyFilters()
        {
            CurrentUnitOfWork.DisableFilter(AbpDataFilters.MustHaveTenant, AbpDataFilters.MayHaveTenant);

        }
        protected virtual Tenant GetCurrentTenant(IAbpSession abpSession)
        {
            using (CurrentUnitOfWork.SetTenantId(null))
            {
                return TenantManager.GetById(abpSession.GetTenantId());
            }
        }

        protected virtual async Task<User> GetCurrentUserAsync(IAbpSession abpSession)
        {
            var user = await UserManager.FindByIdAsync(abpSession.GetUserId().ToString());
            if (user == null)
            {
                throw new Exception("There is no current user!");
            }

            return user;
        }
    }
}