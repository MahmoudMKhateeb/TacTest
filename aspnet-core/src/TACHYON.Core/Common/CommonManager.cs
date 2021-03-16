using Abp.Application.Features;
using Abp.Domain.Uow;
using Abp.Runtime.Session;
using Abp.UI;
using System;

namespace TACHYON.Common
{
    public class CommonManager : TACHYONDomainServiceBase
    {
        private readonly IUnitOfWorkManager _UnitOfWorkManager;
        private IAbpSession _AbpSession { get; set; }
        private readonly IFeatureChecker _featureChecker;
        public CommonManager(
            IUnitOfWorkManager UnitOfWorkManager,
            IAbpSession AbpSession,
            IFeatureChecker featureChecker)
        {
            _UnitOfWorkManager = UnitOfWorkManager;
            _AbpSession = AbpSession;
            _featureChecker = featureChecker;
        }
        public  T ExecuteMethodIfHostOrTenantUsers<T>(Func<T> funcToRun, string featurename = "")
        {

            if (_AbpSession.TenantId.HasValue && (string.IsNullOrEmpty(featurename) || _featureChecker.IsEnabled(featurename)))
            {
                return funcToRun();
            }
            else if (!_AbpSession.TenantId.HasValue)
            {
                using (_UnitOfWorkManager.Current.DisableFilter(AbpDataFilters.MustHaveTenant))
                {
                    return funcToRun();
                }
            }
            else
            {
                throw new UserFriendlyException(L("YouDontHaveThisPermission"));
            }
            
        }


        

    }
}
