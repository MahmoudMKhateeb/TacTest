using Abp.Domain.Services;
using Abp.Domain.Uow;
using Abp.Runtime.Session;
using TACHYON.MultiTenancy;

namespace TACHYON
{
    public abstract class TACHYONDomainServiceBase : DomainService
    {
        public TenantManager TenantManager { get; set; }
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
    }
}