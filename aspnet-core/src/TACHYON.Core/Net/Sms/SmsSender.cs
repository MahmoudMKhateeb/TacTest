using Abp.Dependency;
using Castle.Core.Logging;
using System.Threading.Tasks;
using Unifonic;
using Abp.Localization;
using Abp.Runtime.Session;
using System;
using Abp.Configuration;
using TACHYON.Configuration;

namespace TACHYON.Net.Sms
{
    public class SmsSender : ISmsSender, ITransientDependency
    {
        public ILogger Logger { get; set; }
        private readonly ILocalizationContext _localizationContext;
        private readonly ISettingManager _settingManager;


        public SmsSender(ILocalizationContext localizationContext, ISettingManager settingManager)
        {
            Logger = NullLogger.Instance;
            _localizationContext = localizationContext;
            _settingManager = settingManager;
        }

        public Task SendAsync(string number, string message)
        {
            /* Implement this service to send SMS to users (can be used for two factor auth). */

            //Logger.Warn("Sending SMS is not implemented! Message content:");
            //Logger.Warn("Number  : " + number);
            //Logger.Warn("Message : " + message);

            return Task.FromResult(0);
        }

        public async Task SendReceiverSmsAsync(string number, DateTime date, string shipperName, string driverName, string driverPhone, string waybillNumber, string code, string link)
        {
            
            var l1 = L("AShipmentWasAssignedForYouToReceiveOn").Localize(_localizationContext) + ": "+ date.ToString("MM/dd/yyyy");
            var l2 = "\n" +L("ShipperName").Localize(_localizationContext) +": "+ shipperName;
            var l3 = "\n" +L("DriverName").Localize(_localizationContext) +": "+ driverName;
            var l4 = "\n" +L("PhoneNumber").Localize(_localizationContext) + ": "+ driverPhone;
            var l5 = "\n" +L("WaybillNumber").Localize(_localizationContext) + ": "+ waybillNumber;
            var l6 = "\n" +L("VerificationCode").Localize(_localizationContext) + ": "+ code;
            var l7 = "\n" +L("PleaseOnClickFollowingLinkToTrackYourShipmentAndRateTheShippingCompany").Localize(_localizationContext) +": "+ link;

            var unifonicClint = new UnifonicRestClient( await _settingManager.GetSettingValueAsync(AppSettings.Sms.UnifonicAppSid));

            unifonicClint.SendSmsMessage(number, l1 + l2 + l3 + l4 + l5 + l6 + l7);

        }

        private static ILocalizableString L(string message)
        {
            return new LocalizableString(message, TACHYONConsts.LocalizationSourceName);
        }

    }
}