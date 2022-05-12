using Abp.BackgroundJobs;
using Abp.Configuration;
using Abp.Dependency;
using Abp.Localization;
using Abp.Runtime.Session;
using Castle.Core.Internal;
using Castle.Core.Logging;
using RestSharp;
using System;
using System.Threading.Tasks;
using TACHYON.Configuration;
using TACHYON.Net.Sms.UnifonicSms;

namespace TACHYON.Net.Sms
{
    public class SmsSender : ISmsSender, ITransientDependency
    {
        public ILogger Logger { get; set; }
        private readonly ILocalizationContext _localizationContext;
        private readonly ISettingManager _settingManager;
        private readonly IBackgroundJobManager _backgroundJobManager;


        public SmsSender(ILocalizationContext localizationContext,
            ISettingManager settingManager,
            IBackgroundJobManager backgroundJobManager)
        {
            Logger = NullLogger.Instance;
            _localizationContext = localizationContext;
            _settingManager = settingManager;
            _backgroundJobManager = backgroundJobManager;
        }

        public async Task<bool> SendAsync(string number, string message)
        {
            /* Implement this service to send SMS to users (can be used for two factor auth). */

            if (string.IsNullOrEmpty(number) || string.IsNullOrEmpty(message)) return false;
            var jobId = await _backgroundJobManager.EnqueueAsync<UnifonicSendSmsJob, UnifonicSendSmsJobArgs>(
                new UnifonicSendSmsJobArgs { Recipient = AddCountryCode(number), Text = message });

            return jobId.IsNullOrEmpty();
        }

        public async Task<bool> SendReceiverSmsAsync(string number,
            DateTime date,
            string shipperName,
            string driverName,
            string driverPhone,
            string waybillNumber,
            string code,
            string link)
        {
            var l1 = L("AShipmentWasAssignedForYouToReceiveOn").Localize(_localizationContext) + ": " +
                     date.ToString("MM/dd/yyyy");
            var l2 = "\n" + L("ShipperName").Localize(_localizationContext) + ": " + shipperName;
            var l3 = "\n" + L("DriverName").Localize(_localizationContext) + ": " + driverName;
            var l4 = "\n" + L("PhoneNumber").Localize(_localizationContext) + ": " + driverPhone;
            var l5 = "\n" + L("WaybillNumber").Localize(_localizationContext) + ": " + waybillNumber;
            var l6 = "\n" + L("VerificationCode").Localize(_localizationContext) + ": " + code;
            var l7 = "\n" + L("PleaseOnClickFollowingLinkToTrackYourShipmentAndRateTheShippingCompany")
                .Localize(_localizationContext) + ": " + link;

            var message = l1 + l2 + l3 + l4 + l5 + l6 + l7;
            var jobId = await _backgroundJobManager.EnqueueAsync<UnifonicSendSmsJob, UnifonicSendSmsJobArgs>(
                new UnifonicSendSmsJobArgs { Recipient = AddCountryCode(number), Text = message });

            return jobId.IsNullOrEmpty();
        }

        private static ILocalizableString L(string message)
        {
            return new LocalizableString(message, TACHYONConsts.LocalizationSourceName);
        }

        private string AddCountryCode(string number)
        {
            if (number.StartsWith("0"))

            {
                number = $"966{number.Remove(0, 1)}";
            }
            else if (!number.StartsWith("966"))
                number = $"966{number}";

            return number;
        }
    }
}