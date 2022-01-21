using Abp.Application.Services;
using Abp.Application.Services.Dto;
using System.Threading.Tasks;
using TACHYON.MultiTenancy.TenantCarriers.Dto;

namespace TACHYON.MultiTenancy.TenantCarriers
{
    public interface ITenantCarrierAppService : IApplicationService
    {
        Task<PagedResultDto<TenantCarriersListDto>> GetAll(GetAllForTenantCarrierInput input);
        Task Create(CreateTenantCarrierInput input);
        Task Delete(EntityDto input);
    }
}