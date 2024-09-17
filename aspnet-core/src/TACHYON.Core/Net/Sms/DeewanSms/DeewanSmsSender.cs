using Abp.Dependency;
using Abp.Localization;
using Castle.Core.Logging;
using RestSharp;
using System;
using System.Threading.Tasks;


namespace TACHYON.Net.Sms.DeewanSms
{
    public class DeewanSmsSender : SmsSenderBase, ISmsSender, ITransientDependency
{
   
    private readonly DeewanSmsSenderConfiguration _configuration;
    private readonly ILocalizationContext _localizationContext;
    public ILogger Logger { get; set; }

    public DeewanSmsSender(DeewanSmsSenderConfiguration configuration, ILocalizationContext localizationContext)
    {
        _configuration = configuration;
        _localizationContext = localizationContext;
        Logger = NullLogger.Instance;
    }

    public async Task<bool> SendAsync(string number, string message)
    {
        var client = CreateRestClient();
        var request = CreateRestRequest(message, number);

        var response = await client.ExecutePostAsync(request);

        if (!response.IsSuccessful)
        {
            Logger.Error($"SMS Error: {response.ErrorMessage}");
        }

        return true;
    }

    private RestClient CreateRestClient()
    {
        return new RestClient(_configuration.ApiUrl);
    }

    private IRestRequest CreateRestRequest(string messageText, string recipients)
    {
        return new RestRequest("", Method.POST)
            .AddHeader("accept", "application/json")
            .AddHeader("Authorization", _configuration.AuthToken)
            .AddJsonBody(new
            {
                senderName = _configuration.SenderName,
                messageType = "text",
                messageText,
                recipients
            });
    }

    public async Task<bool> SendReceiverSmsAsync(string number, DateTime date, string shipperName, string driverName, string driverPhone, string waybillNumber, string code, string link)
    {
        var message = GenerateReceiverSmsMessage(date, shipperName, driverName, driverPhone, waybillNumber, code, link);
        return await SendAsync(AddCountryCode(number), message);
    }

    private string GenerateReceiverSmsMessage(DateTime date, string shipperName, string driverName, string driverPhone, string waybillNumber, string code, string link)
    {
        var l1 = L("AShipmentWasAssignedForYouToReceiveOn").Localize(_localizationContext) + ": " + date.ToString("MM/dd/yyyy");
        var l2 = "\n" + L("ShipperName").Localize(_localizationContext) + ": " + shipperName;
        var l3 = "\n" + L("DriverName").Localize(_localizationContext) + ": " + driverName;
        var l4 = "\n" + L("PhoneNumber").Localize(_localizationContext) + ": " + driverPhone;
        var l5 = "\n" + L("WaybillNumber").Localize(_localizationContext) + ": " + waybillNumber;
        var l6 = "\n" + L("VerificationCode").Localize(_localizationContext) + ": " + code;
        var l7 = "\n" + L("PleaseOnClickFollowingLinkToTrackYourShipmentAndRateTheShippingCompany").Localize(_localizationContext) + ": " + link;

        return l1 + l2 + l3 + l4 + l5 + l6 + l7;
    }
}
}