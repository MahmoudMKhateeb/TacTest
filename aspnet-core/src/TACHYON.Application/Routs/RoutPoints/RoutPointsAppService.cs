using Abp.Linq.Extensions;
using Abp.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TACHYON.Routs.RoutPoints.Dtos;
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using TACHYON.Authorization;
using TACHYON.AddressBook.Dtos;

namespace TACHYON.Routs.RoutPoints
{
    public class RoutPointsAppService: TACHYONAppServiceBase,IRoutPointAppService
    {
        private IRepository<RoutPoint, long> _routPointsRepository;
        public RoutPointsAppService(IRepository<RoutPoint, long> routPointsRepository)
        {
            _routPointsRepository = routPointsRepository;
        }

        public async Task<PagedResultDto<GetRoutPointForViewOutput>> GetAll(GetAllRoutPointInput input)
        {
            var filteredRoutPoints = _routPointsRepository.GetAll()
                .Include(x=>x.FacilityFk)
                .ThenInclude(x => x.CityFk)
                .Include(x=>x.PickingTypeFk)
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), x => x.DisplayName == input.Filter)
                .WhereIf(input.PickingTypeId != null, x => x.PickingTypeId == input.PickingTypeId);


            var PagedAndFilteredRoutPoints= filteredRoutPoints
                .OrderBy(input.Sorting ?? "id asc")
                    .PageBy(input);

            var routPoints = PagedAndFilteredRoutPoints.Select(x => new GetRoutPointForViewOutput
            {
                RoutPointDto = ObjectMapper.Map<RoutPointDto>(x),
                
                PickingTypeDisplayName = x.PickingTypeFk.DisplayName,
                facilityDto=new GetFacilityForViewOutput
                {
                    CityDisplayName = x.FacilityFk.CityFk.DisplayName,
                    FacilityName = x.FacilityFk.Name,
                    Facility=ObjectMapper.Map<FacilityDto>(x.FacilityFk)
                }
            });

            var totalCount =await routPoints.CountAsync();
            return new PagedResultDto<GetRoutPointForViewOutput>(totalCount, await routPoints.ToListAsync());

        }

        public async Task CreateOrEditRoutPoint(CreateOrEditRoutPointDto input)
        {
            if (input.Id == null)
            {
                await Create(input);
            }
            else
            {
                await Edit(input);
            }
           
        }

        [AbpAuthorize(AppPermissions.Pages_RoutPoints_Delete)]
        public async Task Delete(EntityDto<long> input)
        {
            await _routPointsRepository.DeleteAsync(input.Id);
        }

        [AbpAuthorize(AppPermissions.Pages_RoutPoints_Create)]
        private async Task Create(CreateOrEditRoutPointDto input)
        {
            var routPoint = ObjectMapper.Map<RoutPoint>(input);
            await _routPointsRepository.InsertAsync(routPoint);
        }

        [AbpAuthorize(AppPermissions.Pages_RoutPoints_Edit)]
        private async Task Edit(CreateOrEditRoutPointDto input)
        {
            var routPoint =await _routPointsRepository.FirstOrDefaultAsync((long)input.Id);
            ObjectMapper.Map(input, routPoint);
        }



    }
}
