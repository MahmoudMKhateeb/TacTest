using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using TACHYON.Packing.PackingTypes.Dtos;
using Abp.Application.Services.Dto;
using TACHYON.Authorization;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;

namespace TACHYON.Packing.PackingTypes
{
    [AbpAuthorize(AppPermissions.Pages_PackingTypes)]
    public class PackingTypesAppService : TACHYONAppServiceBase, IPackingTypesAppService
    {
        private readonly IRepository<PackingType> _packingTypeRepository;
        private readonly IRepository<PackingTypeTranslation> _packingTypeTranslationRepository;

        public PackingTypesAppService(IRepository<PackingType> packingTypeRepository, IRepository<PackingTypeTranslation> packingTypeTranslationRepository)
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
            var packingType = await _packingTypeRepository.GetAll().AsNoTracking()
                .Include(x=>x.Translations)
                .FirstOrDefaultAsync(x=> x.Id == id);

            // TODO Add Localization Here

            if (packingType == null)
                throw new UserFriendlyException(L("PackingTypeWithId"+id+"NotFound"));

            var output = new GetPackingTypeForViewDto
            {
                PackingTypeTranslations =
                    ObjectMapper.Map<List<PackingTypeTranslationDto>>(packingType)
            };

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
            await IsPackingTypeDuplicatedOrEmpty(input.DisplayName);

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

            using (var unitOfWork = UnitOfWorkManager.Begin())
            {
               var coreId = await _packingTypeRepository.InsertAndGetIdAsync(packingType);

                var packingTypeTranslations = ObjectMapper.Map<List<PackingTypeTranslation>>(input.TranslationDtos);

                foreach (var ptt in packingTypeTranslations)
                {
                    ptt.CoreId = coreId;
                    await _packingTypeTranslationRepository.InsertAsync(ptt);
                }

                await unitOfWork.CompleteAsync();
            }
        }

        [AbpAuthorize(AppPermissions.Pages_PackingTypes_Edit)]
        protected virtual async Task Update(CreateOrEditPackingTypeDto input)
        {
            var packingType = await _packingTypeRepository.FirstOrDefaultAsync(input.Id.Value);

            packingType.Translations.Clear();

            ObjectMapper.Map(input, packingType);
        }

        [AbpAuthorize(AppPermissions.Pages_PackingTypes_Delete)]
        public async Task Delete(EntityDto input)
        {
            await _packingTypeRepository.DeleteAsync(input.Id);
        }


        private async Task IsPackingTypeDuplicatedOrEmpty(string displayName)
        {
            if (displayName.IsNullOrEmpty() || displayName.IsNullOrWhiteSpace())
                throw new UserFriendlyException(L("PackingTypeNameCanNotBeEmpty"));


            var isDuplicated = await _packingTypeRepository.GetAll()
                .AnyAsync(x => x.DisplayName.ToUpper().Equals(displayName.ToUpper()));

            if (isDuplicated)
                throw new UserFriendlyException(L("PackingTypeNameCanNotBeDuplicated"));
        }
    }
}