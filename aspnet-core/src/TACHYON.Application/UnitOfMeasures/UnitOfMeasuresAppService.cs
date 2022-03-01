using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using TACHYON.Authorization;
using TACHYON.Dto;
using TACHYON.Extension;
using TACHYON.UnitOfMeasures.Dtos;

namespace TACHYON.UnitOfMeasures
{
    [AbpAuthorize(AppPermissions.Pages_Administration_UnitOfMeasures)]
    public class UnitOfMeasuresAppService : TACHYONAppServiceBase, IUnitOfMeasuresAppService
    {
        private readonly IRepository<UnitOfMeasure> _unitOfMeasureRepository;


        public UnitOfMeasuresAppService(IRepository<UnitOfMeasure> unitOfMeasureRepository)
        {
            _unitOfMeasureRepository = unitOfMeasureRepository;

        }

        public async Task<PagedResultDto<GetUnitOfMeasureForViewDto>> GetAll(GetAllUnitOfMeasuresInput input)
        {

            var filteredUnitOfMeasures = _unitOfMeasureRepository.GetAll()
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.DisplayName.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.DisplayNameFilter), e => e.DisplayName == input.DisplayNameFilter);

            var pagedAndFilteredUnitOfMeasures = filteredUnitOfMeasures
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

            var unitOfMeasures = from o in pagedAndFilteredUnitOfMeasures
                                 select new GetUnitOfMeasureForViewDto()
                                 {
                                     UnitOfMeasure = new UnitOfMeasureDto
                                     {
                                         DisplayName = o.DisplayName,
                                         Id = o.Id
                                     }
                                 };

            var totalCount = await filteredUnitOfMeasures.CountAsync();

            return new PagedResultDto<GetUnitOfMeasureForViewDto>(
                totalCount,
                await unitOfMeasures.ToListAsync()
            );
        }

        public async Task<GetUnitOfMeasureForViewDto> GetUnitOfMeasureForView(int id)
        {
            var unitOfMeasure = await _unitOfMeasureRepository.GetAsync(id);

            var output = new GetUnitOfMeasureForViewDto { UnitOfMeasure = ObjectMapper.Map<UnitOfMeasureDto>(unitOfMeasure) };

            return output;
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_UnitOfMeasures_Edit)]
        public async Task<GetUnitOfMeasureForEditOutput> GetUnitOfMeasureForEdit(EntityDto input)
        {
            var unitOfMeasure = await _unitOfMeasureRepository.FirstOrDefaultAsync(input.Id);

            var output = new GetUnitOfMeasureForEditOutput { UnitOfMeasure = ObjectMapper.Map<CreateOrEditUnitOfMeasureDto>(unitOfMeasure) };

            return output;
        }

        public async Task CreateOrEdit(CreateOrEditUnitOfMeasureDto input)
        {

            await IsUintOfMeasureNameDuplicatedOrEmpty(input.DisplayName);

            if (input.Id == null)
            {
                await Create(input);
            }
            else
            {
                await Update(input);
            }
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_UnitOfMeasures_Create)]
        protected virtual async Task Create(CreateOrEditUnitOfMeasureDto input)
        {
            var unitOfMeasure = ObjectMapper.Map<UnitOfMeasure>(input);



            await _unitOfMeasureRepository.InsertAsync(unitOfMeasure);
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_UnitOfMeasures_Edit)]
        protected virtual async Task Update(CreateOrEditUnitOfMeasureDto input)
        {
            var unitOfMeasure = await _unitOfMeasureRepository.FirstOrDefaultAsync((int)input.Id);

            if (unitOfMeasure.DisplayName.ToLower().Contains(TACHYONConsts.OthersDisplayName)
                && !input.DisplayName.ToLower().Contains(TACHYONConsts.OthersDisplayName))
                throw new UserFriendlyException(L("OtherUnitOfMeasureMustContainTheOtherWord"));

            ObjectMapper.Map(input, unitOfMeasure);
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_UnitOfMeasures_Delete)]
        public async Task Delete(EntityDto input)
        {
            var unitOfMeasure = await _unitOfMeasureRepository.SingleAsync(x => x.Id == input.Id);

            if (unitOfMeasure.DisplayName.ToLower().Contains(TACHYONConsts.OthersDisplayName))
                throw new UserFriendlyException(L("OtherUnitOfMeasureNotRemovable"));

            await _unitOfMeasureRepository.DeleteAsync(unitOfMeasure);
        }

        public async Task<List<SelectItemDto>> GetAllUnitOfMeasuresForDropdown()
        {
            return await _unitOfMeasureRepository.GetAll()
                .Select(x => new SelectItemDto()
                {
                    Id = x.Id.ToString(),
                    DisplayName = x.DisplayName,
                    IsOther = x.DisplayName.ToLower().Contains(TACHYONConsts.OthersDisplayName)
                }).ToListAsync();
        }


        private async Task IsUintOfMeasureNameDuplicatedOrEmpty(string displayName)
        {
            if (displayName.IsNullOrEmpty() || displayName.IsNullOrWhiteSpace())
                throw new UserFriendlyException(L("UintOfMeasureNameCanNotBeEmpty"));

            var isDuplicated = await _unitOfMeasureRepository.GetAll()
                .AnyAsync(x => x.DisplayName.ToUpper().Equals(displayName.ToUpper()));

            if (isDuplicated)
                throw new UserFriendlyException(L("UintOfMeasureNameCanNotBeDuplicated"));
        }

    }
}