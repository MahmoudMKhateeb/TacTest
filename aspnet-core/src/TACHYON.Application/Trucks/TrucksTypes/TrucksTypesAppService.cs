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
using System.Globalization;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using TACHYON.Authorization;
using TACHYON.Common;
using TACHYON.Common.Dto;
using TACHYON.Dto;
using TACHYON.Trucks.TruckCategories.TransportTypes;
using TACHYON.Trucks.TruckCategories.TransportTypes.Dtos;
using TACHYON.Trucks.TrucksTypes.Dtos;
using TACHYON.Trucks.TrucksTypes.TrucksTypesTranslations;
using TACHYON.Trucks.TrucksTypes.TrucksTypesTranslations.Dtos;

namespace TACHYON.Trucks.TrucksTypes
{
    [AbpAuthorize(AppPermissions.Pages_TrucksTypes)]
    public class TrucksTypesAppService : TACHYONAppServiceBase, ITrucksTypesAppService
    {
        private readonly IRepository<TrucksType, long> _trucksTypeRepository;

        private readonly IRepository<TrucksTypesTranslation> _trucksTypeTranslationRepository;

        //! Don't Forget Mapping Configurations
        private readonly IRepository<TransportType, int> _transportTypeRepository;

        public TrucksTypesAppService(IRepository<TrucksType, long> trucksTypeRepository,
            IRepository<TransportType, int> transportTypeRepository,
            IRepository<TrucksTypesTranslation> trucksTypeTranslationRepository)
        {
            _trucksTypeRepository = trucksTypeRepository;
            _transportTypeRepository = transportTypeRepository;
            _trucksTypeTranslationRepository = trucksTypeTranslationRepository;
        }

        public async Task<PagedResultDto<GetTrucksTypeForViewDto>> GetAll(GetAllTrucksTypesInput input)
        {
            var filteredTrucksTypes = _trucksTypeRepository.GetAll()
                .Include(x => x.Translations)
                .Include(x => x.TransportTypeFk)
                .ThenInclude(x => x.Translations)
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                    e => false || e.Translations.Any(x => x.TranslatedDisplayName.Contains(input.Filter)))
                .WhereIf(!string.IsNullOrWhiteSpace(input.DisplayNameFilter),
                    e => e.Translations.Any(x => x.TranslatedDisplayName.Contains(input.DisplayNameFilter)));

            var pagedAndFilteredTrucksTypes = filteredTrucksTypes
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

            var trucksTypes =
                ObjectMapper.Map<List<GetTrucksTypeForViewDto>>(await pagedAndFilteredTrucksTypes.ToListAsync());

            var totalCount = await filteredTrucksTypes.CountAsync();

            return new PagedResultDto<GetTrucksTypeForViewDto>(
                totalCount,
                trucksTypes.ToList()
            );
        }

        public async Task<LoadResult> DxGetAll(LoadOptionsInput input)
        {
            var trucksTypes = _trucksTypeRepository.GetAll().AsNoTracking()
                .ProjectTo<TrucksTypeDto>(AutoMapperConfigurationProvider);

            return await LoadResultAsync(trucksTypes, input.LoadOptions);
        }

        public async Task<GetTrucksTypeForViewDto> GetTrucksTypeForView(long id)
        {
            var trucksType = await _trucksTypeRepository.GetAsync(id);

            var output = new GetTrucksTypeForViewDto { TrucksType = ObjectMapper.Map<TrucksTypeDto>(trucksType) };

            return output;
        }

        [AbpAuthorize(AppPermissions.Pages_TrucksTypes_Edit)]
        public async Task<GetTrucksTypeForEditOutput> GetTrucksTypeForEdit(EntityDto<long> input)
        {
            var trucksType = await _trucksTypeRepository.GetAllIncluding(x => x.Translations)
                .FirstOrDefaultAsync(x => x.Id == input.Id);

            var output = new GetTrucksTypeForEditOutput
            {
                TrucksType = ObjectMapper.Map<CreateOrEditTrucksTypeDto>(trucksType)
            };

            return output;
        }

