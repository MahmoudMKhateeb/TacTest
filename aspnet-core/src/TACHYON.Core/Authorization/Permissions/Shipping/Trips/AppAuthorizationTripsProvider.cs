using Abp.Application.Features;
using Abp.Authorization;
using Abp.Dependency;
using Abp.MultiTenancy;
using TACHYON.Features;

namespace TACHYON.Authorization.Permissions.Shipping.Trips
{
    public class AppAuthorizationTripsProvider : AppAuthorizationBaseProvider
    {
        public override void SetPermissions(IPermissionDefinitionContext context)
        {
            var pages = context.GetPermissionOrNull(AppPermissions.Pages) ??
                        context.CreatePermission(AppPermissions.Pages, L("Pages"));

            var shippingRequestTrips =
                pages.CreateChildPermission(AppPermissions.Pages_ShippingRequestTrips, L("ShippingRequests"));
            shippingRequestTrips.CreateChildPermission(AppPermissions.Pages_ShippingRequestTrips_Create,
                L("CreateNewShippingRequestTrip"), multiTenancySides: MultiTenancySides.Tenant);
            shippingRequestTrips.CreateChildPermission(AppPermissions.Pages_ShippingRequestTrips_Edit,
                L("EditShippingRequestTrip"), multiTenancySides: MultiTenancySides.Tenant);
            shippingRequestTrips.CreateChildPermission(AppPermissions.Pages_ShippingRequestTrips_Delete,
                L("DeleteShippingRequestTrip"), multiTenancySides: MultiTenancySides.Tenant);
            shippingRequestTrips.CreateChildPermission(AppPermissions.Pages_ShippingRequestTrips_Acident_Cancel,
                L("CancelTrip"), multiTenancySides: MultiTenancySides.Tenant);

            var ResoneAccident = pages.CreateChildPermission(AppPermissions.Pages_ShippingRequestResoneAccidents,
                L("ResoneAccidents"), multiTenancySides: MultiTenancySides.Host);
            ResoneAccident.CreateChildPermission(AppPermissions.Pages_ShippingRequestResoneAccidents_Create,
                L("CreateNewResoneAccident"), multiTenancySides: MultiTenancySides.Host);
            ResoneAccident.CreateChildPermission(AppPermissions.Pages_ShippingRequestResoneAccidents_Edit,
                L("EditResoneAccident"), multiTenancySides: MultiTenancySides.Host);
            ResoneAccident.CreateChildPermission(AppPermissions.Pages_ShippingRequestResoneAccidents_Delete,
                L("DeleteResoneAccident"), multiTenancySides: MultiTenancySides.Host);

            var accidentFeatureDependency = new SimpleFeatureDependency(AppFeatures.TachyonDealer);

            var accident = pages.CreateChildPermission(AppPermissions.Pages_ShippingRequest_Accidents,
                L("ShippingRequestsTripsAccident"), multiTenancySides: MultiTenancySides.Tenant);
            accident.CreateChildPermission(AppPermissions.Pages_ShippingRequest_Accidents_Create,
                L("CreateNewShippingRequestTripAccident"), multiTenancySides: MultiTenancySides.Tenant);
            accident.CreateChildPermission(AppPermissions.Pages_ShippingRequest_Accidents_Edit,
                L("EditShippingRequestTripAccident"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: accidentFeatureDependency);
            
            accident.CreateChildPermission(AppPermissions.Pages_ShippingRequest_Accidents_Resolve_Create,
                L("CreateNewShippingRequestTripAccidentResolve"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: accidentFeatureDependency);
            accident.CreateChildPermission(AppPermissions.Pages_ShippingRequest_Accidents_Resolve_Edit,
                L("EditShippingRequestTripAccidentResolve"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: accidentFeatureDependency);
            
            accident.CreateChildPermission(AppPermissions.Pages_ShippingRequest_Accidents_Resolve_ApproveChange,
                L("ApproveChangeTripAccidentResolve"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency:  new SimpleFeatureDependency(AppFeatures.Shipper,AppFeatures.Carrier));

            accident.CreateChildPermission(AppPermissions.Pages_ShippingRequest_Accidents_Resolve_EnforceChange,
                L("EnforceChangeTripAccidentResolve"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: accidentFeatureDependency);

            var RejectReason = pages.CreateChildPermission(AppPermissions.Pages_ShippingRequestTrips_Reject_Reason,
                L("ShippingRequestsTripsRejectReason"), multiTenancySides: MultiTenancySides.Host);
            RejectReason.CreateChildPermission(AppPermissions.Pages_ShippingRequestTrips_Reject_Reason_Create,
                L("CreateNewShippingRequestTripRejectReason"), multiTenancySides: MultiTenancySides.Host);
            RejectReason.CreateChildPermission(AppPermissions.Pages_ShippingRequestTrips_Reject_Reason_Edit,
                L("EditShippingRequestTripRejectReason"), multiTenancySides: MultiTenancySides.Host);
            RejectReason.CreateChildPermission(AppPermissions.Pages_ShippingRequestTrips_Reject_Reason_Delete,
                L("DeleteShippingRequestTripRejectReason"), multiTenancySides: MultiTenancySides.Host);
        }
    }
}