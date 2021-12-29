namespace TACHYON.Authorization
{
    /// <summary>
    /// Defines string constants for application's permission names.
    /// <see cref="AppAuthorizationProvider"/> for permission definitions.
    /// </summary>
    public static class AppPermissions
    {
        public const string Pages_DangerousGoodTypes = "Pages.DangerousGoodTypes";
        public const string Pages_DangerousGoodTypes_Create = "Pages.DangerousGoodTypes.Create";
        public const string Pages_DangerousGoodTypes_Edit = "Pages.DangerousGoodTypes.Edit";
        public const string Pages_DangerousGoodTypes_Delete = "Pages.DangerousGoodTypes.Delete";

        public const string Pages_Receivers = "Pages.Receivers";
        public const string Pages_Receivers_Create = "Pages.Receivers.Create";
        public const string Pages_Receivers_Edit = "Pages.Receivers.Edit";
        public const string Pages_Receivers_Delete = "Pages.Receivers.Delete";

        public const string Pages_TruckCapacitiesTranslations = "Pages.TruckCapacitiesTranslations";
        public const string Pages_TruckCapacitiesTranslations_Create = "Pages.TruckCapacitiesTranslations.Create";
        public const string Pages_TruckCapacitiesTranslations_Edit = "Pages.TruckCapacitiesTranslations.Edit";
        public const string Pages_TruckCapacitiesTranslations_Delete = "Pages.TruckCapacitiesTranslations.Delete";

        public const string Pages_TruckStatusesTranslations = "Pages.TruckStatusesTranslations";
        public const string Pages_TruckStatusesTranslations_Create = "Pages.TruckStatusesTranslations.Create";
        public const string Pages_TruckStatusesTranslations_Edit = "Pages.TruckStatusesTranslations.Edit";
        public const string Pages_TruckStatusesTranslations_Delete = "Pages.TruckStatusesTranslations.Delete";

        public const string App_Shipper = "App.Shipper";
        public const string App_Carrier = "App.Carrier";

        public const string Pages_HostDashboard = "Pages.HostDashboard";
        public const string Pages_ShipperDashboard = "Pages.ShipperDashboard";

        public const string Pages_CitiesTranslations = "Pages.CitiesTranslations";
        public const string Pages_CitiesTranslations_Create = "Pages.CitiesTranslations.Create";
        public const string Pages_CitiesTranslations_Edit = "Pages.CitiesTranslations.Edit";
        public const string Pages_CitiesTranslations_Delete = "Pages.CitiesTranslations.Delete";

        public const string Pages_CountriesTranslations = "Pages.CountriesTranslations";
        public const string Pages_CountriesTranslations_Create = "Pages.CountriesTranslations.Create";
        public const string Pages_CountriesTranslations_Edit = "Pages.CountriesTranslations.Edit";
        public const string Pages_CountriesTranslations_Delete = "Pages.CountriesTranslations.Delete";

        public const string Pages_PlateTypes = "Pages.PlateTypes";
        public const string Pages_PlateTypes_Create = "Pages.PlateTypes.Create";
        public const string Pages_PlateTypes_Edit = "Pages.PlateTypes.Edit";
        public const string Pages_PlateTypes_Delete = "Pages.PlateTypes.Delete";


        #region Shipping Request

        #region Trip
        public const string Pages_ShippingRequestTrips = "Pages.ShippingRequestTrips";
        public const string Pages_ShippingRequestTrips_Create = "Pages.ShippingRequestTrips.Create";
        public const string Pages_ShippingRequestTrips_Edit = "Pages.ShippingRequestTrips.Edit";
        public const string Pages_ShippingRequestTrips_Delete = "Pages.ShippingRequestTrips.Delete";
        public const string Pages_ShippingRequestTrips_Acident_Cancel = "Pages.ShippingRequestTrips.Accident.Cancel";
        #region Accident
        public const string Pages_ShippingRequestResoneAccidents = "Pages.ShippingRequestResoneAccidents";
        public const string Pages_ShippingRequestResoneAccidents_Create = "Pages.ShippingRequestResoneAccidents.Create";
        public const string Pages_ShippingRequestResoneAccidents_Edit = "Pages.ShippingRequestResoneAccidents.Edit";
        public const string Pages_ShippingRequestResoneAccidents_Delete = "Pages.ShippingRequestResoneAccidents.Delete";

        public const string Pages_ShippingRequest_Accidents = "Pages.ShippingRequest.Accidents";
        public const string Pages_ShippingRequest_Accidents_Get = "Pages.ShippingRequest.Accidents.Get";
        public const string Pages_ShippingRequest_Accidents_Create = "Pages.ShippingRequest.Accidents.Create";
        public const string Pages_ShippingRequest_Accidents_Edit = "Pages.ShippingRequest.Accidents.Edit";
        public const string Pages_ShippingRequest_Accidents_Resolve_Create = "Pages.ShippingRequest.Accidents.Resolve.Create";
        public const string Pages_ShippingRequest_Accidents_Resolve_Edit = "Pages.ShippingRequest.Accidents.Resolve.Edit";

        public const string Pages_ShippingRequestTrip_Accident_Comments = "Pages.ShippingRequestTrip.Accident.Comments";
        public const string Pages_ShippingRequest_Accidents_Comments_Create = "Pages.ShippingRequest.Accidents.Comments.Create";
        public const string Pages_ShippingRequest_Accidents_Comments_Edit = "Pages.ShippingRequest.Accidents.Comments.Edit";
        public const string Pages_ShippingRequest_Accidents_Comments_Delete = "Pages.ShippingRequest.Accidents.Comments.Delete";
        #endregion
        #region "Reject Reason Trip"
        public const string Pages_ShippingRequestTrips_Reject_Reason = "Pages.ShippingRequestTrips.Reject.Reason";
        public const string Pages_ShippingRequestTrips_Reject_Reason_Create = "Pages.ShippingRequestTrips.Reject.Reason.Create";
        public const string Pages_ShippingRequestTrips_Reject_Reason_Edit = "Pages.ShippingRequestTrips.Reject.Reason.Edit";
        public const string Pages_ShippingRequestTrips_Reject_Reason_Delete = "Pages.ShippingRequestTrips.Reject.Reason.Delete";
        #endregion
        #endregion
        public const string Pages_AppLocalization = "Pages.AppLocalizations";
        public const string Pages_AppLocalization_Create = "Pages.AppLocalizations.Create";
        public const string Pages_AppLocalization_Edit = "Pages.AppLocalizations.Edit";
        public const string Pages_AppLocalization_Delete = "Pages.AppLocalizations.Delete";
        public const string Pages_AppLocalization_Restore = "Pages.AppLocalizations.Restore";
        public const string Pages_AppLocalization_Generate = "Pages.AppLocalizations.Generate";

        #endregion
        #region Localization
        #endregion
        public const string Pages_PackingTypes = "Pages.PackingTypes";
        public const string Pages_PackingTypes_Create = "Pages.PackingTypes.Create";
        public const string Pages_PackingTypes_Edit = "Pages.PackingTypes.Edit";
        public const string Pages_PackingTypes_Delete = "Pages.PackingTypes.Delete";

        public const string Pages_ShippingTypes = "Pages.ShippingTypes";
        public const string Pages_ShippingTypes_Create = "Pages.ShippingTypes.Create";
        public const string Pages_ShippingTypes_Edit = "Pages.ShippingTypes.Edit";
        public const string Pages_ShippingTypes_Delete = "Pages.ShippingTypes.Delete";


        public const string Pages_Nationalities = "Pages.Nationalities";
        public const string Pages_Nationalities_Create = "Pages.Nationalities.Create";
        public const string Pages_Nationalities_Edit = "Pages.Nationalities.Edit";
        public const string Pages_Nationalities_Delete = "Pages.Nationalities.Delete";

        public const string Pages_NationalityTranslations = "Pages.NationalityTranslations";
        public const string Pages_NationalityTranslations_Create = "Pages.NationalityTranslations.Create";
        public const string Pages_NationalityTranslations_Edit = "Pages.NationalityTranslations.Edit";
        public const string Pages_NationalityTranslations_Delete = "Pages.NationalityTranslations.Delete";

        public const string Pages_TrucksTypesTranslations = "Pages.TrucksTypesTranslations";
        public const string Pages_TrucksTypesTranslations_Create = "Pages.TrucksTypesTranslations.Create";
        public const string Pages_TrucksTypesTranslations_Edit = "Pages.TrucksTypesTranslations.Edit";
        public const string Pages_TrucksTypesTranslations_Delete = "Pages.TrucksTypesTranslations.Delete";

        public const string Pages_TransportTypesTranslations = "Pages.TransportTypesTranslations";
        public const string Pages_TransportTypesTranslations_Create = "Pages.TransportTypesTranslations.Create";
        public const string Pages_TransportTypesTranslations_Edit = "Pages.TransportTypesTranslations.Edit";
        public const string Pages_TransportTypesTranslations_Delete = "Pages.TransportTypesTranslations.Delete";

        public const string Pages_Administration_TermAndConditionTranslations = "Pages.Administration.TermAndConditionTranslations";
        public const string Pages_Administration_TermAndConditionTranslations_Create = "Pages.Administration.TermAndConditionTranslations.Create";
        public const string Pages_Administration_TermAndConditionTranslations_Edit = "Pages.Administration.TermAndConditionTranslations.Edit";
        public const string Pages_Administration_TermAndConditionTranslations_Delete = "Pages.Administration.TermAndConditionTranslations.Delete";

        public const string Pages_TermAndConditions = "Pages.TermAndConditions";
        public const string Pages_TermAndConditions_Create = "Pages.TermAndConditions.Create";
        public const string Pages_TermAndConditions_Edit = "Pages.TermAndConditions.Edit";
        public const string Pages_TermAndConditions_Delete = "Pages.TermAndConditions.Delete";

        public const string Pages_ShippingRequestVases = "Pages.ShippingRequestVases";
        public const string Pages_ShippingRequestVases_Create = "Pages.ShippingRequestVases.Create";
        public const string Pages_ShippingRequestVases_Edit = "Pages.ShippingRequestVases.Edit";
        public const string Pages_ShippingRequestVases_Delete = "Pages.ShippingRequestVases.Delete";



        public const string Pages_VasPrices = "Pages.VasPrices";
        public const string Pages_VasPrices_Create = "Pages.VasPrices.Create";
        public const string Pages_VasPrices_Edit = "Pages.VasPrices.Edit";
        public const string Pages_VasPrices_Delete = "Pages.VasPrices.Delete";

        public const string Pages_Administration_Vases = "Pages.Administration.Vases";
        public const string Pages_Administration_Vases_Create = "Pages.Administration.Vases.Create";
        public const string Pages_Administration_Vases_Edit = "Pages.Administration.Vases.Edit";
        public const string Pages_Administration_Vases_Delete = "Pages.Administration.Vases.Delete";

        public const string Pages_Capacities = "Pages.Capacities";
        public const string Pages_Capacities_Create = "Pages.Capacities.Create";
        public const string Pages_Capacities_Edit = "Pages.Capacities.Edit";
        public const string Pages_Capacities_Delete = "Pages.Capacities.Delete";

        public const string Pages_TransportTypes = "Pages.TransportTypes";
        public const string Pages_TransportTypes_Create = "Pages.TransportTypes.Create";
        public const string Pages_TransportTypes_Edit = "Pages.TransportTypes.Edit";
        public const string Pages_TransportTypes_Delete = "Pages.TransportTypes.Delete";

        public const string Pages_DocumentTypeTranslations = "Pages.DocumentTypeTranslations";
        public const string Pages_DocumentTypeTranslations_Create = "Pages.DocumentTypeTranslations.Create";
        public const string Pages_DocumentTypeTranslations_Edit = "Pages.DocumentTypeTranslations.Edit";
        public const string Pages_DocumentTypeTranslations_Delete = "Pages.DocumentTypeTranslations.Delete";

        public const string Pages_DocumentsEntities = "Pages.DocumentsEntities";
        public const string Pages_DocumentsEntities_Create = "Pages.DocumentsEntities.Create";
        public const string Pages_DocumentsEntities_Edit = "Pages.DocumentsEntities.Edit";
        public const string Pages_DocumentsEntities_Delete = "Pages.DocumentsEntities.Delete";

        public const string Pages_Administration_ShippingRequestStatuses = "Pages.Administration.ShippingRequestStatuses";
        public const string Pages_Administration_ShippingRequestStatuses_Create = "Pages.Administration.ShippingRequestStatuses.Create";
        public const string Pages_Administration_ShippingRequestStatuses_Edit = "Pages.Administration.ShippingRequestStatuses.Edit";
        public const string Pages_Administration_ShippingRequestStatuses_Delete = "Pages.Administration.ShippingRequestStatuses.Delete";

        public const string Pages_Ports = "Pages.Ports";
        public const string Pages_Ports_Create = "Pages.Ports.Create";
        public const string Pages_Ports_Edit = "Pages.Ports.Edit";
        public const string Pages_Ports_Delete = "Pages.Ports.Delete";

        public const string Pages_PickingTypes = "Pages.PickingTypes";
        public const string Pages_PickingTypes_Create = "Pages.PickingTypes.Create";
        public const string Pages_PickingTypes_Edit = "Pages.PickingTypes.Edit";
        public const string Pages_PickingTypes_Delete = "Pages.PickingTypes.Delete";

        public const string Pages_Administration_UnitOfMeasures = "Pages.Administration.UnitOfMeasures";
        public const string Pages_Administration_UnitOfMeasures_Create = "Pages.Administration.UnitOfMeasures.Create";
        public const string Pages_Administration_UnitOfMeasures_Edit = "Pages.Administration.UnitOfMeasures.Edit";
        public const string Pages_Administration_UnitOfMeasures_Delete = "Pages.Administration.UnitOfMeasures.Delete";

        public const string Pages_Facilities = "Pages.Facilities";
        public const string Pages_Facilities_Create = "Pages.Facilities.Create";
        public const string Pages_Facilities_Edit = "Pages.Facilities.Edit";
        public const string Pages_Facilities_Delete = "Pages.Facilities.Delete";

        public const string Pages_DocumentFiles = "Pages.DocumentFiles";
        public const string Pages_DocumentFiles_Create = "Pages.DocumentFiles.Create";
        public const string Pages_DocumentFiles_Edit = "Pages.DocumentFiles.Edit";
        public const string Pages_DocumentFiles_Delete = "Pages.DocumentFiles.Delete";

        public const string Pages_DocumentTypes = "Pages.DocumentTypes";
        public const string Pages_DocumentTypes_Create = "Pages.DocumentTypes.Create";
        public const string Pages_DocumentTypes_Edit = "Pages.DocumentTypes.Edit";
        public const string Pages_DocumentTypes_Delete = "Pages.DocumentTypes.Delete";

        public const string Pages_ShippingRequests = "Pages.ShippingRequests";
        public const string Pages_ShippingRequests_Create = "Pages.ShippingRequests.Create";
        public const string Pages_ShippingRequests_Edit = "Pages.ShippingRequests.Edit";
        public const string Pages_ShippingRequests_Delete = "Pages.ShippingRequests.Delete";

        public const string Pages_ShippingRequestBids = "Pages.ShippingRequestBis";
        public const string Pages_ShippingRequestBids_Create = "Pages.ShippingRequestBis.Create";
        public const string Pages_ShippingRequestBids_Edit = "Pages.ShippingRequestBis.Edite";
        public const string Pages_ShippingRequestBids_Delete = "Pages.ShippingRequestBis.Delete";

        public const string Pages_GoodsDetails = "Pages.GoodsDetails";
        public const string Pages_GoodsDetails_Create = "Pages.GoodsDetails.Create";
        public const string Pages_GoodsDetails_Edit = "Pages.GoodsDetails.Edit";
        public const string Pages_GoodsDetails_Delete = "Pages.GoodsDetails.Delete";

        public const string Pages_Offers = "Pages.Offers";
        public const string Pages_Offers_Create = "Pages.Offers.Create";
        public const string Pages_Offers_Edit = "Pages.Offers.Edit";
        public const string Pages_Offers_Delete = "Pages.Offers.Delete";
        public const string Pages_Offers_Reject = "Pages.Offers.Reject";
        public const string Pages_Offers_Cancel = "Pages.Offers.Cancel";
        public const string Pages_Offers_Accept = "Pages.Offers.Accept";


        public const string Pages_RoutSteps = "Pages.RoutSteps";
        public const string Pages_RoutSteps_Create = "Pages.RoutSteps.Create";
        public const string Pages_RoutSteps_Edit = "Pages.RoutSteps.Edit";
        public const string Pages_RoutSteps_Delete = "Pages.RoutSteps.Delete";

        public const string Pages_Routes = "Pages.Routes";
        public const string Pages_Routes_Create = "Pages.Routes.Create";
        public const string Pages_Routes_Edit = "Pages.Routes.Edit";
        public const string Pages_Routes_Delete = "Pages.Routes.Delete";

        public const string Pages_RoutPoints = "Pages.RoutPoints";
        public const string Pages_RoutPoints_Create = "Pages.RoutPoints.Create";
        public const string Pages_RoutPoints_Edit = "Pages.RoutPoints.Edit";
        public const string Pages_RoutPoints_Delete = "Pages.RoutPoints.Delete";

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

        public const string Pages_Tenant_TMS_Settings = "Pages.Tenant.TMS.Settings";

        public const string Pages_Administration_Host_Languages = "Pages.Administration.Host.Languages";

        public const string Pages_Tenant_ProfileManagement = "Pages.Tenant.ProfileManagement";
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
        #region TenantCarrier
        public const string Pages_TenantCarrier = "Pages.TenantCarrier";
        public const string Pages_TenantCarrier_Create = "Pages.TenantCarrier.Create";
        public const string Pages_TenantCarrier_Delete = "Pages.TenantCarrier.Delete";
        #endregion
        public const string Pages_Administration_Host_Maintenance = "Pages.Administration.Host.Maintenance";
        public const string Pages_Administration_Host_Settings = "Pages.Administration.Host.Settings";
        public const string Pages_Administration_Host_Dashboard = "Pages.Administration.Host.Dashboard";


        /*Invoices */
        public const string Pages_Administration_Host_Invoices_Periods = "Pages.Administration.Host.Invoices.Periods";
        public const string Pages_Administration_Host_Invoices_Period_Create = "Pages.Administration.Host.Invoices.Period.Create";
        public const string Pages_Administration_Host_Invoices_Period_Edit = "Pages.Administration.Host.Invoices.Period.Edit";
        public const string Pages_Administration_Host_Invoices_Period_Delete = "Pages.Administration.Host.Invoices.Period.Delete";
        public const string Pages_Administration_Host_Invoices_Period_Enabled = "Pages.Administration.Host.Invoices.Period.Enbaled";

        public const string Pages_Administration_Host_Invoices_PaymentMethods = "Pages.Administration.Host.Invoices.PaymentMethods";
        public const string Pages_Administration_Host_Invoices_PaymentMethod_Create = "Pages.Administration.Host.Invoices.PaymentMethod.Create";
        public const string Pages_Administration_Host_Invoices_PaymentMethod_Edit = "Pages.Administration.Host.Invoices.PaymentMethod.Edit";
        public const string Pages_Administration_Host_Invoices_PaymentMethod_Delete = "Pages.Administration.Host.Invoices.PaymentMethod.Delete";


        public const string Pages_Invoices = "Pages.Invoices";
        public const string Pages_Administration_Host_Invoices_Delete = "Pages.Administration.Host.Invoices.Invoices.Delete";
        public const string Pages_Administration_Host_Invoices_MakePaid = "Pages.Administration.Host.Invoices.MakePaid";
        public const string Pages_Administration_Host_Invoices_MakeUnPaid = "Pages.Administration.Host.Invoices.MakeUnPaid";


        public const string Pages_Administration_Host_Invoices_Balances = "Pages.Administration.Host.Invoices.Balances";
        public const string Pages_Administration_Host_Invoices_Balances_Create = "Pages.Administration.Host.Invoices.Balances.Create";
        public const string Pages_Administration_Host_Invoices_Balances_Delete = "Pages.Administration.Host.Invoices_Balances.Delete";


        public const string Pages_Invoices_SubmitInvoices = "Pages.Invoices.SubmitInvoices";
        public const string Pages_Invoices_SubmitInvoices_Claim = "Pages.Invoices.SubmitInvoices.Claim";

        public const string Pages_Administration_Host_Invoices_SubmitInvoices_Delete = "Pages.Administration.Host.Invoices.SubmitInvoices.Delete";
        public const string Pages_Administration_Host_Invoices_SubmitInvoices_Accepted = "Pages.Administration.Host.Invoices.SubmitInvoices.Accepted";
        public const string Pages_Administration_Host_Invoices_SubmitInvoices_Rejected = "Pages.Administration.Host.Invoices.SubmitInvoices.Rejected";

        public const string Pages_Invoices_Transaction = "Pages.Invoices.Transaction";

        #region shpping

        public const string Pages_Tracking = "Pages.shipment.Tracking";
        public const string Pages_Tracking_ReceiverCode = "Pages.Shipment.Tracking.ReceiverCode";


        #endregion
    }
}