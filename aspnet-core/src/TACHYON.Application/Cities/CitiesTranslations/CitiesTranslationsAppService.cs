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
using TACHYON.Cities;
using TACHYON.Cities.CitiesTranslations.Dtos;
using TACHYON.Dto;

namespace TACHYON.Cities.CitiesTranslations
{
    [AbpAuthorize(AppPermissions.Pages_CitiesTranslations)]
    public class CitiesTranslationsAppService : TACHYONAppServiceBase, ICitiesTranslationsAppService
    {
        private readonly IRepository<CitiesTranslation> _citiesTranslationRepository;
        private readonly IRepository<City, int> _lookup_cityRepository;

        public CitiesTranslationsAppService(IRepository<CitiesTranslation> citiesTranslationRepository,
            IRepository<City, int> lookup_cityRepository)
        {
            _citiesTranslationRepository = citiesTranslationRepository;
            _lookup_cityRepository = lookup_cityRepository;
        }

        public async Task<LoadResult> GetAll(GetAllCitiesTranslationsInput input)
        {
            var filteredCities = _citiesTranslationRepository
                .GetAll()
                .Where(x => x.CoreId == Convert.ToInt32(input.CoreId))
                .ProjectTo<CitiesTranslationDto>(AutoMapperConfigurationProvider);

            return await LoadResultAsync(filteredCities, input.LoadOptions);
        }

        public async Task CreateOrEdit(CreateOrEditCitiesTranslationDto input)
        {
            var translation = await _citiesTranslationRepository.FirstOrDefaultAsync(x => x.Id == input.Id);
            if (translation == null)
            {
                var newTranslation = ObjectMapper.Map<CitiesTranslation>(input);
                await _citiesTranslationRepository.InsertAsync(newTranslation);
            }
            else
            {
                var duplication = await _citiesTranslationRepository.FirstOrDefaultAsync(x =>
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


        public async Task<GetCitiesTranslationForViewDto> GetCitiesTranslationForView(int id)
        {
            var citiesTranslation = await _citiesTranslationRepository.GetAsync(id);

            var output = new GetCitiesTranslationForViewDto
            {
                CitiesTranslation = ObjectMapper.Map<CitiesTranslationDto>(citiesTranslation)
            };

            if (output.CitiesTranslation.CoreId != null)
            {
                var _lookupCity =
                    await _lookup_cityRepository.FirstOrDefaultAsync((int)output.CitiesTranslation.CoreId);
                output.CityDisplayName = _lookupCity?.DisplayName?.ToString();
            }

            return output;
        }

        [AbpAuthorize(AppPermissions.Pages_CitiesTranslations_Edit)]
        public async Task<GetCitiesTranslationForEditOutput> GetCitiesTranslationForEdit(EntityDto input)
        {
            var citiesTranslation = await _citiesTranslationRepository.FirstOrDefaultAsync(input.Id);

            var output = new GetCitiesTranslationForEditOutput
            {
                CitiesTranslation = ObjectMapper.Map<CreateOrEditCitiesTranslationDto>(citiesTranslation)
            };

            if (output.CitiesTranslation.CoreId != null)
            {
                var _lookupCity =
                    await _lookup_cityRepository.FirstOrDefaultAsync((int)output.CitiesTranslation.CoreId);
                output.CityDisplayName = _lookupCity?.DisplayName?.ToString();
            }

            return output;
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