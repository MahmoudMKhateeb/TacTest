using System;
using System.Threading.Tasks;

namespace TACHYON.Net.Emailing
{
    public interface IEmailTemplateProvider
    {
        Task<String> GetDefaultTemplate(int? tenantId);

        string ShipperNotfiyWhenCreditLimitGreaterOrEqualXPercentage(int? TenantId, int Percentage);
    }
}