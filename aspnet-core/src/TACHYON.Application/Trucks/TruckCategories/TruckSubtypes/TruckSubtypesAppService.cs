using TACHYON.Trucks.TrucksTypes;
					using System.Collections.Generic;


using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using TACHYON.Trucks.TruckCategories.TruckSubtypes.Dtos;
using TACHYON.Dto;
using Abp.Application.Services.Dto;
using TACHYON.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;

namespace TACHYON.Trucks.TruckCategories.TruckSubtypes
{
	[AbpAuthorize(AppPermissions.Pages_TruckSubtypes)]
    public class TruckSubtypesAppService : TACHYONAppServiceBase, ITruckSubtypesAppService
    {
		 private readonly IRepository<TruckSubtype> _truckSubtypeRepository;
		 private readonly IRepository<TrucksType,long> _lookup_trucksTypeRepository;
		 

		  public TruckSubtypesAppService(IRepository<TruckSubtype> truckSubtypeRepository , IRepository<TrucksType, long> lookup_trucksTypeRepository) 
		  {
			_truckSubtypeRepository = truckSubtypeRepository;
			_lookup_trucksTypeRepository = lookup_trucksTypeRepository;
		
		  }

		 public async Task<PagedResultDto<GetTruckSubtypeForViewDto>> GetAll(GetAllTruckSubtypesInput input)
         {
			
			var filteredTruckSubtypes = _truckSubtypeRepository.GetAll()
						.Include( e => e.TrucksTypeFk)
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.DisplayName.Contains(input.Filter))
						.WhereIf(!string.IsNullOrWhiteSpace(input.DisplayNameFilter),  e => e.DisplayName == input.DisplayNameFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.TrucksTypeDisplayNameFilter), e => e.TrucksTypeFk != null && e.TrucksTypeFk.DisplayName == input.TrucksTypeDisplayNameFilter);

			var pagedAndFilteredTruckSubtypes = filteredTruckSubtypes
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

			var truckSubtypes = from o in pagedAndFilteredTruckSubtypes
                         join o1 in _lookup_trucksTypeRepository.GetAll() on o.TrucksTypeId equals o1.Id into j1
                         from s1 in j1.DefaultIfEmpty()
                         
                         select new GetTruckSubtypeForViewDto() {
							TruckSubtype = new TruckSubtypeDto
							{
                                DisplayName = o.DisplayName,
                                Id = o.Id
							},
                         	TrucksTypeDisplayName = s1 == null || s1.DisplayName == null ? "" : s1.DisplayName.ToString()
						};

            var totalCount = await filteredTruckSubtypes.CountAsync();

            return new PagedResultDto<GetTruckSubtypeForViewDto>(
                totalCount,
                await truckSubtypes.ToListAsync()
            );
         }
		 
		 public async Task<GetTruckSubtypeForViewDto> GetTruckSubtypeForView(int id)
         {
            var truckSubtype = await _truckSubtypeRepository.GetAsync(id);

            var output = new GetTruckSubtypeForViewDto { TruckSubtype = ObjectMapper.Map<TruckSubtypeDto>(truckSubtype) };

		    if (output.TruckSubtype.TrucksTypeId != null)
            {
                var _lookupTrucksType = await _lookup_trucksTypeRepository.FirstOrDefaultAsync((long)output.TruckSubtype.TrucksTypeId);
                output.TrucksTypeDisplayName = _lookupTrucksType?.DisplayName?.ToString();
            }
			
            return output;
         }
		 
		 [AbpAuthorize(AppPermissions.Pages_TruckSubtypes_Edit)]
		 public async Task<GetTruckSubtypeForEditOutput> GetTruckSubtypeForEdit(EntityDto input)
         {
            var truckSubtype = await _truckSubtypeRepository.FirstOrDefaultAsync(input.Id);
           
		    var output = new GetTruckSubtypeForEditOutput {TruckSubtype = ObjectMapper.Map<CreateOrEditTruckSubtypeDto>(truckSubtype)};

		    if (output.TruckSubtype.TrucksTypeId != null)
            {
                var _lookupTrucksType = await _lookup_trucksTypeRepository.FirstOrDefaultAsync((long)output.TruckSubtype.TrucksTypeId);
                output.TrucksTypeDisplayName = _lookupTrucksType?.DisplayName?.ToString();
            }
			
            return output;
         }

		 public async Task CreateOrEdit(CreateOrEditTruckSubtypeDto input)
         {
            if(input.Id == null){
				await Create(input);
			}
			else{
				await Update(input);
			}
         }

		 [AbpAuthorize(AppPermissions.Pages_TruckSubtypes_Create)]
		 protected virtual async Task Create(CreateOrEditTruckSubtypeDto input)
         {
            var truckSubtype = ObjectMapper.Map<TruckSubtype>(input);

			

            await _truckSubtypeRepository.InsertAsync(truckSubtype);
         }

		 [AbpAuthorize(AppPermissions.Pages_TruckSubtypes_Edit)]
		 protected virtual async Task Update(CreateOrEditTruckSubtypeDto input)
         {
            var truckSubtype = await _truckSubtypeRepository.FirstOrDefaultAsync((int)input.Id);
             ObjectMapper.Map(input, truckSubtype);
         }

		 [AbpAuthorize(AppPermissions.Pages_TruckSubtypes_Delete)]
         public async Task Delete(EntityDto input)
         {
            await _truckSubtypeRepository.DeleteAsync(input.Id);
         } 
			[AbpAuthorize(AppPermissions.Pages_TruckSubtypes)]
			public async Task<List<TruckSubtypeTrucksTypeLookupTableDto>> GetAllTrucksTypeForTableDropdown()
			{
				return await _lookup_trucksTypeRepository.GetAll()
					.Select(trucksType => new TruckSubtypeTrucksTypeLookupTableDto
					{
						Id = trucksType.Id,
						DisplayName = trucksType == null || trucksType.DisplayName == null ? "" : trucksType.DisplayName.ToString()
					}).ToListAsync();
			}
							
    }
}