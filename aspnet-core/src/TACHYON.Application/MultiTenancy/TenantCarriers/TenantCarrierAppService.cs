using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TACHYON.Authorization;
using TACHYON.MultiTenancy.TenantCarriers.Dto;

namespace TACHYON.MultiTenancy.TenantCarriers
{
    [AbpAuthorize(AppPermissions.Pages_TenantCarrier)]

    public class TenantCarrierAppService : TACHYONAppServiceBase,ITenantCarrierAppService
    {
        private readonly IRepository<TenantCarrier,long> _tenantCarrierRepository;

        public TenantCarrierAppService(IRepository<TenantCarrier, long> tenantCarrierRepository)
        {
            _tenantCarrierRepository = tenantCarrierRepository;
        }

        public async Task<PagedResultDto<TenantCarriersListDto>> GetAll(GetAllForTenantCarrierInput input)
        {
            DisableTenancyFilters();
            var query = _tenantCarrierRepository
                .GetAll().
                AsNoTracking().
                Include(x => x.CarrierShipper)
                .Where(c => c.TenantId == input.Id);
            var ResultPage = await query.PageBy(input).ToListAsync();

            var totalCount = await query.CountAsync();
            return new PagedResultDto<TenantCarriersListDto>(
                totalCount,
                ObjectMapper.Map<List<TenantCarriersListDto>>(ResultPage)

            );
        }
        [AbpAuthorize(AppPermissions.Pages_TenantCarrier_Create)]
        public async Task Create(CreateTenantCarrierInput input)
        {
            if (await _tenantCarrierRepository.GetAll().AnyAsync(x => x.TenantId == input.Id && x.CarrierTenantId == input.CarrierTenantId))
            {
                throw new UserFriendlyException(L("TheCarrierAlreadyAddToTheSipperSelected"));
            }

          await  _tenantCarrierRepository.InsertAsync(ObjectMapper.Map<TenantCarrier>(input));
        }
        [AbpAuthorize(AppPermissions.Pages_TenantCarrier_Delete)]
        public async Task Delete(EntityDto input)
        {
            DisableTenancyFilters();
           await _tenantCarrierRepository.DeleteAsync(input.Id);
        }


    }
}
