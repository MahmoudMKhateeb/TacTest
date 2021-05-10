

using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using TACHYON.Authorization;
using TACHYON.Countries.Dtos;
using TACHYON.Countries.Exporting;
using TACHYON.Dto;

namespace TACHYON.Countries
{
    [AbpAuthorize(AppPermissions.Pages_Counties)]
    public class CountiesAppService : TACHYONAppServiceBase, ICountiesAppService
    {
        private readonly IRepository<County> _countyRepository;
        private readonly ICountiesExcelExporter _countiesExcelExporter;


        public CountiesAppService(IRepository<County> countyRepository, ICountiesExcelExporter countiesExcelExporter)
        {
            _countyRepository = countyRepository;
            _countiesExcelExporter = countiesExcelExporter;

        }

        public async Task<PagedResultDto<GetCountyForViewDto>> GetAll(GetAllCountiesInput input)
        {

            var filteredCounties = _countyRepository.GetAll()
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.DisplayName.Contains(input.Filter) || e.Code.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.DisplayNameFilter), e => e.DisplayName == input.DisplayNameFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.CodeFilter), e => e.Code == input.CodeFilter);

            var pagedAndFilteredCounties = filteredCounties
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

            var counties = from o in pagedAndFilteredCounties
                           select new GetCountyForViewDto()
                           {
                               County = new CountyDto
                               {
                                   DisplayName = o.DisplayName,
                                   Code = o.Code,
                                   Id = o.Id
                               }
                           };

            var totalCount = await filteredCounties.CountAsync();

            return new PagedResultDto<GetCountyForViewDto>(
                totalCount,
                await counties.ToListAsync()
            );
        }

        public async Task<GetCountyForViewDto> GetCountyForView(int id)
        {
            var county = await _countyRepository.GetAsync(id);

            var output = new GetCountyForViewDto { County = ObjectMapper.Map<CountyDto>(county) };

            return output;
        }

        [AbpAuthorize(AppPermissions.Pages_Counties_Edit)]
        public async Task<GetCountyForEditOutput> GetCountyForEdit(EntityDto input)
        {
            var county = await _countyRepository.FirstOrDefaultAsync(input.Id);

            var output = new GetCountyForEditOutput { County = ObjectMapper.Map<CreateOrEditCountyDto>(county) };

            return output;
        }

        public async Task CreateOrEdit(CreateOrEditCountyDto input)
        {
            if (string.IsNullOrWhiteSpace(input.DisplayName))
            {
                throw new UserFriendlyException(L("CannotCreateEmptyCountry"));
            }
            if(await _countyRepository.FirstOrDefaultAsync(x => x.DisplayName.ToLower() == input.DisplayName.ToLower()
            && x.Id!=input.Id) != null)
            {
                throw new UserFriendlyException(L("countryIsAlreadyExistsMessage"));
            }
            if (input.Id == null)
            {
                await Create(input);
            }
            else
            {
                await Update(input);
            }
        }

        [AbpAuthorize(AppPermissions.Pages_Counties_Create)]
        protected virtual async Task Create(CreateOrEditCountyDto input)
        {
            var county = ObjectMapper.Map<County>(input);



            await _countyRepository.InsertAsync(county);
        }

        [AbpAuthorize(AppPermissions.Pages_Counties_Edit)]
        protected virtual async Task Update(CreateOrEditCountyDto input)
        {
            var county = await _countyRepository.FirstOrDefaultAsync((int)input.Id);
            ObjectMapper.Map(input, county);
        }

        [AbpAuthorize(AppPermissions.Pages_Counties_Delete)]
        public async Task Delete(EntityDto input)
        {
            await _countyRepository.DeleteAsync(input.Id);
        }

        public async Task<FileDto> GetCountiesToExcel(GetAllCountiesForExcelInput input)
        {

            var filteredCounties = _countyRepository.GetAll()
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.DisplayName.Contains(input.Filter) || e.Code.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.DisplayNameFilter), e => e.DisplayName == input.DisplayNameFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.CodeFilter), e => e.Code == input.CodeFilter);

            var query = (from o in filteredCounties
                         select new GetCountyForViewDto()
                         {
                             County = new CountyDto
                             {
                                 DisplayName = o.DisplayName,
                                 Code = o.Code,
                                 Id = o.Id
                             }
                         });


            var countyListDtos = await query.ToListAsync();

            return _countiesExcelExporter.ExportToFile(countyListDtos);
        }


    }
}