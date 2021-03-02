using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using TACHYON.Shipping.TripStatuses.Dtos;
using TACHYON.Dto;
using Abp.Application.Services.Dto;
using TACHYON.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;

namespace TACHYON.Shipping.TripStatuses
{
    [AbpAuthorize(AppPermissions.Pages_TripStatuses)]
    public class TripStatusesAppService : TACHYONAppServiceBase, ITripStatusesAppService
    {
        private readonly IRepository<TripStatus> _tripStatusRepository;

        public TripStatusesAppService(IRepository<TripStatus> tripStatusRepository)
        {
            _tripStatusRepository = tripStatusRepository;

        }

        public async Task<PagedResultDto<GetTripStatusForViewDto>> GetAll(GetAllTripStatusesInput input)
        {

            var filteredTripStatuses = _tripStatusRepository.GetAll()
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.DisplayName.Contains(input.Filter));

            var pagedAndFilteredTripStatuses = filteredTripStatuses
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

            var tripStatuses = from o in pagedAndFilteredTripStatuses
                               select new GetTripStatusForViewDto()
                               {
                                   TripStatus = new TripStatusDto
                                   {
                                       DisplayName = o.DisplayName,
                                       Id = o.Id
                                   }
                               };

            var totalCount = await filteredTripStatuses.CountAsync();

            return new PagedResultDto<GetTripStatusForViewDto>(
                totalCount,
                await tripStatuses.ToListAsync()
            );
        }

        public async Task<GetTripStatusForViewDto> GetTripStatusForView(int id)
        {
            var tripStatus = await _tripStatusRepository.GetAsync(id);

            var output = new GetTripStatusForViewDto { TripStatus = ObjectMapper.Map<TripStatusDto>(tripStatus) };

            return output;
        }

        [AbpAuthorize(AppPermissions.Pages_TripStatuses_Edit)]
        public async Task<GetTripStatusForEditOutput> GetTripStatusForEdit(EntityDto input)
        {
            var tripStatus = await _tripStatusRepository.FirstOrDefaultAsync(input.Id);

            var output = new GetTripStatusForEditOutput { TripStatus = ObjectMapper.Map<CreateOrEditTripStatusDto>(tripStatus) };

            return output;
        }

        public async Task CreateOrEdit(CreateOrEditTripStatusDto input)
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

        [AbpAuthorize(AppPermissions.Pages_TripStatuses_Create)]
        protected virtual async Task Create(CreateOrEditTripStatusDto input)
        {
            var tripStatus = ObjectMapper.Map<TripStatus>(input);

            await _tripStatusRepository.InsertAsync(tripStatus);
        }

        [AbpAuthorize(AppPermissions.Pages_TripStatuses_Edit)]
        protected virtual async Task Update(CreateOrEditTripStatusDto input)
        {
            var tripStatus = await _tripStatusRepository.FirstOrDefaultAsync((int)input.Id);
            ObjectMapper.Map(input, tripStatus);
        }

        [AbpAuthorize(AppPermissions.Pages_TripStatuses_Delete)]
        public async Task Delete(EntityDto input)
        {
            await _tripStatusRepository.DeleteAsync(input.Id);
        }
    }
}