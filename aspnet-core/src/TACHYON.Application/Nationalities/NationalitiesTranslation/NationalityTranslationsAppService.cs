using TACHYON.Nationalities;

using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using TACHYON.Nationalities.NationalitiesTranslation.Dtos;
using TACHYON.Dto;
using Abp.Application.Services.Dto;
using TACHYON.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;

namespace TACHYON.Nationalities.NationalitiesTranslation
{
    [AbpAuthorize(AppPermissions.Pages_NationalityTranslations)]
    public class NationalityTranslationsAppService : TACHYONAppServiceBase, INationalityTranslationsAppService
    {
        private readonly IRepository<NationalityTranslation> _nationalityTranslationRepository;
        private readonly IRepository<Nationality, int> _lookup_nationalityRepository;

        public NationalityTranslationsAppService(IRepository<NationalityTranslation> nationalityTranslationRepository, IRepository<Nationality, int> lookup_nationalityRepository)
        {
            _nationalityTranslationRepository = nationalityTranslationRepository;
            _lookup_nationalityRepository = lookup_nationalityRepository;

        }

        public async Task<PagedResultDto<GetNationalityTranslationForViewDto>> GetAll(GetAllNationalityTranslationsInput input)
        {

            var filteredNationalityTranslations = _nationalityTranslationRepository.GetAll()
                        .Include(e => e.Core)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.TranslatedName.Contains(input.Filter) || e.Language.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.TranslatedNameFilter), e => e.TranslatedName == input.TranslatedNameFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.LanguageFilter), e => e.Language == input.LanguageFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.NationalityNameFilter), e => e.Core != null && e.Core.Name == input.NationalityNameFilter)
                        .WhereIf(input.NationalityIdFilter.HasValue, e => false || e.CoreId == input.NationalityIdFilter.Value);

            var pagedAndFilteredNationalityTranslations = filteredNationalityTranslations
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

            var nationalityTranslations = from o in pagedAndFilteredNationalityTranslations
                                          join o1 in _lookup_nationalityRepository.GetAll() on o.CoreId equals o1.Id into j1
                                          from s1 in j1.DefaultIfEmpty()

                                          select new GetNationalityTranslationForViewDto()
                                          {
                                              NationalityTranslation = new NationalityTranslationDto
                                              {
                                                  TranslatedName = o.TranslatedName,
                                                  Language = o.Language,
                                                  Id = o.Id
                                              },
                                              NationalityName = s1 == null || s1.Name == null ? "" : s1.Name.ToString()
                                          };

            var totalCount = await filteredNationalityTranslations.CountAsync();

            return new PagedResultDto<GetNationalityTranslationForViewDto>(
                totalCount,
                await nationalityTranslations.ToListAsync()
            );
        }

        public async Task<GetNationalityTranslationForViewDto> GetNationalityTranslationForView(int id)
        {
            var nationalityTranslation = await _nationalityTranslationRepository.GetAsync(id);

            var output = new GetNationalityTranslationForViewDto { NationalityTranslation = ObjectMapper.Map<NationalityTranslationDto>(nationalityTranslation) };

            if (output.NationalityTranslation.CoreId != null)
            {
                var _lookupNationality = await _lookup_nationalityRepository.FirstOrDefaultAsync((int)output.NationalityTranslation.CoreId);
                output.NationalityName = _lookupNationality?.Name?.ToString();
            }

            return output;
        }

        [AbpAuthorize(AppPermissions.Pages_NationalityTranslations_Edit)]
        public async Task<GetNationalityTranslationForEditOutput> GetNationalityTranslationForEdit(EntityDto input)
        {
            var nationalityTranslation = await _nationalityTranslationRepository.FirstOrDefaultAsync(input.Id);

            var output = new GetNationalityTranslationForEditOutput { NationalityTranslation = ObjectMapper.Map<CreateOrEditNationalityTranslationDto>(nationalityTranslation) };

            if (output.NationalityTranslation.CoreId != null)
            {
                var _lookupNationality = await _lookup_nationalityRepository.FirstOrDefaultAsync((int)output.NationalityTranslation.CoreId);
                output.NationalityName = _lookupNationality?.Name?.ToString();
            }

            return output;
        }

        public async Task CreateOrEdit(CreateOrEditNationalityTranslationDto input)
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

        [AbpAuthorize(AppPermissions.Pages_NationalityTranslations_Create)]
        protected virtual async Task Create(CreateOrEditNationalityTranslationDto input)
        {
            var nationalityTranslation = ObjectMapper.Map<NationalityTranslation>(input);

            await _nationalityTranslationRepository.InsertAsync(nationalityTranslation);
        }

        [AbpAuthorize(AppPermissions.Pages_NationalityTranslations_Edit)]
        protected virtual async Task Update(CreateOrEditNationalityTranslationDto input)
        {
            var nationalityTranslation = await _nationalityTranslationRepository.FirstOrDefaultAsync((int)input.Id);
            ObjectMapper.Map(input, nationalityTranslation);
        }

        [AbpAuthorize(AppPermissions.Pages_NationalityTranslations_Delete)]
        public async Task Delete(EntityDto input)
        {
            await _nationalityTranslationRepository.DeleteAsync(input.Id);
        }
        [AbpAuthorize(AppPermissions.Pages_NationalityTranslations)]
        public async Task<List<NationalityTranslationNationalityLookupTableDto>> GetAllNationalityForTableDropdown()
        {
            return await _lookup_nationalityRepository.GetAll()
                .Select(nationality => new NationalityTranslationNationalityLookupTableDto
                {
                    Id = nationality.Id,
                    DisplayName = nationality == null || nationality.Name == null ? "" : nationality.Name.ToString()
                }).ToListAsync();
        }

    }
}