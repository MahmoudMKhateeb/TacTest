using TACHYON.Cities;

using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using TACHYON.Cities.CitiesTranslations.Dtos;
using TACHYON.Dto;
using Abp.Application.Services.Dto;
using TACHYON.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;

namespace TACHYON.Cities.CitiesTranslations
{
    [AbpAuthorize(AppPermissions.Pages_CitiesTranslations)]
    public class CitiesTranslationsAppService : TACHYONAppServiceBase, ICitiesTranslationsAppService
    {
        private readonly IRepository<CitiesTranslation> _citiesTranslationRepository;
        private readonly IRepository<City, int> _lookup_cityRepository;

        public CitiesTranslationsAppService(IRepository<CitiesTranslation> citiesTranslationRepository, IRepository<City, int> lookup_cityRepository)
        {
            _citiesTranslationRepository = citiesTranslationRepository;
            _lookup_cityRepository = lookup_cityRepository;

        }

        public async Task<PagedResultDto<GetCitiesTranslationForViewDto>> GetAll(GetAllCitiesTranslationsInput input)
        {

            var filteredCitiesTranslations = _citiesTranslationRepository.GetAll()
                        .Include(e => e.Core)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.TranslatedDisplayName.Contains(input.Filter) || e.Language.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.TranslatedDisplayNameFilter), e => e.TranslatedDisplayName == input.TranslatedDisplayNameFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.LanguageFilter), e => e.Language == input.LanguageFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.CityDisplayNameFilter), e => e.Core != null && e.Core.DisplayName == input.CityDisplayNameFilter);

            var pagedAndFilteredCitiesTranslations = filteredCitiesTranslations
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

            var citiesTranslations = from o in pagedAndFilteredCitiesTranslations
                                     join o1 in _lookup_cityRepository.GetAll() on o.CoreId equals o1.Id into j1
                                     from s1 in j1.DefaultIfEmpty()

                                     select new GetCitiesTranslationForViewDto()
                                     {
                                         CitiesTranslation = new CitiesTranslationDto
                                         {
                                             TranslatedDisplayName = o.TranslatedDisplayName,
                                             Language = o.Language,
                                             Id = o.Id
                                         },
                                         CityDisplayName = s1 == null || s1.DisplayName == null ? "" : s1.DisplayName.ToString()
                                     };

            var totalCount = await filteredCitiesTranslations.CountAsync();

            return new PagedResultDto<GetCitiesTranslationForViewDto>(
                totalCount,
                await citiesTranslations.ToListAsync()
            );
        }

        public async Task<GetCitiesTranslationForViewDto> GetCitiesTranslationForView(int id)
        {
            var citiesTranslation = await _citiesTranslationRepository.GetAsync(id);

            var output = new GetCitiesTranslationForViewDto { CitiesTranslation = ObjectMapper.Map<CitiesTranslationDto>(citiesTranslation) };

            if (output.CitiesTranslation.CoreId != null)
            {
                var _lookupCity = await _lookup_cityRepository.FirstOrDefaultAsync((int)output.CitiesTranslation.CoreId);
                output.CityDisplayName = _lookupCity?.DisplayName?.ToString();
            }

            return output;
        }

        [AbpAuthorize(AppPermissions.Pages_CitiesTranslations_Edit)]
        public async Task<GetCitiesTranslationForEditOutput> GetCitiesTranslationForEdit(EntityDto input)
        {
            var citiesTranslation = await _citiesTranslationRepository.FirstOrDefaultAsync(input.Id);

            var output = new GetCitiesTranslationForEditOutput { CitiesTranslation = ObjectMapper.Map<CreateOrEditCitiesTranslationDto>(citiesTranslation) };

            if (output.CitiesTranslation.CoreId != null)
            {
                var _lookupCity = await _lookup_cityRepository.FirstOrDefaultAsync((int)output.CitiesTranslation.CoreId);
                output.CityDisplayName = _lookupCity?.DisplayName?.ToString();
            }

            return output;
        }

        public async Task CreateOrEdit(CreateOrEditCitiesTranslationDto input)
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

        [AbpAuthorize(AppPermissions.Pages_CitiesTranslations_Create)]
        protected virtual async Task Create(CreateOrEditCitiesTranslationDto input)
        {
            var citiesTranslation = ObjectMapper.Map<CitiesTranslation>(input);

            await _citiesTranslationRepository.InsertAsync(citiesTranslation);
        }

        [AbpAuthorize(AppPermissions.Pages_CitiesTranslations_Edit)]
        protected virtual async Task Update(CreateOrEditCitiesTranslationDto input)
        {
            var citiesTranslation = await _citiesTranslationRepository.FirstOrDefaultAsync((int)input.Id);
            ObjectMapper.Map(input, citiesTranslation);
        }

        [AbpAuthorize(AppPermissions.Pages_CitiesTranslations_Delete)]
        public async Task Delete(EntityDto input)
        {
            await _citiesTranslationRepository.DeleteAsync(input.Id);
        }
        [AbpAuthorize(AppPermissions.Pages_CitiesTranslations)]
        public async Task<List<CitiesTranslationCityLookupTableDto>> GetAllCityForTableDropdown()
        {
            return await _lookup_cityRepository.GetAll()
                .Select(city => new CitiesTranslationCityLookupTableDto
                {
                    Id = city.Id,
                    DisplayName = city == null || city.DisplayName == null ? "" : city.DisplayName.ToString()
                }).ToListAsync();
        }

    }
}