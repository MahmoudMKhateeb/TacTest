﻿

using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using TACHYON.UnitOfMeasures.Dtos;
using TACHYON.Dto;
using Abp.Application.Services.Dto;
using TACHYON.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;

namespace TACHYON.UnitOfMeasures
{
	[AbpAuthorize(AppPermissions.Pages_Administration_UnitOfMeasures)]
    public class UnitOfMeasuresAppService : TACHYONAppServiceBase, IUnitOfMeasuresAppService
    {
		 private readonly IRepository<UnitOfMeasure> _unitOfMeasureRepository;
		 

		  public UnitOfMeasuresAppService(IRepository<UnitOfMeasure> unitOfMeasureRepository ) 
		  {
			_unitOfMeasureRepository = unitOfMeasureRepository;
			
		  }

		 public async Task<PagedResultDto<GetUnitOfMeasureForViewDto>> GetAll(GetAllUnitOfMeasuresInput input)
         {
			
			var filteredUnitOfMeasures = _unitOfMeasureRepository.GetAll()
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.DisplayName.Contains(input.Filter))
						.WhereIf(!string.IsNullOrWhiteSpace(input.DisplayNameFilter),  e => e.DisplayName == input.DisplayNameFilter);

			var pagedAndFilteredUnitOfMeasures = filteredUnitOfMeasures
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

			var unitOfMeasures = from o in pagedAndFilteredUnitOfMeasures
                         select new GetUnitOfMeasureForViewDto() {
							UnitOfMeasure = new UnitOfMeasureDto
							{
                                DisplayName = o.DisplayName,
                                Id = o.Id
							}
						};

            var totalCount = await filteredUnitOfMeasures.CountAsync();

            return new PagedResultDto<GetUnitOfMeasureForViewDto>(
                totalCount,
                await unitOfMeasures.ToListAsync()
            );
         }
		 
		 public async Task<GetUnitOfMeasureForViewDto> GetUnitOfMeasureForView(int id)
         {
            var unitOfMeasure = await _unitOfMeasureRepository.GetAsync(id);

            var output = new GetUnitOfMeasureForViewDto { UnitOfMeasure = ObjectMapper.Map<UnitOfMeasureDto>(unitOfMeasure) };
			
            return output;
         }
		 
		 [AbpAuthorize(AppPermissions.Pages_Administration_UnitOfMeasures_Edit)]
		 public async Task<GetUnitOfMeasureForEditOutput> GetUnitOfMeasureForEdit(EntityDto input)
         {
            var unitOfMeasure = await _unitOfMeasureRepository.FirstOrDefaultAsync(input.Id);
           
		    var output = new GetUnitOfMeasureForEditOutput {UnitOfMeasure = ObjectMapper.Map<CreateOrEditUnitOfMeasureDto>(unitOfMeasure)};
			
            return output;
         }

		 public async Task CreateOrEdit(CreateOrEditUnitOfMeasureDto input)
         {
            if(input.Id == null){
				await Create(input);
			}
			else{
				await Update(input);
			}
         }

		 [AbpAuthorize(AppPermissions.Pages_Administration_UnitOfMeasures_Create)]
		 protected virtual async Task Create(CreateOrEditUnitOfMeasureDto input)
         {
            var unitOfMeasure = ObjectMapper.Map<UnitOfMeasure>(input);

			

            await _unitOfMeasureRepository.InsertAsync(unitOfMeasure);
         }

		 [AbpAuthorize(AppPermissions.Pages_Administration_UnitOfMeasures_Edit)]
		 protected virtual async Task Update(CreateOrEditUnitOfMeasureDto input)
         {
            var unitOfMeasure = await _unitOfMeasureRepository.FirstOrDefaultAsync((int)input.Id);
             ObjectMapper.Map(input, unitOfMeasure);
         }

		 [AbpAuthorize(AppPermissions.Pages_Administration_UnitOfMeasures_Delete)]
         public async Task Delete(EntityDto input)
         {
            await _unitOfMeasureRepository.DeleteAsync(input.Id);
         } 
    }
}