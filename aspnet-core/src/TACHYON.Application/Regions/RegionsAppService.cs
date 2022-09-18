using TACHYON.Countries;

using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using TACHYON.Regions.Dtos;
using TACHYON.Dto;
using Abp.Application.Services.Dto;
using TACHYON.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;
using Abp.UI;
using TACHYON.Storage;

namespace TACHYON.Regions
{
    [AbpAuthorize(AppPermissions.Pages_Administration_Regions)]
    public class RegionsAppService : TACHYONAppServiceBase, IRegionsAppService
    {
        private readonly IRepository<Region> _regionRepository;
        private readonly IRepository<County, int> _lookup_countyRepository;

        public RegionsAppService(IRepository<Region> regionRepository, IRepository<County, int> lookup_countyRepository)
        {
            _regionRepository = regionRepository;
            _lookup_countyRepository = lookup_countyRepository;

        }

        public async Task<PagedResultDto<GetRegionForViewDto>> GetAll(GetAllRegionsInput input)
        {

            var filteredRegions = _regionRepository.GetAll()
                        .Include(e => e.CountyFk)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.Name.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.NameFilter), e => e.Name == input.NameFilter)
                        .WhereIf(input.MinBayanIntegrationIdFilter != null, e => e.BayanIntegrationId >= input.MinBayanIntegrationIdFilter)
                        .WhereIf(input.MaxBayanIntegrationIdFilter != null, e => e.BayanIntegrationId <= input.MaxBayanIntegrationIdFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.CountyDisplayNameFilter), e => e.CountyFk != null && e.CountyFk.DisplayName == input.CountyDisplayNameFilter);

            var pagedAndFilteredRegions = filteredRegions
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

            var regions = from o in pagedAndFilteredRegions
                          join o1 in _lookup_countyRepository.GetAll() on o.CountyId equals o1.Id into j1
                          from s1 in j1.DefaultIfEmpty()

                          select new
                          {

                              o.Name,
                              o.BayanIntegrationId,
                              Id = o.Id,
                              CountyDisplayName = s1 == null || s1.DisplayName == null ? "" : s1.DisplayName.ToString()
                          };

            var totalCount = await filteredRegions.CountAsync();

            var dbList = await regions.ToListAsync();
            var results = new List<GetRegionForViewDto>();

            foreach (var o in dbList)
            {
                var res = new GetRegionForViewDto()
                {
                    Region = new RegionDto
                    {

                        Name = o.Name,
                        BayanIntegrationId = o.BayanIntegrationId,
                        Id = o.Id,
                    },
                    CountyDisplayName = o.CountyDisplayName
                };

                results.Add(res);
            }

            return new PagedResultDto<GetRegionForViewDto>(
                totalCount,
                results
            );

        }

        public async Task<GetRegionForViewDto> GetRegionForView(int id)
        {
            var region = await _regionRepository.GetAsync(id);

            var output = new GetRegionForViewDto { Region = ObjectMapper.Map<RegionDto>(region) };

            if (output.Region.CountyId != null)
            {
                var _lookupCounty = await _lookup_countyRepository.FirstOrDefaultAsync((int)output.Region.CountyId);
                output.CountyDisplayName = _lookupCounty?.DisplayName?.ToString();
            }

            return output;
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_Regions_Edit)]
        public async Task<GetRegionForEditOutput> GetRegionForEdit(EntityDto input)
        {
            var region = await _regionRepository.FirstOrDefaultAsync(input.Id);

            var output = new GetRegionForEditOutput { Region = ObjectMapper.Map<CreateOrEditRegionDto>(region) };

            if (output.Region.CountyId != null)
            {
                var _lookupCounty = await _lookup_countyRepository.FirstOrDefaultAsync((int)output.Region.CountyId);
                output.CountyDisplayName = _lookupCounty?.DisplayName?.ToString();
            }

            return output;
        }

        public async Task CreateOrEdit(CreateOrEditRegionDto input)
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

        [AbpAuthorize(AppPermissions.Pages_Administration_Regions_Create)]
        protected virtual async Task Create(CreateOrEditRegionDto input)
        {
            var region = ObjectMapper.Map<Region>(input);

            await _regionRepository.InsertAsync(region);

        }

        [AbpAuthorize(AppPermissions.Pages_Administration_Regions_Edit)]
        protected virtual async Task Update(CreateOrEditRegionDto input)
        {
            var region = await _regionRepository.FirstOrDefaultAsync((int)input.Id);
            ObjectMapper.Map(input, region);

        }

        [AbpAuthorize(AppPermissions.Pages_Administration_Regions_Delete)]
        public async Task Delete(EntityDto input)
        {
            await _regionRepository.DeleteAsync(input.Id);
        }
        [AbpAuthorize(AppPermissions.Pages_Administration_Regions)]
        public async Task<List<RegionCountyLookupTableDto>> GetAllCountyForTableDropdown()
        {
            return await _lookup_countyRepository.GetAll()
                .Select(county => new RegionCountyLookupTableDto
                {
                    Id = county.Id,
                    DisplayName = county == null || county.DisplayName == null ? "" : county.DisplayName.ToString()
                }).ToListAsync();
        }

    }
}