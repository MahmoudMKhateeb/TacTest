

using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using TACHYON.Trailers.PayloadMaxWeight.Exporting;
using TACHYON.Trailers.PayloadMaxWeight.Dtos;
using TACHYON.Dto;
using Abp.Application.Services.Dto;
using TACHYON.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;

namespace TACHYON.Trailers.PayloadMaxWeight
{
	[AbpAuthorize(AppPermissions.Pages_PayloadMaxWeights)]
    public class PayloadMaxWeightsAppService : TACHYONAppServiceBase, IPayloadMaxWeightsAppService
    {
		 private readonly IRepository<PayloadMaxWeight> _payloadMaxWeightRepository;
		 private readonly IPayloadMaxWeightsExcelExporter _payloadMaxWeightsExcelExporter;
		 

		  public PayloadMaxWeightsAppService(IRepository<PayloadMaxWeight> payloadMaxWeightRepository, IPayloadMaxWeightsExcelExporter payloadMaxWeightsExcelExporter ) 
		  {
			_payloadMaxWeightRepository = payloadMaxWeightRepository;
			_payloadMaxWeightsExcelExporter = payloadMaxWeightsExcelExporter;
			
		  }

		 public async Task<PagedResultDto<GetPayloadMaxWeightForViewDto>> GetAll(GetAllPayloadMaxWeightsInput input)
         {
			
			var filteredPayloadMaxWeights = _payloadMaxWeightRepository.GetAll()
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.DisplayName.Contains(input.Filter))
						.WhereIf(!string.IsNullOrWhiteSpace(input.DisplayNameFilter),  e => e.DisplayName == input.DisplayNameFilter)
						.WhereIf(input.MinMaxWeightFilter != null, e => e.MaxWeight >= input.MinMaxWeightFilter)
						.WhereIf(input.MaxMaxWeightFilter != null, e => e.MaxWeight <= input.MaxMaxWeightFilter);

			var pagedAndFilteredPayloadMaxWeights = filteredPayloadMaxWeights
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

			var payloadMaxWeights = from o in pagedAndFilteredPayloadMaxWeights
                         select new GetPayloadMaxWeightForViewDto() {
							PayloadMaxWeight = new PayloadMaxWeightDto
							{
                                DisplayName = o.DisplayName,
                                MaxWeight = o.MaxWeight,
                                Id = o.Id
							}
						};

            var totalCount = await filteredPayloadMaxWeights.CountAsync();

            return new PagedResultDto<GetPayloadMaxWeightForViewDto>(
                totalCount,
                await payloadMaxWeights.ToListAsync()
            );
         }
		 
		 public async Task<GetPayloadMaxWeightForViewDto> GetPayloadMaxWeightForView(int id)
         {
            var payloadMaxWeight = await _payloadMaxWeightRepository.GetAsync(id);

            var output = new GetPayloadMaxWeightForViewDto { PayloadMaxWeight = ObjectMapper.Map<PayloadMaxWeightDto>(payloadMaxWeight) };
			
            return output;
         }
		 
		 [AbpAuthorize(AppPermissions.Pages_PayloadMaxWeights_Edit)]
		 public async Task<GetPayloadMaxWeightForEditOutput> GetPayloadMaxWeightForEdit(EntityDto input)
         {
            var payloadMaxWeight = await _payloadMaxWeightRepository.FirstOrDefaultAsync(input.Id);
           
		    var output = new GetPayloadMaxWeightForEditOutput {PayloadMaxWeight = ObjectMapper.Map<CreateOrEditPayloadMaxWeightDto>(payloadMaxWeight)};
			
            return output;
         }

		 public async Task CreateOrEdit(CreateOrEditPayloadMaxWeightDto input)
         {
            if(input.Id == null){
				await Create(input);
			}
			else{
				await Update(input);
			}
         }

		 [AbpAuthorize(AppPermissions.Pages_PayloadMaxWeights_Create)]
		 protected virtual async Task Create(CreateOrEditPayloadMaxWeightDto input)
         {
            var payloadMaxWeight = ObjectMapper.Map<PayloadMaxWeight>(input);

			

            await _payloadMaxWeightRepository.InsertAsync(payloadMaxWeight);
         }

		 [AbpAuthorize(AppPermissions.Pages_PayloadMaxWeights_Edit)]
		 protected virtual async Task Update(CreateOrEditPayloadMaxWeightDto input)
         {
            var payloadMaxWeight = await _payloadMaxWeightRepository.FirstOrDefaultAsync((int)input.Id);
             ObjectMapper.Map(input, payloadMaxWeight);
         }

		 [AbpAuthorize(AppPermissions.Pages_PayloadMaxWeights_Delete)]
         public async Task Delete(EntityDto input)
         {
            await _payloadMaxWeightRepository.DeleteAsync(input.Id);
         } 

		public async Task<FileDto> GetPayloadMaxWeightsToExcel(GetAllPayloadMaxWeightsForExcelInput input)
         {
			
			var filteredPayloadMaxWeights = _payloadMaxWeightRepository.GetAll()
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.DisplayName.Contains(input.Filter))
						.WhereIf(!string.IsNullOrWhiteSpace(input.DisplayNameFilter),  e => e.DisplayName == input.DisplayNameFilter)
						.WhereIf(input.MinMaxWeightFilter != null, e => e.MaxWeight >= input.MinMaxWeightFilter)
						.WhereIf(input.MaxMaxWeightFilter != null, e => e.MaxWeight <= input.MaxMaxWeightFilter);

			var query = (from o in filteredPayloadMaxWeights
                         select new GetPayloadMaxWeightForViewDto() { 
							PayloadMaxWeight = new PayloadMaxWeightDto
							{
                                DisplayName = o.DisplayName,
                                MaxWeight = o.MaxWeight,
                                Id = o.Id
							}
						 });


            var payloadMaxWeightListDtos = await query.ToListAsync();

            return _payloadMaxWeightsExcelExporter.ExportToFile(payloadMaxWeightListDtos);
         }


    }
}