using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using TACHYON.Trailers.PayloadMaxWeight.Dtos;
using TACHYON.Dto;


namespace TACHYON.Trailers.PayloadMaxWeight
{
    public interface IPayloadMaxWeightsAppService : IApplicationService 
    {
        Task<PagedResultDto<GetPayloadMaxWeightForViewDto>> GetAll(GetAllPayloadMaxWeightsInput input);

        Task<GetPayloadMaxWeightForViewDto> GetPayloadMaxWeightForView(int id);

		Task<GetPayloadMaxWeightForEditOutput> GetPayloadMaxWeightForEdit(EntityDto input);

		Task CreateOrEdit(CreateOrEditPayloadMaxWeightDto input);

		Task Delete(EntityDto input);

		Task<FileDto> GetPayloadMaxWeightsToExcel(GetAllPayloadMaxWeightsForExcelInput input);

		
    }
}