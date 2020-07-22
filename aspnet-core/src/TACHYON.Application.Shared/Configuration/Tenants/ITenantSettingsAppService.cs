using Abp.Application.Services;
using System.Threading.Tasks;
using TACHYON.Configuration.Tenants.Dto;

namespace TACHYON.Configuration.Tenants
{
    public interface ITenantSettingsAppService : IApplicationService
    {
        Task<TenantSettingsEditDto> GetAllSettings();

        Task UpdateAllSettings(TenantSettingsEditDto input);

        Task ClearLogo();

        Task ClearCustomCss();
    }
}