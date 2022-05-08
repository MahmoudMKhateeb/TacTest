using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Abp.UI;
using AutoMapper.QueryableExtensions;
using DevExtreme.AspNet.Data.ResponseModel;
using Microsoft.EntityFrameworkCore;
using System;
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
    public class GetAllTranslationsInput
    {

        public string LoadOptions { get; set; }
        public string CoreId { get; set; }
    }

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

        public async Task<LoadResult> GetAll(GetAllUnitOfMeasuresInput input)
        {
            var filteredUnitOfMeasures = _unitOfMeasureRepository.GetAll()
                .ProjectTo<UnitOfMeasureDto>(AutoMapperConfigurationProvider);

            return await LoadResultAsync(filteredUnitOfMeasures, input.LoadOptions);
        }

        public async Task<GetUnitOfMeasureForViewDto> GetUnitOfMeasureForView(int id)
        {
            var unitOfMeasure = await _unitOfMeasureRepository.GetAsync(id);

            var output =
                new GetUnitOfMeasureForViewDto { UnitOfMeasure = ObjectMapper.Map<UnitOfMeasureDto>(unitOfMeasure) };

            return output;
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_UnitOfMeasures_Edit)]
        public async Task<GetUnitOfMeasureForEditOutput> GetUnitOfMeasureForEdit(EntityDto input)
        {
            var unitOfMeasure = await _unitOfMeasureRepository.FirstOrDefaultAsync(input.Id);

            var output = new GetUnitOfMeasureForEditOutput
            {
                UnitOfMeasure = ObjectMapper.Map<CreateOrEditUnitOfMeasureDto>(unitOfMeasure)
            };

            return output;
        }

        public async Task CreateOrEdit(CreateOrEditUnitOfMeasureDto input)
        {

            await IsUnitOfMeasureKeyDuplicatedOrEmpty(input);

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
                .FirstOrDefaultAsync(x => x.Id == (int)input.Id);

            if (unitOfMeasure.Key.ToLower().Contains(TACHYONConsts.OthersDisplayName)
                && !input.Key.ToLower().Contains(TACHYONConsts.OthersDisplayName))
                throw new UserFriendlyException(L("OtherUnitOfMeasureMustContainTheOtherWord"));
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
            await _unitOfMeasureRepository.DeleteAsync(unitOfMeasure);
        }

        public async Task<List<GetAllUnitOfMeasureForDropDownOutput>> GetAllUnitOfMeasuresForDropdown()
        {
            var unitOfMeasures = await _unitOfMeasureRepository.GetAll()
                .Include(x => x.Translations)
                .ToListAsync();
            return ObjectMapper.Map<List<GetAllUnitOfMeasureForDropDownOutput>>(unitOfMeasures);
        }


        private async Task IsUnitOfMeasureKeyDuplicatedOrEmpty(CreateOrEditUnitOfMeasureDto input)
        {
            if (await _unitOfMeasureRepository.GetAll().AnyAsync(x => x.Key == input.Key && x.Id != input.Id))
            {
                throw new UserFriendlyException(L("UintOfMeasureKeyCanNotBeDuplicated"));
            }

        }


        #region MultiLingual

        public async Task CreateOrEditTranslation(UnitOfmeasureTranslationDto input)
        {

            var translation = await _unitOfMeasureTranslationRepository.FirstOrDefaultAsync(x => x.Id == input.Id);
            if (translation == null)
            {
                var newTranslation = ObjectMapper.Map<UnitOfMeasureTranslation>(input);
                await _unitOfMeasureTranslationRepository.InsertAsync(newTranslation);
            }
            else
            {
                var duplication = await _unitOfMeasureTranslationRepository.FirstOrDefaultAsync(x => x.CoreId == translation.CoreId && x.Language.Contains(translation.Language) && x.Id != translation.Id);
                if (duplication != null)
                {
                    throw new UserFriendlyException(
                        "The translation for this language already exists, you can modify it");
                }
                ObjectMapper.Map(input, translation);
            }
        }

        public async Task<LoadResult> GetAllTranslations(GetAllTranslationsInput input)
        {
            var filteredPackingTypes = _unitOfMeasureTranslationRepository
                .GetAll()
                .Where(x => x.CoreId == Convert.ToInt32(input.CoreId))
                .ProjectTo<UnitOfmeasureTranslationDto>(AutoMapperConfigurationProvider);

            return await LoadResultAsync(filteredPackingTypes, input.LoadOptions);
        }


        public async Task DeleteTranslation(EntityDto input)
        {
            await _unitOfMeasureTranslationRepository.DeleteAsync(input.Id);
        }

        #endregion

    }
}