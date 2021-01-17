using TACHYON.Trucks.TrucksTypes;

using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using TACHYON.Trucks.TrucksTypes.TrucksTypesTranslations.Dtos;
using TACHYON.Dto;
using Abp.Application.Services.Dto;
using TACHYON.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;

namespace TACHYON.Trucks.TrucksTypes.TrucksTypesTranslations
{
    [AbpAuthorize(AppPermissions.Pages_TrucksTypesTranslations)]
    public class TrucksTypesTranslationsAppService : TACHYONAppServiceBase, ITrucksTypesTranslationsAppService
    {
        private readonly IRepository<TrucksTypesTranslation> _trucksTypesTranslationRepository;
        private readonly IRepository<TrucksType, long> _lookup_trucksTypeRepository;

        public TrucksTypesTranslationsAppService(IRepository<TrucksTypesTranslation> trucksTypesTranslationRepository, IRepository<TrucksType, long> lookup_trucksTypeRepository)
        {
            _trucksTypesTranslationRepository = trucksTypesTranslationRepository;
            _lookup_trucksTypeRepository = lookup_trucksTypeRepository;

        }

        public async Task<PagedResultDto<GetTrucksTypesTranslationForViewDto>> GetAll(GetAllTrucksTypesTranslationsInput input)
        {

            var filteredTrucksTypesTranslations = _trucksTypesTranslationRepository.GetAll()
                        .Include(e => e.Core)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.TranslatedDisplayName.Contains(input.Filter) || e.Language.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.TranslatedDisplayNameFilter), e => e.TranslatedDisplayName == input.TranslatedDisplayNameFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.LanguageFilter), e => e.Language == input.LanguageFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.TrucksTypeDisplayNameFilter), e => e.Core != null && e.Core.DisplayName == input.TrucksTypeDisplayNameFilter);

            var pagedAndFilteredTrucksTypesTranslations = filteredTrucksTypesTranslations
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

            var trucksTypesTranslations = from o in pagedAndFilteredTrucksTypesTranslations
                                          join o1 in _lookup_trucksTypeRepository.GetAll() on o.CoreId equals o1.Id into j1
                                          from s1 in j1.DefaultIfEmpty()

                                          select new GetTrucksTypesTranslationForViewDto()
                                          {
                                              TrucksTypesTranslation = new TrucksTypesTranslationDto
                                              {
                                                  TranslatedDisplayName = o.TranslatedDisplayName,
                                                  Language = o.Language,
                                                  Id = o.Id
                                              },
                                              TrucksTypeDisplayName = s1 == null || s1.DisplayName == null ? "" : s1.DisplayName.ToString()
                                          };

            var totalCount = await filteredTrucksTypesTranslations.CountAsync();

            return new PagedResultDto<GetTrucksTypesTranslationForViewDto>(
                totalCount,
                await trucksTypesTranslations.ToListAsync()
            );
        }

        public async Task<GetTrucksTypesTranslationForViewDto> GetTrucksTypesTranslationForView(int id)
        {
            var trucksTypesTranslation = await _trucksTypesTranslationRepository.GetAsync(id);

            var output = new GetTrucksTypesTranslationForViewDto { TrucksTypesTranslation = ObjectMapper.Map<TrucksTypesTranslationDto>(trucksTypesTranslation) };

            if (output.TrucksTypesTranslation.CoreId != null)
            {
                var _lookupTrucksType = await _lookup_trucksTypeRepository.FirstOrDefaultAsync((long)output.TrucksTypesTranslation.CoreId);
                output.TrucksTypeDisplayName = _lookupTrucksType?.DisplayName?.ToString();
            }

            return output;
        }

        [AbpAuthorize(AppPermissions.Pages_TrucksTypesTranslations_Edit)]
        public async Task<GetTrucksTypesTranslationForEditOutput> GetTrucksTypesTranslationForEdit(EntityDto input)
        {
            var trucksTypesTranslation = await _trucksTypesTranslationRepository.FirstOrDefaultAsync(input.Id);

            var output = new GetTrucksTypesTranslationForEditOutput { TrucksTypesTranslation = ObjectMapper.Map<CreateOrEditTrucksTypesTranslationDto>(trucksTypesTranslation) };

            if (output.TrucksTypesTranslation.CoreId != null)
            {
                var _lookupTrucksType = await _lookup_trucksTypeRepository.FirstOrDefaultAsync((long)output.TrucksTypesTranslation.CoreId);
                output.TrucksTypeDisplayName = _lookupTrucksType?.DisplayName?.ToString();
            }

            return output;
        }

        public async Task CreateOrEdit(CreateOrEditTrucksTypesTranslationDto input)
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

        [AbpAuthorize(AppPermissions.Pages_TrucksTypesTranslations_Create)]
        protected virtual async Task Create(CreateOrEditTrucksTypesTranslationDto input)
        {
            var trucksTypesTranslation = ObjectMapper.Map<TrucksTypesTranslation>(input);

            await _trucksTypesTranslationRepository.InsertAsync(trucksTypesTranslation);
        }

        [AbpAuthorize(AppPermissions.Pages_TrucksTypesTranslations_Edit)]
        protected virtual async Task Update(CreateOrEditTrucksTypesTranslationDto input)
        {
            var trucksTypesTranslation = await _trucksTypesTranslationRepository.FirstOrDefaultAsync((int)input.Id);
            ObjectMapper.Map(input, trucksTypesTranslation);
        }

        [AbpAuthorize(AppPermissions.Pages_TrucksTypesTranslations_Delete)]
        public async Task Delete(EntityDto input)
        {
            await _trucksTypesTranslationRepository.DeleteAsync(input.Id);
        }
        [AbpAuthorize(AppPermissions.Pages_TrucksTypesTranslations)]
        public async Task<List<TrucksTypesTranslationTrucksTypeLookupTableDto>> GetAllTrucksTypeForTableDropdown()
        {
            return await _lookup_trucksTypeRepository.GetAll()
                .Select(trucksType => new TrucksTypesTranslationTrucksTypeLookupTableDto
                {
                    Id = trucksType.Id,
                    DisplayName = trucksType == null || trucksType.DisplayName == null ? "" : trucksType.DisplayName.ToString()
                }).ToListAsync();
        }

    }
}