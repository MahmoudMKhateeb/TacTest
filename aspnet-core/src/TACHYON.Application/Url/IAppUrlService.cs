namespace TACHYON.Url
{
    public interface IAppUrlService
    {
        string CreateEmailActivationUrlFormat(int? tenantId);

        string CreatePasswordResetUrlFormat(int? tenantId);

        string CreateEmailActivationUrlFormat(string tenancyName);

        string CreatePasswordResetUrlFormat(string tenancyName);
        string CreateInvoiceDetailsFormat(long invoiceId);
        string GetTachyonPlatformLoginUrl();
    }
}