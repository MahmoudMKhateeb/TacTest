

using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using TACHYON.Shipping.ShippingRequestStatuses.Dtos;
using TACHYON.Dto;
using Abp.Application.Services.Dto;
using TACHYON.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;

namespace TACHYON.Shipping.ShippingRequestStatuses
{
	[AbpAuthorize(AppPermissions.Pages_Administration_ShippingRequestStatuses)]
    public class ShippingRequestStatusesAppService : TACHYONAppServiceBase, IShippingRequestStatusesAppService
    {
		 private readonly IRepository<ShippingRequestStatus> _shippingRequestStatusRepository;
		 

		  public ShippingRequestStatusesAppService(IRepository<ShippingRequestStatus> shippingRequestStatusRepository ) 
		  {
			_shippingRequestStatusRepository = shippingRequestStatusRepository;
			
		  }

		 public async Task<PagedResultDto<GetShippingRequestStatusForViewDto>> GetAll(GetAllShippingRequestStatusesInput input)
         {
			
			var filteredShippingRequestStatuses = _shippingRequestStatusRepository.GetAll()
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.DisplayName.Contains(input.Filter))
						.WhereIf(!string.IsNullOrWhiteSpace(input.DisplayNameFilter),  e => e.DisplayName == input.DisplayNameFilter);

			var pagedAndFilteredShippingRequestStatuses = filteredShippingRequestStatuses
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

			var shippingRequestStatuses = from o in pagedAndFilteredShippingRequestStatuses
                         select new GetShippingRequestStatusForViewDto() {
							ShippingRequestStatus = new ShippingRequestStatusDto
							{
                                DisplayName = o.DisplayName,
                                Id = o.Id
							}
						};

            var totalCount = await filteredShippingRequestStatuses.CountAsync();

            return new PagedResultDto<GetShippingRequestStatusForViewDto>(
                totalCount,
                await shippingRequestStatuses.ToListAsync()
            );
         }
		 
		 public async Task<GetShippingRequestStatusForViewDto> GetShippingRequestStatusForView(int id)
         {
            var shippingRequestStatus = await _shippingRequestStatusRepository.GetAsync(id);

            var output = new GetShippingRequestStatusForViewDto { ShippingRequestStatus = ObjectMapper.Map<ShippingRequestStatusDto>(shippingRequestStatus) };
			
            return output;
         }
		 
		 [AbpAuthorize(AppPermissions.Pages_Administration_ShippingRequestStatuses_Edit)]
		 public async Task<GetShippingRequestStatusForEditOutput> GetShippingRequestStatusForEdit(EntityDto input)
         {
            var shippingRequestStatus = await _shippingRequestStatusRepository.FirstOrDefaultAsync(input.Id);
           
		    var output = new GetShippingRequestStatusForEditOutput {ShippingRequestStatus = ObjectMapper.Map<CreateOrEditShippingRequestStatusDto>(shippingRequestStatus)};
			
            return output;
         }

		 public async Task CreateOrEdit(CreateOrEditShippingRequestStatusDto input)
         {
            if(input.Id == null){
				await Create(input);
			}
			else{
				await Update(input);
			}
         }

		 [AbpAuthorize(AppPermissions.Pages_Administration_ShippingRequestStatuses_Create)]
		 protected virtual async Task Create(CreateOrEditShippingRequestStatusDto input)
         {
            var shippingRequestStatus = ObjectMapper.Map<ShippingRequestStatus>(input);

			

            await _shippingRequestStatusRepository.InsertAsync(shippingRequestStatus);
         }

		 [AbpAuthorize(AppPermissions.Pages_Administration_ShippingRequestStatuses_Edit)]
		 protected virtual async Task Update(CreateOrEditShippingRequestStatusDto input)
         {
            var shippingRequestStatus = await _shippingRequestStatusRepository.FirstOrDefaultAsync((int)input.Id);
             ObjectMapper.Map(input, shippingRequestStatus);
         }

		 [AbpAuthorize(AppPermissions.Pages_Administration_ShippingRequestStatuses_Delete)]
         public async Task Delete(EntityDto input)
         {
            await _shippingRequestStatusRepository.DeleteAsync(input.Id);
         } 
    }
}