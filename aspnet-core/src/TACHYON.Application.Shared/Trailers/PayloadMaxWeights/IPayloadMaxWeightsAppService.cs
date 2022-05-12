using Abp.Application.Services;
using Abp.Application.Services.Dto;
using System.Threading.Tasks;
using TACHYON.Dto;
using TACHYON.Trailers.PayloadMaxWeights.Dtos;

namespace TACHYON.Trailers.PayloadMaxWeights
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