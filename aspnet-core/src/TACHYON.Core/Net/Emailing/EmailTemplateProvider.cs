using Abp.Configuration;
using Abp.Dependency;
using Abp.Extensions;
using Abp.IO.Extensions;
using Abp.MultiTenancy;
using Abp.Reflection.Extensions;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using TACHYON.Configuration;
using TACHYON.MultiTenancy;
using TACHYON.Url;

namespace TACHYON.Net.Emailing
{
    public class EmailTemplateProvider : IEmailTemplateProvider, ISingletonDependency
    {
        private readonly IWebUrlService _webUrlService;
        private readonly ITenantCache _tenantCache;
        private readonly ConcurrentDictionary<string, string> _defaultTemplates;
        private readonly ISettingManager _settingManager;

        public EmailTemplateProvider(IWebUrlService webUrlService,
            ITenantCache tenantCache,
            ISettingManager settingManager)
        {
            _webUrlService = webUrlService;
            _tenantCache = tenantCache;
            _settingManager = settingManager;
            _defaultTemplates = new ConcurrentDictionary<string, string>();
        }

        public async Task<String> GetDefaultTemplate(int? tenantId)
        {
            var tenancyKey = tenantId.HasValue ? tenantId.Value.ToString() : "host";

            await using var stream = typeof(EmailTemplateProvider).GetAssembly()
                .GetManifestResourceStream("TACHYON.Net.Emailing.EmailTemplates.EmailTemplate.html");
            StreamReader sr = new StreamReader(stream, Encoding.UTF8);
            var template = await sr.ReadToEndAsync();
            template = template.Replace("{THIS_YEAR}", DateTime.Now.Year.ToString());
            template = template.Replace("{EMAIL_LOGO_URL}", GetTenantLogoUrl(tenantId));
            template = template.Replace("{OUTLINE_LOGO_URL}", GetOutlineLogoUrl());

            return _defaultTemplates.GetOrAdd(tenancyKey, key => template);
        }


        public string ShipperNotfiyWhenCreditLimitGreaterOrEqualXPercentage(int? TenantId, int Percentage)
        {
            var tenancyKey = TenantId.ToString();

            return _defaultTemplates.GetOrAdd(tenancyKey, key =>
            {
                using (var stream = typeof(EmailTemplateProvider).GetAssembly()
                           .GetManifestResourceStream(
                               "TACHYON.Net.Emailing.EmailTemplates.ShipperNotfiyWhenCreditLimitGreaterOrEqualXPercentage.html"))
                {
                    var bytes = stream.GetAllBytes();
                    var template = Encoding.UTF8.GetString(bytes, 3, bytes.Length - 3);
                    template = template.Replace("{EMAIL_PERCENTAGE}", Percentage.ToString());
                    template = template.Replace("{THIS_YEAR}", DateTime.Now.Year.ToString());
                    return template.Replace("{EMAIL_LOGO_URL}", GetTenantLogoUrl(TenantId));
                }
            });
        }

        private string GetTenantLogoUrl(int? tenantId)
        {
            if (!tenantId.HasValue)
            {
                return _webUrlService.GetServerRootAddress().EnsureEndsWith('/') +
                       "TenantCustomization/GetTenantLogo?skin=light";
            }

            var tenant = _tenantCache.Get(tenantId.Value);
            return _webUrlService.GetServerRootAddress(tenant.TenancyName).EnsureEndsWith('/') +
                   "TenantCustomization/GetTenantLogo?skin=light&tenantId=" + tenantId.Value;
        }

        private string GetOutlineLogoUrl()
        {
            var logoPath = _settingManager.GetSettingValue(AppSettings.Email.EmailLogoPath);
            return _webUrlService.GetServerRootAddress().EnsureEndsWith('/') + logoPath;
        }
    }
}