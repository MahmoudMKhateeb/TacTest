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

namespace TACHYON.Routs.RoutPoints
{
    public class RoutPointsAppService: TACHYONAppServiceBase,IRoutPointAppService
    {
        private IRepository<RoutPoint, long> _routPointsRepository;
        public RoutPointsAppService(IRepository<RoutPoint, long> routPointsRepository)
        {
            _routPointsRepository = routPointsRepository;
        }

        public async Task<PagedResultDto<GetRoutPointForViewDto>> GetAll(GetAllRoutPointInput input)
        {
            var filteredRoutPoints = _routPointsRepository.GetAll()
                .Include(x=>x.CityFk)
                .Include(x=>x.FacilityFk)
                .Include(x=>x.PickingTypeFk)
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), x => x.DisplayName == input.Filter)
                .WhereIf(input.PickingTypeId != null, x => x.PickingTypeId == input.PickingTypeId);


            var PagedAndFilteredRoutPoints= filteredRoutPoints
                .OrderBy(input.Sorting ?? "id asc")
                    .PageBy(input);

            var routPoints = PagedAndFilteredRoutPoints.Select(x => new GetRoutPointForViewDto
            {
                RoutPointDto = ObjectMapper.Map<RoutPointDto>(x),
                CityName = x.CityFk.DisplayName,
                FacilityName = x.FacilityFk.Name,
                PickingTypeDisplayName = x.PickingTypeFk.DisplayName
            });

            var totalCount =await routPoints.CountAsync();
            return new PagedResultDto<GetRoutPointForViewDto>(totalCount, await routPoints.ToListAsync());

        }
    }
}
