using TACHYON.Vases;

using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using TACHYON.ShippingRequestVases.Dtos;
using TACHYON.Dto;
using Abp.Application.Services.Dto;
using TACHYON.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;

namespace TACHYON.ShippingRequestVases
{
    [AbpAuthorize(AppPermissions.Pages_ShippingRequestVases)]
    public class ShippingRequestVasesAppService : TACHYONAppServiceBase, IShippingRequestVasesAppService
    {
        private readonly IRepository<ShippingRequestVas, long> _shippingRequestVasRepository;
        private readonly IRepository<Vas, int> _lookup_vasRepository;

        public ShippingRequestVasesAppService(IRepository<ShippingRequestVas, long> shippingRequestVasRepository, IRepository<Vas, int> lookup_vasRepository)
        {
            _shippingRequestVasRepository = shippingRequestVasRepository;
            _lookup_vasRepository = lookup_vasRepository;

        }

        public async Task<PagedResultDto<GetShippingRequestVasForViewDto>> GetAll(GetAllShippingRequestVasesInput input)
        {

            var filteredShippingRequestVases = _shippingRequestVasRepository.GetAll()
                        .Include(e => e.VasFk)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.VasNameFilter), e => e.VasFk != null && e.VasFk.Name == input.VasNameFilter);

            var pagedAndFilteredShippingRequestVases = filteredShippingRequestVases
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

            var shippingRequestVases = from o in pagedAndFilteredShippingRequestVases
                                       join o1 in _lookup_vasRepository.GetAll() on o.VasId equals o1.Id into j1
                                       from s1 in j1.DefaultIfEmpty()

                                       select new GetShippingRequestVasForViewDto()
                                       {
                                           ShippingRequestVas = new ShippingRequestVasDto
                                           {
                                               Id = o.Id
                                           },
                                           VasName = s1 == null || s1.Name == null ? "" : s1.Name.ToString()
                                       };

            var totalCount = await filteredShippingRequestVases.CountAsync();

            return new PagedResultDto<GetShippingRequestVasForViewDto>(
                totalCount,
                await shippingRequestVases.ToListAsync()
            );
        }

        public async Task<GetShippingRequestVasForViewDto> GetShippingRequestVasForView(long id)
        {
            var shippingRequestVas = await _shippingRequestVasRepository.GetAsync(id);

            var output = new GetShippingRequestVasForViewDto { ShippingRequestVas = ObjectMapper.Map<ShippingRequestVasDto>(shippingRequestVas) };

            if (output.ShippingRequestVas.VasId != null)
            {
                var _lookupVas = await _lookup_vasRepository.FirstOrDefaultAsync((int)output.ShippingRequestVas.VasId);
                output.VasName = _lookupVas?.Name?.ToString();
            }

            return output;
        }

        [AbpAuthorize(AppPermissions.Pages_ShippingRequestVases_Edit)]
        public async Task<GetShippingRequestVasForEditOutput> GetShippingRequestVasForEdit(EntityDto<long> input)
        {
            var shippingRequestVas = await _shippingRequestVasRepository.FirstOrDefaultAsync(input.Id);

            var output = new GetShippingRequestVasForEditOutput { ShippingRequestVas = ObjectMapper.Map<CreateOrEditShippingRequestVasDto>(shippingRequestVas) };

            if (output.ShippingRequestVas.VasId != null)
            {
                var _lookupVas = await _lookup_vasRepository.FirstOrDefaultAsync((int)output.ShippingRequestVas.VasId);
                output.VasName = _lookupVas?.Name?.ToString();
            }

            return output;
        }

        public async Task CreateOrEdit(CreateOrEditShippingRequestVasDto input)
        {
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

            if (AbpSession.TenantId != null)
            {
                shippingRequestVas.TenantId = (int)AbpSession.TenantId;
            }

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
                    DisplayName = vas == null || vas.Name == null ? "" : vas.Name.ToString()
                }).ToListAsync();
        }

    }
}