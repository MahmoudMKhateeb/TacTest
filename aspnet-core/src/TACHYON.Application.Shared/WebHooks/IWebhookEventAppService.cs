using Abp.Webhooks;
using System.Threading.Tasks;

namespace TACHYON.WebHooks
{
    public interface IWebhookEventAppService
    {
        Task<WebhookEvent> Get(string id);
    }
}