using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using TACHYON.Authorization;
using TACHYON.Dto;
using TACHYON.Trucks.PlateTypes.Dtos;

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

        public async Task<PagedResultDto<PlateTypeDto>> GetAll(GetAllPlateTypesInput input)
        {

            var filteredPlateTypes = _plateTypeRepository.GetAll().Include(x => x.Translations)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.Translations.Any(x => x.DisplayName.Contains(input.Filter)));

            var pagedAndFilteredPlateTypes = filteredPlateTypes
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

            var plateTypes = ObjectMapper.Map<List<PlateTypeDto>>(await pagedAndFilteredPlateTypes.ToListAsync());
            //var plateTypes = from o in pagedAndFilteredPlateTypes
            //                 select new GetPlateTypeForViewDto()
            //                 {
            //                     PlateType = new PlateTypeDto
            //                     {
            //                         DisplayName = o.DisplayName,
            //                         Id = o.Id
            //                     }
            //                 };

            var totalCount = await filteredPlateTypes.CountAsync();

            return new PagedResultDto<PlateTypeDto>(
                totalCount,
                plateTypes
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
            var plateType = await _plateTypeRepository.GetAllIncluding(x => x.Translations)
                .FirstOrDefaultAsync(x => x.Id == input.Id);

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
            var plateType = await _plateTypeRepository.GetAllIncluding(x => x.Translations)
                .FirstOrDefaultAsync(x => x.Id == (int)input.Id);
            plateType.Translations.Clear();
            ObjectMapper.Map(input, plateType);
        }
        private async Task CheckIfNameExists(CreateOrEditPlateTypeDto input)
        {
            foreach (var item in input.Translations)
            {
                var nameExists = await _plateTypeRepository.FirstOrDefaultAsync(x =>
                x.Translations.Any(i => i.Language == item.Language &&
                i.DisplayName.ToLower().Equals(item.DisplayName.ToLower())) &&
                x.Id != input.Id);
                if (nameExists != null)
                {
                    throw new UserFriendlyException(L("CannotCreateDuplicatedNameMessage"));
                }
            }
        }

    }
}