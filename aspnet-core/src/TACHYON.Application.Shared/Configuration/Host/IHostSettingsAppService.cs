using Abp.Application.Services;
using System.Threading.Tasks;
using TACHYON.Configuration.Host.Dto;

namespace TACHYON.Configuration.Host
{
    public interface IHostSettingsAppService : IApplicationService
    {
        Task<HostSettingsEditDto> GetAllSettings();

        Task UpdateAllSettings(HostSettingsEditDto input);

        Task SendTestEmail(SendTestEmailInput input);
    }
}