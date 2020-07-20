

using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using TACHYON.Routs.RoutTypes.Exporting;
using TACHYON.Routs.RoutTypes.Dtos;
using TACHYON.Dto;
using Abp.Application.Services.Dto;
using TACHYON.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;

namespace TACHYON.Routs.RoutTypes
{
	[AbpAuthorize(AppPermissions.Pages_RoutTypes)]
    public class RoutTypesAppService : TACHYONAppServiceBase, IRoutTypesAppService
    {
		 private readonly IRepository<RoutType> _routTypeRepository;
		 private readonly IRoutTypesExcelExporter _routTypesExcelExporter;
		 

		  public RoutTypesAppService(IRepository<RoutType> routTypeRepository, IRoutTypesExcelExporter routTypesExcelExporter ) 
		  {
			_routTypeRepository = routTypeRepository;
			_routTypesExcelExporter = routTypesExcelExporter;
			
		  }

		 public async Task<PagedResultDto<GetRoutTypeForViewDto>> GetAll(GetAllRoutTypesInput input)
         {
			
			var filteredRoutTypes = _routTypeRepository.GetAll()
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.DisplayName.Contains(input.Filter) || e.Description.Contains(input.Filter))
						.WhereIf(!string.IsNullOrWhiteSpace(input.DisplayNameFilter),  e => e.DisplayName == input.DisplayNameFilter);

			var pagedAndFilteredRoutTypes = filteredRoutTypes
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

			var routTypes = from o in pagedAndFilteredRoutTypes
                         select new GetRoutTypeForViewDto() {
							RoutType = new RoutTypeDto
							{
                                DisplayName = o.DisplayName,
                                Description = o.Description,
                                Id = o.Id
							}
						};

            var totalCount = await filteredRoutTypes.CountAsync();

            return new PagedResultDto<GetRoutTypeForViewDto>(
                totalCount,
                await routTypes.ToListAsync()
            );
         }
		 
		 public async Task<GetRoutTypeForViewDto> GetRoutTypeForView(int id)
         {
            var routType = await _routTypeRepository.GetAsync(id);

            var output = new GetRoutTypeForViewDto { RoutType = ObjectMapper.Map<RoutTypeDto>(routType) };
			
            return output;
         }
		 
		 [AbpAuthorize(AppPermissions.Pages_RoutTypes_Edit)]
		 public async Task<GetRoutTypeForEditOutput> GetRoutTypeForEdit(EntityDto input)
         {
            var routType = await _routTypeRepository.FirstOrDefaultAsync(input.Id);
           
		    var output = new GetRoutTypeForEditOutput {RoutType = ObjectMapper.Map<CreateOrEditRoutTypeDto>(routType)};
			
            return output;
         }

		 public async Task CreateOrEdit(CreateOrEditRoutTypeDto input)
         {
            if(input.Id == null){
				await Create(input);
			}
			else{
				await Update(input);
			}
         }

		 [AbpAuthorize(AppPermissions.Pages_RoutTypes_Create)]
		 protected virtual async Task Create(CreateOrEditRoutTypeDto input)
         {
            var routType = ObjectMapper.Map<RoutType>(input);

			

            await _routTypeRepository.InsertAsync(routType);
         }

		 [AbpAuthorize(AppPermissions.Pages_RoutTypes_Edit)]
		 protected virtual async Task Update(CreateOrEditRoutTypeDto input)
         {
            var routType = await _routTypeRepository.FirstOrDefaultAsync((int)input.Id);
             ObjectMapper.Map(input, routType);
         }

		 [AbpAuthorize(AppPermissions.Pages_RoutTypes_Delete)]
         public async Task Delete(EntityDto input)
         {
            await _routTypeRepository.DeleteAsync(input.Id);
         } 

		public async Task<FileDto> GetRoutTypesToExcel(GetAllRoutTypesForExcelInput input)
         {
			
			var filteredRoutTypes = _routTypeRepository.GetAll()
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.DisplayName.Contains(input.Filter) || e.Description.Contains(input.Filter))
						.WhereIf(!string.IsNullOrWhiteSpace(input.DisplayNameFilter),  e => e.DisplayName == input.DisplayNameFilter);

			var query = (from o in filteredRoutTypes
                         select new GetRoutTypeForViewDto() { 
							RoutType = new RoutTypeDto
							{
                                DisplayName = o.DisplayName,
                                Description = o.Description,
                                Id = o.Id
							}
						 });


            var routTypeListDtos = await query.ToListAsync();

            return _routTypesExcelExporter.ExportToFile(routTypeListDtos);
         }


    }
}