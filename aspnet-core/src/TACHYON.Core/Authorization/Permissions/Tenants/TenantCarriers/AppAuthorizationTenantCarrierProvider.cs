using Abp.Authorization;
using Abp.MultiTenancy;
using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Authorization.Permissions.Tenants.TenantCarriers
{
    public class AppAuthorizationTenantCarrierProvider : AppAuthorizationBaseProvider
    {
        public override void SetPermissions(IPermissionDefinitionContext context)
        {
            var pages = context.GetPermissionOrNull(AppPermissions.Pages) ?? context.CreatePermission(AppPermissions.Pages, L("Pages"));

            var node = pages.CreateChildPermission(AppPermissions.Pages_ShippingRequestTrips, L("ShippingRequests"), multiTenancySides: MultiTenancySides.Host);
            shippingRequestTrips.CreateChildPermission(AppPermissions.Pages_ShippingRequestTrips_Create, L("CreateNewShippingRequestTrip"), multiTenancySides: MultiTenancySides.Tenant);
            shippingRequestTrips.CreateChildPermission(AppPermissions.Pages_ShippingRequestTrips_Edit, L("EditShippingRequestTrip"), multiTenancySides: MultiTenancySides.Tenant);
            shippingRequestTrips.CreateChildPermission(AppPermissions.Pages_ShippingRequestTrips_Delete, L("DeleteShippingRequestTrip"), multiTenancySides: MultiTenancySides.Tenant);
            shippingRequestTrips.CreateChildPermission(AppPermissions.Pages_ShippingRequestTrips_Acident_Cancel, L("CancelTrip"), multiTenancySides: MultiTenancySides.Tenant);

        }
    }
}