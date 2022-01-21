using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using TACHYON.PickingTypes.Dtos;
using TACHYON.Dto;
using Abp.Application.Services.Dto;
using TACHYON.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;

namespace TACHYON.PickingTypes
{
    [AbpAuthorize(AppPermissions.Pages_PickingTypes)]
    public class PickingTypesAppService : TACHYONAppServiceBase, IPickingTypesAppService
    {
        private readonly IRepository<PickingType> _pickingTypeRepository;


        public PickingTypesAppService(IRepository<PickingType> pickingTypeRepository)
        {
            _pickingTypeRepository = pickingTypeRepository;
        }

        public async Task<PagedResultDto<GetPickingTypeForViewDto>> GetAll(GetAllPickingTypesInput input)
        {
            var filteredPickingTypes = _pickingTypeRepository.GetAll()
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.DisplayName.Contains(input.Filter))
                .WhereIf(!string.IsNullOrWhiteSpace(input.DisplayNameFilter),
                    e => e.DisplayName == input.DisplayNameFilter);

            var pagedAndFilteredPickingTypes = filteredPickingTypes
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

            var pickingTypes = from o in pagedAndFilteredPickingTypes
                select new GetPickingTypeForViewDto()
                {
                    PickingType = new PickingTypeDto { DisplayName = o.DisplayName, Id = o.Id }
                };

            var totalCount = await filteredPickingTypes.CountAsync();

            return new PagedResultDto<GetPickingTypeForViewDto>(
                totalCount,
                await pickingTypes.ToListAsync()
            );
        }

        public async Task<GetPickingTypeForViewDto> GetPickingTypeForView(int id)
        {
            var pickingType = await _pickingTypeRepository.GetAsync(id);

            var output = new GetPickingTypeForViewDto { PickingType = ObjectMapper.Map<PickingTypeDto>(pickingType) };

            return output;
        }

        [AbpAuthorize(AppPermissions.Pages_PickingTypes_Edit)]
        public async Task<GetPickingTypeForEditOutput> GetPickingTypeForEdit(EntityDto input)
        {
            var pickingType = await _pickingTypeRepository.FirstOrDefaultAsync(input.Id);

            var output = new GetPickingTypeForEditOutput
            {
                PickingType = ObjectMapper.Map<CreateOrEditPickingTypeDto>(pickingType)
            };

            return output;
        }

        public async Task CreateOrEdit(CreateOrEditPickingTypeDto input)
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

        [AbpAuthorize(AppPermissions.Pages_PickingTypes_Create)]
        protected virtual async Task Create(CreateOrEditPickingTypeDto input)
        {
            var pickingType = ObjectMapper.Map<PickingType>(input);


            await _pickingTypeRepository.InsertAsync(pickingType);
        }

        [AbpAuthorize(AppPermissions.Pages_PickingTypes_Edit)]
        protected virtual async Task Update(CreateOrEditPickingTypeDto input)
        {
            var pickingType = await _pickingTypeRepository.FirstOrDefaultAsync((int)input.Id);
            ObjectMapper.Map(input, pickingType);
        }

        [AbpAuthorize(AppPermissions.Pages_PickingTypes_Delete)]
        public async Task Delete(EntityDto input)
        {
            await _pickingTypeRepository.DeleteAsync(input.Id);
        }
    }
}