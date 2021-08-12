using System.Threading.Tasks;

namespace TACHYON.Net.Emailing
{
    public interface IEmailTemplateProvider
    {
        string GetDefaultTemplate(int? tenantId);
        Task<string> GetActivationTemplateBody();

        string ShipperNotfiyWhenCreditLimitGreaterOrEqualXPercentage(int? TenantId, int Percentage);
    }
}