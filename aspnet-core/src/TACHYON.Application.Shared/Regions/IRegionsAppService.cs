using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using TACHYON.Regions.Dtos;
using TACHYON.Dto;
using System.Collections.Generic;

namespace TACHYON.Regions
{
    public interface IRegionsAppService : IApplicationService
    {
        Task<PagedResultDto<GetRegionForViewDto>> GetAll(GetAllRegionsInput input);

        Task<GetRegionForViewDto> GetRegionForView(int id);

        Task<GetRegionForEditOutput> GetRegionForEdit(EntityDto input);

        Task CreateOrEdit(CreateOrEditRegionDto input);

        Task Delete(EntityDto input);

        Task<List<RegionCountyLookupTableDto>> GetAllCountyForTableDropdown();

    }
}