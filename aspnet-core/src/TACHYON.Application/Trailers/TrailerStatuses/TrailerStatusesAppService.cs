

using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using TACHYON.Trailers.TrailerStatuses.Exporting;
using TACHYON.Trailers.TrailerStatuses.Dtos;
using TACHYON.Dto;
using Abp.Application.Services.Dto;
using TACHYON.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;

namespace TACHYON.Trailers.TrailerStatuses
{
	[AbpAuthorize(AppPermissions.Pages_TrailerStatuses)]
    public class TrailerStatusesAppService : TACHYONAppServiceBase, ITrailerStatusesAppService
    {
		 private readonly IRepository<TrailerStatus> _trailerStatusRepository;
		 private readonly ITrailerStatusesExcelExporter _trailerStatusesExcelExporter;
		 

		  public TrailerStatusesAppService(IRepository<TrailerStatus> trailerStatusRepository, ITrailerStatusesExcelExporter trailerStatusesExcelExporter ) 
		  {
			_trailerStatusRepository = trailerStatusRepository;
			_trailerStatusesExcelExporter = trailerStatusesExcelExporter;
			
		  }

		 public async Task<PagedResultDto<GetTrailerStatusForViewDto>> GetAll(GetAllTrailerStatusesInput input)
         {
			
			var filteredTrailerStatuses = _trailerStatusRepository.GetAll()
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.DisplayName.Contains(input.Filter))
						.WhereIf(!string.IsNullOrWhiteSpace(input.DisplayNameFilter),  e => e.DisplayName == input.DisplayNameFilter);

			var pagedAndFilteredTrailerStatuses = filteredTrailerStatuses
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

			var trailerStatuses = from o in pagedAndFilteredTrailerStatuses
                         select new GetTrailerStatusForViewDto() {
							TrailerStatus = new TrailerStatusDto
							{
                                DisplayName = o.DisplayName,
                                Id = o.Id
							}
						};

            var totalCount = await filteredTrailerStatuses.CountAsync();

            return new PagedResultDto<GetTrailerStatusForViewDto>(
                totalCount,
                await trailerStatuses.ToListAsync()
            );
         }
		 
		 public async Task<GetTrailerStatusForViewDto> GetTrailerStatusForView(int id)
         {
            var trailerStatus = await _trailerStatusRepository.GetAsync(id);

            var output = new GetTrailerStatusForViewDto { TrailerStatus = ObjectMapper.Map<TrailerStatusDto>(trailerStatus) };
			
            return output;
         }
		 
		 [AbpAuthorize(AppPermissions.Pages_TrailerStatuses_Edit)]
		 public async Task<GetTrailerStatusForEditOutput> GetTrailerStatusForEdit(EntityDto input)
         {
            var trailerStatus = await _trailerStatusRepository.FirstOrDefaultAsync(input.Id);
           
		    var output = new GetTrailerStatusForEditOutput {TrailerStatus = ObjectMapper.Map<CreateOrEditTrailerStatusDto>(trailerStatus)};
			
            return output;
         }

		 public async Task CreateOrEdit(CreateOrEditTrailerStatusDto input)
         {
            if(input.Id == null){
				await Create(input);
			}
			else{
				await Update(input);
			}
         }

		 [AbpAuthorize(AppPermissions.Pages_TrailerStatuses_Create)]
		 protected virtual async Task Create(CreateOrEditTrailerStatusDto input)
         {
            var trailerStatus = ObjectMapper.Map<TrailerStatus>(input);

			

            await _trailerStatusRepository.InsertAsync(trailerStatus);
         }

		 [AbpAuthorize(AppPermissions.Pages_TrailerStatuses_Edit)]
		 protected virtual async Task Update(CreateOrEditTrailerStatusDto input)
         {
            var trailerStatus = await _trailerStatusRepository.FirstOrDefaultAsync((int)input.Id);
             ObjectMapper.Map(input, trailerStatus);
         }

		 [AbpAuthorize(AppPermissions.Pages_TrailerStatuses_Delete)]
         public async Task Delete(EntityDto input)
         {
            await _trailerStatusRepository.DeleteAsync(input.Id);
         } 

		public async Task<FileDto> GetTrailerStatusesToExcel(GetAllTrailerStatusesForExcelInput input)
         {
			
			var filteredTrailerStatuses = _trailerStatusRepository.GetAll()
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.DisplayName.Contains(input.Filter))
						.WhereIf(!string.IsNullOrWhiteSpace(input.DisplayNameFilter),  e => e.DisplayName == input.DisplayNameFilter);

			var query = (from o in filteredTrailerStatuses
                         select new GetTrailerStatusForViewDto() { 
							TrailerStatus = new TrailerStatusDto
							{
                                DisplayName = o.DisplayName,
                                Id = o.Id
							}
						 });


            var trailerStatusListDtos = await query.ToListAsync();

            return _trailerStatusesExcelExporter.ExportToFile(trailerStatusListDtos);
         }


    }
}