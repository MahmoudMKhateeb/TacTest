

using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using TACHYON.Trailers.TrailerTypes.Exporting;
using TACHYON.Trailers.TrailerTypes.Dtos;
using TACHYON.Dto;
using Abp.Application.Services.Dto;
using TACHYON.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;

namespace TACHYON.Trailers.TrailerTypes
{
	[AbpAuthorize(AppPermissions.Pages_TrailerTypes)]
    public class TrailerTypesAppService : TACHYONAppServiceBase, ITrailerTypesAppService
    {
		 private readonly IRepository<TrailerType> _trailerTypeRepository;
		 private readonly ITrailerTypesExcelExporter _trailerTypesExcelExporter;
		 

		  public TrailerTypesAppService(IRepository<TrailerType> trailerTypeRepository, ITrailerTypesExcelExporter trailerTypesExcelExporter ) 
		  {
			_trailerTypeRepository = trailerTypeRepository;
			_trailerTypesExcelExporter = trailerTypesExcelExporter;
			
		  }

		 public async Task<PagedResultDto<GetTrailerTypeForViewDto>> GetAll(GetAllTrailerTypesInput input)
         {
			
			var filteredTrailerTypes = _trailerTypeRepository.GetAll()
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.DisplayName.Contains(input.Filter))
						.WhereIf(!string.IsNullOrWhiteSpace(input.DisplayNameFilter),  e => e.DisplayName == input.DisplayNameFilter);

			var pagedAndFilteredTrailerTypes = filteredTrailerTypes
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

			var trailerTypes = from o in pagedAndFilteredTrailerTypes
                         select new GetTrailerTypeForViewDto() {
							TrailerType = new TrailerTypeDto
							{
                                DisplayName = o.DisplayName,
                                Id = o.Id
							}
						};

            var totalCount = await filteredTrailerTypes.CountAsync();

            return new PagedResultDto<GetTrailerTypeForViewDto>(
                totalCount,
                await trailerTypes.ToListAsync()
            );
         }
		 
		 public async Task<GetTrailerTypeForViewDto> GetTrailerTypeForView(int id)
         {
            var trailerType = await _trailerTypeRepository.GetAsync(id);

            var output = new GetTrailerTypeForViewDto { TrailerType = ObjectMapper.Map<TrailerTypeDto>(trailerType) };
			
            return output;
         }
		 
		 [AbpAuthorize(AppPermissions.Pages_TrailerTypes_Edit)]
		 public async Task<GetTrailerTypeForEditOutput> GetTrailerTypeForEdit(EntityDto input)
         {
            var trailerType = await _trailerTypeRepository.FirstOrDefaultAsync(input.Id);
           
		    var output = new GetTrailerTypeForEditOutput {TrailerType = ObjectMapper.Map<CreateOrEditTrailerTypeDto>(trailerType)};
			
            return output;
         }

		 public async Task CreateOrEdit(CreateOrEditTrailerTypeDto input)
         {
            if(input.Id == null){
				await Create(input);
			}
			else{
				await Update(input);
			}
         }

		 [AbpAuthorize(AppPermissions.Pages_TrailerTypes_Create)]
		 protected virtual async Task Create(CreateOrEditTrailerTypeDto input)
         {
            var trailerType = ObjectMapper.Map<TrailerType>(input);

			

            await _trailerTypeRepository.InsertAsync(trailerType);
         }

		 [AbpAuthorize(AppPermissions.Pages_TrailerTypes_Edit)]
		 protected virtual async Task Update(CreateOrEditTrailerTypeDto input)
         {
            var trailerType = await _trailerTypeRepository.FirstOrDefaultAsync((int)input.Id);
             ObjectMapper.Map(input, trailerType);
         }

		 [AbpAuthorize(AppPermissions.Pages_TrailerTypes_Delete)]
         public async Task Delete(EntityDto input)
         {
            await _trailerTypeRepository.DeleteAsync(input.Id);
         } 

		public async Task<FileDto> GetTrailerTypesToExcel(GetAllTrailerTypesForExcelInput input)
         {
			
			var filteredTrailerTypes = _trailerTypeRepository.GetAll()
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.DisplayName.Contains(input.Filter))
						.WhereIf(!string.IsNullOrWhiteSpace(input.DisplayNameFilter),  e => e.DisplayName == input.DisplayNameFilter);

			var query = (from o in filteredTrailerTypes
                         select new GetTrailerTypeForViewDto() { 
							TrailerType = new TrailerTypeDto
							{
                                DisplayName = o.DisplayName,
                                Id = o.Id
							}
						 });


            var trailerTypeListDtos = await query.ToListAsync();

            return _trailerTypesExcelExporter.ExportToFile(trailerTypeListDtos);
         }


    }
}