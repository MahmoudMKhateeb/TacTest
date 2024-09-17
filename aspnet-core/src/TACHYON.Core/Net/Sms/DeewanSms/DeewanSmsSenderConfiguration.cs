using Abp.Dependency;
using Microsoft.Extensions.Configuration;
using TACHYON.Configuration;

namespace TACHYON.Net.Sms.DeewanSms
{
    public class DeewanSmsSenderConfiguration : ITransientDependency
    {
        
        private readonly IConfigurationRoot _appConfiguration;

        public string ApiUrl => _appConfiguration["DeewanSms:ApiUrl"];

        public string AuthToken => _appConfiguration["DeewanSms:AuthToken"];

        public string SenderName => _appConfiguration["DeewanSms:SenderName"];

        public DeewanSmsSenderConfiguration(IAppConfigurationAccessor configurationAccessor)
        {
            _appConfiguration = configurationAccessor.Configuration;
        }
    }
}