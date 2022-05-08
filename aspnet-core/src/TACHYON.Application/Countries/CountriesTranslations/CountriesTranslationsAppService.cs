using TACHYON.Countries;
using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using TACHYON.Countries.CountriesTranslations.Dtos;
using TACHYON.Dto;
using Abp.Application.Services.Dto;
using TACHYON.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;

namespace TACHYON.Countries.CountriesTranslations
{
    [AbpAuthorize(AppPermissions.Pages_CountriesTranslations)]
    public class CountriesTranslationsAppService : TACHYONAppServiceBase, ICountriesTranslationsAppService
    {
        private readonly IRepository<CountriesTranslation> _countriesTranslationRepository;
        private readonly IRepository<County, int> _lookup_countyRepository;

        public CountriesTranslationsAppService(IRepository<CountriesTranslation> countriesTranslationRepository,
            IRepository<County, int> lookup_countyRepository)
        {
            _countriesTranslationRepository = countriesTranslationRepository;
            _lookup_countyRepository = lookup_countyRepository;
        }

        public async Task<PagedResultDto<GetCountriesTranslationForViewDto>> GetAll(
            GetAllCountriesTranslationsInput input)
        {
            var filteredCountriesTranslations = _countriesTranslationRepository.GetAll()
                .Include(e => e.Core)
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                    e => false || e.TranslatedDisplayName.Contains(input.Filter) || e.Language.Contains(input.Filter))
                .WhereIf(!string.IsNullOrWhiteSpace(input.TranslatedDisplayNameFilter),
                    e => e.TranslatedDisplayName == input.TranslatedDisplayNameFilter)
                .WhereIf(!string.IsNullOrWhiteSpace(input.LanguageFilter), e => e.Language == input.LanguageFilter)
                .WhereIf(!string.IsNullOrWhiteSpace(input.CountyDisplayNameFilter),
                    e => e.Core != null && e.Core.DisplayName == input.CountyDisplayNameFilter);

            var pagedAndFilteredCountriesTranslations = filteredCountriesTranslations
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

            var countriesTranslations = from o in pagedAndFilteredCountriesTranslations
                join o1 in _lookup_countyRepository.GetAll() on o.CoreId equals o1.Id into j1
                from s1 in j1.DefaultIfEmpty()
                select new GetCountriesTranslationForViewDto()
                {
                    CountriesTranslation = new CountriesTranslationDto
                    {
                        TranslatedDisplayName = o.TranslatedDisplayName, Language = o.Language, Id = o.Id
                    },
                    CountyDisplayName = s1 == null || s1.DisplayName == null ? "" : s1.DisplayName.ToString()
                };

            var totalCount = await filteredCountriesTranslations.CountAsync();

            return new PagedResultDto<GetCountriesTranslationForViewDto>(
                totalCount,
                await countriesTranslations.ToListAsync()
            );
        }

        public async Task<GetCountriesTranslationForViewDto> GetCountriesTranslationForView(int id)
        {
            var countriesTranslation = await _countriesTranslationRepository.GetAsync(id);

            var output = new GetCountriesTranslationForViewDto
            {
                CountriesTranslation = ObjectMapper.Map<CountriesTranslationDto>(countriesTranslation)
            };

            if (output.CountriesTranslation.CoreId != null)
            {
                var _lookupCounty =
                    await _lookup_countyRepository.FirstOrDefaultAsync((int)output.CountriesTranslation.CoreId);
                output.CountyDisplayName = _lookupCounty?.DisplayName?.ToString();
            }

            return output;
        }

        [AbpAuthorize(AppPermissions.Pages_CountriesTranslations_Edit)]
        public async Task<GetCountriesTranslationForEditOutput> GetCountriesTranslationForEdit(EntityDto input)
        {
            var countriesTranslation = await _countriesTranslationRepository.FirstOrDefaultAsync(input.Id);

            var output = new GetCountriesTranslationForEditOutput
            {
                CountriesTranslation = ObjectMapper.Map<CreateOrEditCountriesTranslationDto>(countriesTranslation)
            };

            if (output.CountriesTranslation.CoreId != null)
            {
                var _lookupCounty =
                    await _lookup_countyRepository.FirstOrDefaultAsync((int)output.CountriesTranslation.CoreId);
                output.CountyDisplayName = _lookupCounty?.DisplayName?.ToString();
            }

            return output;
        }

        public async Task CreateOrEdit(CreateOrEditCountriesTranslationDto input)
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

        [AbpAuthorize(AppPermissions.Pages_CountriesTranslations_Create)]
        protected virtual async Task Create(CreateOrEditCountriesTranslationDto input)
        {
            var countriesTranslation = ObjectMapper.Map<CountriesTranslation>(input);

            await _countriesTranslationRepository.InsertAsync(countriesTranslation);
        }

        [AbpAuthorize(AppPermissions.Pages_CountriesTranslations_Edit)]
        protected virtual async Task Update(CreateOrEditCountriesTranslationDto input)
        {
            var countriesTranslation = await _countriesTranslationRepository.FirstOrDefaultAsync((int)input.Id);
            ObjectMapper.Map(input, countriesTranslation);
        }

        [AbpAuthorize(AppPermissions.Pages_CountriesTranslations_Delete)]
        public async Task Delete(EntityDto input)
        {
            await _countriesTranslationRepository.DeleteAsync(input.Id);
        }

        [AbpAuthorize(AppPermissions.Pages_CountriesTranslations)]
        public async Task<List<CountriesTranslationCountyLookupTableDto>> GetAllCountyForTableDropdown()
        {
            return await _lookup_countyRepository.GetAll()
                .Select(county => new CountriesTranslationCountyLookupTableDto
                {
                    Id = county.Id,
                    DisplayName = county == null || county.DisplayName == null ? "" : county.DisplayName.ToString()
                }).ToListAsync();
        }
    }
}