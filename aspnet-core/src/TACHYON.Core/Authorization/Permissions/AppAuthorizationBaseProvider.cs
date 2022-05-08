using Abp.Authorization;
using Abp.Localization;
using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Authorization.Permissions
{
    public abstract class AppAuthorizationBaseProvider : AuthorizationProvider
    {
        protected ILocalizableString L(string name)
        {
            return new LocalizableString(name, TACHYONConsts.LocalizationSourceName);
        }

        public override void SetPermissions(IPermissionDefinitionContext context)
        {
        }
    }
}