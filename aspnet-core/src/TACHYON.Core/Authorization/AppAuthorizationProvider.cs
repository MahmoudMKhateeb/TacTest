using Abp.Authorization;
using Abp.Configuration.Startup;
using Abp.Localization;
using Abp.MultiTenancy;

namespace TACHYON.Authorization
{
    /// <summary>
    /// Application's authorization provider.
    /// Defines permissions for the application.
    /// See <see cref="AppPermissions"/> for all permission names.
    /// </summary>
    public class AppAuthorizationProvider : AuthorizationProvider
    {
        private readonly bool _isMultiTenancyEnabled;

        public AppAuthorizationProvider(bool isMultiTenancyEnabled)
        {
            _isMultiTenancyEnabled = isMultiTenancyEnabled;
        }

        public AppAuthorizationProvider(IMultiTenancyConfig multiTenancyConfig)
        {
            _isMultiTenancyEnabled = multiTenancyConfig.IsEnabled;
        }

        public override void SetPermissions(IPermissionDefinitionContext context)
        {
            //COMMON PERMISSIONS (FOR BOTH OF TENANTS AND HOST)

            var pages = context.GetPermissionOrNull(AppPermissions.Pages) ?? context.CreatePermission(AppPermissions.Pages, L("Pages"));

            var documentFiles = pages.CreateChildPermission(AppPermissions.Pages_DocumentFiles, L("DocumentFiles"));
            documentFiles.CreateChildPermission(AppPermissions.Pages_DocumentFiles_Create, L("CreateNewDocumentFile"));
            documentFiles.CreateChildPermission(AppPermissions.Pages_DocumentFiles_Edit, L("EditDocumentFile"));
            documentFiles.CreateChildPermission(AppPermissions.Pages_DocumentFiles_Delete, L("DeleteDocumentFile"));



            var documentTypes = pages.CreateChildPermission(AppPermissions.Pages_DocumentTypes, L("DocumentTypes"), multiTenancySides: MultiTenancySides.Host);
            documentTypes.CreateChildPermission(AppPermissions.Pages_DocumentTypes_Create, L("CreateNewDocumentType"), multiTenancySides: MultiTenancySides.Host);
            documentTypes.CreateChildPermission(AppPermissions.Pages_DocumentTypes_Edit, L("EditDocumentType"), multiTenancySides: MultiTenancySides.Host);
            documentTypes.CreateChildPermission(AppPermissions.Pages_DocumentTypes_Delete, L("DeleteDocumentType"), multiTenancySides: MultiTenancySides.Host);



            var shippingRequests = pages.CreateChildPermission(AppPermissions.Pages_ShippingRequests, L("ShippingRequests"), multiTenancySides: MultiTenancySides.Tenant);
            shippingRequests.CreateChildPermission(AppPermissions.Pages_ShippingRequests_Create, L("CreateNewShippingRequest"), multiTenancySides: MultiTenancySides.Tenant);
            shippingRequests.CreateChildPermission(AppPermissions.Pages_ShippingRequests_Edit, L("EditShippingRequest"), multiTenancySides: MultiTenancySides.Tenant);
            shippingRequests.CreateChildPermission(AppPermissions.Pages_ShippingRequests_Delete, L("DeleteShippingRequest"), multiTenancySides: MultiTenancySides.Tenant);



            var goodsDetails = pages.CreateChildPermission(AppPermissions.Pages_GoodsDetails, L("GoodsDetails"), multiTenancySides: MultiTenancySides.Tenant);
            goodsDetails.CreateChildPermission(AppPermissions.Pages_GoodsDetails_Create, L("CreateNewGoodsDetail"), multiTenancySides: MultiTenancySides.Tenant);
            goodsDetails.CreateChildPermission(AppPermissions.Pages_GoodsDetails_Edit, L("EditGoodsDetail"), multiTenancySides: MultiTenancySides.Tenant);
            goodsDetails.CreateChildPermission(AppPermissions.Pages_GoodsDetails_Delete, L("DeleteGoodsDetail"), multiTenancySides: MultiTenancySides.Tenant);



            var offers = pages.CreateChildPermission(AppPermissions.Pages_Offers, L("Offers"), multiTenancySides: MultiTenancySides.Tenant);
            offers.CreateChildPermission(AppPermissions.Pages_Offers_Create, L("CreateNewOffer"), multiTenancySides: MultiTenancySides.Tenant);
            offers.CreateChildPermission(AppPermissions.Pages_Offers_Edit, L("EditOffer"), multiTenancySides: MultiTenancySides.Tenant);
            offers.CreateChildPermission(AppPermissions.Pages_Offers_Delete, L("DeleteOffer"), multiTenancySides: MultiTenancySides.Tenant);



            var routSteps = pages.CreateChildPermission(AppPermissions.Pages_RoutSteps, L("RoutSteps"), multiTenancySides: MultiTenancySides.Tenant);
            routSteps.CreateChildPermission(AppPermissions.Pages_RoutSteps_Create, L("CreateNewRoutStep"), multiTenancySides: MultiTenancySides.Tenant);
            routSteps.CreateChildPermission(AppPermissions.Pages_RoutSteps_Edit, L("EditRoutStep"), multiTenancySides: MultiTenancySides.Tenant);
            routSteps.CreateChildPermission(AppPermissions.Pages_RoutSteps_Delete, L("DeleteRoutStep"), multiTenancySides: MultiTenancySides.Tenant);



            var routes = pages.CreateChildPermission(AppPermissions.Pages_Routes, L("Routes"), multiTenancySides: MultiTenancySides.Tenant);
            routes.CreateChildPermission(AppPermissions.Pages_Routes_Create, L("CreateNewRoute"), multiTenancySides: MultiTenancySides.Tenant);
            routes.CreateChildPermission(AppPermissions.Pages_Routes_Edit, L("EditRoute"), multiTenancySides: MultiTenancySides.Tenant);
            routes.CreateChildPermission(AppPermissions.Pages_Routes_Delete, L("DeleteRoute"), multiTenancySides: MultiTenancySides.Tenant);



            var cities = pages.CreateChildPermission(AppPermissions.Pages_Cities, L("Cities"), multiTenancySides: MultiTenancySides.Host);
            cities.CreateChildPermission(AppPermissions.Pages_Cities_Create, L("CreateNewCity"), multiTenancySides: MultiTenancySides.Host);
            cities.CreateChildPermission(AppPermissions.Pages_Cities_Edit, L("EditCity"), multiTenancySides: MultiTenancySides.Host);
            cities.CreateChildPermission(AppPermissions.Pages_Cities_Delete, L("DeleteCity"), multiTenancySides: MultiTenancySides.Host);



            var counties = pages.CreateChildPermission(AppPermissions.Pages_Counties, L("Counties"), multiTenancySides: MultiTenancySides.Host);
            counties.CreateChildPermission(AppPermissions.Pages_Counties_Create, L("CreateNewCounty"), multiTenancySides: MultiTenancySides.Host);
            counties.CreateChildPermission(AppPermissions.Pages_Counties_Edit, L("EditCounty"), multiTenancySides: MultiTenancySides.Host);
            counties.CreateChildPermission(AppPermissions.Pages_Counties_Delete, L("DeleteCounty"), multiTenancySides: MultiTenancySides.Host);



            var routTypes = pages.CreateChildPermission(AppPermissions.Pages_RoutTypes, L("RoutTypes"), multiTenancySides: MultiTenancySides.Host);
            routTypes.CreateChildPermission(AppPermissions.Pages_RoutTypes_Create, L("CreateNewRoutType"), multiTenancySides: MultiTenancySides.Host);
            routTypes.CreateChildPermission(AppPermissions.Pages_RoutTypes_Edit, L("EditRoutType"), multiTenancySides: MultiTenancySides.Host);
            routTypes.CreateChildPermission(AppPermissions.Pages_RoutTypes_Delete, L("DeleteRoutType"), multiTenancySides: MultiTenancySides.Host);



            var goodCategories = pages.CreateChildPermission(AppPermissions.Pages_GoodCategories, L("GoodCategories"), multiTenancySides: MultiTenancySides.Host);
            goodCategories.CreateChildPermission(AppPermissions.Pages_GoodCategories_Create, L("CreateNewGoodCategory"), multiTenancySides: MultiTenancySides.Host);
            goodCategories.CreateChildPermission(AppPermissions.Pages_GoodCategories_Edit, L("EditGoodCategory"), multiTenancySides: MultiTenancySides.Host);
            goodCategories.CreateChildPermission(AppPermissions.Pages_GoodCategories_Delete, L("DeleteGoodCategory"), multiTenancySides: MultiTenancySides.Host);



            var trailers = pages.CreateChildPermission(AppPermissions.Pages_Trailers, L("Trailers"), multiTenancySides: MultiTenancySides.Tenant);
            trailers.CreateChildPermission(AppPermissions.Pages_Trailers_Create, L("CreateNewTrailer"), multiTenancySides: MultiTenancySides.Tenant);
            trailers.CreateChildPermission(AppPermissions.Pages_Trailers_Edit, L("EditTrailer"), multiTenancySides: MultiTenancySides.Tenant);
            trailers.CreateChildPermission(AppPermissions.Pages_Trailers_Delete, L("DeleteTrailer"), multiTenancySides: MultiTenancySides.Tenant);



            var trailerStatuses = pages.CreateChildPermission(AppPermissions.Pages_TrailerStatuses, L("TrailerStatuses"), multiTenancySides: MultiTenancySides.Host);
            trailerStatuses.CreateChildPermission(AppPermissions.Pages_TrailerStatuses_Create, L("CreateNewTrailerStatus"), multiTenancySides: MultiTenancySides.Host);
            trailerStatuses.CreateChildPermission(AppPermissions.Pages_TrailerStatuses_Edit, L("EditTrailerStatus"), multiTenancySides: MultiTenancySides.Host);
            trailerStatuses.CreateChildPermission(AppPermissions.Pages_TrailerStatuses_Delete, L("DeleteTrailerStatus"), multiTenancySides: MultiTenancySides.Host);



            var payloadMaxWeights = pages.CreateChildPermission(AppPermissions.Pages_PayloadMaxWeights, L("PayloadMaxWeights"), multiTenancySides: MultiTenancySides.Host);
            payloadMaxWeights.CreateChildPermission(AppPermissions.Pages_PayloadMaxWeights_Create, L("CreateNewPayloadMaxWeight"), multiTenancySides: MultiTenancySides.Host);
            payloadMaxWeights.CreateChildPermission(AppPermissions.Pages_PayloadMaxWeights_Edit, L("EditPayloadMaxWeight"), multiTenancySides: MultiTenancySides.Host);
            payloadMaxWeights.CreateChildPermission(AppPermissions.Pages_PayloadMaxWeights_Delete, L("DeletePayloadMaxWeight"), multiTenancySides: MultiTenancySides.Host);



            var trailerTypes = pages.CreateChildPermission(AppPermissions.Pages_TrailerTypes, L("TrailerTypes"), multiTenancySides: MultiTenancySides.Host);
            trailerTypes.CreateChildPermission(AppPermissions.Pages_TrailerTypes_Create, L("CreateNewTrailerType"), multiTenancySides: MultiTenancySides.Host);
            trailerTypes.CreateChildPermission(AppPermissions.Pages_TrailerTypes_Edit, L("EditTrailerType"), multiTenancySides: MultiTenancySides.Host);
            trailerTypes.CreateChildPermission(AppPermissions.Pages_TrailerTypes_Delete, L("DeleteTrailerType"), multiTenancySides: MultiTenancySides.Host);



            var trucks = pages.CreateChildPermission(AppPermissions.Pages_Trucks, L("Trucks"), multiTenancySides: MultiTenancySides.Tenant);
            trucks.CreateChildPermission(AppPermissions.Pages_Trucks_Create, L("CreateNewTruck"), multiTenancySides: MultiTenancySides.Tenant);
            trucks.CreateChildPermission(AppPermissions.Pages_Trucks_Edit, L("EditTruck"), multiTenancySides: MultiTenancySides.Tenant);
            trucks.CreateChildPermission(AppPermissions.Pages_Trucks_Delete, L("DeleteTruck"), multiTenancySides: MultiTenancySides.Tenant);



            var trucksTypes = pages.CreateChildPermission(AppPermissions.Pages_TrucksTypes, L("TrucksTypes"), multiTenancySides: MultiTenancySides.Host);
            trucksTypes.CreateChildPermission(AppPermissions.Pages_TrucksTypes_Create, L("CreateNewTrucksType"), multiTenancySides: MultiTenancySides.Host);
            trucksTypes.CreateChildPermission(AppPermissions.Pages_TrucksTypes_Edit, L("EditTrucksType"), multiTenancySides: MultiTenancySides.Host);
            trucksTypes.CreateChildPermission(AppPermissions.Pages_TrucksTypes_Delete, L("DeleteTrucksType"), multiTenancySides: MultiTenancySides.Host);


            pages.CreateChildPermission(AppPermissions.Pages_DemoUiComponents, L("DemoUiComponents"));

            var administration = pages.CreateChildPermission(AppPermissions.Pages_Administration, L("Administration"));

            var truckStatuses = administration.CreateChildPermission(AppPermissions.Pages_Administration_TruckStatuses, L("TruckStatuses"), multiTenancySides: MultiTenancySides.Tenant);
            truckStatuses.CreateChildPermission(AppPermissions.Pages_Administration_TruckStatuses_Create, L("CreateNewTruckStatus"), multiTenancySides: MultiTenancySides.Tenant);
            truckStatuses.CreateChildPermission(AppPermissions.Pages_Administration_TruckStatuses_Edit, L("EditTruckStatus"), multiTenancySides: MultiTenancySides.Tenant);
            truckStatuses.CreateChildPermission(AppPermissions.Pages_Administration_TruckStatuses_Delete, L("DeleteTruckStatus"), multiTenancySides: MultiTenancySides.Tenant);



            var roles = administration.CreateChildPermission(AppPermissions.Pages_Administration_Roles, L("Roles"));
            roles.CreateChildPermission(AppPermissions.Pages_Administration_Roles_Create, L("CreatingNewRole"));
            roles.CreateChildPermission(AppPermissions.Pages_Administration_Roles_Edit, L("EditingRole"));
            roles.CreateChildPermission(AppPermissions.Pages_Administration_Roles_Delete, L("DeletingRole"));

            var users = administration.CreateChildPermission(AppPermissions.Pages_Administration_Users, L("Users"));
            users.CreateChildPermission(AppPermissions.Pages_Administration_Users_Create, L("CreatingNewUser"));
            users.CreateChildPermission(AppPermissions.Pages_Administration_Users_Edit, L("EditingUser"));
            users.CreateChildPermission(AppPermissions.Pages_Administration_Users_Delete, L("DeletingUser"));
            users.CreateChildPermission(AppPermissions.Pages_Administration_Users_ChangePermissions, L("ChangingPermissions"));
            users.CreateChildPermission(AppPermissions.Pages_Administration_Users_Impersonation, L("LoginForUsers"));
            users.CreateChildPermission(AppPermissions.Pages_Administration_Users_Unlock, L("Unlock"));

            var languages = administration.CreateChildPermission(AppPermissions.Pages_Administration_Languages, L("Languages"));
            languages.CreateChildPermission(AppPermissions.Pages_Administration_Languages_Create, L("CreatingNewLanguage"), multiTenancySides: _isMultiTenancyEnabled ? MultiTenancySides.Host : MultiTenancySides.Tenant);
            languages.CreateChildPermission(AppPermissions.Pages_Administration_Languages_Edit, L("EditingLanguage"), multiTenancySides: _isMultiTenancyEnabled ? MultiTenancySides.Host : MultiTenancySides.Tenant);
            languages.CreateChildPermission(AppPermissions.Pages_Administration_Languages_Delete, L("DeletingLanguages"), multiTenancySides: _isMultiTenancyEnabled ? MultiTenancySides.Host : MultiTenancySides.Tenant);
            languages.CreateChildPermission(AppPermissions.Pages_Administration_Languages_ChangeTexts, L("ChangingTexts"));

            administration.CreateChildPermission(AppPermissions.Pages_Administration_AuditLogs, L("AuditLogs"));

            var organizationUnits = administration.CreateChildPermission(AppPermissions.Pages_Administration_OrganizationUnits, L("OrganizationUnits"));
            organizationUnits.CreateChildPermission(AppPermissions.Pages_Administration_OrganizationUnits_ManageOrganizationTree, L("ManagingOrganizationTree"));
            organizationUnits.CreateChildPermission(AppPermissions.Pages_Administration_OrganizationUnits_ManageMembers, L("ManagingMembers"));
            organizationUnits.CreateChildPermission(AppPermissions.Pages_Administration_OrganizationUnits_ManageRoles, L("ManagingRoles"));

            administration.CreateChildPermission(AppPermissions.Pages_Administration_UiCustomization, L("VisualSettings"));

            var webhooks = administration.CreateChildPermission(AppPermissions.Pages_Administration_WebhookSubscription, L("Webhooks"));
            webhooks.CreateChildPermission(AppPermissions.Pages_Administration_WebhookSubscription_Create, L("CreatingWebhooks"));
            webhooks.CreateChildPermission(AppPermissions.Pages_Administration_WebhookSubscription_Edit, L("EditingWebhooks"));
            webhooks.CreateChildPermission(AppPermissions.Pages_Administration_WebhookSubscription_ChangeActivity, L("ChangingWebhookActivity"));
            webhooks.CreateChildPermission(AppPermissions.Pages_Administration_WebhookSubscription_Detail, L("DetailingSubscription"));
            webhooks.CreateChildPermission(AppPermissions.Pages_Administration_Webhook_ListSendAttempts, L("ListingSendAttempts"));
            webhooks.CreateChildPermission(AppPermissions.Pages_Administration_Webhook_ResendWebhook, L("ResendingWebhook"));

            var dynamicParameters = administration.CreateChildPermission(AppPermissions.Pages_Administration_DynamicParameters, L("DynamicParameters"));
            dynamicParameters.CreateChildPermission(AppPermissions.Pages_Administration_DynamicParameters_Create, L("CreatingDynamicParameters"));
            dynamicParameters.CreateChildPermission(AppPermissions.Pages_Administration_DynamicParameters_Edit, L("EditingDynamicParameters"));
            dynamicParameters.CreateChildPermission(AppPermissions.Pages_Administration_DynamicParameters_Delete, L("DeletingDynamicParameters"));

            var dynamicParameterValues = dynamicParameters.CreateChildPermission(AppPermissions.Pages_Administration_DynamicParameterValue, L("DynamicParameterValue"));
            dynamicParameterValues.CreateChildPermission(AppPermissions.Pages_Administration_DynamicParameterValue_Create, L("CreatingDynamicParameterValue"));
            dynamicParameterValues.CreateChildPermission(AppPermissions.Pages_Administration_DynamicParameterValue_Edit, L("EditingDynamicParameterValue"));
            dynamicParameterValues.CreateChildPermission(AppPermissions.Pages_Administration_DynamicParameterValue_Delete, L("DeletingDynamicParameterValue"));

            var entityDynamicParameters = dynamicParameters.CreateChildPermission(AppPermissions.Pages_Administration_EntityDynamicParameters, L("EntityDynamicParameters"));
            entityDynamicParameters.CreateChildPermission(AppPermissions.Pages_Administration_EntityDynamicParameters_Create, L("CreatingEntityDynamicParameters"));
            entityDynamicParameters.CreateChildPermission(AppPermissions.Pages_Administration_EntityDynamicParameters_Edit, L("EditingEntityDynamicParameters"));
            entityDynamicParameters.CreateChildPermission(AppPermissions.Pages_Administration_EntityDynamicParameters_Delete, L("DeletingEntityDynamicParameters"));

            var entityDynamicParameterValues = dynamicParameters.CreateChildPermission(AppPermissions.Pages_Administration_EntityDynamicParameterValue, L("EntityDynamicParameterValue"));
            entityDynamicParameterValues.CreateChildPermission(AppPermissions.Pages_Administration_EntityDynamicParameterValue_Create, L("CreatingEntityDynamicParameterValue"));
            entityDynamicParameterValues.CreateChildPermission(AppPermissions.Pages_Administration_EntityDynamicParameterValue_Edit, L("EditingEntityDynamicParameterValue"));
            entityDynamicParameterValues.CreateChildPermission(AppPermissions.Pages_Administration_EntityDynamicParameterValue_Delete, L("DeletingEntityDynamicParameterValue"));

            //TENANT-SPECIFIC PERMISSIONS

            pages.CreateChildPermission(AppPermissions.Pages_Tenant_Dashboard, L("Dashboard"), multiTenancySides: MultiTenancySides.Tenant);

            administration.CreateChildPermission(AppPermissions.Pages_Administration_Tenant_Settings, L("Settings"), multiTenancySides: MultiTenancySides.Tenant);
            administration.CreateChildPermission(AppPermissions.Pages_Administration_Tenant_SubscriptionManagement, L("Subscription"), multiTenancySides: MultiTenancySides.Tenant);

            //HOST-SPECIFIC PERMISSIONS

            var editions = pages.CreateChildPermission(AppPermissions.Pages_Editions, L("Editions"), multiTenancySides: MultiTenancySides.Host);
            editions.CreateChildPermission(AppPermissions.Pages_Editions_Create, L("CreatingNewEdition"), multiTenancySides: MultiTenancySides.Host);
            editions.CreateChildPermission(AppPermissions.Pages_Editions_Edit, L("EditingEdition"), multiTenancySides: MultiTenancySides.Host);
            editions.CreateChildPermission(AppPermissions.Pages_Editions_Delete, L("DeletingEdition"), multiTenancySides: MultiTenancySides.Host);
            editions.CreateChildPermission(AppPermissions.Pages_Editions_MoveTenantsToAnotherEdition, L("MoveTenantsToAnotherEdition"), multiTenancySides: MultiTenancySides.Host);

            var tenants = pages.CreateChildPermission(AppPermissions.Pages_Tenants, L("Tenants"), multiTenancySides: MultiTenancySides.Host);
            tenants.CreateChildPermission(AppPermissions.Pages_Tenants_Create, L("CreatingNewTenant"), multiTenancySides: MultiTenancySides.Host);
            tenants.CreateChildPermission(AppPermissions.Pages_Tenants_Edit, L("EditingTenant"), multiTenancySides: MultiTenancySides.Host);
            tenants.CreateChildPermission(AppPermissions.Pages_Tenants_ChangeFeatures, L("ChangingFeatures"), multiTenancySides: MultiTenancySides.Host);
            tenants.CreateChildPermission(AppPermissions.Pages_Tenants_Delete, L("DeletingTenant"), multiTenancySides: MultiTenancySides.Host);
            tenants.CreateChildPermission(AppPermissions.Pages_Tenants_Impersonation, L("LoginForTenants"), multiTenancySides: MultiTenancySides.Host);

            administration.CreateChildPermission(AppPermissions.Pages_Administration_Host_Settings, L("Settings"), multiTenancySides: MultiTenancySides.Host);
            administration.CreateChildPermission(AppPermissions.Pages_Administration_Host_Maintenance, L("Maintenance"), multiTenancySides: _isMultiTenancyEnabled ? MultiTenancySides.Host : MultiTenancySides.Tenant);
            administration.CreateChildPermission(AppPermissions.Pages_Administration_HangfireDashboard, L("HangfireDashboard"), multiTenancySides: _isMultiTenancyEnabled ? MultiTenancySides.Host : MultiTenancySides.Tenant);
            administration.CreateChildPermission(AppPermissions.Pages_Administration_Host_Dashboard, L("Dashboard"), multiTenancySides: MultiTenancySides.Host);
        }

        private static ILocalizableString L(string name)
        {
            return new LocalizableString(name, TACHYONConsts.LocalizationSourceName);
        }
    }
}