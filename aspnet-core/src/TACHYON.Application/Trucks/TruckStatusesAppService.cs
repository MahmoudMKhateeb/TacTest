

using Abp.Application.Features;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using TACHYON.Authorization;
using TACHYON.Dto;
using TACHYON.Features;
using TACHYON.Trucks.Dtos;

namespace TACHYON.Trucks
{
    [AbpAuthorize(AppPermissions.Pages_Administration_TruckStatuses)]
    [RequiresFeature(AppFeatures.Carrier)]
    public class TruckStatusesAppService : TACHYONAppServiceBase, ITruckStatusesAppService
    {
        private readonly IRepository<TruckStatus, long> _truckStatusRepository;


        public TruckStatusesAppService(IRepository<TruckStatus, long> truckStatusRepository)
        {
            _truckStatusRepository = truckStatusRepository;

        }

        public async Task<PagedResultDto<GetTruckStatusForViewDto>> GetAll(GetAllTruckStatusesInput input)
        {

            var filteredTruckStatuses = _truckStatusRepository.GetAll()
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.DisplayName.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.DisplayNameFilter), e => e.DisplayName == input.DisplayNameFilter);

            var pagedAndFilteredTruckStatuses = filteredTruckStatuses
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

            var truckStatuses = from o in pagedAndFilteredTruckStatuses
                                select new GetTruckStatusForViewDto()
                                {
                                    TruckStatus = new TruckStatusDto
                                    {
                                        DisplayName = o.DisplayName,
                                        Id = o.Id
                                    }
                                };

            var totalCount = await filteredTruckStatuses.CountAsync();

            return new PagedResultDto<GetTruckStatusForViewDto>(
                totalCount,
                await truckStatuses.ToListAsync()
            );
        }

        public async Task<GetTruckStatusForViewDto> GetTruckStatusForView(long id)
        {
            var truckStatus = await _truckStatusRepository.GetAsync(id);

            var output = new GetTruckStatusForViewDto { TruckStatus = ObjectMapper.Map<TruckStatusDto>(truckStatus) };

            return output;
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_TruckStatuses_Edit)]
        public async Task<GetTruckStatusForEditOutput> GetTruckStatusForEdit(EntityDto<long> input)
        {
            var truckStatus = await _truckStatusRepository.FirstOrDefaultAsync(input.Id);

            var output = new GetTruckStatusForEditOutput { TruckStatus = ObjectMapper.Map<CreateOrEditTruckStatusDto>(truckStatus) };

            return output;
        }

        public async Task CreateOrEdit(CreateOrEditTruckStatusDto input)
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

        [AbpAuthorize(AppPermissions.Pages_Administration_TruckStatuses_Create)]
        protected virtual async Task Create(CreateOrEditTruckStatusDto input)
        {
            var truckStatus = ObjectMapper.Map<TruckStatus>(input);


            if (AbpSession.TenantId != null)
            {
                truckStatus.TenantId = (int)AbpSession.TenantId;
            }


            await _truckStatusRepository.InsertAsync(truckStatus);
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_TruckStatuses_Edit)]
        protected virtual async Task Update(CreateOrEditTruckStatusDto input)
        {
            var truckStatus = await _truckStatusRepository.FirstOrDefaultAsync((long)input.Id);
            ObjectMapper.Map(input, truckStatus);
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_TruckStatuses_Delete)]
        public async Task Delete(EntityDto<long> input)
        {
            await _truckStatusRepository.DeleteAsync(input.Id);
        }
    }
}