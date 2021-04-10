using Abp.Domain.Services;
using Abp.Domain.Uow;
using Abp.Runtime.Session;

namespace TACHYON
{
    public abstract class TACHYONDomainServiceBase : DomainService
    {
        /* Add your common members for all your domain services. */
        protected TACHYONDomainServiceBase()
        {
            LocalizationSourceName = TACHYONConsts.LocalizationSourceName;
        }

        protected virtual void DisableTenancyFilters()
        {
            CurrentUnitOfWork.DisableFilter(AbpDataFilters.MustHaveTenant, AbpDataFilters.MayHaveTenant);

        }
    }
}