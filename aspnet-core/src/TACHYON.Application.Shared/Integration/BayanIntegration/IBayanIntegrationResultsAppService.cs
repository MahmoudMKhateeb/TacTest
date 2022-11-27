using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using TACHYON.Integration.BayanIntegration.Dtos;
using TACHYON.Dto;

namespace TACHYON.Integration.BayanIntegration
{
    public interface IBayanIntegrationResultsAppService : IApplicationService
    {
        Task<PagedResultDto<GetBayanIntegrationResultForViewDto>> GetAll(GetAllBayanIntegrationResultsInput input);

        Task<GetBayanIntegrationResultForViewDto> GetBayanIntegrationResultForView(long id);

        Task<GetBayanIntegrationResultForEditOutput> GetBayanIntegrationResultForEdit(EntityDto<long> input);

        Task CreateOrEdit(CreateOrEditBayanIntegrationResultDto input);

        Task Delete(EntityDto<long> input);

        Task<FileDto> GetBayanIntegrationResultsToExcel(GetAllBayanIntegrationResultsForExcelInput input);

        Task<PagedResultDto<BayanIntegrationResultShippingRequestTripLookupTableDto>> GetAllShippingRequestTripForLookupTable(GetAllForLookupTableInput input);

    }
}