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
        private readonly IRepository<UnitOfMeasureTranslation> _unitOfMeasureTranslationRepository;


        public UnitOfMeasuresAppService(IRepository<UnitOfMeasure> unitOfMeasureRepository, IRepository<UnitOfMeasureTranslation> unitOfMeasureTranslationRepository)
        {
            _unitOfMeasureRepository = unitOfMeasureRepository;
            _unitOfMeasureTranslationRepository = unitOfMeasureTranslationRepository;
        }

        public async Task<PagedResultDto<GetUnitOfMeasureForViewDto>> GetAll(GetAllUnitOfMeasuresInput input)
        {

            var filteredUnitOfMeasures = _unitOfMeasureRepository.GetAll()
                .Include(x => x.Translations)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.Translations.Any(x => x.DisplayName.Contains(input.Filter)))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.DisplayNameFilter), e => e.Translations.Any(x => x.DisplayName == input.DisplayNameFilter));

            var pagedAndFilteredUnitOfMeasures = filteredUnitOfMeasures
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

            var unitOfMeasures = from o in pagedAndFilteredUnitOfMeasures
                                 select new GetUnitOfMeasureForViewDto()
                                 {
                                     UnitOfMeasure = ObjectMapper.Map<UnitOfMeasureDto>(o)
                                     //UnitOfMeasure = new UnitOfMeasureDto
                                     //{
                                     //    DisplayName = o.DisplayName,
                                     //    Id = o.Id
                                     //}
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

            await IsUnitOfMeasureNameDuplicatedOrEmpty(input.Translations);

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
            var unitOfMeasure = await _unitOfMeasureRepository.GetAll()
                .Include(x => x.Translations)
                .FirstOrDefaultAsync(x => x.Id == (int)input.Id);

            if (unitOfMeasure.Key.ToLower().Contains(TACHYONConsts.OthersDisplayName)
                && !input.Key.ToLower().Contains(TACHYONConsts.OthersDisplayName))
                throw new UserFriendlyException(L("OtherUnitOfMeasureMustContainTheOtherWord"));
            unitOfMeasure.Translations.Clear();
            ObjectMapper.Map(input, unitOfMeasure);
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_UnitOfMeasures_Delete)]
        public async Task Delete(EntityDto input)
        {
            var unitOfMeasure = await _unitOfMeasureRepository
                .GetAll()
                .Include(x => x.Translations)
                .SingleAsync(x => x.Id == input.Id);

            if (unitOfMeasure.Translations.Any(x => x.DisplayName.ToLower().Contains(TACHYONConsts.OthersDisplayName)))
                throw new UserFriendlyException(L("OtherUnitOfMeasureNotRemovable"));
            unitOfMeasure.Translations.Clear();
            await _unitOfMeasureRepository.DeleteAsync(unitOfMeasure);
        }

        public async Task<List<GetAllUnitOfMeasureForDropDownOutput>> GetAllUnitOfMeasuresForDropdown()
        {
            var unitOfMeasures = await _unitOfMeasureRepository.GetAll()
                .Include(x => x.Translations)
                .ToListAsync();
            return ObjectMapper.Map<List<GetAllUnitOfMeasureForDropDownOutput>>(unitOfMeasures);
        }


        private async Task IsUnitOfMeasureNameDuplicatedOrEmpty(ICollection<UnitOfmeasureTranslationDto> unitOfmeasureTranslations)
        {
            var unitOfMeasureTranslationList = await _unitOfMeasureTranslationRepository.GetAll().ToListAsync();
            foreach (var item in unitOfmeasureTranslations)
            {
                if (item.DisplayName.IsNullOrEmpty() || item.DisplayName.IsNullOrWhiteSpace())
                    throw new UserFriendlyException(L("UintOfMeasureNameCanNotBeEmpty"));

                var isDuplicated = unitOfMeasureTranslationList
                    .Any(x => x.DisplayName.ToUpper().Equals(item.DisplayName.ToUpper()));

                if (isDuplicated)
                    throw new UserFriendlyException(L("UintOfMeasureNameCanNotBeDuplicated"));
            }

        }

    }
}