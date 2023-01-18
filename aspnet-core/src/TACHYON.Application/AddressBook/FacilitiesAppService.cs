﻿using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using PayPalCheckoutSdk.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using TACHYON.AddressBook.Dtos;
using TACHYON.AddressBook.Exporting;
using TACHYON.Authorization;
using TACHYON.Cities;
using TACHYON.Dto;

namespace TACHYON.AddressBook
{
    [AbpAuthorize(AppPermissions.Pages_Facilities)]
    public class FacilitiesAppService : TACHYONAppServiceBase, IFacilitiesAppService
    {
        private readonly IRepository<Facility, long> _facilityRepository;
        private readonly IRepository<FacilityWorkingHour> _facilityWorkingHourRepository;
        private readonly IFacilitiesExcelExporter _facilitiesExcelExporter;
        private readonly IRepository<City, int> _lookup_cityRepository;


        public FacilitiesAppService(IRepository<Facility, long> facilityRepository,
            IFacilitiesExcelExporter facilitiesExcelExporter,
            IRepository<City, int> lookup_cityRepository, IRepository<FacilityWorkingHour> facilityWorkingHourRepository)
        {
            _facilityRepository = facilityRepository;
            _facilitiesExcelExporter = facilitiesExcelExporter;
            _lookup_cityRepository = lookup_cityRepository;
            _facilityWorkingHourRepository = facilityWorkingHourRepository;
        }

        public async Task<PagedResultDto<GetFacilityForViewOutput>> GetAll(GetAllFacilitiesInput input)
        {
            DisableTenancyFiltersIfHost();
            await DisableTenancyFiltersIfTachyonDealer();


            var filteredFacilities = _facilityRepository.GetAll()
                .Include(e => e.CityFk)
                .ThenInclude(c => c.CountyFk)
                .ThenInclude(t => t.Translations)
                .Include(x=>x.FacilityWorkingHours)
                .Include(x=>x.Tenant)
                .WhereIf(input.FromDate.HasValue && input.ToDate.HasValue,
                    i => i.CreationTime >= input.FromDate && i.CreationTime <= input.ToDate)
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                    e => false || e.Name.Contains(input.Filter) || e.Address.Contains(input.Filter))
                .WhereIf(!string.IsNullOrWhiteSpace(input.NameFilter), e => e.Name == input.NameFilter)
                .WhereIf(!string.IsNullOrWhiteSpace(input.AdressFilter), e => e.Address == input.AdressFilter)
                .WhereIf(!string.IsNullOrWhiteSpace(input.CityDisplayNameFilter),
                    e => e.CityFk != null && e.CityFk.DisplayName == input.CityDisplayNameFilter);

            var pagedAndFilteredFacilities = filteredFacilities
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

            var facilities = from o in pagedAndFilteredFacilities
                join o2 in _lookup_cityRepository.GetAll() on o.CityId equals o2.Id into j2
                from s2 in j2.DefaultIfEmpty()
                select new GetFacilityForViewOutput()
                {
                    Facility = new FacilityDto
                    {
                        Name = o.Name,
                        Address = o.Address,
                        Longitude = o.Location.X,
                        Latitude = o.Location.Y,
                        Id = o.Id
                    },
                    CityDisplayName = s2 == null || s2.DisplayName == null ? "" : s2.DisplayName.ToString(),
                    Country = o.CityFk.CountyFk.DisplayName ?? "",
                    CreationTime = o.CreationTime,
                    ShipperName = o.Tenant.TenancyName,
                    FacilityType = o.FacilityType,
                    FacilityTypeTitle = o.FacilityType.GetEnumDescription(),
                    FacilityWorkingHours = ObjectMapper.Map<List<FacilityWorkingHourDto>>(o.FacilityWorkingHours)
                };

            var totalCount = await filteredFacilities.CountAsync();

            return new PagedResultDto<GetFacilityForViewOutput>(
                totalCount,
                await facilities.ToListAsync()
            );
        }

        public async Task<GetFacilityForViewOutput> GetFacilityForView(long id)
        {
            await DisableTenancyFiltersIfTachyonDealer();
            var facility = await _facilityRepository.GetAsync(id);

            var output = new GetFacilityForViewOutput { Facility = ObjectMapper.Map<FacilityDto>(facility) };


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
            var facility = await _facilityRepository.GetAll()
                .Include(x=>x.FacilityWorkingHours)
                .FirstOrDefaultAsync(x=> x.Id==input.Id);

            var output =
                new GetFacilityForEditOutput { Facility = ObjectMapper.Map<CreateOrEditFacilityDto>(facility) };


            if (output.Facility.CityId != null)
            {
                var _lookupCity = await _lookup_cityRepository
                    .GetAll().Where(x => x.Id == (int)output.Facility.CityId)
                    .Include(x => x.CountyFk)
                    .FirstOrDefaultAsync();
                output.CityDisplayName = _lookupCity?.DisplayName?.ToString();
                output.CountryId = _lookupCity?.CountyId;
                output.CountryDisplayName = _lookupCity?.CountyFk?.DisplayName;
                output.CountryCode = _lookupCity?.CountyFk?.Code;
            }

            return output;
        }

        public async Task<long> CreateOrEdit(CreateOrEditFacilityDto input)
        {
            await ValidateFacilityName(input);
            if(!await IsTachyonDealer())
            {
                input.ShipperId = null;
            }
            if (input.Id == null)
            {
                return await Create(input);
            }
            else
            {
                return await Update(input);
            }
        }

