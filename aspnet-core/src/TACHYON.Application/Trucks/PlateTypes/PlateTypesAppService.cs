using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using TACHYON.Trucks.PlateTypes.Dtos;
using TACHYON.Dto;
using Abp.Application.Services.Dto;
using TACHYON.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;
using Abp.UI;

namespace TACHYON.Trucks.PlateTypes
{
    [AbpAuthorize(AppPermissions.Pages_PlateTypes)]
    public class PlateTypesAppService : TACHYONAppServiceBase, IPlateTypesAppService
    {
        private readonly IRepository<PlateType> _plateTypeRepository;

        public PlateTypesAppService(IRepository<PlateType> plateTypeRepository)
        {
            _plateTypeRepository = plateTypeRepository;

        }

        public async Task<PagedResultDto<GetPlateTypeForViewDto>> GetAll(GetAllPlateTypesInput input)
        {

            var filteredPlateTypes = _plateTypeRepository.GetAll()
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.DisplayName.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.DisplayNameFilter), e => e.DisplayName == input.DisplayNameFilter);

            var pagedAndFilteredPlateTypes = filteredPlateTypes
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

            var plateTypes = from o in pagedAndFilteredPlateTypes
                             select new GetPlateTypeForViewDto()
                             {
                                 PlateType = new PlateTypeDto
                                 {
                                     DisplayName = o.DisplayName,
                                     Id = o.Id
                                 }
                             };

            var totalCount = await filteredPlateTypes.CountAsync();

            return new PagedResultDto<GetPlateTypeForViewDto>(
                totalCount,
                await plateTypes.ToListAsync()
            );
        }

        public async Task<GetPlateTypeForViewDto> GetPlateTypeForView(int id)
        {
            var plateType = await _plateTypeRepository.GetAsync(id);

            var output = new GetPlateTypeForViewDto { PlateType = ObjectMapper.Map<PlateTypeDto>(plateType) };

            return output;
        }

        [AbpAuthorize(AppPermissions.Pages_PlateTypes_Edit)]
        public async Task<GetPlateTypeForEditOutput> GetPlateTypeForEdit(EntityDto input)
        {
            var plateType = await _plateTypeRepository.FirstOrDefaultAsync(input.Id);

            var output = new GetPlateTypeForEditOutput { PlateType = ObjectMapper.Map<CreateOrEditPlateTypeDto>(plateType) };

            return output;
        }

        public async Task CreateOrEdit(CreateOrEditPlateTypeDto input)
        {
            await CheckIfNameExists(input);
            if (input.Id == null)
            {
                await Create(input);
            }
            else
            {
                await Update(input);
            }
        }

        [AbpAuthorize(AppPermissions.Pages_PlateTypes_Delete)]
        public async Task Delete(EntityDto input)
        {
            await _plateTypeRepository.DeleteAsync(input.Id);
        }

        [AbpAuthorize(AppPermissions.Pages_PlateTypes_Create)]
        protected virtual async Task Create(CreateOrEditPlateTypeDto input)
        {
            var plateType = ObjectMapper.Map<PlateType>(input);

            await _plateTypeRepository.InsertAsync(plateType);
        }

        [AbpAuthorize(AppPermissions.Pages_PlateTypes_Edit)]
        protected virtual async Task Update(CreateOrEditPlateTypeDto input)
        {
            var plateType = await _plateTypeRepository.FirstOrDefaultAsync((int)input.Id);
            ObjectMapper.Map(input, plateType);
        }
        private async Task CheckIfNameExists(CreateOrEditPlateTypeDto input)
        {
            var nameExists = await _plateTypeRepository.FirstOrDefaultAsync(x => x.DisplayName.ToLower() == input.DisplayName.ToLower());
            if (nameExists != null)
            {
                throw new UserFriendlyException(L("CannotCreateDuplicatedNameMessage"));
            }
        }

    }
}