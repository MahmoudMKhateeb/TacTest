using Abp.Configuration;
using Abp.Dependency;
using System;
using System.Collections.Generic;
using System.Text;
using RestSharp;
using System.Threading.Tasks;
using TACHYON.Configuration;

namespace TACHYON.Net.Sms
{
    public class UnifonicSmsClient : ITransientDependency
    {
        private readonly ISettingManager _settingManager;

        public UnifonicSmsClient(ISettingManager settingManager)
        {
            _settingManager = settingManager;
        }

        public async Task<IRestResponse<UnifonicResponseRoot>> SendSmsAsync(string recipient, string text)
        {
            string appSid = await _settingManager.GetSettingValueAsync(AppSettings.Sms.UnifonicAppSid);
            string senderId = await _settingManager.GetSettingValueAsync(AppSettings.Sms.UnifonicSenderId);

            var client = new RestClient($"http://basic.unifonic.com/rest/SMS/messages?SenderID={senderId}&Body={text}&Recipient={recipient}&AppSid={appSid}");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Accept", "application/json");
            request.AddHeader("Authorization", "Basic Og==");
            var body = @"";
            request.AddParameter("text/plain", body, ParameterType.RequestBody);
            //IRestResponse response = client.Execute(request);
            var response = await client.ExecuteTaskAsync<UnifonicResponseRoot>(request);
            if (response.ErrorException != null)
            {
                const string message = "Error retrieving response from Unifonic sms API.  Check inner details for more info.";
                var exception = new Exception(message, response.ErrorException);
                throw exception;
            }

            return response;
        }

    }
    public class UnifonicResponseRootData
    {
        public long MessageID { get; set; }
        public string CorrelationID { get; set; }
        public string Status { get; set; }
        public int NumberOfUnits { get; set; }
        public int Cost { get; set; }
        public int Balance { get; set; }
        public string Recipient { get; set; }
        public string TimeCreated { get; set; }
        public string CurrencyCode { get; set; }
    }

    public class UnifonicResponseRoot
    {
        public bool success { get; set; }
        public string message { get; set; }
        public string errorCode { get; set; }
        public UnifonicResponseRootData data { get; set; }
    }

}




