﻿using TACHYON.Countries;
using System.Collections.Generic;
using TACHYON.Cities;
using System.Collections.Generic;


using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using TACHYON.AddressBook.Exporting;
using TACHYON.AddressBook.Dtos;
using TACHYON.Dto;
using Abp.Application.Services.Dto;
using TACHYON.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;

namespace TACHYON.AddressBook
{
    [AbpAuthorize(AppPermissions.Pages_Facilities)]
    public class FacilitiesAppService : TACHYONAppServiceBase, IFacilitiesAppService
    {
        private readonly IRepository<Facility, long> _facilityRepository;
        private readonly IFacilitiesExcelExporter _facilitiesExcelExporter;
        private readonly IRepository<City, int> _lookup_cityRepository;


        public FacilitiesAppService(IRepository<Facility, long> facilityRepository, IFacilitiesExcelExporter facilitiesExcelExporter, IRepository<City, int> lookup_cityRepository)
        {
            _facilityRepository = facilityRepository;
            _facilitiesExcelExporter = facilitiesExcelExporter;
            _lookup_cityRepository = lookup_cityRepository;

        }

        public async Task<PagedResultDto<GetFacilityForViewDto>> GetAll(GetAllFacilitiesInput input)
        {

            var filteredFacilities = _facilityRepository.GetAll()
                        .Include(e => e.CityFk)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.Name.Contains(input.Filter) || e.Adress.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.NameFilter), e => e.Name == input.NameFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.AdressFilter), e => e.Adress == input.AdressFilter)
                        .WhereIf(input.MinLongitudeFilter != null, e => e.Longitude >= input.MinLongitudeFilter)
                        .WhereIf(input.MaxLongitudeFilter != null, e => e.Longitude <= input.MaxLongitudeFilter)
                        .WhereIf(input.MinLatitudeFilter != null, e => e.Latitude >= input.MinLatitudeFilter)
                        .WhereIf(input.MaxLatitudeFilter != null, e => e.Latitude <= input.MaxLatitudeFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.CityDisplayNameFilter), e => e.CityFk != null && e.CityFk.DisplayName == input.CityDisplayNameFilter);

            var pagedAndFilteredFacilities = filteredFacilities
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

            var facilities = from o in pagedAndFilteredFacilities
                             join o2 in _lookup_cityRepository.GetAll() on o.CityId equals o2.Id into j2
                             from s2 in j2.DefaultIfEmpty()

                             select new GetFacilityForViewDto()
                             {
                                 Facility = new FacilityDto
                                 {
                                     Name = o.Name,
                                     Adress = o.Adress,
                                     Longitude = o.Longitude,
                                     Latitude = o.Latitude,
                                     Id = o.Id
                                 },
                                 CityDisplayName = s2 == null || s2.DisplayName == null ? "" : s2.DisplayName.ToString()
                             };

            var totalCount = await filteredFacilities.CountAsync();

            return new PagedResultDto<GetFacilityForViewDto>(
                totalCount,
                await facilities.ToListAsync()
            );
        }

        public async Task<GetFacilityForViewDto> GetFacilityForView(long id)
        {
            var facility = await _facilityRepository.GetAsync(id);

            var output = new GetFacilityForViewDto { Facility = ObjectMapper.Map<FacilityDto>(facility) };


            if (output.Facility.CityId != null)
            {
                var _lookupCity = await _lookup_cityRepository.FirstOrDefaultAsync((int)output.Facility.CityId);
                output.CityDisplayName = _lookupCity?.DisplayName?.ToString();
            }

            return output;
        }

        [AbpAuthorize(AppPermissions.Pages_Facilities_Edit)]
        public async Task<GetFacilityForEditOutput> GetFacilityForEdit(EntityDto<long> input)
        {
            var facility = await _facilityRepository.FirstOrDefaultAsync(input.Id);

            var output = new GetFacilityForEditOutput { Facility = ObjectMapper.Map<CreateOrEditFacilityDto>(facility) };


            if (output.Facility.CityId != null)
            {
                var _lookupCity = await _lookup_cityRepository.FirstOrDefaultAsync((int)output.Facility.CityId);
                output.CityDisplayName = _lookupCity?.DisplayName?.ToString();
            }

            return output;
        }

        public async Task CreateOrEdit(CreateOrEditFacilityDto input)
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

        [AbpAuthorize(AppPermissions.Pages_Facilities_Create)]
        protected virtual async Task Create(CreateOrEditFacilityDto input)
        {
            var facility = ObjectMapper.Map<Facility>(input);


            if (AbpSession.TenantId != null)
            {
                facility.TenantId = (int?)AbpSession.TenantId;
            }


            await _facilityRepository.InsertAsync(facility);
        }

        [AbpAuthorize(AppPermissions.Pages_Facilities_Edit)]
        protected virtual async Task Update(CreateOrEditFacilityDto input)
        {
            var facility = await _facilityRepository.FirstOrDefaultAsync((long)input.Id);
            ObjectMapper.Map(input, facility);
        }

        [AbpAuthorize(AppPermissions.Pages_Facilities_Delete)]
        public async Task Delete(EntityDto<long> input)
        {
            await _facilityRepository.DeleteAsync(input.Id);
        }

        public async Task<FileDto> GetFacilitiesToExcel(GetAllFacilitiesForExcelInput input)
        {

            var filteredFacilities = _facilityRepository.GetAll()
                        .Include(e => e.CityFk)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.Name.Contains(input.Filter) || e.Adress.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.NameFilter), e => e.Name == input.NameFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.AdressFilter), e => e.Adress == input.AdressFilter)
                        .WhereIf(input.MinLongitudeFilter != null, e => e.Longitude >= input.MinLongitudeFilter)
                        .WhereIf(input.MaxLongitudeFilter != null, e => e.Longitude <= input.MaxLongitudeFilter)
                        .WhereIf(input.MinLatitudeFilter != null, e => e.Latitude >= input.MinLatitudeFilter)
                        .WhereIf(input.MaxLatitudeFilter != null, e => e.Latitude <= input.MaxLatitudeFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.CityDisplayNameFilter), e => e.CityFk != null && e.CityFk.DisplayName == input.CityDisplayNameFilter);

            var query = (from o in filteredFacilities
                         join o2 in _lookup_cityRepository.GetAll() on o.CityId equals o2.Id into j2
                         from s2 in j2.DefaultIfEmpty()

                         select new GetFacilityForViewDto()
                         {
                             Facility = new FacilityDto
                             {
                                 Name = o.Name,
                                 Adress = o.Adress,
                                 Longitude = o.Longitude,
                                 Latitude = o.Latitude,
                                 Id = o.Id
                             },
                             CityDisplayName = s2 == null || s2.DisplayName == null ? "" : s2.DisplayName.ToString()
                         });


            var facilityListDtos = await query.ToListAsync();

            return _facilitiesExcelExporter.ExportToFile(facilityListDtos);
        }

        [AbpAuthorize(AppPermissions.Pages_Facilities)]
        public async Task<List<FacilityCityLookupTableDto>> GetAllCityForTableDropdown()
        {
            return await _lookup_cityRepository.GetAll()
                .Select(city => new FacilityCityLookupTableDto
                {
                    Id = city.Id,
                    DisplayName = city == null || city.DisplayName == null ? "" : city.DisplayName.ToString()
                }).ToListAsync();
        }

    }
}