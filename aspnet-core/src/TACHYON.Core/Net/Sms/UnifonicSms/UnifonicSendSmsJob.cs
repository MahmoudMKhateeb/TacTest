using Abp.BackgroundJobs;
using Abp.Configuration;
using Abp.Dependency;
using Abp.Threading;
using RestSharp;
using System;
using TACHYON.Configuration;

namespace TACHYON.Net.Sms.UnifonicSms
{
    public class UnifonicSendSmsJob : BackgroundJob<UnifonicSendSmsJobArgs>, ITransientDependency
    {
        private readonly ISettingManager _settingManager;

        public UnifonicSendSmsJob(ISettingManager settingManager)
        {
            _settingManager = settingManager;
        }

        public override void Execute(UnifonicSendSmsJobArgs args)
        {
            string appSid = AsyncHelper.RunSync(() => _settingManager.GetSettingValueAsync(AppSettings.Sms.UnifonicAppSid));
            string senderId = AsyncHelper.RunSync(() => _settingManager.GetSettingValueAsync(AppSettings.Sms.UnifonicSenderId));

            var client = new RestClient($"http://basic.unifonic.com/rest/SMS/messages?SenderID={senderId}&Body={args.Text}&Recipient={args.Recipient}&AppSid={appSid}");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Accept", "application/json");
            request.AddHeader("Authorization", "Basic Og==");
            var body = @"";
            request.AddParameter("text/plain", body, ParameterType.RequestBody);
            //IRestResponse response = client.Execute(request);
            var response = AsyncHelper.RunSync(() => client.ExecuteTaskAsync<UnifonicResponseRoot>(request));
            if (response.ErrorException != null)
            {
                const string message = "Error retrieving response from Unifonic SMS API.  Check inner details for more info.";
                var exception = new Exception(message, response.ErrorException);
                throw exception;
            }

        }
    }
}
