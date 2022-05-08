using Abp.Authorization;
using Abp.MultiTenancy;

namespace TACHYON.Authorization.Permissions.PriceOffers
{
    public class AppAuthorizationPriceOffersProvider : AppAuthorizationBaseProvider
    {
        public override void SetPermissions(IPermissionDefinitionContext context)
        {
            var pages = context.GetPermissionOrNull(AppPermissions.Pages) ??
                        context.CreatePermission(AppPermissions.Pages, L("Pages"));

            var offers = pages.CreateChildPermission(AppPermissions.Pages_Offers, L("Offers"),
                multiTenancySides: MultiTenancySides.Tenant);
            offers.CreateChildPermission(AppPermissions.Pages_Offers_Create, L("CreateOffer"),
                multiTenancySides: MultiTenancySides.Tenant);
            //offers.CreateChildPermission(AppPermissions.Pages_Offers_Edit, L("EditOffer"), multiTenancySides: MultiTenancySides.Tenant);
            offers.CreateChildPermission(AppPermissions.Pages_Offers_Delete, L("DeleteOffer"),
                multiTenancySides: MultiTenancySides.Tenant);
            offers.CreateChildPermission(AppPermissions.Pages_Offers_Reject, L("RejectOffer"),
                multiTenancySides: MultiTenancySides.Tenant);
            offers.CreateChildPermission(AppPermissions.Pages_Offers_Accept, L("AcceptOffer"),
                multiTenancySides: MultiTenancySides.Tenant);
        }
    }
}