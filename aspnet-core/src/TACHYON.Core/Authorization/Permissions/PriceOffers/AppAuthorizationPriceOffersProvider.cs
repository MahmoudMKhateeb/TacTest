using Abp.Authorization;
using Abp.MultiTenancy;

namespace TACHYON.Authorization.Permissions.PriceOffers
{
    public class AppAuthorizationPriceOffersProvider : AppAuthorizationBaseProvider
    {
        public override void SetPermissions(IPermissionDefinitionContext context)
        {
            var pages = context.GetPermissionOrNull(AppPermissions.Pages) ?? context.CreatePermission(AppPermissions.Pages, L("Pages"));
             pages.CreateChildPermission(AppPermissions.Pages_MarketPlace, L("Marketplace"));
           
        }
    }
}
