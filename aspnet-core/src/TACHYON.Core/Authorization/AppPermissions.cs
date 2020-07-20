namespace TACHYON.Authorization
{
    /// <summary>
    /// Defines string constants for application's permission names.
    /// <see cref="AppAuthorizationProvider"/> for permission definitions.
    /// </summary>
    public static class AppPermissions
    {
        public const string Pages_Routes = "Pages.Routes";
        public const string Pages_Routes_Create = "Pages.Routes.Create";
        public const string Pages_Routes_Edit = "Pages.Routes.Edit";
        public const string Pages_Routes_Delete = "Pages.Routes.Delete";

        public const string Pages_Cities = "Pages.Cities";
        public const string Pages_Cities_Create = "Pages.Cities.Create";
        public const string Pages_Cities_Edit = "Pages.Cities.Edit";
        public const string Pages_Cities_Delete = "Pages.Cities.Delete";

        public const string Pages_Counties = "Pages.Counties";
        public const string Pages_Counties_Create = "Pages.Counties.Create";
        public const string Pages_Counties_Edit = "Pages.Counties.Edit";
        public const string Pages_Counties_Delete = "Pages.Counties.Delete";

        public const string Pages_RoutTypes = "Pages.RoutTypes";
        public const string Pages_RoutTypes_Create = "Pages.RoutTypes.Create";
        public const string Pages_RoutTypes_Edit = "Pages.RoutTypes.Edit";
        public const string Pages_RoutTypes_Delete = "Pages.RoutTypes.Delete";

        public const string Pages_GoodCategories = "Pages.GoodCategories";
        public const string Pages_GoodCategories_Create = "Pages.GoodCategories.Create";
        public const string Pages_GoodCategories_Edit = "Pages.GoodCategories.Edit";
        public const string Pages_GoodCategories_Delete = "Pages.GoodCategories.Delete";

        public const string Pages_Trailers = "Pages.Trailers";
        public const string Pages_Trailers_Create = "Pages.Trailers.Create";
        public const string Pages_Trailers_Edit = "Pages.Trailers.Edit";
        public const string Pages_Trailers_Delete = "Pages.Trailers.Delete";

        public const string Pages_TrailerStatuses = "Pages.TrailerStatuses";
        public const string Pages_TrailerStatuses_Create = "Pages.TrailerStatuses.Create";
        public const string Pages_TrailerStatuses_Edit = "Pages.TrailerStatuses.Edit";
        public const string Pages_TrailerStatuses_Delete = "Pages.TrailerStatuses.Delete";

        public const string Pages_PayloadMaxWeights = "Pages.PayloadMaxWeights";
        public const string Pages_PayloadMaxWeights_Create = "Pages.PayloadMaxWeights.Create";
        public const string Pages_PayloadMaxWeights_Edit = "Pages.PayloadMaxWeights.Edit";
        public const string Pages_PayloadMaxWeights_Delete = "Pages.PayloadMaxWeights.Delete";

        public const string Pages_TrailerTypes = "Pages.TrailerTypes";
        public const string Pages_TrailerTypes_Create = "Pages.TrailerTypes.Create";
        public const string Pages_TrailerTypes_Edit = "Pages.TrailerTypes.Edit";
        public const string Pages_TrailerTypes_Delete = "Pages.TrailerTypes.Delete";

        public const string Pages_Trucks = "Pages.Trucks";
        public const string Pages_Trucks_Create = "Pages.Trucks.Create";
        public const string Pages_Trucks_Edit = "Pages.Trucks.Edit";
        public const string Pages_Trucks_Delete = "Pages.Trucks.Delete";

        public const string Pages_TrucksTypes = "Pages.TrucksTypes";
        public const string Pages_TrucksTypes_Create = "Pages.TrucksTypes.Create";
        public const string Pages_TrucksTypes_Edit = "Pages.TrucksTypes.Edit";
        public const string Pages_TrucksTypes_Delete = "Pages.TrucksTypes.Delete";

        public const string Pages_Administration_TruckStatuses = "Pages.Administration.TruckStatuses";
        public const string Pages_Administration_TruckStatuses_Create = "Pages.Administration.TruckStatuses.Create";
        public const string Pages_Administration_TruckStatuses_Edit = "Pages.Administration.TruckStatuses.Edit";
        public const string Pages_Administration_TruckStatuses_Delete = "Pages.Administration.TruckStatuses.Delete";

        //COMMON PERMISSIONS (FOR BOTH OF TENANTS AND HOST)

        public const string Pages = "Pages";

        public const string Pages_DemoUiComponents = "Pages.DemoUiComponents";
        public const string Pages_Administration = "Pages.Administration";

        public const string Pages_Administration_Roles = "Pages.Administration.Roles";
        public const string Pages_Administration_Roles_Create = "Pages.Administration.Roles.Create";
        public const string Pages_Administration_Roles_Edit = "Pages.Administration.Roles.Edit";
        public const string Pages_Administration_Roles_Delete = "Pages.Administration.Roles.Delete";

        public const string Pages_Administration_Users = "Pages.Administration.Users";
        public const string Pages_Administration_Users_Create = "Pages.Administration.Users.Create";
        public const string Pages_Administration_Users_Edit = "Pages.Administration.Users.Edit";
        public const string Pages_Administration_Users_Delete = "Pages.Administration.Users.Delete";
        public const string Pages_Administration_Users_ChangePermissions = "Pages.Administration.Users.ChangePermissions";
        public const string Pages_Administration_Users_Impersonation = "Pages.Administration.Users.Impersonation";
        public const string Pages_Administration_Users_Unlock = "Pages.Administration.Users.Unlock";

        public const string Pages_Administration_Languages = "Pages.Administration.Languages";
        public const string Pages_Administration_Languages_Create = "Pages.Administration.Languages.Create";
        public const string Pages_Administration_Languages_Edit = "Pages.Administration.Languages.Edit";
        public const string Pages_Administration_Languages_Delete = "Pages.Administration.Languages.Delete";
        public const string Pages_Administration_Languages_ChangeTexts = "Pages.Administration.Languages.ChangeTexts";

        public const string Pages_Administration_AuditLogs = "Pages.Administration.AuditLogs";

        public const string Pages_Administration_OrganizationUnits = "Pages.Administration.OrganizationUnits";
        public const string Pages_Administration_OrganizationUnits_ManageOrganizationTree = "Pages.Administration.OrganizationUnits.ManageOrganizationTree";
        public const string Pages_Administration_OrganizationUnits_ManageMembers = "Pages.Administration.OrganizationUnits.ManageMembers";
        public const string Pages_Administration_OrganizationUnits_ManageRoles = "Pages.Administration.OrganizationUnits.ManageRoles";

        public const string Pages_Administration_HangfireDashboard = "Pages.Administration.HangfireDashboard";

        public const string Pages_Administration_UiCustomization = "Pages.Administration.UiCustomization";

        public const string Pages_Administration_WebhookSubscription = "Pages.Administration.WebhookSubscription";
        public const string Pages_Administration_WebhookSubscription_Create = "Pages.Administration.WebhookSubscription.Create";
        public const string Pages_Administration_WebhookSubscription_Edit = "Pages.Administration.WebhookSubscription.Edit";
        public const string Pages_Administration_WebhookSubscription_ChangeActivity = "Pages.Administration.WebhookSubscription.ChangeActivity";
        public const string Pages_Administration_WebhookSubscription_Detail = "Pages.Administration.WebhookSubscription.Detail";
        public const string Pages_Administration_Webhook_ListSendAttempts = "Pages.Administration.Webhook.ListSendAttempts";
        public const string Pages_Administration_Webhook_ResendWebhook = "Pages.Administration.Webhook.ResendWebhook";

        public const string Pages_Administration_DynamicParameters = "Pages.Administration.DynamicParameters";
        public const string Pages_Administration_DynamicParameters_Create = "Pages.Administration.DynamicParameters.Create";
        public const string Pages_Administration_DynamicParameters_Edit = "Pages.Administration.DynamicParameters.Edit";
        public const string Pages_Administration_DynamicParameters_Delete = "Pages.Administration.DynamicParameters.Delete";

        public const string Pages_Administration_DynamicParameterValue = "Pages.Administration.DynamicParameterValue";
        public const string Pages_Administration_DynamicParameterValue_Create = "Pages.Administration.DynamicParameterValue.Create";
        public const string Pages_Administration_DynamicParameterValue_Edit = "Pages.Administration.DynamicParameterValue.Edit";
        public const string Pages_Administration_DynamicParameterValue_Delete = "Pages.Administration.DynamicParameterValue.Delete";

        public const string Pages_Administration_EntityDynamicParameters = "Pages.Administration.EntityDynamicParameters";
        public const string Pages_Administration_EntityDynamicParameters_Create = "Pages.Administration.EntityDynamicParameters.Create";
        public const string Pages_Administration_EntityDynamicParameters_Edit = "Pages.Administration.EntityDynamicParameters.Edit";
        public const string Pages_Administration_EntityDynamicParameters_Delete = "Pages.Administration.EntityDynamicParameters.Delete";

        public const string Pages_Administration_EntityDynamicParameterValue = "Pages.Administration.EntityDynamicParameterValue";
        public const string Pages_Administration_EntityDynamicParameterValue_Create = "Pages.Administration.EntityDynamicParameterValue.Create";
        public const string Pages_Administration_EntityDynamicParameterValue_Edit = "Pages.Administration.EntityDynamicParameterValue.Edit";
        public const string Pages_Administration_EntityDynamicParameterValue_Delete = "Pages.Administration.EntityDynamicParameterValue.Delete";
        //TENANT-SPECIFIC PERMISSIONS

        public const string Pages_Tenant_Dashboard = "Pages.Tenant.Dashboard";

        public const string Pages_Administration_Tenant_Settings = "Pages.Administration.Tenant.Settings";

        public const string Pages_Administration_Tenant_SubscriptionManagement = "Pages.Administration.Tenant.SubscriptionManagement";

        //HOST-SPECIFIC PERMISSIONS

        public const string Pages_Editions = "Pages.Editions";
        public const string Pages_Editions_Create = "Pages.Editions.Create";
        public const string Pages_Editions_Edit = "Pages.Editions.Edit";
        public const string Pages_Editions_Delete = "Pages.Editions.Delete";
        public const string Pages_Editions_MoveTenantsToAnotherEdition = "Pages.Editions.MoveTenantsToAnotherEdition";

        public const string Pages_Tenants = "Pages.Tenants";
        public const string Pages_Tenants_Create = "Pages.Tenants.Create";
        public const string Pages_Tenants_Edit = "Pages.Tenants.Edit";
        public const string Pages_Tenants_ChangeFeatures = "Pages.Tenants.ChangeFeatures";
        public const string Pages_Tenants_Delete = "Pages.Tenants.Delete";
        public const string Pages_Tenants_Impersonation = "Pages.Tenants.Impersonation";

        public const string Pages_Administration_Host_Maintenance = "Pages.Administration.Host.Maintenance";
        public const string Pages_Administration_Host_Settings = "Pages.Administration.Host.Settings";
        public const string Pages_Administration_Host_Dashboard = "Pages.Administration.Host.Dashboard";

    }
}