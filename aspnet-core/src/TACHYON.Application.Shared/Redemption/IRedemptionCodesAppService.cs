using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using TACHYON.Redemption.Dtos;
using TACHYON.Dto;

namespace TACHYON.Redemption
{
    public interface IRedemptionCodesAppService : IApplicationService
    {
        Task<PagedResultDto<GetRedemptionCodeForViewDto>> GetAll(GetAllRedemptionCodesInput input);

        Task<GetRedemptionCodeForViewDto> GetRedemptionCodeForView(long id);

        Task<GetRedemptionCodeForEditOutput> GetRedemptionCodeForEdit(EntityDto<long> input);

        Task CreateOrEdit(CreateOrEditRedemptionCodeDto input);

        Task Delete(EntityDto<long> input);

        Task<FileDto> GetRedemptionCodesToExcel(GetAllRedemptionCodesForExcelInput input);

    }
}