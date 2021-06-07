//using Abp.Authorization;
//using Abp.MultiTenancy;

//namespace TACHYON.Authorization.Permissions.Tenants.TenantCarriers
//{
//    public class AppAuthorizationTenantCarrierProvider : AppAuthorizationBaseProvider
//    {
//        public override void SetPermissions(IPermissionDefinitionContext context)
//        {
//            var Tenants = context.GetPermissionOrNull(AppPermissions.Pages_Tenants) ?? context.CreatePermission(AppPermissions.Pages_Tenants, L("Tenants"));

//            var node = Tenants.CreateChildPermission(AppPermissions.Pages_TenantCarrier, L("TenantCarrier"), multiTenancySides: MultiTenancySides.Host);
//            node.CreateChildPermission(AppPermissions.Pages_TenantCarrier_Create, L("Create"), multiTenancySides: MultiTenancySides.Host);
//            node.CreateChildPermission(AppPermissions.Pages_TenantCarrier_Delete, L("Delete"), multiTenancySides: MultiTenancySides.Host);

//        }
//    }
//}