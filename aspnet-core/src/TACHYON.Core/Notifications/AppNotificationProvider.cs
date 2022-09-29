using Abp.Application.Features;
using Abp.Authorization;
using Abp.Localization;
using Abp.Notifications;
using TACHYON.Authorization;
using TACHYON.Features;
using TACHYON.Invoices;
using TACHYON.Invoices.SubmitInvoices;
using TACHYON.PriceOffers;
using TACHYON.Shipping.ShippingRequests;
using TACHYON.Shipping.ShippingRequestTrips;

namespace TACHYON.Notifications
{
    public class AppNotificationProvider : NotificationProvider
    {
        public override void SetNotifications(INotificationDefinitionContext context)
        {
            context.Manager.Add(
                new NotificationDefinition(
                    AppNotificationNames.NewUserRegistered,
                    displayName: L("NewUserRegisteredNotificationDefinition"),
                    permissionDependency: new SimplePermissionDependency(AppPermissions.Pages_Administration_Users)
                )
            );

            context.Manager.Add(
                new NotificationDefinition(
                    AppNotificationNames.NewTenantRegistered,
                    displayName: L("NewTenantRegisteredNotificationDefinition"),
                    permissionDependency: new SimplePermissionDependency(AppPermissions.Pages_Tenants)
                )
            );

            #region Tychon notifications

            context.Manager.Add(
                new NotificationDefinition(
                    AppNotificationNames.AssignDriverToTruck,
                    displayName: L("AssignDriverToTruckNotificationDefinition"),
                    featureDependency: new SimpleFeatureDependency(AppFeatures.Carrier)
                )
            );

            context.Manager.Add(
                new NotificationDefinition(
                    AppNotificationNames.UpdateShippingRequestPrice,
                    displayName: L("UpdateShippingRequestPriceNotificationDefinition"),
                    featureDependency: new SimpleFeatureDependency(AppFeatures.Shipper)
                )
            );

            context.Manager.Add(
                new NotificationDefinition(
                    AppNotificationNames.AcceptShippingRequestPrice,
                    displayName: L("AcceptShippingRequestPriceNotificationDefinition"),
                    featureDependency: new SimpleFeatureDependency(AppFeatures.TachyonDealer)
                )
            );

            context.Manager.Add(
                new NotificationDefinition(
                    AppNotificationNames.RejectShippingRequest,
                    displayName: L("RejectShippingRequestNotificationDefinition"),
                    featureDependency: new SimpleFeatureDependency(AppFeatures.Shipper)
                )
            );
            context.Manager.Add(
                new NotificationDefinition(
                    AppNotificationNames.CreateShippingRequestBid,
                    displayName: L("CreateShippingRequestBidNotificationDefinition"),
                    featureDependency: new SimpleFeatureDependency(AppFeatures.Carrier)
                )
            );
            context.Manager.Add(
                new NotificationDefinition(
                    AppNotificationNames.UpdateShippingRequestBid,
                    displayName: L("UpdateShippingRequestBidNotificationDefinition"),
                    featureDependency: new SimpleFeatureDependency(AppFeatures.Carrier)
                )
            );
            context.Manager.Add(
                new NotificationDefinition(
                    AppNotificationNames.CancelShippingRequestBid,
                    displayName: L("CancelShippingRequestBidNotificationDefinition"),
                    featureDependency: new SimpleFeatureDependency(AppFeatures.Carrier)
                )
            );
            context.Manager.Add(
                new NotificationDefinition(
                    AppNotificationNames.AcceptShippingRequestBid,
                    displayName: L("AcceptShippingRequestBidNotificationDefinition"),
                    featureDependency: new SimpleFeatureDependency(AppFeatures.Carrier)
                )
            );

            context.Manager.Add(
                new NotificationDefinition(
                    AppNotificationNames.ShippingRequestAsBidWithSameTruck,
                    displayName: L("ShippingRequestAsBidWithSameTruckNotificationDefinition"),
                    featureDependency: new SimpleFeatureDependency(AppFeatures.Carrier)
                )
            );

            context.Manager.Add(
                new NotificationDefinition(
                    AppNotificationNames.TachyonDealOfferCreated,
                    displayName: L("TachyonDealerOfferCreatedNotificationDefinition"),
                    featureDependency: new SimpleFeatureDependency(AppFeatures.Carrier)
                )
            );
            context.Manager.Add(
                new NotificationDefinition(
                    AppNotificationNames.DocumentFileBeforExpiration,
                    displayName: L("DocumentFileBeforExpirationNotificationDefinition")
                )
            );

            context.Manager.Add(
                new NotificationDefinition(
                    AppNotificationNames.DocumentFileExpiration,
                    displayName: L("DocumentFileExpirationNotificationDefinition")
                )
            );

            context.Manager.Add(
                new NotificationDefinition(
                    AppNotificationNames.TenantDocumentFileUpdate,
                    displayName: L("TenantDocumentFileUpdateNotificationDefinition"),
                    permissionDependency: new SimplePermissionDependency(AppPermissions.Pages_Tenants)
                )
            );

            //context.Manager.Add(
            //    new NotificationDefinition(
            //        ))


            #region Invoices

            #region Shipper

            context.Manager.Add(
                new NotificationDefinition(
                    AppNotificationNames.InvoiceShipperGenerated,
                    displayName: L("NewInvoiceShipperGenerated"),
                    permissionDependency: new SimplePermissionDependency(AppFeatures.Shipper)
                )
            );

            context.Manager.Add(
                new NotificationDefinition(
                    AppNotificationNames.ShipperNotfiyWhenCreditLimitGreaterOrEqualXPercentage,
                    displayName: L("ShipperNotfiyWhenCreditLimitGreaterOrEqualXPercentage"),
                    permissionDependency: new SimplePermissionDependency(AppFeatures.Shipper)
                )
            );
            context.Manager.Add(
                new NotificationDefinition(
                    AppNotificationNames.StartShippment,
                    displayName: L("StartShippmentNotificationDefinition")
                )
            );
            context.Manager.Add(
                new NotificationDefinition(
                    AppNotificationNames.ShipperShippingRequestFinish,
                    displayName: L("ShipperShippingRequestFinishNotificationDefinition")
                )
            );

            #endregion

            #region Host

            #endregion

            #endregion

            #region Shipping Tracking Notifications

            context.Manager.Add(new NotificationDefinition(
                AppNotificationNames.DriverAcceptTrip,
                displayName: L("DriverAcceptTripNotificationDefinition"),
                featureDependency: new SimpleFeatureDependency(AppFeatures.Carrier)
            ));

            #endregion

            #region Offers Notifications

            context.Manager.Add(new NotificationDefinition(
                AppNotificationNames.TMSAcceptedOffer,
                displayName: L("TachyonDealerAcceptPriceOfferNotificationDefinition")));    
            
            context.Manager.Add(new NotificationDefinition(
                AppNotificationNames.ShipperAcceptedOffer,
                displayName: L("ShipperAcceptPriceOfferNotificationDefinition")));
            
            context.Manager.Add(new NotificationDefinition(
                AppNotificationNames.PendingOffer,
                displayName: L("PendingPriceOfferNotificationDefinition"),
                description: L("PendingOffer"),
                featureDependency: new SimpleFeatureDependency(AppFeatures.Carrier)));
            
            context.Manager.Add(new NotificationDefinition(
                AppNotificationNames.RejectedOffer,
                displayName: L("RejectedPriceOfferNotificationDefinition"),
                description: L("RejectedOffer"),
                featureDependency: new SimpleFeatureDependency(AppFeatures.TachyonDealer,AppFeatures.Carrier)));
            
            context.Manager.Add(new NotificationDefinition(
                AppNotificationNames.RejectedPostPriceOffer,
                displayName: L("RejectedPostPriceOfferNotificationDefinition"),
                description: L("RejectedOffer"),
                featureDependency: new SimpleFeatureDependency(AppFeatures.Carrier)));
            
            context.Manager.Add(new NotificationDefinition(
                AppNotificationNames.ShippingRequestSendOfferWhenAddPrice,
                displayName: L("SendOfferWhenAddPriceNotificationDefinition"),
                description: L("ShippingRequestSendOfferWhenAddPrice"),
                featureDependency: new SimpleFeatureDependency(AppFeatures.TachyonDealer,AppFeatures.Shipper)));
            
            context.Manager.Add(new NotificationDefinition(
                AppNotificationNames.ShippingRequestSendOfferWhenUpdatePrice,
                 displayName: L("SendOfferWhenUpdatePriceNotificationDefinition"),
                description: L("ShippingRequestSendOfferWhenUpdatePrice"),
                featureDependency: new SimpleFeatureDependency(AppFeatures.TachyonDealer,AppFeatures.Shipper)));

            #endregion

            #region Invoicing Notifications

            context.Manager.Add(new NotificationDefinition(AppNotificationNames.SubmitInvoiceOnClaim,
                 displayName: L("SubmitInvoiceOnClaimNotificationDefinition"),
                 permissionDependency: new SimplePermissionDependency(AppPermissions.Pages_Invoices_SubmitInvoices_Claim)));
            
            context.Manager.Add(new NotificationDefinition(AppNotificationNames.SubmitInvoiceOnAccepted,
                 displayName: L("SubmitInvoiceOnAcceptedNotificationDefinition"),
                 permissionDependency: new SimplePermissionDependency(AppPermissions.Pages_Invoices_SubmitInvoices)));
            
            context.Manager.Add(new NotificationDefinition(AppNotificationNames.SubmitInvoiceOnRejected,
                displayName: L("SubmitInvoiceOnRejectedNotificationDefinition"),
                permissionDependency: new SimplePermissionDependency(AppPermissions.Pages_Invoices_SubmitInvoices)));

            #endregion
            
            
            #endregion
        }

        private static ILocalizableString L(string name)
        {
            return new LocalizableString(name, TACHYONConsts.LocalizationSourceName);
        }
    }
}