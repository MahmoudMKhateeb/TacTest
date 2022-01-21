using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Abp.UI;
using Castle.Core.Internal;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using TACHYON.Authorization;
using TACHYON.Extension;
using TACHYON.ShippingRequestVases.Dtos;
using TACHYON.Vases;
using TACHYON.Vases.Dtos;

namespace TACHYON.ShippingRequestVases
{
    [AbpAuthorize(AppPermissions.Pages_ShippingRequestVases)]
    public class ShippingRequestVasesAppService : TACHYONAppServiceBase, IShippingRequestVasesAppService
    {
        private readonly IRepository<ShippingRequestVas, long> _shippingRequestVasRepository;
        private readonly IRepository<Vas, int> _lookup_vasRepository;

        public ShippingRequestVasesAppService(IRepository<ShippingRequestVas, long> shippingRequestVasRepository,
            IRepository<Vas, int> lookup_vasRepository)
        {
            _shippingRequestVasRepository = shippingRequestVasRepository;
            _lookup_vasRepository = lookup_vasRepository;
        } // TO DO ADD ALL MAPPING CONFIGURATION => done

        public async Task<PagedResultDto<ShippingRequestVasDto>> GetAll(GetAllShippingRequestVasesInput input)
        {
            var shippingRequestVases = _shippingRequestVasRepository.GetAll()
                .AsNoTracking().Include(e => e.VasFk)
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                    e => false)
                .WhereIf(!string.IsNullOrWhiteSpace(input.VasNameFilter),
                    e => e.VasFk != null && e.VasFk.Name == input.VasNameFilter)
                .OrderBy(input.Sorting ?? "id asc");

            var pageResult = await shippingRequestVases.PageBy(input).ToListAsync();

            var totalCount = await shippingRequestVases.CountAsync();

            var items = pageResult
                .Select(x => new ShippingRequestVasDto()
                {
                    Id = x.Id,
                    OtherVasName = x.OtherVasName,
                    RequestMaxAmount = x.RequestMaxAmount,
                    RequestMaxCount = x.RequestMaxCount,
                    ShippingRequestId = x.ShippingRequestId,
                    Vas = ObjectMapper.Map<VasDto>(x.VasFk)
                }).ToList();

            return new PagedResultDto<ShippingRequestVasDto>() { Items = items, TotalCount = totalCount };
        }

        public async Task<ShippingRequestVasDto> GetShippingRequestVasForView(long id)
        {
            var shippingRequestVas = await _shippingRequestVasRepository
                .GetAll().AsNoTracking().Include(x => x.VasFk)
                .ThenInclude(x => x.Translations).FirstOrDefaultAsync(x => x.Id == id);

            return new ShippingRequestVasDto()
            {
                Id = shippingRequestVas.Id,
                OtherVasName = shippingRequestVas.OtherVasName,
                RequestMaxAmount = shippingRequestVas.RequestMaxAmount,
                RequestMaxCount = shippingRequestVas.RequestMaxCount,
                ShippingRequestId = shippingRequestVas.ShippingRequestId,
                Vas = ObjectMapper.Map<VasDto>(shippingRequestVas.VasFk)
            };
        }

        [AbpAuthorize(AppPermissions.Pages_ShippingRequestVases_Edit)]
        public async Task<GetShippingRequestVasForEditOutput> GetShippingRequestVasForEdit(EntityDto<long> input)
        {
            var shippingRequestVas = await _shippingRequestVasRepository.FirstOrDefaultAsync(input.Id);

            var output = new GetShippingRequestVasForEditOutput
            {
                ShippingRequestVas = ObjectMapper.Map<CreateOrEditShippingRequestVasDto>(shippingRequestVas)
            };

            if (output.ShippingRequestVas.VasId != null)
            {
                var _lookupVas = await _lookup_vasRepository.FirstOrDefaultAsync((int)output.ShippingRequestVas.VasId);
                output.VasName = _lookupVas?.Name?.ToString();
            }

            return output;
        }

        public async Task CreateOrEdit(CreateOrEditShippingRequestVasDto input)
        {
            await ValidateOtherVasName(input);

            if (input.Id == null)
            {
                await Create(input);
            }
            else
            {
                await Update(input);
            }
        }

        [AbpAuthorize(AppPermissions.Pages_ShippingRequestVases_Create)]
        protected virtual async Task Create(CreateOrEditShippingRequestVasDto input)
        {
            var shippingRequestVas = ObjectMapper.Map<ShippingRequestVas>(input);

            await _shippingRequestVasRepository.InsertAsync(shippingRequestVas);
        }

        [AbpAuthorize(AppPermissions.Pages_ShippingRequestVases_Edit)]
        protected virtual async Task Update(CreateOrEditShippingRequestVasDto input)
        {
            var shippingRequestVas = await _shippingRequestVasRepository.FirstOrDefaultAsync((long)input.Id);
            ObjectMapper.Map(input, shippingRequestVas);
        }

        [AbpAuthorize(AppPermissions.Pages_ShippingRequestVases_Delete)]
        public async Task Delete(EntityDto<long> input)
        {
            await _shippingRequestVasRepository.DeleteAsync(input.Id);
        }

        [AbpAuthorize(AppPermissions.Pages_ShippingRequestVases)]
        public async Task<List<ShippingRequestVasVasLookupTableDto>> GetAllVasForTableDropdown()
        {
            return await _lookup_vasRepository.GetAll()
                .Select(vas => new ShippingRequestVasVasLookupTableDto
                {
                    Id = vas.Id,
                    DisplayName = vas == null || vas.Name == null ? "" : vas.Name.ToString(),
                    IsOther = vas.ContainsOther()
                }).ToListAsync();
        }

        #region Helpers

        private async Task ValidateOtherVasName(CreateOrEditShippingRequestVasDto input)
        {
            var vas = await _lookup_vasRepository.FirstOrDefaultAsync(input.VasId);

            if (vas.Name.ToLower().Contains(TACHYONConsts.OthersDisplayName) && input.OtherVasName.IsNullOrEmpty())
                throw new UserFriendlyException(L("OtherVasNameMustBeNotEmpty"));
        }

        #endregion
    }
}