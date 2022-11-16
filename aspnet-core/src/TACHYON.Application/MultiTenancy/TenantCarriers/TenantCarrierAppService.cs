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
    public class TenantCarrierAppService : TACHYONAppServiceBase, ITenantCarrierAppService
    {
        private readonly IRepository<TenantCarrier, long> _tenantCarrierRepository;
        private readonly IRepository<Tenant> _tenantRepository;


        public TenantCarrierAppService(IRepository<TenantCarrier, long> tenantCarrierRepository,
            IRepository<Tenant> tenantRepository)
        {
            _tenantCarrierRepository = tenantCarrierRepository;
            _tenantRepository = tenantRepository;
        }

        public async Task<PagedResultDto<TenantCarriersListDto>> GetAll(GetAllForTenantCarrierInput input)
        {
            DisableTenancyFilters();
            var query = _tenantCarrierRepository
                .GetAll().AsNoTracking().Include(x => x.CarrierShipper)
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
            if (!await _tenantRepository.GetAll().AnyAsync(x =>
                    x.Id == input.CarrierTenantId &&
                    x.Edition.DisplayName.ToLower() == TACHYONConsts.CarrierEdtionName))
            {
                throw new UserFriendlyException(L("TheCarrierSelectedIsNotFound"));
            }

            var isShipperOrBroker = await _tenantRepository.GetAll().AnyAsync(x =>
                x.Id == input.TenantId && x.Edition.DisplayName.ToLower() == TACHYONConsts.ShipperEdtionName 
                || x.Edition.DisplayName.ToLower().Trim() == TACHYONConsts.BrokerEditionName);
            
            if (!isShipperOrBroker)
            {
                throw new UserFriendlyException(L("TheShipperSelectedIsNotFound"));
            }

            if (await _tenantCarrierRepository.GetAll().AnyAsync(x =>
                    x.TenantId == input.TenantId && x.CarrierTenantId == input.CarrierTenantId))
            {
                throw new UserFriendlyException(L("TheCarrierAlreadyAddToTheSipperSelected"));
            }

            TenantCarrier tenantCarrier = ObjectMapper.Map<TenantCarrier>(input);
            await _tenantCarrierRepository.InsertAsync(tenantCarrier);
        }

        [AbpAuthorize(AppPermissions.Pages_TenantCarrier_Delete)]
        public async Task Delete(EntityDto input)
        {
            DisableTenancyFilters();
            await _tenantCarrierRepository.DeleteAsync(input.Id);
        }
    }
}