using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using TACHYON.Authorization;
using TACHYON.Cities.Dtos;
using TACHYON.Cities.Exporting;
using TACHYON.Countries;
using TACHYON.Dto;

namespace TACHYON.Cities
{
    [AbpAuthorize(AppPermissions.Pages_Cities)]
    public class CitiesAppService : TACHYONAppServiceBase, ICitiesAppService
    {
        private readonly IRepository<City> _cityRepository;
        private readonly ICitiesExcelExporter _citiesExcelExporter;
        private readonly IRepository<County, int> _lookup_countyRepository;


        public CitiesAppService(IRepository<City> cityRepository, ICitiesExcelExporter citiesExcelExporter, IRepository<County, int> lookup_countyRepository)
        {
            _cityRepository = cityRepository;
            _citiesExcelExporter = citiesExcelExporter;
            _lookup_countyRepository = lookup_countyRepository;

        }

        public async Task<PagedResultDto<GetCityForViewDto>> GetAll(GetAllCitiesInput input)
        {

            var filteredCities = _cityRepository.GetAll()
                        .Include(e => e.CountyFk)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.DisplayName.Contains(input.Filter) || e.Code.Contains(input.Filter) )//|| e.Latitude.Contains(input.Filter) || e.Longitude.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.DisplayNameFilter), e => e.DisplayName == input.DisplayNameFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.CodeFilter), e => e.Code == input.CodeFilter)
                       // .WhereIf(!string.IsNullOrWhiteSpace(input.LatitudeFilter), e => e.Location.Y == input.LatitudeFilter)
                       // .WhereIf(!string.IsNullOrWhiteSpace(input.LongitudeFilter), e => e.Longitude == input.LongitudeFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.CountyDisplayNameFilter), e => e.CountyFk != null && e.CountyFk.DisplayName == input.CountyDisplayNameFilter);

            var pagedAndFilteredCities = filteredCities
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

            var cities = from o in pagedAndFilteredCities
                         join o1 in _lookup_countyRepository.GetAll() on o.CountyId equals o1.Id into j1
                         from s1 in j1.DefaultIfEmpty()

                         select new GetCityForViewDto()
                         {
                             City = new CityDto
                             {
                                 DisplayName = o.DisplayName,
                                 Code = o.Code,
                                 Latitude = o.Location.Y,
                                 Longitude = o.Location.X,
                                 Id = o.Id
                             },
                             CountyDisplayName = s1 == null || s1.DisplayName == null ? "" : s1.DisplayName.ToString()
                         };

            var totalCount = await filteredCities.CountAsync();

            return new PagedResultDto<GetCityForViewDto>(
                totalCount,
                await cities.ToListAsync()
            );
        }

        public async Task<GetCityForViewDto> GetCityForView(int id)
        {
            var city = await _cityRepository.GetAsync(id);

            var output = new GetCityForViewDto { City = ObjectMapper.Map<CityDto>(city) };

            if (output.City.CountyId != null)
            {
                var _lookupCounty = await _lookup_countyRepository.FirstOrDefaultAsync((int)output.City.CountyId);
                output.CountyDisplayName = _lookupCounty?.DisplayName?.ToString();
            }

            return output;
        }

        [AbpAuthorize(AppPermissions.Pages_Cities_Edit)]
        public async Task<GetCityForEditOutput> GetCityForEdit(EntityDto input)
        {
            var city = await _cityRepository.FirstOrDefaultAsync(input.Id);

            var output = new GetCityForEditOutput { City = ObjectMapper.Map<CreateOrEditCityDto>(city) };

            if (output.City.CountyId != null)
            {
                var _lookupCounty = await _lookup_countyRepository.FirstOrDefaultAsync((int)output.City.CountyId);
                output.CountyDisplayName = _lookupCounty?.DisplayName?.ToString();
            }

            return output;
        }

