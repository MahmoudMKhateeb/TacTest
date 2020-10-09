using TACHYON.Trucks.TruckCategories.TruckSubtypes;
					using System.Collections.Generic;


using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using TACHYON.Trucks.TruckCategories.TruckCapacities.Dtos;
using TACHYON.Dto;
using Abp.Application.Services.Dto;
using TACHYON.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;

namespace TACHYON.Trucks.TruckCategories.TruckCapacities
{
	[AbpAuthorize(AppPermissions.Pages_Capacities)]
    public class CapacitiesAppService : TACHYONAppServiceBase, ICapacitiesAppService
    {
		 private readonly IRepository<Capacity> _capacityRepository;
		 private readonly IRepository<TruckSubtype,int> _lookup_truckSubtypeRepository;
		 

		  public CapacitiesAppService(IRepository<Capacity> capacityRepository , IRepository<TruckSubtype, int> lookup_truckSubtypeRepository) 
		  {
			_capacityRepository = capacityRepository;
			_lookup_truckSubtypeRepository = lookup_truckSubtypeRepository;
		
		  }

		 public async Task<PagedResultDto<GetCapacityForViewDto>> GetAll(GetAllCapacitiesInput input)
         {
			
			var filteredCapacities = _capacityRepository.GetAll()
						.Include( e => e.TruckSubtypeFk)
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.DisplayName.Contains(input.Filter))
						.WhereIf(!string.IsNullOrWhiteSpace(input.DisplayNameFilter),  e => e.DisplayName == input.DisplayNameFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.TruckSubtypeDisplayNameFilter), e => e.TruckSubtypeFk != null && e.TruckSubtypeFk.DisplayName == input.TruckSubtypeDisplayNameFilter);

			var pagedAndFilteredCapacities = filteredCapacities
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

			var capacities = from o in pagedAndFilteredCapacities
                         join o1 in _lookup_truckSubtypeRepository.GetAll() on o.TruckSubtypeId equals o1.Id into j1
                         from s1 in j1.DefaultIfEmpty()
                         
                         select new GetCapacityForViewDto() {
							Capacity = new CapacityDto
							{
                                DisplayName = o.DisplayName,
                                Id = o.Id
							},
                         	TruckSubtypeDisplayName = s1 == null || s1.DisplayName == null ? "" : s1.DisplayName.ToString()
						};

            var totalCount = await filteredCapacities.CountAsync();

            return new PagedResultDto<GetCapacityForViewDto>(
                totalCount,
                await capacities.ToListAsync()
            );
         }
		 
		 public async Task<GetCapacityForViewDto> GetCapacityForView(int id)
         {
            var capacity = await _capacityRepository.GetAsync(id);

            var output = new GetCapacityForViewDto { Capacity = ObjectMapper.Map<CapacityDto>(capacity) };

		    if (output.Capacity.TruckSubtypeId != null)
            {
                var _lookupTruckSubtype = await _lookup_truckSubtypeRepository.FirstOrDefaultAsync((int)output.Capacity.TruckSubtypeId);
                output.TruckSubtypeDisplayName = _lookupTruckSubtype?.DisplayName?.ToString();
            }
			
            return output;
         }
		 
		 [AbpAuthorize(AppPermissions.Pages_Capacities_Edit)]
		 public async Task<GetCapacityForEditOutput> GetCapacityForEdit(EntityDto input)
         {
            var capacity = await _capacityRepository.FirstOrDefaultAsync(input.Id);
           
		    var output = new GetCapacityForEditOutput {Capacity = ObjectMapper.Map<CreateOrEditCapacityDto>(capacity)};

		    if (output.Capacity.TruckSubtypeId != null)
            {
                var _lookupTruckSubtype = await _lookup_truckSubtypeRepository.FirstOrDefaultAsync((int)output.Capacity.TruckSubtypeId);
                output.TruckSubtypeDisplayName = _lookupTruckSubtype?.DisplayName?.ToString();
            }
			
            return output;
         }

		 public async Task CreateOrEdit(CreateOrEditCapacityDto input)
         {
            if(input.Id == null){
				await Create(input);
			}
			else{
				await Update(input);
			}
         }

		 [AbpAuthorize(AppPermissions.Pages_Capacities_Create)]
		 protected virtual async Task Create(CreateOrEditCapacityDto input)
         {
            var capacity = ObjectMapper.Map<Capacity>(input);

			

            await _capacityRepository.InsertAsync(capacity);
         }

		 [AbpAuthorize(AppPermissions.Pages_Capacities_Edit)]
		 protected virtual async Task Update(CreateOrEditCapacityDto input)
         {
            var capacity = await _capacityRepository.FirstOrDefaultAsync((int)input.Id);
             ObjectMapper.Map(input, capacity);
         }

		 [AbpAuthorize(AppPermissions.Pages_Capacities_Delete)]
         public async Task Delete(EntityDto input)
         {
            await _capacityRepository.DeleteAsync(input.Id);
         } 
			[AbpAuthorize(AppPermissions.Pages_Capacities)]
			public async Task<List<CapacityTruckSubtypeLookupTableDto>> GetAllTruckSubtypeForTableDropdown()
			{
				return await _lookup_truckSubtypeRepository.GetAll()
					.Select(truckSubtype => new CapacityTruckSubtypeLookupTableDto
					{
						Id = truckSubtype.Id,
						DisplayName = truckSubtype == null || truckSubtype.DisplayName == null ? "" : truckSubtype.DisplayName.ToString()
					}).ToListAsync();
			}
							
    }
}