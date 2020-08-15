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

            #endregion
        }

        private static ILocalizableString L(string name)
        {
            return new LocalizableString(name, TACHYONConsts.LocalizationSourceName);
        }
    }
}