        public async Task CreateOrEdit(CreateOrEditCityDto input)
        {
            await CheckCityValidation(input);

            if (input.Id == null)
            {
                await Create(input);
            }
            else
            {
                await Update(input);
            }
        }

        private async Task CheckCityValidation(CreateOrEditCityDto input)
        {
            if (string.IsNullOrWhiteSpace(input.DisplayName))
            {
                throw new UserFriendlyException(L("CannotCreateEmptyName"));
            }
            if (await _cityRepository.FirstOrDefaultAsync(x => x.DisplayName.ToLower() == input.DisplayName.ToLower() && x.CountyId == input.CountyId && x.Id!=input.Id) != null)
            {
                throw new UserFriendlyException(L("CityIsAlreadyExistsForThisCountry"));
            }
        }

        [AbpAuthorize(AppPermissions.Pages_Cities_Create)]
        protected virtual async Task Create(CreateOrEditCityDto input)
        {
            var point = new Point
                (input.Longitude, input.Latitude)
            {
                SRID = 4326
            };

            var city = ObjectMapper.Map<City>(input);
            city.Location = point;

            await _cityRepository.InsertAsync(city);
        }

        [AbpAuthorize(AppPermissions.Pages_Cities_Edit)]
        protected virtual async Task Update(CreateOrEditCityDto input)
        {
            var city = await _cityRepository.FirstOrDefaultAsync((int)input.Id);
            if( (city.Location?.X!=input.Longitude || city.Location?.Y!=input.Latitude))
            {
                var point = new Point
                (input.Longitude, input.Latitude)
                {
                    SRID = 4326
                };

                city.Location = point;
            }
            ObjectMapper.Map(input, city);
        }

        [AbpAuthorize(AppPermissions.Pages_Cities_Delete)]
        public async Task Delete(EntityDto input)
        {
            await _cityRepository.DeleteAsync(input.Id);
        }

        public async Task<FileDto> GetCitiesToExcel(GetAllCitiesForExcelInput input)
        {

            var filteredCities = _cityRepository.GetAll()
                        .Include(e => e.CountyFk)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.DisplayName.Contains(input.Filter) || e.Code.Contains(input.Filter))// || e.Latitude.Contains(input.Filter) || e.Longitude.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.DisplayNameFilter), e => e.DisplayName == input.DisplayNameFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.CodeFilter), e => e.Code == input.CodeFilter)
                       // .WhereIf(!string.IsNullOrWhiteSpace(input.LatitudeFilter), e => e.Latitude == input.LatitudeFilter)
                        //.WhereIf(!string.IsNullOrWhiteSpace(input.LongitudeFilter), e => e.Longitude == input.LongitudeFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.CountyDisplayNameFilter), e => e.CountyFk != null && e.CountyFk.DisplayName == input.CountyDisplayNameFilter);

            var query = (from o in filteredCities
                         join o1 in _lookup_countyRepository.GetAll() on o.CountyId equals o1.Id into j1
                         from s1 in j1.DefaultIfEmpty()

                         select new GetCityForViewDto()
                         {
                             City = new CityDto
                             {
                                 DisplayName = o.DisplayName,
                                 Code = o.Code,
                                 Latitude = o.Location.Y,
                                 Longitude = o.Location.X,
                                 Id = o.Id
                             },
                             CountyDisplayName = s1 == null || s1.DisplayName == null ? "" : s1.DisplayName.ToString()
                         });


            var cityListDtos = await query.ToListAsync();

            return _citiesExcelExporter.ExportToFile(cityListDtos);
        }


        [AbpAuthorize(AppPermissions.Pages_Cities)]
        public async Task<List<CityCountyLookupTableDto>> GetAllCountyForTableDropdown()
        {
            return await _lookup_countyRepository.GetAll()
                .Select(county => new CityCountyLookupTableDto
                {
                    Id = county.Id,
                    DisplayName = county == null || county.DisplayName == null ? "" : county.DisplayName.ToString()
                }).ToListAsync();
        }

    }
}