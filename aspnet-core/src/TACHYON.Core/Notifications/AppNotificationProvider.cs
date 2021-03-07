using Abp.Application.Features;
using Abp.Authorization;
using Abp.Localization;
using Abp.Notifications;
using TACHYON.Authorization;
using TACHYON.Features;

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

            context.Manager.Add(
                new NotificationDefinition(
                    AppNotificationNames.StartShippment,
                    displayName:L("StartShippmentNotificationDefinition")
                    )
                );

            #endregion
        }

        private static ILocalizableString L(string name)
        {
            return new LocalizableString(name, TACHYONConsts.LocalizationSourceName);
        }
    }
}