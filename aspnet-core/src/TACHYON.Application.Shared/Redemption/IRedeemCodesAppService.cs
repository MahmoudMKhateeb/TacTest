using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using TACHYON.Redemption.Dtos;
using TACHYON.Dto;

namespace TACHYON.Redemption
{
    public interface IRedeemCodesAppService : IApplicationService
    {
        Task<PagedResultDto<GetRedeemCodeForViewDto>> GetAll(GetAllRedeemCodesInput input);

        Task<GetRedeemCodeForViewDto> GetRedeemCodeForView(long id);

        Task<GetRedeemCodeForEditOutput> GetRedeemCodeForEdit(EntityDto<long> input);

        Task CreateOrEdit(CreateOrEditRedeemCodeDto input);

        Task Delete(EntityDto<long> input);

        Task<FileDto> GetRedeemCodesToExcel(GetAllRedeemCodesForExcelInput input);

    }
}