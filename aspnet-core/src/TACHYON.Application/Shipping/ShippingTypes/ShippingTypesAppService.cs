using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using TACHYON.Shipping.ShippingTypes.Dtos;
using TACHYON.Dto;
using Abp.Application.Services.Dto;
using TACHYON.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;

namespace TACHYON.Shipping.ShippingTypes
{
    [AbpAuthorize(AppPermissions.Pages_ShippingTypes)]
    public class ShippingTypesAppService : TACHYONAppServiceBase, IShippingTypesAppService
    {
        private readonly IRepository<ShippingType> _shippingTypeRepository;

        public ShippingTypesAppService(IRepository<ShippingType> shippingTypeRepository)
        {
            _shippingTypeRepository = shippingTypeRepository;

        }

        public async Task<PagedResultDto<GetShippingTypeForViewDto>> GetAll(GetAllShippingTypesInput input)
        {

            var filteredShippingTypes = _shippingTypeRepository.GetAll()
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.DisplayName.Contains(input.Filter) || e.Description.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.DisplayNameFilter), e => e.DisplayName == input.DisplayNameFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.DescriptionFilter), e => e.Description == input.DescriptionFilter);

            var pagedAndFilteredShippingTypes = filteredShippingTypes
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

            var shippingTypes = from o in pagedAndFilteredShippingTypes
                                select new GetShippingTypeForViewDto()
                                {
                                    ShippingType = new ShippingTypeDto
                                    {
                                        DisplayName = o.DisplayName,
                                        Description = o.Description,
                                        Id = o.Id
                                    }
                                };

            var totalCount = await filteredShippingTypes.CountAsync();

            return new PagedResultDto<GetShippingTypeForViewDto>(
                totalCount,
                await shippingTypes.ToListAsync()
            );
        }

        public async Task<GetShippingTypeForViewDto> GetShippingTypeForView(int id)
        {
            var shippingType = await _shippingTypeRepository.GetAsync(id);

            var output = new GetShippingTypeForViewDto { ShippingType = ObjectMapper.Map<ShippingTypeDto>(shippingType) };

            return output;
        }

        [AbpAuthorize(AppPermissions.Pages_ShippingTypes_Edit)]
        public async Task<GetShippingTypeForEditOutput> GetShippingTypeForEdit(EntityDto input)
        {
            var shippingType = await _shippingTypeRepository.FirstOrDefaultAsync(input.Id);

            var output = new GetShippingTypeForEditOutput { ShippingType = ObjectMapper.Map<CreateOrEditShippingTypeDto>(shippingType) };

            return output;
        }

        public async Task CreateOrEdit(CreateOrEditShippingTypeDto input)
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

        [AbpAuthorize(AppPermissions.Pages_ShippingTypes_Create)]
        protected virtual async Task Create(CreateOrEditShippingTypeDto input)
        {
            var shippingType = ObjectMapper.Map<ShippingType>(input);

            await _shippingTypeRepository.InsertAsync(shippingType);
        }

        [AbpAuthorize(AppPermissions.Pages_ShippingTypes_Edit)]
        protected virtual async Task Update(CreateOrEditShippingTypeDto input)
        {
            var shippingType = await _shippingTypeRepository.FirstOrDefaultAsync((int)input.Id);
            ObjectMapper.Map(input, shippingType);
        }

        [AbpAuthorize(AppPermissions.Pages_ShippingTypes_Delete)]
        public async Task Delete(EntityDto input)
        {
            await _shippingTypeRepository.DeleteAsync(input.Id);
        }
    }
}