        [AbpAuthorize(AppPermissions.Pages_Facilities_Create)]
        protected virtual async Task<long> Create(CreateOrEditFacilityDto input)
        {
            var point = default(Point);
            if (input.Longitude!=null && input.Latitude != null)
            {
                 point = new Point
                    (input.Longitude.Value, input.Latitude.Value)
                { SRID = 4326 };
            }


            var facility = ObjectMapper.Map<Facility>(input);
            facility.Location = point;
            if((await IsTachyonDealer() || AbpSession.TenantId==null) && input.ShipperId != null)
            {
                facility.TenantId = input.ShipperId;
            }
            else
            {
                facility.TenantId = AbpSession.TenantId;
            }

            return await _facilityRepository.InsertAndGetIdAsync(facility);
        }

        [AbpAuthorize(AppPermissions.Pages_Facilities_Edit)]
        protected virtual async Task<long> Update(CreateOrEditFacilityDto input)
        {
            var facility = await _facilityRepository.GetAll().Include(x => x.FacilityWorkingHours).FirstOrDefaultAsync(x => x.Id == (long)input.Id);

            await RemoveDeletedWorkingHours(input, facility);
            ObjectMapper.Map(input, facility);
            if((await IsTachyonDealer() || AbpSession.TenantId == null) && input.ShipperId!=null)
            {
                facility.TenantId = input.ShipperId;
            }
            return facility.Id;
        }

        

        [AbpAuthorize(AppPermissions.Pages_Facilities_Delete)]
        public async Task Delete(EntityDto<long> input)
        {
            var facility = await _facilityRepository.GetAll().Include(x => x.FacilityWorkingHours).FirstOrDefaultAsync(x => x.Id == (long)input.Id);
            facility.FacilityWorkingHours.Clear();
            await _facilityRepository.DeleteAsync(facility);
        }

        public async Task<FileDto> GetFacilitiesToExcel(GetAllFacilitiesForExcelInput input)
        {
            var filteredFacilities = _facilityRepository.GetAll()
                .Include(e => e.CityFk)
                .Include(e => e.FacilityWorkingHours)
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                    e => false || e.Name.Contains(input.Filter) || e.Address.Contains(input.Filter))
                .WhereIf(!string.IsNullOrWhiteSpace(input.NameFilter), e => e.Name == input.NameFilter)
                .WhereIf(!string.IsNullOrWhiteSpace(input.AdressFilter), e => e.Address == input.AdressFilter)
                .WhereIf(!string.IsNullOrWhiteSpace(input.CityDisplayNameFilter),
                    e => e.CityFk != null && e.CityFk.DisplayName == input.CityDisplayNameFilter);

            var query = (from o in filteredFacilities
                join o2 in _lookup_cityRepository.GetAll() on o.CityId equals o2.Id into j2
                from s2 in j2.DefaultIfEmpty()
                select new GetFacilityForViewOutput()
                {
                    Facility = new FacilityDto
                    {
                        Name = o.Name,
                        Address = o.Address,
                        Longitude = o.Location.X,
                        Latitude = o.Location.Y,
                        Id = o.Id,
                        FacilityWorkingHours = string.Join(",", o.FacilityWorkingHours.Select(r=>"Day " + r.DayOfWeek.ToString() + " : "+ "("+r.StartTime.Duration().Hours + ":" + r.StartTime.Duration().Minutes + ")" + " To "+ "(" + r.EndTime.Duration().Hours + ":" + r.EndTime.Duration().Minutes + ")").ToList())
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

        public async Task<List<SelectFacilityItemDto>> GetAllPortsForTableDropdown()
        {
            return await _facilityRepository.GetAll().Include(x=>x.CityFk).Where(x=>x.FacilityType == FacilityType.Port)
                .Select(item => new SelectFacilityItemDto
                {
                    Id = item.Id.ToString(),
                    DisplayName = item == null || item.Name == null ? "" : $"{item.Name} - {item.CityFk.DisplayName}",
                    CityId = item.CityId
                }).ToListAsync();
        }



        private async Task ValidateFacilityName(CreateOrEditFacilityDto input)
        {
            if (!string.IsNullOrEmpty(input.Name))
            {
                var ExsistItem = await _facilityRepository.GetAll().AsNoTracking()
                .FirstOrDefaultAsync(e => e.Name.Equals(input.Name) && e.TenantId == AbpSession.TenantId);
                if (input.Id != null)
                    ExsistItem = await _facilityRepository.GetAll().AsNoTracking().FirstOrDefaultAsync(e =>
                        e.Name.Equals(input.Name) && e.Id != input.Id && e.TenantId == AbpSession.TenantId);
                if (ExsistItem != null)
                    throw new UserFriendlyException(L("facilityNameIsAlreadyExistsForThisTenant"));
            }
        }

        private async Task RemoveDeletedWorkingHours(CreateOrEditFacilityDto input, Facility facility)
        {
            foreach (var facilityWorkHour in facility.FacilityWorkingHours)
            {
                if (!input.FacilityWorkingHours.Any(x => x.Id == facilityWorkHour.Id))
                {
                    await _facilityWorkingHourRepository.DeleteAsync(facilityWorkHour);
                }
            }
        }
    }
}