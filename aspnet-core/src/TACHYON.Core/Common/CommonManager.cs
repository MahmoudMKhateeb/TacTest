using Abp.Domain.Uow;
using Abp.Runtime.Session;
using System;

namespace TACHYON.Common
{
    public class CommonManager : TACHYONDomainServiceBase
    {
        private readonly IUnitOfWorkManager _UnitOfWorkManager;
        private IAbpSession _AbpSession { get; set; }
        public CommonManager(IUnitOfWorkManager UnitOfWorkManager, IAbpSession AbpSession)
        {
            _UnitOfWorkManager = UnitOfWorkManager;
            _AbpSession = AbpSession;
        }
        public T ExecuteMethodIfHostOrTenantUsers<T>(Func<T> funcToRun)
        {
            if (_AbpSession.TenantId.HasValue)
            {
                return funcToRun();
            }
            else
            {
                using (_UnitOfWorkManager.Current.DisableFilter(AbpDataFilters.MustHaveTenant))
                {
                    return funcToRun();
                }
            }
        }

    }
}
