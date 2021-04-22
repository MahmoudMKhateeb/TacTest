using Abp.Authorization;
using Abp.MultiTenancy;

namespace TACHYON.Authorization.Permissions.Languages
{
    public class AppLocalizationProvider : AppAuthorizationBaseProvider
    {
        public override void SetPermissions(IPermissionDefinitionContext context)
        {
            var pages = context.GetPermissionOrNull(AppPermissions.Pages) ?? context.CreatePermission(AppPermissions.Pages, L("Pages"));
            var AppLocalization = pages.CreateChildPermission(AppPermissions.Pages_AppLocalization, L("AppLocalization"), multiTenancySides: MultiTenancySides.Host);
            AppLocalization.CreateChildPermission(AppPermissions.Pages_AppLocalization_Create, L("CreateAppLocalization"), multiTenancySides: MultiTenancySides.Host);
            AppLocalization.CreateChildPermission(AppPermissions.Pages_AppLocalization_Edit, L("EditAppLocalization"), multiTenancySides: MultiTenancySides.Host);
            AppLocalization.CreateChildPermission(AppPermissions.Pages_AppLocalization_Delete, L("DeleteAppLocalization"), multiTenancySides: MultiTenancySides.Host);
            AppLocalization.CreateChildPermission(AppPermissions.Pages_AppLocalization_Restore, L("RestoreAppLocalization"), multiTenancySides: MultiTenancySides.Host);
            AppLocalization.CreateChildPermission(AppPermissions.Pages_AppLocalization_Generate, L("GenerateAppLocalization"), multiTenancySides: MultiTenancySides.Host);

        }
    }
}