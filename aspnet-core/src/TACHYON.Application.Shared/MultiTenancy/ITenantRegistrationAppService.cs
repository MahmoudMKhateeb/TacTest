using Abp.Application.Services;
using System.Threading.Tasks;
using TACHYON.Editions.Dto;
using TACHYON.MultiTenancy.Dto;

namespace TACHYON.MultiTenancy
{
    public interface ITenantRegistrationAppService : IApplicationService
    {
        Task<RegisterTenantOutput> RegisterTenant(RegisterTenantInput input);

        Task<EditionsSelectOutput> GetEditionsForSelect();

        Task<EditionSelectDto> GetEdition(int editionId);
    }
}