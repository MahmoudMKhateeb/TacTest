using Abp.Configuration.Startup;
using Abp.Dependency;
using Abp.MultiTenancy;
using Abp.Runtime;
using Abp.Runtime.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TACHYON.Common
{

    //Define your own session and add your custom field to it
    //Then, you can inject MyAppSession and use it's new property in your project.
    public class TachyonMobileAppSession : ClaimsAbpSession, ITransientDependency
    {
        public TachyonMobileAppSession(
            IPrincipalAccessor principalAccessor,
            IMultiTenancyConfig multiTenancy,
            ITenantResolver tenantResolver,
            IAmbientScopeProvider<SessionOverride> sessionOverrideScopeProvider) :
            base(principalAccessor, multiTenancy, tenantResolver, sessionOverrideScopeProvider)
        {

        }

        public string DeviceId
        {
            get
            {
                var deviceIdClaim = PrincipalAccessor.Principal?.Claims.FirstOrDefault(c => c.Type == AppConsts.MobileDeviceId);
                if (string.IsNullOrEmpty(deviceIdClaim?.Value))
                {
                    return null;
                }

                return deviceIdClaim.Value;
            }
        }

        public string DeviceToken
        {
            get
            {
                var mobileDeviceTokenClaim = PrincipalAccessor.Principal?.Claims.FirstOrDefault(c => c.Type == AppConsts.MobileDeviceToken);
                if (string.IsNullOrEmpty(mobileDeviceTokenClaim?.Value))
                {
                    return null;
                }

                return mobileDeviceTokenClaim.Value;
            }
        }
    }
}
