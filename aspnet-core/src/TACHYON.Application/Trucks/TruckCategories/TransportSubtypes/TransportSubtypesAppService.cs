using TACHYON.Trucks.TruckCategories.TransportTypes;
					using System.Collections.Generic;


using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using TACHYON.Trucks.TruckCategories.TransportSubtypes.Dtos;
using TACHYON.Dto;
using Abp.Application.Services.Dto;
using TACHYON.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;

namespace TACHYON.Trucks.TruckCategories.TransportSubtypes
{
	[AbpAuthorize(AppPermissions.Pages_TransportSubtypes)]
    public class TransportSubtypesAppService : TACHYONAppServiceBase, ITransportSubtypesAppService
    {
		 private readonly IRepository<TransportSubtype> _transportSubtypeRepository;
		 private readonly IRepository<TransportType,int> _lookup_transportTypeRepository;
		 

		  public TransportSubtypesAppService(IRepository<TransportSubtype> transportSubtypeRepository , IRepository<TransportType, int> lookup_transportTypeRepository) 
		  {
			_transportSubtypeRepository = transportSubtypeRepository;
			_lookup_transportTypeRepository = lookup_transportTypeRepository;
		
		  }

		 public async Task<PagedResultDto<GetTransportSubtypeForViewDto>> GetAll(GetAllTransportSubtypesInput input)
         {
			
			var filteredTransportSubtypes = _transportSubtypeRepository.GetAll()
						.Include( e => e.TransportTypeFk)
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.DisplayName.Contains(input.Filter))
						.WhereIf(!string.IsNullOrWhiteSpace(input.DisplayNameFilter),  e => e.DisplayName == input.DisplayNameFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.TransportTypeDisplayNameFilter), e => e.TransportTypeFk != null && e.TransportTypeFk.DisplayName == input.TransportTypeDisplayNameFilter);

			var pagedAndFilteredTransportSubtypes = filteredTransportSubtypes
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

			var transportSubtypes = from o in pagedAndFilteredTransportSubtypes
                         join o1 in _lookup_transportTypeRepository.GetAll() on o.TransportTypeId equals o1.Id into j1
                         from s1 in j1.DefaultIfEmpty()
                         
                         select new GetTransportSubtypeForViewDto() {
							TransportSubtype = new TransportSubtypeDto
							{
                                DisplayName = o.DisplayName,
                                Id = o.Id
							},
                         	TransportTypeDisplayName = s1 == null || s1.DisplayName == null ? "" : s1.DisplayName.ToString()
						};

            var totalCount = await filteredTransportSubtypes.CountAsync();

            return new PagedResultDto<GetTransportSubtypeForViewDto>(
                totalCount,
                await transportSubtypes.ToListAsync()
            );
         }
		 
		 public async Task<GetTransportSubtypeForViewDto> GetTransportSubtypeForView(int id)
         {
            var transportSubtype = await _transportSubtypeRepository.GetAsync(id);

            var output = new GetTransportSubtypeForViewDto { TransportSubtype = ObjectMapper.Map<TransportSubtypeDto>(transportSubtype) };

		    if (output.TransportSubtype.TransportTypeId != null)
            {
                var _lookupTransportType = await _lookup_transportTypeRepository.FirstOrDefaultAsync((int)output.TransportSubtype.TransportTypeId);
                output.TransportTypeDisplayName = _lookupTransportType?.DisplayName?.ToString();
            }
			
            return output;
         }
		 
		 [AbpAuthorize(AppPermissions.Pages_TransportSubtypes_Edit)]
		 public async Task<GetTransportSubtypeForEditOutput> GetTransportSubtypeForEdit(EntityDto input)
         {
            var transportSubtype = await _transportSubtypeRepository.FirstOrDefaultAsync(input.Id);
           
		    var output = new GetTransportSubtypeForEditOutput {TransportSubtype = ObjectMapper.Map<CreateOrEditTransportSubtypeDto>(transportSubtype)};

		    if (output.TransportSubtype.TransportTypeId != null)
            {
                var _lookupTransportType = await _lookup_transportTypeRepository.FirstOrDefaultAsync((int)output.TransportSubtype.TransportTypeId);
                output.TransportTypeDisplayName = _lookupTransportType?.DisplayName?.ToString();
            }
			
            return output;
         }

		 public async Task CreateOrEdit(CreateOrEditTransportSubtypeDto input)
         {
            if(input.Id == null){
				await Create(input);
			}
			else{
				await Update(input);
			}
         }

		 [AbpAuthorize(AppPermissions.Pages_TransportSubtypes_Create)]
		 protected virtual async Task Create(CreateOrEditTransportSubtypeDto input)
         {
            var transportSubtype = ObjectMapper.Map<TransportSubtype>(input);

			

            await _transportSubtypeRepository.InsertAsync(transportSubtype);
         }

		 [AbpAuthorize(AppPermissions.Pages_TransportSubtypes_Edit)]
		 protected virtual async Task Update(CreateOrEditTransportSubtypeDto input)
         {
            var transportSubtype = await _transportSubtypeRepository.FirstOrDefaultAsync((int)input.Id);
             ObjectMapper.Map(input, transportSubtype);
         }

		 [AbpAuthorize(AppPermissions.Pages_TransportSubtypes_Delete)]
         public async Task Delete(EntityDto input)
         {
            await _transportSubtypeRepository.DeleteAsync(input.Id);
         } 
			[AbpAuthorize(AppPermissions.Pages_TransportSubtypes)]
			public async Task<List<TransportSubtypeTransportTypeLookupTableDto>> GetAllTransportTypeForTableDropdown()
			{
				return await _lookup_transportTypeRepository.GetAll()
					.Select(transportType => new TransportSubtypeTransportTypeLookupTableDto
					{
						Id = transportType.Id,
						DisplayName = transportType == null || transportType.DisplayName == null ? "" : transportType.DisplayName.ToString()
					}).ToListAsync();
			}
							
    }
}