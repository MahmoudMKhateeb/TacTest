using Abp.Dependency;
using RestSharp;
using System;
using System.Threading.Tasks;
using TACHYON.Identity;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace TACHYON.Net.Sms
{
    public class TwilioSmsSender : ISmsSender, ITransientDependency
    {
        private TwilioSmsSenderConfiguration _twilioSmsSenderConfiguration;

        public TwilioSmsSender(TwilioSmsSenderConfiguration twilioSmsSenderConfiguration)
        {
            _twilioSmsSenderConfiguration = twilioSmsSenderConfiguration;
        }

        public async Task<bool> SendAsync(string number, string message)
        {
            TwilioClient.Init(_twilioSmsSenderConfiguration.AccountSid, _twilioSmsSenderConfiguration.AuthToken);

            MessageResource resource = await MessageResource.CreateAsync(
                body: message,
                @from: new Twilio.Types.PhoneNumber(_twilioSmsSenderConfiguration.SenderNumber),
                to: new Twilio.Types.PhoneNumber(number)
            );
            return true;
        }

        public Task<bool> SendReceiverSmsAsync(string number, DateTime date, string shipperName, string driverName,
            string driverPhone, string waybillNumber, string code, string link)
        {
            /* Implement this service to send SMS to users (can be used for two factor auth). */

            return Task.FromResult(false);
        }
    }
}