using Abp.Application.Services.Dto;
using System.Threading.Tasks;
using TACHYON.WebHooks.Dto;

namespace TACHYON.WebHooks
{
    public interface IWebhookAttemptAppService
    {
        Task<PagedResultDto<GetAllSendAttemptsOutput>> GetAllSendAttempts(GetAllSendAttemptsInput input);

        Task<ListResultDto<GetAllSendAttemptsOfWebhookEventOutput>> GetAllSendAttemptsOfWebhookEvent(
            GetAllSendAttemptsOfWebhookEventInput input);
    }
}