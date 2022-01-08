using Abp.Authorization;
using Abp.Configuration.Startup;
using Abp.Localization;
using Abp.MultiTenancy;
using TACHYON.Authorization.Permissions.Shipping.Trips;

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

            var driverLicenseTypes = pages.CreateChildPermission(AppPermissions.Pages_DriverLicenseTypes, L("DriverLicenseTypes"), multiTenancySides: MultiTenancySides.Host);
            driverLicenseTypes.CreateChildPermission(AppPermissions.Pages_DriverLicenseTypes_Create, L("CreateNewDriverLicenseType"), multiTenancySides: MultiTenancySides.Host);
            driverLicenseTypes.CreateChildPermission(AppPermissions.Pages_DriverLicenseTypes_Edit, L("EditDriverLicenseType"), multiTenancySides: MultiTenancySides.Host);
            driverLicenseTypes.CreateChildPermission(AppPermissions.Pages_DriverLicenseTypes_Delete, L("DeleteDriverLicenseType"), multiTenancySides: MultiTenancySides.Host);



            var dangerousGoodTypes = pages.CreateChildPermission(AppPermissions.Pages_DangerousGoodTypes, L("DangerousGoodTypes"), multiTenancySides: MultiTenancySides.Host);
            dangerousGoodTypes.CreateChildPermission(AppPermissions.Pages_DangerousGoodTypes_Create, L("CreateNewDangerousGoodType"), multiTenancySides: MultiTenancySides.Host);
            dangerousGoodTypes.CreateChildPermission(AppPermissions.Pages_DangerousGoodTypes_Edit, L("EditDangerousGoodType"), multiTenancySides: MultiTenancySides.Host);
            dangerousGoodTypes.CreateChildPermission(AppPermissions.Pages_DangerousGoodTypes_Delete, L("DeleteDangerousGoodType"), multiTenancySides: MultiTenancySides.Host);



            var receivers = pages.CreateChildPermission(AppPermissions.Pages_Receivers, L("Receivers"), multiTenancySides: MultiTenancySides.Tenant);
            receivers.CreateChildPermission(AppPermissions.Pages_Receivers_Create, L("CreateNewReceiver"), multiTenancySides: MultiTenancySides.Tenant);
            receivers.CreateChildPermission(AppPermissions.Pages_Receivers_Edit, L("EditReceiver"), multiTenancySides: MultiTenancySides.Tenant);
            receivers.CreateChildPermission(AppPermissions.Pages_Receivers_Delete, L("DeleteReceiver"), multiTenancySides: MultiTenancySides.Tenant);

            var citiesTranslations = pages.CreateChildPermission(AppPermissions.Pages_CitiesTranslations, L("CitiesTranslations"), multiTenancySides: MultiTenancySides.Host);
            citiesTranslations.CreateChildPermission(AppPermissions.Pages_CitiesTranslations_Create, L("CreateNewCitiesTranslation"), multiTenancySides: MultiTenancySides.Host);
            citiesTranslations.CreateChildPermission(AppPermissions.Pages_CitiesTranslations_Edit, L("EditCitiesTranslation"), multiTenancySides: MultiTenancySides.Host);
            citiesTranslations.CreateChildPermission(AppPermissions.Pages_CitiesTranslations_Delete, L("DeleteCitiesTranslation"), multiTenancySides: MultiTenancySides.Host);




            var packingTypes = pages.CreateChildPermission(AppPermissions.Pages_PackingTypes, L("PackingTypes"), multiTenancySides: MultiTenancySides.Host);
            packingTypes.CreateChildPermission(AppPermissions.Pages_PackingTypes_Create, L("CreateNewPackingType"), multiTenancySides: MultiTenancySides.Host);
            packingTypes.CreateChildPermission(AppPermissions.Pages_PackingTypes_Edit, L("EditPackingType"), multiTenancySides: MultiTenancySides.Host);
            packingTypes.CreateChildPermission(AppPermissions.Pages_PackingTypes_Delete, L("DeletePackingType"), multiTenancySides: MultiTenancySides.Host);

            var shippingTypes = pages.CreateChildPermission(AppPermissions.Pages_ShippingTypes, L("ShippingTypes"), multiTenancySides: MultiTenancySides.Host);
            shippingTypes.CreateChildPermission(AppPermissions.Pages_ShippingTypes_Create, L("CreateNewShippingType"), multiTenancySides: MultiTenancySides.Host);
            shippingTypes.CreateChildPermission(AppPermissions.Pages_ShippingTypes_Edit, L("EditShippingType"), multiTenancySides: MultiTenancySides.Host);
            shippingTypes.CreateChildPermission(AppPermissions.Pages_ShippingTypes_Delete, L("DeleteShippingType"), multiTenancySides: MultiTenancySides.Host);

            var truckCapacitiesTranslations = pages.CreateChildPermission(AppPermissions.Pages_TruckCapacitiesTranslations, L("TruckCapacitiesTranslations"), multiTenancySides: MultiTenancySides.Host);
            truckCapacitiesTranslations.CreateChildPermission(AppPermissions.Pages_TruckCapacitiesTranslations_Create, L("CreateNewTruckCapacitiesTranslation"), multiTenancySides: MultiTenancySides.Host);
            truckCapacitiesTranslations.CreateChildPermission(AppPermissions.Pages_TruckCapacitiesTranslations_Edit, L("EditTruckCapacitiesTranslation"), multiTenancySides: MultiTenancySides.Host);
            truckCapacitiesTranslations.CreateChildPermission(AppPermissions.Pages_TruckCapacitiesTranslations_Delete, L("DeleteTruckCapacitiesTranslation"), multiTenancySides: MultiTenancySides.Host);

            var truckStatusesTranslations = pages.CreateChildPermission(AppPermissions.Pages_TruckStatusesTranslations, L("TruckStatusesTranslations"), multiTenancySides: MultiTenancySides.Host);
            truckStatusesTranslations.CreateChildPermission(AppPermissions.Pages_TruckStatusesTranslations_Create, L("CreateNewTruckStatusesTranslation"), multiTenancySides: MultiTenancySides.Host);
            truckStatusesTranslations.CreateChildPermission(AppPermissions.Pages_TruckStatusesTranslations_Edit, L("EditTruckStatusesTranslation"), multiTenancySides: MultiTenancySides.Host);
            truckStatusesTranslations.CreateChildPermission(AppPermissions.Pages_TruckStatusesTranslations_Delete, L("DeleteTruckStatusesTranslation"), multiTenancySides: MultiTenancySides.Host);


            var countriesTranslations = pages.CreateChildPermission(AppPermissions.Pages_CountriesTranslations, L("CountriesTranslations"), multiTenancySides: MultiTenancySides.Host);
            countriesTranslations.CreateChildPermission(AppPermissions.Pages_CountriesTranslations_Create, L("CreateNewCountriesTranslation"), multiTenancySides: MultiTenancySides.Host);
            countriesTranslations.CreateChildPermission(AppPermissions.Pages_CountriesTranslations_Edit, L("EditCountriesTranslation"), multiTenancySides: MultiTenancySides.Host);
            countriesTranslations.CreateChildPermission(AppPermissions.Pages_CountriesTranslations_Delete, L("DeleteCountriesTranslation"), multiTenancySides: MultiTenancySides.Host);

            var plateTypes = pages.CreateChildPermission(AppPermissions.Pages_PlateTypes, L("PlateTypes"), multiTenancySides: MultiTenancySides.Host);
            plateTypes.CreateChildPermission(AppPermissions.Pages_PlateTypes_Create, L("CreateNewPlateType"), multiTenancySides: MultiTenancySides.Host);
            plateTypes.CreateChildPermission(AppPermissions.Pages_PlateTypes_Edit, L("EditPlateType"), multiTenancySides: MultiTenancySides.Host);
            plateTypes.CreateChildPermission(AppPermissions.Pages_PlateTypes_Delete, L("DeletePlateType"), multiTenancySides: MultiTenancySides.Host);

            var nationalities = pages.CreateChildPermission(AppPermissions.Pages_Nationalities, L("Nationalities"), multiTenancySides: MultiTenancySides.Host);
            nationalities.CreateChildPermission(AppPermissions.Pages_Nationalities_Create, L("CreateNewNationality"), multiTenancySides: MultiTenancySides.Host);
            nationalities.CreateChildPermission(AppPermissions.Pages_Nationalities_Edit, L("EditNationality"), multiTenancySides: MultiTenancySides.Host);
            nationalities.CreateChildPermission(AppPermissions.Pages_Nationalities_Delete, L("DeleteNationality"), multiTenancySides: MultiTenancySides.Host);

            var nationalityTranslations = pages.CreateChildPermission(AppPermissions.Pages_NationalityTranslations, L("NationalityTranslations"), multiTenancySides: MultiTenancySides.Host);
            nationalityTranslations.CreateChildPermission(AppPermissions.Pages_NationalityTranslations_Create, L("CreateNewNationalityTranslation"), multiTenancySides: MultiTenancySides.Host);
            nationalityTranslations.CreateChildPermission(AppPermissions.Pages_NationalityTranslations_Edit, L("EditNationalityTranslation"), multiTenancySides: MultiTenancySides.Host);
            nationalityTranslations.CreateChildPermission(AppPermissions.Pages_NationalityTranslations_Delete, L("DeleteNationalityTranslation"), multiTenancySides: MultiTenancySides.Host);

            var trucksTypesTranslations = pages.CreateChildPermission(AppPermissions.Pages_TrucksTypesTranslations, L("TrucksTypesTranslations"), multiTenancySides: MultiTenancySides.Host);
            trucksTypesTranslations.CreateChildPermission(AppPermissions.Pages_TrucksTypesTranslations_Create, L("CreateNewTrucksTypesTranslation"), multiTenancySides: MultiTenancySides.Host);
            trucksTypesTranslations.CreateChildPermission(AppPermissions.Pages_TrucksTypesTranslations_Edit, L("EditTrucksTypesTranslation"), multiTenancySides: MultiTenancySides.Host);
            trucksTypesTranslations.CreateChildPermission(AppPermissions.Pages_TrucksTypesTranslations_Delete, L("DeleteTrucksTypesTranslation"), multiTenancySides: MultiTenancySides.Host);

            var transportTypesTranslations = pages.CreateChildPermission(AppPermissions.Pages_TransportTypesTranslations, L("TransportTypesTranslations"), multiTenancySides: MultiTenancySides.Host);
            transportTypesTranslations.CreateChildPermission(AppPermissions.Pages_TransportTypesTranslations_Create, L("CreateNewTransportTypesTranslation"), multiTenancySides: MultiTenancySides.Host);
            transportTypesTranslations.CreateChildPermission(AppPermissions.Pages_TransportTypesTranslations_Edit, L("EditTransportTypesTranslation"), multiTenancySides: MultiTenancySides.Host);
            transportTypesTranslations.CreateChildPermission(AppPermissions.Pages_TransportTypesTranslations_Delete, L("DeleteTransportTypesTranslation"), multiTenancySides: MultiTenancySides.Host);

            var shippingRequestVases = pages.CreateChildPermission(AppPermissions.Pages_ShippingRequestVases, L("ShippingRequestVases"), multiTenancySides: MultiTenancySides.Tenant);
            shippingRequestVases.CreateChildPermission(AppPermissions.Pages_ShippingRequestVases_Create, L("CreateNewShippingRequestVas"), multiTenancySides: MultiTenancySides.Tenant);
            shippingRequestVases.CreateChildPermission(AppPermissions.Pages_ShippingRequestVases_Edit, L("EditShippingRequestVas"), multiTenancySides: MultiTenancySides.Tenant);
            shippingRequestVases.CreateChildPermission(AppPermissions.Pages_ShippingRequestVases_Delete, L("DeleteShippingRequestVas"), multiTenancySides: MultiTenancySides.Tenant);

            var vasPrices = pages.CreateChildPermission(AppPermissions.Pages_VasPrices, L("VasPrices"), multiTenancySides: MultiTenancySides.Tenant);
            vasPrices.CreateChildPermission(AppPermissions.Pages_VasPrices_Create, L("CreateNewVasPrice"), multiTenancySides: MultiTenancySides.Tenant);
            vasPrices.CreateChildPermission(AppPermissions.Pages_VasPrices_Edit, L("EditVasPrice"), multiTenancySides: MultiTenancySides.Tenant);
            vasPrices.CreateChildPermission(AppPermissions.Pages_VasPrices_Delete, L("DeleteVasPrice"), multiTenancySides: MultiTenancySides.Tenant);

            var capacities = pages.CreateChildPermission(AppPermissions.Pages_Capacities, L("Capacities"), multiTenancySides: MultiTenancySides.Host);
            capacities.CreateChildPermission(AppPermissions.Pages_Capacities_Create, L("CreateNewCapacity"), multiTenancySides: MultiTenancySides.Host);
            capacities.CreateChildPermission(AppPermissions.Pages_Capacities_Edit, L("EditCapacity"), multiTenancySides: MultiTenancySides.Host);
            capacities.CreateChildPermission(AppPermissions.Pages_Capacities_Delete, L("DeleteCapacity"), multiTenancySides: MultiTenancySides.Host);

            var transportTypes = pages.CreateChildPermission(AppPermissions.Pages_TransportTypes, L("TransportTypes"), multiTenancySides: MultiTenancySides.Host);
            transportTypes.CreateChildPermission(AppPermissions.Pages_TransportTypes_Create, L("CreateNewTransportType"), multiTenancySides: MultiTenancySides.Host);
            transportTypes.CreateChildPermission(AppPermissions.Pages_TransportTypes_Edit, L("EditTransportType"), multiTenancySides: MultiTenancySides.Host);
            transportTypes.CreateChildPermission(AppPermissions.Pages_TransportTypes_Delete, L("DeleteTransportType"), multiTenancySides: MultiTenancySides.Host);

            var documentTypeTranslations = pages.CreateChildPermission(AppPermissions.Pages_DocumentTypeTranslations, L("DocumentTypeTranslations"), multiTenancySides: MultiTenancySides.Host);
            documentTypeTranslations.CreateChildPermission(AppPermissions.Pages_DocumentTypeTranslations_Create, L("CreateNewDocumentTypeTranslation"), multiTenancySides: MultiTenancySides.Host);
            documentTypeTranslations.CreateChildPermission(AppPermissions.Pages_DocumentTypeTranslations_Edit, L("EditDocumentTypeTranslation"), multiTenancySides: MultiTenancySides.Host);
            documentTypeTranslations.CreateChildPermission(AppPermissions.Pages_DocumentTypeTranslations_Delete, L("DeleteDocumentTypeTranslation"), multiTenancySides: MultiTenancySides.Host);

            var documentsEntities = pages.CreateChildPermission(AppPermissions.Pages_DocumentsEntities, L("DocumentsEntities"), multiTenancySides: MultiTenancySides.Host);
            documentsEntities.CreateChildPermission(AppPermissions.Pages_DocumentsEntities_Create, L("CreateNewDocumentsEntity"), multiTenancySides: MultiTenancySides.Host);
            documentsEntities.CreateChildPermission(AppPermissions.Pages_DocumentsEntities_Edit, L("EditDocumentsEntity"), multiTenancySides: MultiTenancySides.Host);
            documentsEntities.CreateChildPermission(AppPermissions.Pages_DocumentsEntities_Delete, L("DeleteDocumentsEntity"), multiTenancySides: MultiTenancySides.Host);

            var ports = pages.CreateChildPermission(AppPermissions.Pages_Ports, L("Ports"), multiTenancySides: MultiTenancySides.Host);
            ports.CreateChildPermission(AppPermissions.Pages_Ports_Create, L("CreateNewPort"), multiTenancySides: MultiTenancySides.Host);
            ports.CreateChildPermission(AppPermissions.Pages_Ports_Edit, L("EditPort"), multiTenancySides: MultiTenancySides.Host);
            ports.CreateChildPermission(AppPermissions.Pages_Ports_Delete, L("DeletePort"), multiTenancySides: MultiTenancySides.Host);

            var pickingTypes = pages.CreateChildPermission(AppPermissions.Pages_PickingTypes, L("PickingTypes"), multiTenancySides: MultiTenancySides.Host);
            pickingTypes.CreateChildPermission(AppPermissions.Pages_PickingTypes_Create, L("CreateNewPickingType"), multiTenancySides: MultiTenancySides.Host);
            pickingTypes.CreateChildPermission(AppPermissions.Pages_PickingTypes_Edit, L("EditPickingType"), multiTenancySides: MultiTenancySides.Host);
            pickingTypes.CreateChildPermission(AppPermissions.Pages_PickingTypes_Delete, L("DeletePickingType"), multiTenancySides: MultiTenancySides.Host);

            var facilities = pages.CreateChildPermission(AppPermissions.Pages_Facilities, L("Facilities"));
            facilities.CreateChildPermission(AppPermissions.Pages_Facilities_Create, L("CreateNewFacility"));
            facilities.CreateChildPermission(AppPermissions.Pages_Facilities_Edit, L("EditFacility"));
            facilities.CreateChildPermission(AppPermissions.Pages_Facilities_Delete, L("DeleteFacility"));

            var shipper = pages.CreateChildPermission(AppPermissions.App_Shipper, L("Shipper"));
            var carrier = pages.CreateChildPermission(AppPermissions.App_Carrier, L("Carrier"));

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

            // new AppAuthorizationTripsProvider(pages);

            var ShippingRequestBis = pages.CreateChildPermission(AppPermissions.Pages_ShippingRequestBids, L("ShippingRerquestBids"), multiTenancySides: MultiTenancySides.Tenant);
            ShippingRequestBis.CreateChildPermission(AppPermissions.Pages_ShippingRequestBids_Create, L("CreateNewShippingRequestBid"), multiTenancySides: MultiTenancySides.Tenant);
            ShippingRequestBis.CreateChildPermission(AppPermissions.Pages_ShippingRequestBids_Edit, L("EditNewShippingRequestBid"), multiTenancySides: MultiTenancySides.Tenant);
            ShippingRequestBis.CreateChildPermission(AppPermissions.Pages_ShippingRequestBids_Delete, L("DeleteNewShippingRequestBid"), multiTenancySides: MultiTenancySides.Tenant);

            var goodsDetails = pages.CreateChildPermission(AppPermissions.Pages_GoodsDetails, L("GoodsDetails"), multiTenancySides: MultiTenancySides.Tenant);
            goodsDetails.CreateChildPermission(AppPermissions.Pages_GoodsDetails_Create, L("CreateNewGoodsDetail"), multiTenancySides: MultiTenancySides.Tenant);
            goodsDetails.CreateChildPermission(AppPermissions.Pages_GoodsDetails_Edit, L("EditGoodsDetail"), multiTenancySides: MultiTenancySides.Tenant);
            goodsDetails.CreateChildPermission(AppPermissions.Pages_GoodsDetails_Delete, L("DeleteGoodsDetail"), multiTenancySides: MultiTenancySides.Tenant);

            //var offers = pages.CreateChildPermission(AppPermissions.Pages_Offers, L("Offers"), multiTenancySides: MultiTenancySides.Tenant);
            //offers.CreateChildPermission(AppPermissions.Pages_Offers_Create, L("CreateNewOffer"), multiTenancySides: MultiTenancySides.Tenant);
            //offers.CreateChildPermission(AppPermissions.Pages_Offers_Edit, L("EditOffer"), multiTenancySides: MultiTenancySides.Tenant);
            //offers.CreateChildPermission(AppPermissions.Pages_Offers_Delete, L("DeleteOffer"), multiTenancySides: MultiTenancySides.Tenant);

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

            var routPoints = pages.CreateChildPermission(AppPermissions.Pages_RoutPoints, L("RoutPoints"), multiTenancySides: MultiTenancySides.Tenant);
            routPoints.CreateChildPermission(AppPermissions.Pages_RoutPoints_Create, L("CreateRoutPoint"), multiTenancySides: MultiTenancySides.Tenant);
            routPoints.CreateChildPermission(AppPermissions.Pages_RoutPoints_Edit, L("EditRoutPoint"), multiTenancySides: MultiTenancySides.Tenant);
            routPoints.CreateChildPermission(AppPermissions.Pages_RoutPoints_Delete, L("DeleteRoutPoint"), multiTenancySides: MultiTenancySides.Tenant);

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

            var vases = administration.CreateChildPermission(AppPermissions.Pages_Administration_Vases, L("Vases"), multiTenancySides: MultiTenancySides.Host);
            vases.CreateChildPermission(AppPermissions.Pages_Administration_Vases_Create, L("CreateNewVas"), multiTenancySides: MultiTenancySides.Host);
            vases.CreateChildPermission(AppPermissions.Pages_Administration_Vases_Edit, L("EditVas"), multiTenancySides: MultiTenancySides.Host);
            vases.CreateChildPermission(AppPermissions.Pages_Administration_Vases_Delete, L("DeleteVas"), multiTenancySides: MultiTenancySides.Host);

            var termAndConditionTranslations = administration.CreateChildPermission(AppPermissions.Pages_Administration_TermAndConditionTranslations, L("TermAndConditionTranslations"), multiTenancySides: MultiTenancySides.Host);
            termAndConditionTranslations.CreateChildPermission(AppPermissions.Pages_Administration_TermAndConditionTranslations_Create, L("CreateNewTermAndConditionTranslation"), multiTenancySides: MultiTenancySides.Host);
            termAndConditionTranslations.CreateChildPermission(AppPermissions.Pages_Administration_TermAndConditionTranslations_Edit, L("EditTermAndConditionTranslation"), multiTenancySides: MultiTenancySides.Host);
            termAndConditionTranslations.CreateChildPermission(AppPermissions.Pages_Administration_TermAndConditionTranslations_Delete, L("DeleteTermAndConditionTranslation"), multiTenancySides: MultiTenancySides.Host);

            var shippingRequestStatuses = administration.CreateChildPermission(AppPermissions.Pages_Administration_ShippingRequestStatuses, L("ShippingRequestStatuses"), multiTenancySides: MultiTenancySides.Host);
            shippingRequestStatuses.CreateChildPermission(AppPermissions.Pages_Administration_ShippingRequestStatuses_Create, L("CreateNewShippingRequestStatus"), multiTenancySides: MultiTenancySides.Host);
            shippingRequestStatuses.CreateChildPermission(AppPermissions.Pages_Administration_ShippingRequestStatuses_Edit, L("EditShippingRequestStatus"), multiTenancySides: MultiTenancySides.Host);
            shippingRequestStatuses.CreateChildPermission(AppPermissions.Pages_Administration_ShippingRequestStatuses_Delete, L("DeleteShippingRequestStatus"), multiTenancySides: MultiTenancySides.Host);

            var unitOfMeasures = administration.CreateChildPermission(AppPermissions.Pages_Administration_UnitOfMeasures, L("UnitOfMeasures"), multiTenancySides: MultiTenancySides.Host);
            unitOfMeasures.CreateChildPermission(AppPermissions.Pages_Administration_UnitOfMeasures_Create, L("CreateNewUnitOfMeasure"), multiTenancySides: MultiTenancySides.Host);
            unitOfMeasures.CreateChildPermission(AppPermissions.Pages_Administration_UnitOfMeasures_Edit, L("EditUnitOfMeasure"), multiTenancySides: MultiTenancySides.Host);
            unitOfMeasures.CreateChildPermission(AppPermissions.Pages_Administration_UnitOfMeasures_Delete, L("DeleteUnitOfMeasure"), multiTenancySides: MultiTenancySides.Host);

            var truckStatuses = administration.CreateChildPermission(AppPermissions.Pages_Administration_TruckStatuses, L("TruckStatuses"), multiTenancySides: MultiTenancySides.Host);
            truckStatuses.CreateChildPermission(AppPermissions.Pages_Administration_TruckStatuses_Create, L("CreateNewTruckStatus"), multiTenancySides: MultiTenancySides.Host);
            truckStatuses.CreateChildPermission(AppPermissions.Pages_Administration_TruckStatuses_Edit, L("EditTruckStatus"), multiTenancySides: MultiTenancySides.Host);
            truckStatuses.CreateChildPermission(AppPermissions.Pages_Administration_TruckStatuses_Delete, L("DeleteTruckStatus"), multiTenancySides: MultiTenancySides.Host);

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
            administration.CreateChildPermission(AppPermissions.Pages_Tenant_TMS_Settings, L("TMSSettings"), multiTenancySides: MultiTenancySides.Tenant);

            //HOST-SPECIFIC PERMISSIONS

            var termAndConditions = pages.CreateChildPermission(AppPermissions.Pages_TermAndConditions, L("TermAndConditions"), multiTenancySides: MultiTenancySides.Host);
            termAndConditions.CreateChildPermission(AppPermissions.Pages_TermAndConditions_Create, L("CreateNewTermAndCondition"), multiTenancySides: MultiTenancySides.Host);
            termAndConditions.CreateChildPermission(AppPermissions.Pages_TermAndConditions_Edit, L("EditTermAndCondition"), multiTenancySides: MultiTenancySides.Host);
            termAndConditions.CreateChildPermission(AppPermissions.Pages_TermAndConditions_Delete, L("DeleteTermAndCondition"), multiTenancySides: MultiTenancySides.Host);

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

            var tenantCarrier = tenants.CreateChildPermission(AppPermissions.Pages_TenantCarrier, L("TenantCarrier"), multiTenancySides: MultiTenancySides.Host);
            tenantCarrier.CreateChildPermission(AppPermissions.Pages_TenantCarrier_Create, L("TenantCarrierCreate"), multiTenancySides: MultiTenancySides.Host);
            tenantCarrier.CreateChildPermission(AppPermissions.Pages_TenantCarrier_Delete, L("TenantCarrierDelete"), multiTenancySides: MultiTenancySides.Host);

            administration.CreateChildPermission(AppPermissions.Pages_Administration_Host_Settings, L("Settings"), multiTenancySides: MultiTenancySides.Host);
            administration.CreateChildPermission(AppPermissions.Pages_Administration_Host_Languages, L("Settings"), multiTenancySides: MultiTenancySides.Host);
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