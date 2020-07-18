

using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using TACHYON.Trucks.Dtos;
using TACHYON.Dto;
using Abp.Application.Services.Dto;
using TACHYON.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;

namespace TACHYON.Trucks
{
	[AbpAuthorize(AppPermissions.Pages_Administration_TruckStatuses)]
    public class TruckStatusesAppService : TACHYONAppServiceBase, ITruckStatusesAppService
    {
		 private readonly IRepository<TruckStatus, Guid> _truckStatusRepository;
		 

		  public TruckStatusesAppService(IRepository<TruckStatus, Guid> truckStatusRepository ) 
		  {
			_truckStatusRepository = truckStatusRepository;
			
		  }

		 public async Task<PagedResultDto<GetTruckStatusForViewDto>> GetAll(GetAllTruckStatusesInput input)
         {
			
			var filteredTruckStatuses = _truckStatusRepository.GetAll()
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.DisplayName.Contains(input.Filter))
						.WhereIf(!string.IsNullOrWhiteSpace(input.DisplayNameFilter),  e => e.DisplayName == input.DisplayNameFilter);

			var pagedAndFilteredTruckStatuses = filteredTruckStatuses
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

			var truckStatuses = from o in pagedAndFilteredTruckStatuses
                         select new GetTruckStatusForViewDto() {
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
		 
		 public async Task<GetTruckStatusForViewDto> GetTruckStatusForView(Guid id)
         {
            var truckStatus = await _truckStatusRepository.GetAsync(id);

            var output = new GetTruckStatusForViewDto { TruckStatus = ObjectMapper.Map<TruckStatusDto>(truckStatus) };
			
            return output;
         }
		 
		 [AbpAuthorize(AppPermissions.Pages_Administration_TruckStatuses_Edit)]
		 public async Task<GetTruckStatusForEditOutput> GetTruckStatusForEdit(EntityDto<Guid> input)
         {
            var truckStatus = await _truckStatusRepository.FirstOrDefaultAsync(input.Id);
           
		    var output = new GetTruckStatusForEditOutput {TruckStatus = ObjectMapper.Map<CreateOrEditTruckStatusDto>(truckStatus)};
			
            return output;
         }

		 public async Task CreateOrEdit(CreateOrEditTruckStatusDto input)
         {
            if(input.Id == null){
				await Create(input);
			}
			else{
				await Update(input);
			}
         }

		 [AbpAuthorize(AppPermissions.Pages_Administration_TruckStatuses_Create)]
		 protected virtual async Task Create(CreateOrEditTruckStatusDto input)
         {
            var truckStatus = ObjectMapper.Map<TruckStatus>(input);

			
			if (AbpSession.TenantId != null)
			{
				truckStatus.TenantId = (int) AbpSession.TenantId;
			}
		

            await _truckStatusRepository.InsertAsync(truckStatus);
         }

		 [AbpAuthorize(AppPermissions.Pages_Administration_TruckStatuses_Edit)]
		 protected virtual async Task Update(CreateOrEditTruckStatusDto input)
         {
            var truckStatus = await _truckStatusRepository.FirstOrDefaultAsync((Guid)input.Id);
             ObjectMapper.Map(input, truckStatus);
         }

		 [AbpAuthorize(AppPermissions.Pages_Administration_TruckStatuses_Delete)]
         public async Task Delete(EntityDto<Guid> input)
         {
            await _truckStatusRepository.DeleteAsync(input.Id);
         } 
    }
}