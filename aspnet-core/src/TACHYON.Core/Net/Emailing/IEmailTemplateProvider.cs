namespace TACHYON.Net.Emailing
{
    public interface IEmailTemplateProvider
    {
        string GetDefaultTemplate(int? tenantId);

        string ShipperNotfiyWhenCreditLimitGreaterOrEqualXPercentage(int? TenantId, int Percentage);
    }
}