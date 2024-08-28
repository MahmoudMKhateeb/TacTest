using Abp.Application.Features;
using Abp.Localization;
using Abp.Webhooks;
using TACHYON.Features;

namespace TACHYON.WebHooks
{
    public class AppWebhookDefinitionProvider : WebhookDefinitionProvider
    {
        public override void SetWebhooks(IWebhookDefinitionContext context)
        {
            context.Manager.Add(new WebhookDefinition(
                name: AppWebHookNames.TestWebhook
            ));

            //Add your webhook definitions here 

            context.Manager.Add(new WebhookDefinition(
                name: AppWebHookNames.NewTripCreated,
                displayName: L("NewTripCreatedWebhookDefinition"),
                description: L("DescriptionNewTripCreatedWebhookDefinition"),
                featureDependency: new SimpleFeatureDependency(AppFeatures.CreateDirectTrip)
                ));

            context.Manager.Add(new WebhookDefinition(
                name: AppWebHookNames.DeliveredTripUpdated,
                displayName: L("DeliveredTripUpdatedWebhookDefinition"),
                description: L("DescriptionDeliveredTripUpdatedWebhookDefinition"),
                featureDependency: new SimpleFeatureDependency(AppFeatures.CreateDirectTrip)
            ));

            context.Manager.Add(new WebhookDefinition(
                name: AppWebHookNames.DriverCreated,
                displayName: L("DriverCreatedWebhookDefinition"),
                description: L("DescriptionDriverCreatedWebhookDefinition"),
                featureDependency: new SimpleFeatureDependency(AppFeatures.Sab)
            ));

            context.Manager.Add(new WebhookDefinition(
               name: AppWebHookNames.DriverUpdated,
               displayName: L("DriverUpdatedWebhookDefinition"),
               description: L("DescriptionDriverUpdatedWebhookDefinition"),
               featureDependency: new SimpleFeatureDependency(AppFeatures.Sab)
           ));


           context.Manager.Add(new WebhookDefinition(
                name: AppWebHookNames.TruckCreated,
                displayName: L("TruckCreatedWebhookDefinition"),
                description: L("DescriptionTruckCreatedWebhookDefinition"),
                featureDependency: new SimpleFeatureDependency(AppFeatures.Sab)
            ));

            context.Manager.Add(new WebhookDefinition(
               name: AppWebHookNames.TruckUpdated,
               displayName: L("TruckUpdatedWebhookDefinition"),
               description: L("DescriptionTruckUpdatedWebhookDefinition"),
               featureDependency: new SimpleFeatureDependency(AppFeatures.Sab)
           ));

        }

        private static ILocalizableString L(string name)
        {
            return new LocalizableString(name, TACHYONConsts.LocalizationSourceName);
        }
    }
}