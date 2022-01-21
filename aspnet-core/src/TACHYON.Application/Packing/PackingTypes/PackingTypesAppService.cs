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
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using TACHYON.Authorization;
using TACHYON.Packing.PackingTypes.Dtos;

namespace TACHYON.Packing.PackingTypes
{
    public class GetAllTranslationsInput
    {
        public string LoadOptions { get; set; }
        public string CoreId { get; set; }
    }

    [AbpAuthorize(AppPermissions.Pages_PackingTypes)]
    public class PackingTypesAppService : TACHYONAppServiceBase, IPackingTypesAppService
    {
        private readonly IRepository<PackingType> _packingTypeRepository;
        private readonly IRepository<PackingTypeTranslation> _packingTypeTranslationRepository;

        public PackingTypesAppService(IRepository<PackingType> packingTypeRepository,
            IRepository<PackingTypeTranslation> packingTypeTranslationRepository)
        {
            _packingTypeRepository = packingTypeRepository;
            _packingTypeTranslationRepository = packingTypeTranslationRepository;
        }

        public async Task<LoadResult> GetAll(GetAllPackingTypesInput input)
        {
            DisableTenancyFiltersIfHost();
            var filteredPackingTypes = _packingTypeRepository.GetAll()
                .ProjectTo<PackingTypeDto>(AutoMapperConfigurationProvider);


            return await LoadResultAsync(filteredPackingTypes, input.LoadOptions);
        }

        public async Task<GetPackingTypeForViewDto> GetPackingTypeForView(int id)
        {
            var packingType = await _packingTypeRepository
                .GetAll()
                .AsNoTracking()
                .Include(x => x.Translations)
                .FirstOrDefaultAsync(x => x.Id == id);


            if (packingType == null)
                throw new UserFriendlyException(L("PackingTypeWithId" + id + "NotFound"));

            var output = new GetPackingTypeForViewDto { PackingType = ObjectMapper.Map<PackingTypeDto>(packingType) };

            return output;
        }

        [AbpAuthorize(AppPermissions.Pages_PackingTypes_Edit)]
        public async Task<GetPackingTypeForEditOutput> GetPackingTypeForEdit(EntityDto input)
        {
            var packingType = await _packingTypeRepository.FirstOrDefaultAsync(input.Id);

            var output = new GetPackingTypeForEditOutput
            {
                PackingType = ObjectMapper.Map<CreateOrEditPackingTypeDto>(packingType)
            };

            return output;
        }

        public async Task CreateOrEdit(CreateOrEditPackingTypeDto input)
        {
            await IsPackingTypeDuplicatedOrEmpty(input);

            if (input.Id == null)
            {
                await Create(input);
            }
            else
            {
                await Update(input);
            }
        }

        [AbpAuthorize(AppPermissions.Pages_PackingTypes_Create)]
        protected virtual async Task Create(CreateOrEditPackingTypeDto input)
        {
            // TO DO Ignore Translation List Mapping  ===> Done

            var packingType = ObjectMapper.Map<PackingType>(input);

            await _packingTypeRepository.InsertAsync(packingType);
        }

        [AbpAuthorize(AppPermissions.Pages_PackingTypes_Edit)]
        protected virtual async Task Update(CreateOrEditPackingTypeDto input)
        {
            var packingType = await _packingTypeRepository.FirstOrDefaultAsync(input.Id.Value);

            // packingType.Translations.Clear();

            ObjectMapper.Map(input, packingType);
        }

        [AbpAuthorize(AppPermissions.Pages_PackingTypes_Delete)]
        public async Task Delete(EntityDto input)
        {
            await _packingTypeRepository.DeleteAsync(input.Id);
        }


        private async Task IsPackingTypeDuplicatedOrEmpty(CreateOrEditPackingTypeDto input)
        {
            if (input.DisplayName.IsNullOrEmpty() || input.DisplayName.IsNullOrWhiteSpace())
                throw new UserFriendlyException(L("PackingTypeNameCanNotBeEmpty"));


            var isDuplicated = await _packingTypeRepository.GetAll()
                .Where(x => x.Id != input.Id)
                .AnyAsync(x => x.DisplayName.ToUpper().Equals(input.DisplayName.ToUpper()));

            if (isDuplicated)
                throw new UserFriendlyException(L("PackingTypeNameCanNotBeDuplicated"));
        }


        #region MultiLingual

        public async Task CreateOrEditTranslation(PackingTypeTranslationDto input)
        {
            var translation = await _packingTypeTranslationRepository.FirstOrDefaultAsync(x => x.Id == input.Id);
            if (translation == null)
            {
                var newTranslation = ObjectMapper.Map<PackingTypeTranslation>(input);
                await _packingTypeTranslationRepository.InsertAsync(newTranslation);
            }
            else
            {
                var duplication = await _packingTypeTranslationRepository.FirstOrDefaultAsync(x =>
                    x.CoreId == translation.CoreId && x.Language.Contains(translation.Language) &&
                    x.Id != translation.Id);
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
            var filteredPackingTypes = _packingTypeTranslationRepository
                .GetAll()
                .Where(x => x.CoreId == Convert.ToInt32(input.CoreId))
                .ProjectTo<PackingTypeTranslationDto>(AutoMapperConfigurationProvider);

            return await LoadResultAsync(filteredPackingTypes, input.LoadOptions);
        }


        public async Task DeleteTranslation(EntityDto input)
        {
            await _packingTypeTranslationRepository.DeleteAsync(input.Id);
        }

        #endregion
    }
}