        public async Task CreateOrEdit(CreateOrEditTrucksTypeDto input)
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

        [AbpAuthorize(AppPermissions.Pages_TrucksTypes_Create)]
        protected virtual async Task Create(CreateOrEditTrucksTypeDto input)
        {
            var trucksType = ObjectMapper.Map<TrucksType>(input);
            trucksType.Key = input.DisplayName;
            await _trucksTypeRepository.InsertAsync(trucksType);
        }

        [AbpAuthorize(AppPermissions.Pages_TrucksTypes_Edit)]
        protected virtual async Task Update(CreateOrEditTrucksTypeDto input)
        {
            var trucksType = await _trucksTypeRepository.FirstOrDefaultAsync(x => x.Id == input.Id.Value);
            //trucksType.Translations.Clear();
            ObjectMapper.Map(input, trucksType);
        }

        //[AbpAuthorize(AppPermissions.Pages_TrucksTypes_Delete)]
        //public async Task Delete(EntityDto<long> input)
        //{
        //    await _trucksTypeRepository.DeleteAsync(input.Id);
        //}

        public async Task<IEnumerable<ISelectItemDto>> GetAllTransportTypeForTableDropdown()
        {
            List<TransportType> transportTypes = await _transportTypeRepository
                .GetAllIncluding(x => x.Translations)
                .ToListAsync();

            List<TransportTypeSelectItemDto> transportTypeDtos =
                ObjectMapper.Map<List<TransportTypeSelectItemDto>>(transportTypes);

            return transportTypeDtos;
        }

        [AbpAuthorize(AppPermissions.Pages_TrucksTypesTranslations)]
        public async Task<LoadResult> GetAllTranslations(GetAllTranslationInput<long> input)
        {
            var translations = _trucksTypeTranslationRepository
                .GetAll().Where(x => x.CoreId == input.CoreId)
                .AsNoTracking().ProjectTo<TrucksTypesTranslationDto>(AutoMapperConfigurationProvider);

            return await LoadResultAsync(translations, input.LoadOptions);
        }

        public async Task CreateOrEditTranslation(CreateOrEditTrucksTypesTranslationDto input)
        {


            if (!input.Id.HasValue)
            {
                var d = await _trucksTypeTranslationRepository
                    .GetAll()
                    .Where(x => x.CoreId == input.CoreId)
                    .Where(x => x.TranslatedDisplayName == input.TranslatedDisplayName)
                    .Where(x => x.Language.Contains(input.Language))
                    .FirstOrDefaultAsync();
                if (d != null)
                {
                    throw new UserFriendlyException(L("TranslationDuplicated"));
                }

                var createdTranslation = ObjectMapper.Map<TrucksTypesTranslation>(input);
                createdTranslation.DisplayName = input.TranslatedDisplayName;
                await _trucksTypeTranslationRepository.InsertAsync(createdTranslation);
            }
            else
            {
                TrucksTypesTranslation updatedTranslation = await _trucksTypeTranslationRepository.SingleAsync(x => x.Id == input.Id);
                ObjectMapper.Map(input, updatedTranslation);
            }

        }

        [AbpAuthorize(AppPermissions.Pages_TrucksTypesTranslations_Create)]
        protected virtual async Task CreateTranslation(CreateOrEditTrucksTypesTranslationDto input)
        {
            var translation = ObjectMapper.Map<TrucksTypesTranslation>(input);

            await _trucksTypeTranslationRepository.InsertAsync(translation);
        }

        [AbpAuthorize(AppPermissions.Pages_TrucksTypesTranslations_Edit)]
        protected virtual async Task UpdateTranslation(CreateOrEditTrucksTypesTranslationDto input)
        {
            var translation = await _trucksTypeTranslationRepository
                .SingleAsync(x => x.Id == input.Id.Value);

            ObjectMapper.Map(translation, input);
        }

        [AbpAuthorize(AppPermissions.Pages_TrucksTypesTranslations_Delete)]
        public async Task DeleteTranslation(EntityDto input)
        {
            await _trucksTypeTranslationRepository.DeleteAsync(input.Id);
        }
    }
}